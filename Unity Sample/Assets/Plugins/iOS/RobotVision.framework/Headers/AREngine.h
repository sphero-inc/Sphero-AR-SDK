/*!
 *  @file:  AREngine.h
 *  RobotVision
 *
 *  Created by Brandon Dorris on 2/7/13.
 *  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
 *
 */

#ifndef RobotVision_AREngine_h
#define RobotVision_AREngine_h

#include "AureDef.h"
#include "ARResult.h"
#include "Camera.h"
#include "PlatformParameters.h"
#include "ARMacros.h"
#include <pthread.h>

namespace RobotVision {
    
    class DeviceSensors;
    class SpheroSensors;
    class Camera;
    
    typedef enum {
        AR_SUCCESS = 0,
        AR_FAILURE = 1,
        AR_VISION_ERROR,
        AR_CAMERA_ERROR,
        AR_SPHERO_ERROR,
        AR_DEVICE_ERROR
    } ARError;
    
    class ARResultsDelegate {
    public:
        ARResultsDelegate() {}
        virtual ~ARResultsDelegate() {}
        virtual void onNewResultReady(ARResult* result) = 0;
    };

    class AREngine : public FrameDeliveryDelegate {
    public:
        
        static AREngine& sharedInstance();
        
        void initializeEngine(PlatformParameters parameters);
        
        /*!
         *  Pauses the vision engine and prevents results from being delivered.
         */
        void pauseVision();
        
        /*!
         *  Quits the vision engine and destroys threads
         */
        void quitVision();
        
        /*!
         *  Starts the vision engine and begins processing frames and delivering results.
         *  Also doubles as the resumeVision() function after a pauseVision()
         */
        void startVision();
        
        /*!
         *  Used to set where the results of the vision processing go. The delegate
         *  must implement the ResultsDelegate interface.
         */
        void setResultsDelegate(ARResultsDelegate* delegate);
        
        /*!
         *  The camera being used to capture frames.
         */
        Camera* camera() { return camera_; }
        
        /*!
         *  Processes the incoming camera frame by inserting into the vision library.
         *
         *  @param image   The frame to be processed.
         */
        void onFrameDelivered(AuImage* image);
        
        /*! 
         * Static method callbacks for talking to Fabrizio's Vision Engine
         */
        static AU_ERROR acquireFrame(AuImage** auImageFrame, au_time* time);
        static AU_ERROR relinquishFrame(AuImage* frame);
        static void logMethod(char* message);
        static au_time getTime(void);
        static void onVisionTaskComplete(AuVisionTask* task, AuImage* img, au_time time, AU_ERROR err);
        static void requestColorChange(const au_scalar*const rgb);
        static void focusCameraAtPointMethod(const au_scalar row, const au_scalar col);
        static au_bool cameraIsFocusingMethod(void);
        static void* startVisionThreadCall(void*);
        
    private:
        
        AREngine();
        virtual ~AREngine();
        static AREngine instance;
        
        /**
         * Initializes the entire engine including the robot data streaming.
         * @return An error code describing any error that occurred or AR_SUCCESS if successful.
         */
        ARError initializeEngine();
        
        DISALLOW_COPY_AND_ASSIGN(AREngine);
        
        pthread_t       auVisionThread_;
        
        Camera*         camera_;
        DeviceSensors*  deviceSensors_;
        SpheroSensors*  spheroSensors_;
        
        pthread_mutex_t visionStateLock_;
        bool firstImageReceived_;
        bool startVision_;
        bool visionPaused_;
        bool visionInitialized_;
        
        static AuImage* latestImage_;
        static pthread_mutex_t latestFrameLock_;
        static ARResultsDelegate* resultsDelegate_;
    };
    
} // namespace RobotVision

#endif // #ifndef RobotVision_AREngine_h
