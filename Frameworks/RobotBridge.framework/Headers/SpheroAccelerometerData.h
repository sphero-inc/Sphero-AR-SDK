//
//  SpheroAccelerometerData.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotVision__SpheroAccelerometerData__
#define __RobotVision__SpheroAccelerometerData__

#include <iostream>
#include "SpheroAxisSensor.h"
#include "SpheroDeviceMessageDecoder.h"

namespace RobotBridge {
    
    class SpheroAccelerometerData : public DecodableObject {
        
    public:
        
        SpheroAccelerometerData(): normalizedAcceleration_(new SpheroThreeAxisSensor()),
                                   rawAcceleration_(new SpheroThreeAxisSensor()) {};
        SpheroAccelerometerData(
            SpheroThreeAxisSensor* raw, SpheroThreeAxisSensor* normalized):
            rawAcceleration_(raw), normalizedAcceleration_(normalized) {};
        virtual ~SpheroAccelerometerData();
        
        SpheroAccelerometerData(const SpheroAccelerometerData& data);
        SpheroAccelerometerData& operator=(const SpheroAccelerometerData& data);
        
        SpheroThreeAxisSensor* rawAcceleration();
        SpheroThreeAxisSensor* normalizedAcceleration();
        
        DecodableObject* decode(SpheroDeviceMessageDecoder* decoder);
        
    private:
        SpheroThreeAxisSensor* rawAcceleration_;
        SpheroThreeAxisSensor* normalizedAcceleration_;

        
    }; // class SpheroAccelerometerData
};  // namespace RobotBridge

#endif /* defined(__RobotVision__SpheroAccelerometerData__) */
