//
//  SpheroGyroData.h
//  RobotBridge
//
//  Created by Michael DePhillips on 2/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotVision__SpheroGyroData__
#define __RobotVision__SpheroGyroData__

#include <iostream>
#include "SpheroAxisSensor.h"
#include "SpheroDeviceMessageDecoder.h"

namespace RobotBridge {
    
    class SpheroGyroData : public DecodableObject {
        
    public:
        
        SpheroGyroData(): rawRotationRate_(new SpheroThreeAxisSensor()),
                          normalizedRotationRate_(new SpheroThreeAxisSensor()) {};
        SpheroGyroData(SpheroThreeAxisSensor* raw, SpheroThreeAxisSensor* normalized):
                       rawRotationRate_(raw), normalizedRotationRate_(normalized) {};
        virtual ~SpheroGyroData();
        
        SpheroGyroData(const SpheroGyroData& data);
        SpheroGyroData& operator=(const SpheroGyroData& data);
    
        DecodableObject* decode(SpheroDeviceMessageDecoder* decoder);
        
        SpheroThreeAxisSensor* rawRotationRate();
        SpheroThreeAxisSensor* normalizedRotationRate();
        
    private:
        SpheroThreeAxisSensor* rawRotationRate_;
        SpheroThreeAxisSensor* normalizedRotationRate_;
        
        
    }; // class SpheroGyroData
};  // namespace RobotBridge

#endif /* defined(__RobotVision__SpheroGyroData__) */
