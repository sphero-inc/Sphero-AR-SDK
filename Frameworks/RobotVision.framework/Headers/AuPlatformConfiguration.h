//
//  AuPlatformConfiguration.h
//  RobotVision
//
//  Created by Fabrizio Polo on 10/18/12.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_AuPlatformConfiguration_h
#define RobotVision_AuPlatformConfiguration_h

#include "Aure.h"

//CLASS_DEF(AuCameraGeometry);

//  Actual numbers to the right.  Adjusted values for our purposes are defined.
#define AU_COMPUTE_A5_800_RATIO             1.0f
#define AU_COMPUTE_A5_800_24hz_RATIO        (AU_COMPUTE_A5_800_RATIO * 30.0f / 24.0f)   //  More CPU time at lower hz
#define AU_COMPUTE_A5_1000_RATIO            1.0f    //  1.2f
#define AU_COMPUTE_A5X_RATIO                1.0f    //  1.4f
#define AU_COMPUTE_A6_RATIO                 1.4f    //  2.0f
#define AU_COMPUTE_A6X_RATIO                1.4f    //  2.0f

#define AU_DEVICE_DISPLAY_DEFAULT_NEAR_DISTANCE         0.05f
#define AU_DEVICE_DISPLAY_DEFAULT_BILLBOARD_DISTANCE    1200.0f

#pragma mark AuDeviceCameraConfiguration


CLASS_BEGIN(AuDeviceCameraConfiguration)
    au_scalar near, width, height;
    int rows, cols;
CLASS_END

void AuDeviceCameraConfigurationGetCameraGeometry(const AuDeviceCameraConfiguration*const camConfig, AuCameraGeometry*const camGeoemtryOut);

#pragma mark AuDeviceDisplayConfiguration


CLASS_BEGIN(AuDeviceDisplayConfiguration)
    int rows, cols;             //  Resolution of the actual output
    au_scalar width, height;    //  Ratio should match ratio of rows/cols
    au_scalar near;             //  Distance to the near plane (in pixels)
    au_scalar widthInMeters;    //  Width of the actual physical display (width <-> cols)
CLASS_END

void AuDeviceDisplayConfigurationGetCameraGeometry(const AuDeviceDisplayConfiguration*const displayConfig, AuCameraGeometry*const screenGeometryOut);

#pragma mark AuComputeConfiguration

CLASS_BEGIN(AuComputeConfiguration)
    int numberOfCores;
    au_scalar cpuPerformanceRatio;      //  iPhone4S := 1
CLASS_END

#pragma mark AuPlatformConfiguration


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

void AuPlatformConfigurationInitWithDefaultsFromDeviceName(AuPlatformConfiguration*const platformConfig, const char*const deviceName);

void AuPlatformConfigurationAutoFitDisplayToCamera(AuPlatformConfiguration*const platformConfig, int rows, int cols,
                                                   au_scalar widthInMeters, au_scalar billboardDepth, au_scalar projFrustNear);

au_scalar AuPlatformConfigurationGetNearPlaneMetersFromCameraImagePixels(AuPlatformConfiguration*const platformConfig,
                                                                         au_scalar camPixels,
                                                                         AuImage*const uncroppedSourceFrame);
#endif
