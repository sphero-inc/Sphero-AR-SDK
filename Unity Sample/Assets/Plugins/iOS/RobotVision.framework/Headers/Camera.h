//
//  Camera.h
//  RobotVision
//
//  Created by Brandon Dorris on 2/11/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotVision__Camera__
#define __RobotVision__Camera__

#include "AuPlatformConfiguration.h"

namespace RobotVision {
    
    class FrameDeliveryDelegate {
    public:
        FrameDeliveryDelegate() {}
        virtual ~FrameDeliveryDelegate() {}
        
        virtual void onFrameDelivered(AuImage* image) = 0;
    };

    class Camera {
    public:
        Camera() {}
        virtual ~Camera() {}
        
        virtual AuPlatformConfiguration* platformConfiguration() {}
        virtual void setFrameDeliveryDelegate(FrameDeliveryDelegate* delegate) {}
        virtual void pauseCamera() {}
        virtual void resumeCamera() {}
        
    private:
        FrameDeliveryDelegate* delegate_;
    };
    
}

#endif /* defined(__RobotVision__Camera__) */
