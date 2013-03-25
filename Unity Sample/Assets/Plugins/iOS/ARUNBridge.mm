//
//  ARUNBridge.m
//  Unity-iPhone
//
//  Modified by Mike DePhillips on 3/11/13.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#import "ARUNBridge.h"
#import <RobotKit/RobotKit.h>
#import <RobotBridge/MessengerBridge_iOS.h>
#import <OpenGLES/EAGL.h>
#import <OpenGLES/ES1/gl.h>

static ARUNBridge *sharedBridge = nil;

void ARUNBridgeResultDelegate::onNewResultReady(RobotVision::ARResult *result) {
    [[ARUNBridge sharedBridge] onDeliveryWithResult:result];
}

@implementation ARUNBridge

@synthesize robotOnline;
@synthesize isInitialized;

-(id)init {
    self = [super init];
    
    robotOnline = NO;
    isInitialized = NO;
    
    // Initialize iOS part of robot bridge.
    // TODO: IS THIS WHAT'S CAUSING CRASH ON LOAD????
    [MessengerBridge_iOS sharedBridge];
    
    isArEngineSetup = NO;
    arEngine = &RobotVision::AREngine::sharedInstance();
    [self initializeEngine];

    return self;
}

-(void) initializeEngine {
    // Set up AREngine
    platformParams.mode = AUX_SHARKY_CAMERA_MOTION_MODE;
    
    motionManager = [[CMMotionManager alloc] init];
    motionManager.deviceMotionUpdateInterval = 1.0/60.0;
    platformParams.motionManager = motionManager;
    
    arEngine->initializeEngine(platformParams);
    
    // Initialize result thread lock
    pthread_mutexattr_t reentrant;
    pthread_mutexattr_init(&reentrant);
    pthread_mutexattr_settype(&reentrant, PTHREAD_MUTEX_RECURSIVE);
    int err = pthread_mutex_init(&queuedResultLock_, &reentrant);
    err = pthread_mutex_init(&currentResultLock_, &reentrant);
    
    // Set results delegate
    resultsDelegate = new ARUNBridgeResultDelegate();
    arEngine->setResultsDelegate(resultsDelegate);
    
    isArEngineSetup = YES;
    
    //[self startVisionEngine];
}

+(ARUNBridge*)sharedBridge {
    if(sharedBridge==nil) {
        sharedBridge = [[ARUNBridge alloc] init];
    }
    return sharedBridge;
}

-(BOOL)startVisionEngine {
    
    if( !robotOnline ) return NO;
    [RKRGBLEDOutputCommand sendCommandWithRed:1.0f green:1.0f blue:1.0f];
    [RKConfigureLocatorCommand sendCommandForFlag:RKConfigureLocatorRotateWithCalibrateFlagOn
                                             newX:0
                                             newY:0
                                           newYaw:0];
    arEngine->startVision();
    return YES;
}

-(void)pauseVisionEngine {
    arEngine->pauseVision();
}

-(void)updateResults {
    // Replace old frame with new queued frame
    pthread_mutex_lock(&currentResultLock_);
    {
        if( currentArResult != NULL ) {
            currentArResult->release();
        }
        
        pthread_mutex_lock(&queuedResultLock_);
        {
            currentArResult = queuedArResult;
            currentArResult->retain();
        }
        pthread_mutex_unlock(&queuedResultLock_);
        
    }
    pthread_mutex_unlock(&currentResultLock_);
    
}

-(void)onDeliveryWithResult:(RobotVision::ARResult*)result {
    
    pthread_mutex_lock(&queuedResultLock_);
    {
        if( queuedArResult != NULL ) {
            queuedArResult->release();
        }
        queuedArResult = result;
        queuedArResult->retain();
    }
    pthread_mutex_unlock(&queuedResultLock_);
    
    // If it is the first result to come in, it should also be the current frame
    if( currentArResult == NULL ) {
        pthread_mutex_lock(&currentResultLock_);
        {
            currentArResult = queuedArResult;
            currentArResult->retain();
        }
        pthread_mutex_unlock(&currentResultLock_);
    }
    
    isInitialized = YES;
}

-(void)getFrame:(unsigned char**)img :(int*)width :(int*) height :(int*) stride; {
    // Get result frame data
    AuImage* resultImage = (currentArResult->virtualCamera())->cameraFrame();
	*img = resultImage->data;
	*height = resultImage->rows;
	*width = resultImage->cols;
	*stride = resultImage->bytesPerRow;
}

-(ARUNResult) getCurrentResultStruct {
    
    [self updateResults];
    
    ARUNResult result;
    result.locatorAlignmentAngle = auxGetLocatorAlignmentAngle();
    result.calibratingPutCount = auxGetLocatorCalibratingPutCount();
    
    ARUNSize backgroundSize;
    backgroundSize.width = currentArResult->virtualCamera()->cameraFrame()->cols;
    backgroundSize.height = currentArResult->virtualCamera()->cameraFrame()->rows;
    result.backgroundVideoSize = backgroundSize;
    
    result.trackingState = currentArResult->virtualSphero()->trackingState();
    
    ARUNVector3D position;
    position.x = currentArResult->virtualSphero()->pose()->position().x;
    position.y = currentArResult->virtualSphero()->pose()->position().y;
    position.z = currentArResult->virtualSphero()->pose()->position().z;
    result.spheroPosition = position;
    
    ARUNVector2D velocity;
    velocity.x = currentArResult->virtualSphero()->velocity().x;
    velocity.y = currentArResult->virtualSphero()->velocity().y;
    result.spheroVelocity = velocity;
    
    result.cameraMatrix = currentArResult->virtualCamera()->pose()->matrix();
    
    result.platformConfig = (*auGetPlatformConfiguration());
    
    return result;
}

-(void)dealloc {
    [motionManager release];
    delete currentArResult;
    delete queuedArResult;
    delete resultsDelegate;
    pthread_mutex_destroy(&currentResultLock_);
    pthread_mutex_destroy(&queuedResultLock_);
    [super dealloc];
}

extern "C" {
    
    BOOL _ARUNBridgeStartVisionEngine() {
        return [[ARUNBridge sharedBridge] startVisionEngine];
    }
    
    void _ARUNBridgePauseVisionEngine() {
        [[ARUNBridge sharedBridge] pauseVisionEngine];
    }
    
    BOOL _ARUNBridgeVisionIsInitialized() {
        return [ARUNBridge sharedBridge].isInitialized;
    }
    
    ARUNResult _ARUNBridgeGetCurrentResult() {
        return [[ARUNBridge sharedBridge] getCurrentResultStruct];
    }
    
    void _ARUNBridgeUpdateVideoTexture(int textureID) {
		
		//Get the latest image data to draw
		unsigned char* imgdata;
		int rows;
		int cols;
		int stride;
        [[ARUNBridge sharedBridge] getFrame:&imgdata :&cols :&rows :&stride];
		
		//Don't do anything if there isn't a frame to draw
		if(imgdata==NULL)
		{
			return;
		};
        
		//Send the camera frame to the GPU
		glBindTexture(GL_TEXTURE_2D, textureID);
		
		// unforunately we can't set stride in gles, adjust the rows instead
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE );
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, stride / 4, rows, 0, GL_BGRA, GL_UNSIGNED_BYTE, imgdata);
	}    
}

@end

