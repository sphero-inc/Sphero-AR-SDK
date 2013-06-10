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

typedef struct {
    void* motionManager;
    AUX_CAMERA_MOTION_MODE mode;
} PlatformParameters;

#endif
