//
//  SpheroAttitudeData.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotVision__SpheroAttitudeData__
#define __RobotVision__SpheroAttitudeData__

#include <iostream>
#include "SpheroDeviceMessageDecoder.h"

namespace RobotBridge {

    class SpheroAttitudeData : public DecodableObject {
        
    public:
        
        SpheroAttitudeData(): roll_(0), pitch_(0), yaw_(0) {};
        SpheroAttitudeData(float roll, float pitch, float yaw):
        roll_(roll), pitch_(pitch), yaw_(yaw) {};
        virtual ~SpheroAttitudeData();
        
        SpheroAttitudeData(const SpheroAttitudeData& data);
        SpheroAttitudeData& operator=(const SpheroAttitudeData& data);
        
        DecodableObject* decode(SpheroDeviceMessageDecoder* decoder);
        
        float roll();
        float pitch();
        float yaw();
        
    private:
        float roll_;
        float pitch_;
        float yaw_;
        
    }; // class SpheroQuaternionData
};  // namespace RobotBridge

#endif /* defined(__RobotVision__SpheroAttitudeData__) */
