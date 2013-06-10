//
//  AuxDef.h
//  RobotVision
//
//  Created by Michael DePhillips on 3/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_AuxDef_h
#define RobotVision_AuxDef_h

//  The state of the Sphero tracking algorithm
typedef enum AUX_FIND_SPHERO_STATE
{
    AUX_NO_INFORMATION = 0,
    AUX_LOST,
    AUX_CONFUSED,
    AUX_TRACKING,
    AUX_TRACKING_POORLY
} AUX_FIND_SPHERO_STATE;

typedef enum AUX_CAMERA_MOTION_MODE
{
    AUX_SHARKY_CAMERA_MOTION_MODE,
    AUX_ZOMBIE_CAMERA_MOTION_MODE
}
AUX_CAMERA_MOTION_MODE;

#endif
