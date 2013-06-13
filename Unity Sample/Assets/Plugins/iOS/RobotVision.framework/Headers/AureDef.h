//
//  AureDef.h
//  RobotVision
//
//  Created by Michael DePhillips on 3/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_AureDef_h
#define RobotVision_AureDef_h

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

//
//      Little Type Utilities
//
typedef uint8_t au_byte;
typedef uint16_t au_ushort;
typedef uint32_t au_uint32;
typedef int au_bool;
typedef float au_scalar;
typedef double au_time;         //  au_time always represents time in seconds

typedef unsigned short int ushort;      //  DEPRECTATED

/*-------------------------------------------------------------------------------------------------*/
//
//      Macros for OO programming in C.
//
#define CLASS_DEF(CLASS_NAME) struct CLASS_NAME##Struct; typedef struct CLASS_NAME##Struct CLASS_NAME;
#define CLASS_BEGIN(CLASS_NAME) struct CLASS_NAME##Struct {
#define CLASS_END };

//  Create/cast to make a "this" keyword.
#define SYNTH_THIS(CLASS_NAME, PTR)  CLASS_NAME*const this = (CLASS_NAME*) PTR;

//  Declare and create sizeof_ methods
#define DECLARE_SIZEOF(CLASS_NAME)  int sizeof_##CLASS_NAME(void);
#define SYNTH_SIZEOF(CLASS_NAME)    int sizeof_##CLASS_NAME(void) { return sizeof(CLASS_NAME);  }


/*-------------------------------------------------------------------------------------------------*/
//
//      Enums
//
//  Various properties, paramters, etc. used by Aure.
enum _AU_ENUM
{
    //  Dimensionality
    AU_2D = 2,
    AU_3D = 3,
    AU_4D = 4,
    
    
    //  Threading modes.
    AU_SINGLE_THREADED = 100,
    AU_MULTI_THREADED,
    
    //  Matrices
    AU_EXTERNAL_IMU_MATRIX = 200,
    AU_OPENGL_MATRIX,
    
    //  Identify the caller as the client, or part of Aure.
    AU_CLIENT = 300,
    AU_AURE,
    
    //  Color formats
    AU_COLOR_RGB_888 = 1000,
    AU_COLOR_BGR_888,
    //AU_2VUY_16
    
    //  Image formats
    AU_RGBX_32 = 2000,
    AU_BGRX_32,
    AU_2VUY_16,
    AU_RGB_24,
    AU_YUV420_PLANAR,
    
    AU_GRAY_16,
    AU_GRAY_32
    
};
typedef enum _AU_ENUM AU_ENUM;


//  Error codes returned by Aure
enum _AU_ERROR
{
    AU_SUCCESS = 0,
    AU_FAILURE = 1,
    
    AU_OUT_OF_MEMORY = 50,
    
    AU_OVERFLOW = 100,
    AU_MULTIPLE_INITIIALIZATION,
    AU_UNNECESSARY_FINALIZATION,
    AU_METHOD_UNAVAILABLE_WHILE_RUNNING,
    AU_METHOD_REQUIRES_MULTI_THREADED_MODE,
    AU_METHOD_REQUIRES_SINGLE_THREADED_MODE,
    AU_INVALID_THREADING_MODE,
    
    AU_ACQUIRE_IMAGE_FAILED = 200,
    AU_RELINQUISH_IMAGE_FAILED,
    AU_IMAGE_ALREADY_PROCESSED,
    
    AU_BAD_SOURCE_IMAGE_FORMAT = 1000,
    AU_BAD_TARGET_IMAGE_FORMAT,
    
    AU_BAD_COMPRESSION_FACTOR,
    
    AU_BAD_SOURCE_IMAGE_SIZE,
    AU_BAD_TARGET_IMAGE_SIZE,
    
    AU_INSUFFICIENT_SOURCE_IMAGE_MARGIN,
    AU_SOURCE_GEOMETRY_TOO_SMALL,
    
    AU_MEMORY_MANAGEMENT_OVER_RELEASE = 1100,
};
typedef enum _AU_ERROR AU_ERROR;

/*-------------------------------------------------------------------------------------------------*/
//
//      AuVisionTask
//

CLASS_DEF(AuVisionTask)

//
//      Initialization / Finalization
//

CLASS_DEF(AuDeviceCameraConfiguration)
CLASS_DEF(AuDeviceDisplayConfiguration)
CLASS_DEF(AuComputeConfiguration)
CLASS_DEF(AuPlatformConfiguration)

CLASS_DEF(AuQMatrix);
CLASS_DEF(AuMatrix);
CLASS_DEF(AuCameraGeometry);

//  Class AuColor
#pragma mark AuColor

/*-------------------------------------------------------------------------------------------------*/
//
//      AuImage
//

CLASS_DEF(AuImage)

/*-------------------------------------------------------------------------------------------------*/
//
//      AuColor
//

typedef union {
    struct {
        au_scalar R, G, B;
    };
    struct {
        au_scalar Y, U, V;
    };
    au_scalar values[3];
} AuColor;

#ifdef __cplusplus
}   //  End of extern "C"
#endif

#endif
