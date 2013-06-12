//
//  AuPlatformConfigurationDef.h
//  RobotVision
//
//  Created by Michael DePhillips on 3/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_AuPlatformConfigurationDef_h
#define RobotVision_AuPlatformConfigurationDef_h

CLASS_BEGIN(AuDeviceCameraConfiguration)
    au_scalar near, width, height;
    int rows, cols;
//    AU_ENUM videoFormat;
CLASS_END

CLASS_BEGIN(AuDeviceDisplayConfiguration)
    int rows, cols;             //  Resolution of the actual output
    au_scalar width, height;    //  Ratio should match ratio of rows/cols
    au_scalar near;             //  Distance to the near plane (in pixels)
    au_scalar widthInMeters;    //  Width of the actual physical display (width <-> cols)
CLASS_END

CLASS_BEGIN(AuComputeConfiguration)
    int numberOfCores;
    au_scalar cpuPerformanceRatio;      //  iPhone4S := 1
CLASS_END

CLASS_BEGIN(AuPlatformConfiguration)
    //  Hardware Configuration
    AuDeviceCameraConfiguration camera;
    AuDeviceDisplayConfiguration display;

    //  Derived Data
    au_scalar vidRectTop, vidRectLeft, vidRectNear;             //  Video render rect
    au_scalar projFrustTop, projFrustLeft, projFrustNear;       //  Rendering projection frustum
    au_bool matchedWidth;                                       //  true->cropped height, false->cropped width
    au_scalar pixelsToNearPlaneMetersConversionFactor;

    //  Device dependent settings
    au_bool shouldUseManualFocusControl;
    AuComputeConfiguration compute;
CLASS_END

#endif
