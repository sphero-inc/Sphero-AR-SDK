//
//  SpheroBackEMFData.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotVision__SpheroBackEMFData__
#define __RobotVision__SpheroBackEMFData__

#include <iostream>
#include "SpheroDeviceMessageDecoder.h"

namespace RobotBridge {

    struct Motor {
        int left;
        int right;
        
        Motor(): left(0), right(0) {};
        Motor(int left_, int right_): left(left_), right(right_) {};
    };
    
    class SpheroBackEMFData : public DecodableObject {
        
    public:
        
        SpheroBackEMFData(): rawMotor_(new Motor()), normalizedMotor_(new Motor()) {};
        SpheroBackEMFData(Motor* raw, Motor* normalized):
                          rawMotor_(raw), normalizedMotor_(normalized) {};
        virtual ~SpheroBackEMFData();
        
        SpheroBackEMFData(const SpheroBackEMFData& data);
        SpheroBackEMFData& operator=(const SpheroBackEMFData& data);
        
        DecodableObject* decode(SpheroDeviceMessageDecoder* decoder);
        
        Motor* rawMotor();
        Motor* normalizedMotor();
        
    private:
        Motor* rawMotor_;
        Motor* normalizedMotor_;
        
    }; // class SpheroBackEMFData
};  // namespace RobotBridge

#endif /* defined(__RobotVision__SpheroBackEMFData__) */
