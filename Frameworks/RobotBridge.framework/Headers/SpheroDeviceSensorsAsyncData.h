//
//  SpheroDeviceSensorsAsyncData.h
//  RobotBridge
//
//  Created by Michael DePhillips on 2/21/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotBridge__SpheroDeviceSensorsAsyncData__
#define __RobotBridge__SpheroDeviceSensorsAsyncData__

#include <iostream>
#include <set>
#include <string>
#include "DecodableObject.h"
#include "SpheroDeviceSensorsData.h"
#include "SpheroDeviceMessage.h"

namespace RobotBridge {
    class SpheroDeviceSensorsAsyncData : public SpheroDeviceMessage, public DecodableObject {
    public:
        
        SpheroDeviceSensorsAsyncData(): SpheroDeviceMessage(), mask_(0), frameCount_(0) {}
        SpheroDeviceSensorsAsyncData(
            int frameCount, uint64_t mask, std::set<SpheroDeviceSensorsData*> frames):
            frameCount_(frameCount), mask_(mask), frames_(frames) {}
        
        SpheroDeviceSensorsAsyncData(uint64_t timestamp, std::string robotId,
                                     int frameCount, uint64_t mask,
                                     std::set<SpheroDeviceSensorsData*> frames):
                                     SpheroDeviceMessage(timestamp, robotId, "SpheroDeviceSensorsAsyncData"),
                                     frameCount_(frameCount), mask_(mask),
                                     frames_(frames) {}
        
        virtual ~SpheroDeviceSensorsAsyncData();
        
        SpheroDeviceSensorsAsyncData(const SpheroDeviceSensorsAsyncData& data);
        SpheroDeviceSensorsAsyncData& operator=(const SpheroDeviceSensorsAsyncData& data);
        
        SpheroDeviceMessage* clone(SpheroDeviceMessageDecoder* decoder);
        
        DecodableObject* decode(SpheroDeviceMessageDecoder* decoder);
        
        int frameCount();
        uint64_t mask();
        std::set<SpheroDeviceSensorsData*> frames();
        
    private:
        uint64_t mask_;
        int frameCount_;
        std::set<SpheroDeviceSensorsData*> frames_;
    };
};

#endif /* defined(__RobotBridge__SpheroDeviceSensorsAsyncData__) */
