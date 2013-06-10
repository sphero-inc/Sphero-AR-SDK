//
//  SpheroQuaternionData.h
//  RobotBridge
//
//  Created by Michael DePhillips on 2/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotVision__SpheroQuaternionData__
#define __RobotVision__SpheroQuaternionData__

#include <iostream>
#include "SpheroDeviceMessageDecoder.h"

namespace RobotBridge {
    
    class SpheroQuaternionData : public DecodableObject {
        
    public:
        
        SpheroQuaternionData(): w_(0), x_(0), y_(0), z_(0) {};
        SpheroQuaternionData(float w, float x, float y, float z):
                             w_(w), x_(x), y_(y), z_(z) {};
        virtual ~SpheroQuaternionData();
        
        SpheroQuaternionData(const SpheroQuaternionData& data);
        SpheroQuaternionData& operator=(const SpheroQuaternionData& data);
        
        DecodableObject* decode(SpheroDeviceMessageDecoder* decoder);
        
        float w();
        float x();
        float y();
        float z();
    
    private:
        float w_;
        float x_;
        float y_;
        float z_;
    
        
    }; // class SpheroQuaternionData
};  // namespace RobotBridge

#endif /* defined(__RobotVision__SpheroQuaternionData__) */
