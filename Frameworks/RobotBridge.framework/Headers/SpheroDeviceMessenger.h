//
//  SpheroDeviceMessenger.h
//  RobotBridge
//
//  Created by Michael DePhillips on 2/21/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotBridge__SpheroDeviceMessenger__
#define __RobotBridge__SpheroDeviceMessenger__

#include <iostream>
#include <map>
#include "SpheroDeviceMessageDelegate.h"

extern "C" {
    typedef void (*ReceiveDeviceMessageCallback)(const char *);
    typedef void (*UpdateMaskCallback)(const char *);
    extern void receiveSpheroDeviceMessage(const char*);
}

namespace RobotBridge {

class SpheroDeviceMessenger {

    public:
    	static SpheroDeviceMessenger& sharedInstance();
        void receiveDeviceMessage(const char* msg);
        void receiveDeviceMessage(SpheroDeviceMessage* message);
    
        void addSpheroDeviceMessageDelegate(SpheroDeviceMessageDelegate* delegate, unsigned long long mask);
        void removeSpheroDeviceMessageDelegate(SpheroDeviceMessageDelegate* delegate);
    
    private:
        static SpheroDeviceMessenger instance;
        SpheroDeviceMessenger() {}
        ~SpheroDeviceMessenger() {}
    
        SpheroDeviceMessenger(const SpheroDeviceMessenger&);
        SpheroDeviceMessenger& operator=(const SpheroDeviceMessenger&);
    
        std::map<SpheroDeviceMessageDelegate*, unsigned long long> delegates;
    
        unsigned long long getCurrentDataStreamingMask();
        void sendDataStreamingCommand();
    
    }; // class SpheroDeviceMessenger
}; // namespace RobotBridge

#endif /* defined(__RobotBridge__SpheroDeviceMessenger__) */
