//
//  SpheroThreeAxisSensor.h
//  RobotBridge
//
//  Created by Michael DePhillips on 2/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_SpheroThreeAxisSensor_h
#define RobotVision_SpheroThreeAxisSensor_h

namespace RobotBridge {

    struct SpheroThreeAxisSensor {
        float x;
        float y;
        float z;
        
        SpheroThreeAxisSensor():x(0),y(0),z(0){};
        SpheroThreeAxisSensor(float x_, float y_, float z_):
        x(x_), y(y_), z(z_) {};
    };
    
    struct SpheroTwoAxisSensor {
        float x;
        float y;
        
        SpheroTwoAxisSensor():x(0),y(0){};
        SpheroTwoAxisSensor(float x_, float y_):
        x(x_), y(y_) {};
    }; 
    
}; // namespace RobotBridge

#endif
