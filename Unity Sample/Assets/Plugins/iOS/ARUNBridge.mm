//
//  ARUNBridge.m
//  Unity-iPhone
//
//  Modified by Mike DePhillips on 3/11/13.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#import "ARUNBridge.h"
#import <OpenGLES/EAGL.h>
#import <OpenGLES/ES1/gl.h>
#include <pthread.h>
#include "AppController.h"

ARUNBridge* ARUNBridge::s_pInstance = NULL;

static pthread_mutex_t singletonLock = PTHREAD_MUTEX_INITIALIZER;

ARUNBridge& ARUNBridge::sharedInstance() {
	if (!s_pInstance)
	{
		pthread_mutex_lock(&singletonLock);
		if (!s_pInstance)
		{
			s_pInstance = new ARUNBridge();
		}
		pthread_mutex_unlock(&singletonLock);
	}
	return *s_pInstance;
}

ARUNBridge::ARUNBridge():
	arEngine_(&RobotVision::AREngine::sharedInstance()),
	isInitialized_(false),
	isArEngineSetup_(false),
	queuedArResult_(NULL),
	currentArResult_(NULL),
	platformParams_(),
	currentResultLock_(),
	queuedResultLock_()
{}

ARUNBridge::~ARUNBridge() {
	if(currentArResult_) {
		pthread_mutex_lock(&currentResultLock_);
		{
			if (currentArResult_)
			{
				currentArResult_->release();
				currentArResult_ = NULL;
			}
		}
		pthread_mutex_unlock(&currentResultLock_);
	}
	if (queuedArResult_) {
		pthread_mutex_lock(&queuedResultLock_);
		{
			if (queuedArResult_) {
				queuedArResult_->release();
				queuedArResult_ = NULL;
			}
		}
		pthread_mutex_unlock(&queuedResultLock_);
	}

	pthread_mutex_destroy(&currentResultLock_);
	pthread_mutex_destroy(&queuedResultLock_);

	if (s_pInstance == this) {
		s_pInstance = NULL;
	}
}

void ARUNBridge::initializeBridge(RobotVision::PlatformParameters parameters) {
	platformParams_ = parameters;
	arEngine_->initializeEngine(platformParams_);
	
	// Initialize result thread lock
	pthread_mutexattr_t reentrant;
	pthread_mutexattr_init(&reentrant);
	pthread_mutexattr_settype(&reentrant, PTHREAD_MUTEX_RECURSIVE);
	int err = pthread_mutex_init(&queuedResultLock_, &reentrant);
	err = pthread_mutex_init(&currentResultLock_, &reentrant);
	
	// Set results delegate
	arEngine_->setResultsDelegate(this);
	
	isArEngineSetup_ = true;
}

void ARUNBridge::onNewResultReady(RobotVision::ARResult *result) {
	if (result)
	{
		pthread_mutex_lock(&queuedResultLock_);
		{
			RobotVision::ARResult* tempResult = queuedArResult_;

			queuedArResult_ = result;
			queuedArResult_->retain();

			if (tempResult)
			{
				tempResult->release();
			}
		}
		pthread_mutex_unlock(&queuedResultLock_);
	}
	
	// If it is the first result to come in, it should also be the current frame
	if( currentArResult_ == NULL ) {
		pthread_mutex_lock(&currentResultLock_);
		{
			if (!currentArResult_) {
				currentArResult_ = queuedArResult_;
				currentArResult_->retain();
			}
		}
		pthread_mutex_unlock(&currentResultLock_);
	}
	
	isInitialized_ = true;
}

void ARUNBridge::getFrame(unsigned char **img, int *width, int *height, int *stride) {
	pthread_mutex_lock(&currentResultLock_);
	{
		// Get result frame data
		RobotVision::ARImage* resultImage = currentArResult_->virtualCamera()->cameraFrame();
		*img = resultImage->data();
		*height = resultImage->height();
		*width = resultImage->width();
		*stride = resultImage->stride();
	}
	pthread_mutex_unlock(&currentResultLock_);
}

void ARUNBridge::updateResults() {
	// Replace old frame with new queued frame
	if (queuedArResult_)
	{
		pthread_mutex_lock(&currentResultLock_);
		{
			pthread_mutex_lock(&queuedResultLock_);
			{
				if (queuedArResult_)
				{
					RobotVision::ARResult* tempResult = currentArResult_;

					currentArResult_ = queuedArResult_;
					currentArResult_->retain();
					queuedArResult_->release();
					queuedArResult_ = NULL;

					if( tempResult != NULL ) {
						tempResult->release();
					}
				}
			}
			pthread_mutex_unlock(&queuedResultLock_);
		}
		pthread_mutex_unlock(&currentResultLock_);
	}
	
}

ARUNResult ARUNBridge::getCurrentResultStruct() {
	
	updateResults();
	
	ARUNResult result;
	
	pthread_mutex_lock(&currentResultLock_);
	{
		result.locatorAlignmentAngle = currentArResult_->virtualSphero()->locatorAlignmentAngle();
		result.calibratingPutCount = currentArResult_->virtualSphero()->calibrationPutCount();
		
		ARUNSize backgroundSize;
		backgroundSize.width = currentArResult_->virtualCamera()->cameraFrame()->width();
		backgroundSize.height = currentArResult_->virtualCamera()->cameraFrame()->height();
		result.backgroundVideoSize = backgroundSize;
		
		result.trackingState = currentArResult_->virtualSphero()->trackingState();
		
		ARUNVector3D position;
		position.x = currentArResult_->virtualSphero()->pose()->position().x;
		position.y = currentArResult_->virtualSphero()->pose()->position().y;
		position.z = currentArResult_->virtualSphero()->pose()->position().z;
		result.spheroPosition = position;
		
		ARUNVector2D velocity;
		velocity.x = currentArResult_->virtualSphero()->velocity().x;
		velocity.y = currentArResult_->virtualSphero()->velocity().y;
		result.spheroVelocity = velocity;
		
		result.cameraMatrix = currentArResult_->virtualCamera()->pose()->matrix();    
		result.platformConfig = (*(arEngine_->camera()->platformConfiguration()));
	}
	pthread_mutex_unlock(&currentResultLock_);
	
	return result;
}

extern "C" {

	void _ARUNBridgeInitializeVisionEngine(RobotVision::ARCameraMode mode)
	{
		// Initialize iOS bridge and ARUNBridge and try to start vision
		[[ARUNBridge_iOS sharedBridge] initializeEngineWithCameraMode:mode];
	}
		
	bool _ARUNBridgeStartVisionEngine(RobotVision::ARCameraMode mode) {
		ARUNBridge::sharedInstance().arEngine()->startVision();
		// TODO: implement a test if it actually started properlly
		return true;
	}
	
	void _ARUNBridgePauseVisionEngine() {
		ARUNBridge::sharedInstance().arEngine()->pauseVision();
	}
	
	void _ARUNBridgeQuitVisionEngine() {
		ARUNBridge::sharedInstance().arEngine()->quitVision();
	}
	
	bool _ARUNBridgeVisionIsInitialized() {
		return ARUNBridge::sharedInstance().isInitialized();
	}

	char* _GetVersionString()
	{
		NSString* version = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleVersion"];
		return strdup([version UTF8String]);
	}

	BOOL _ARUNHasNewFrame()
	{
		return ARUNBridge::sharedInstance().hasNewFrame();
	}
	
	ARUNResult _ARUNBridgeGetCurrentResult() {
		return ARUNBridge::sharedInstance().getCurrentResultStruct();
	}
	
	void _ARUNBridgeUpdateVideoTexture(int textureID) {
		
		//Get the latest image data to draw
		unsigned char* imgdata;
		int rows;
		int cols;
		int stride;
		ARUNBridge::sharedInstance().getFrame(&imgdata, &cols, &rows, &stride);
		
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

static ARUNBridge_iOS *sharedBridge = nil;
extern CMMotionManager *sMotionManager;

@implementation ARUNBridge_iOS

-(id)init {
	self = [super init];
	if (self == nil) return nil;
    
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(appWillEnterBackground) name:UIApplicationWillTerminateNotification object:nil];
	
	return self;
}

-(void)appWillEnterBackground {
    ARUNBridge::sharedInstance().arEngine()->quitVision();
}

-(AUX_CAMERA_MOTION_MODE)convertMotionMode:(RobotVision::ARCameraMode)mode {
	AUX_CAMERA_MOTION_MODE newMode = AUX_SHARKY_CAMERA_MOTION_MODE;
	if( mode == RobotVision::CAMERA_MODE_ALLOW_VERTICAL_MOVEMENT ) {
		newMode = AUX_SHARKY_CAMERA_MOTION_MODE;
	}
	else if( mode == RobotVision::CAMERA_MODE_STATIC_HEIGHT ) {
		newMode = AUX_ZOMBIE_CAMERA_MOTION_MODE;
	}
	return newMode;
}

-(void) initializeEngineWithCameraMode:(RobotVision::ARCameraMode)mode {
    //Only one instance of CMMotion Manager is allowed per app, if we have already created it don't init the rest of the app
    if(motionManager) return;
	// Set up platform dependent code
	RobotVision::PlatformParameters platformParams;
	platformParams.mode = [self convertMotionMode:mode];
    if(motionManager==nil) {
        NSLog(@"ARUNBRidge - Getting or creating motion manager");
        if(sMotionManager==nil) {
            NSLog(@"ARUNBRidge - Shared motion mangaer doesn't exist, creating one.");
            sMotionManager = [[CMMotionManager alloc] init];
        }
        motionManager = sMotionManager;
        motionManager.deviceMotionUpdateInterval = 1.0/60.0;
    }
    //if([AppController getQueue]==nil) {
    //    [AppController setQueue:[[NSOperationQueue alloc] init]];
    //}
    NSLog(@"ARUNBRidge - Setting new platform params");
    
	platformParams.motionManager = motionManager;
    platformParams.queue = [[NSOperationQueue alloc] init];
	
	ARUNBridge::sharedInstance().initializeBridge(platformParams);
}

+(ARUNBridge_iOS*)sharedBridge {
	if(sharedBridge==nil) {
		sharedBridge = [[ARUNBridge_iOS alloc] init];
	}
	return sharedBridge;
}

-(void)dealloc {
	[motionManager release];
	[super dealloc];
}

@end

