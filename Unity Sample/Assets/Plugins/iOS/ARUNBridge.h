//
//  ARUNBridge.h
//  Unity-iPhone
//
//  Created by Mike DePhillips on 3/11/13.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <RobotVision/RobotVision.h>
#import <CoreMotion/CoreMotion.h>

extern "C" {
    
#ifndef ARUNBridge_Def
#define ARUNBridge_Def
    typedef struct ARUNQuaternion {
        float w;
        float x;
        float y;
        float z;
    }ARUNQuaternion;
    typedef struct ARUNVector3D {
        float x;
        float y;
        float z;
    }ARUNVector3D;
    
    typedef struct ARUNVector2D {
        float x;
        float y;
    }ARUNVector2D;
    
    typedef struct ARUNSize {
        int width;
        int height;
    }ARUNSize;
    
    typedef struct ARUNResult {
        float locatorAlignmentAngle;
        int calibratingPutCount;
        ARUNSize backgroundVideoSize;
        RobotVision::TrackingState trackingState;
        ARUNVector3D spheroPosition;
        ARUNVector2D spheroVelocity;
        RobotVision::ARMatrix cameraMatrix;
        AuPlatformConfiguration platformConfig;
    }ARUNResult;
    
#endif // #ifdef ARUNBridge_Def
    
    //typedef void (*ReceiveDeviceMessageCallback)(const char *);
}

/*!
 *  iOS specific implementation of the result delegate in RobotVision
 */
class ARUNBridgeResultDelegate : public RobotVision::ARResultsDelegate {
public:
    ARUNBridgeResultDelegate() {}
    virtual ~ARUNBridgeResultDelegate() {}
    void onNewResultReady(RobotVision::ARResult* result);
};

@interface ARUNBridge : NSObject {
    BOOL robotOnline;
    RobotVision::AREngine* arEngine;
    RobotVision::ARResult* currentArResult;
    RobotVision::ARResult* queuedArResult;
    PlatformParameters platformParams;
    CMMotionManager* motionManager;
    ARUNBridgeResultDelegate* resultsDelegate;
    pthread_mutex_t currentResultLock_;
    pthread_mutex_t queuedResultLock_;
    BOOL isInitialized;
    BOOL isArEngineSetup;
}

@property (nonatomic, readwrite) BOOL robotOnline;
@property (nonatomic, readwrite) BOOL isInitialized;


+(ARUNBridge*)sharedBridge;
-(void)onDeliveryWithResult:(RobotVision::ARResult*)result;

@end
