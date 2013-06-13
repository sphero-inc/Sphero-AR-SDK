//
//  PlatformParameters.h
//  RobotVision
//
//  Created by Brandon Dorris on 2/19/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_PlatformParameters_h
#define RobotVision_PlatformParameters_h

#include "AuxDef.h"

namespace RobotVision {

typedef struct {
    void* motionManager;
    AUX_CAMERA_MOTION_MODE mode;
    void* queue;
} PlatformParameters;

typedef enum  ARCameraMode
{
    CAMERA_MODE_ALLOW_VERTICAL_MOVEMENT = 0,
    CAMERA_MODE_STATIC_HEIGHT
} ARCameraMode;
    
}; // namespace RobotVision

#endif
