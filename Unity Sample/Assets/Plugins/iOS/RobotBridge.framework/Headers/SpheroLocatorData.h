//
//  SpheroLocatorData.h
//  RobotBridge
//
//  Created by Michael DePhillips on 2/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotVision__SpheroLocatorData__
#define __RobotVision__SpheroLocatorData__

#include <iostream>
#include "SpheroAxisSensor.h"
#include "SpheroDeviceMessageDecoder.h"

namespace RobotBridge {
    
    class SpheroLocatorData : public DecodableObject {
        
    public:
        
        SpheroLocatorData(): position_(new SpheroTwoAxisSensor()),
                             velocity_(new SpheroTwoAxisSensor()) {};
        SpheroLocatorData(SpheroTwoAxisSensor* position, SpheroTwoAxisSensor* velocity):
                          position_(position), velocity_(velocity) {};
        virtual ~SpheroLocatorData();
        
        SpheroLocatorData(const SpheroLocatorData& data);
        SpheroLocatorData& operator=(const SpheroLocatorData& data);
    
        DecodableObject* decode(SpheroDeviceMessageDecoder* decoder);
        
        SpheroTwoAxisSensor* position();
        SpheroTwoAxisSensor* velocity();
        
    private:
        SpheroTwoAxisSensor* position_;
        SpheroTwoAxisSensor* velocity_;
        
        
    }; // class SpheroLocatorData
};  // namespace RobotBridge

#endif /* defined(__RobotVision__SpheroLocatorData__) */
