//
//  SpheroDeviceMessage.h
//  RobotBridge
//
//  Created by Michael DePhillips on 2/21/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotBridge__SpheroDeviceMessage__
#define __RobotBridge__SpheroDeviceMessage__

#include <iostream>

#include "DecodableObject.h"

namespace RobotBridge {
    
    class SpheroDeviceMessageDecoder;
    class SpheroDeviceMessage {
    public:
        
        SpheroDeviceMessage(): timestamp_(0), robotId_(""), className_("") {}
        SpheroDeviceMessage(uint64_t timestamp, std::string robotId, std::string className):
            timestamp_(timestamp), robotId_(robotId), className_(className) {}
        virtual ~SpheroDeviceMessage();
        
        SpheroDeviceMessage(const SpheroDeviceMessage& msg);
        SpheroDeviceMessage& operator=(const SpheroDeviceMessage& msg);
        
        virtual SpheroDeviceMessage* clone(SpheroDeviceMessageDecoder* decoder);

        std::string robotId();
        std::string className();
        uint64_t timestamp();
        
    protected:
        uint64_t timestamp_;
        std::string robotId_;
        std::string className_;
    };
};


#endif /* defined(__RobotBridge__SpheroDeviceMessage__) */
