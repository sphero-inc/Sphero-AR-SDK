//
//  SpheroDeviceMessageDelegate.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/21/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotBridge_SpheroDeviceMessageDelegate_h
#define RobotBridge_SpheroDeviceMessageDelegate_h

#include "SpheroDeviceMessage.h"

namespace RobotBridge {
    
    class SpheroDeviceMessageDelegate {
    public:
        SpheroDeviceMessageDelegate() {}
        virtual ~SpheroDeviceMessageDelegate() {}
        
        virtual void onMessageDelivered(SpheroDeviceMessage* message) = 0;
        virtual void onSerializedMessageDelivered(const char* message) = 0;
    }; // class SpheroDeviceMessageDelegate
    
}; // namespace RobotBridge

#endif // defined(RobotBridge_SpheroDeviceMessageDelegate_h)
