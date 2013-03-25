//
//  TrackingState.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/12/13.
//  Copyright (c) 2013 Orbotix, Inc All rights reserved.
//

#ifndef __RobotVision__TrackingState__
#define __RobotVision__TrackingState__

#include <iostream>

namespace RobotVision {
    
    typedef enum ARTrackingState
    {
        TRACKING_STATE_NO_INFORMATION = 0,
        TRACKING_STATE_LOST,
        TRACKING_STATE_CONFUSED,
        TRACKING_STATE_TRACKING,
        TRACKING_STATE_TRACKING_POORLY
    } TrackingState;
    
    typedef enum AREngineState
    {
        ENGINE_STATE_OFF = 0,
        ENGINE_STATE_ON
    } EngineState;
    
} // namespace RobotVision

#endif /* defined(__RobotVision__TrackingState__) */
