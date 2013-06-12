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
}

/*!
 *  iOS specific implementation of the result delegate in RobotVision
 */
class ARUNBridge : public RobotVision::ARResultsDelegate {
public:
    
    static ARUNBridge& sharedInstance();
    
    void onNewResultReady(RobotVision::ARResult* result);
    
    void initializeBridge(RobotVision::PlatformParameters parameters);
    
    RobotVision::AREngine* arEngine();
    bool isInitialized();
    void getFrame(unsigned char** img, int* width, int* height, int* stride);
    ARUNResult getCurrentResultStruct();
    void updateResults();

    bool hasNewFrame() const;
    
private:
    
    static ARUNBridge *s_pInstance;
    ARUNBridge();
    virtual ~ARUNBridge();
    
    RobotVision::AREngine* arEngine_;
    RobotVision::ARResult* currentArResult_;
    RobotVision::ARResult* queuedArResult_;
    
    pthread_mutex_t currentResultLock_;
    pthread_mutex_t queuedResultLock_;
    
    RobotVision::PlatformParameters platformParams_;
    
    bool isInitialized_;
    bool isArEngineSetup_;
};

inline RobotVision::AREngine* ARUNBridge::arEngine() {
    return arEngine_;
}

inline bool ARUNBridge::isInitialized() {
    return isInitialized_;
}


@interface ARUNBridge_iOS : NSObject {
    CMMotionManager* motionManager;
}

+(ARUNBridge_iOS*)sharedBridge;
-(void)initializeEngineWithCameraMode:(RobotVision::ARCameraMode)mode;

@end

inline bool ARUNBridge::hasNewFrame() const
{
    return queuedArResult_ != NULL;
}
