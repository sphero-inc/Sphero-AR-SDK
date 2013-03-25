//
//  Aure.h
//  DroneBattle2
//
//  Created by Fabrizio Polo on 2/27/12.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#ifndef com_Orbotix_FabrizioPolo_Aure_Aure_h
#define com_Orbotix_FabrizioPolo_Aure_Aure_h

#ifdef __cplusplus
extern "C" {
#endif



#include <stdint.h>
#include <pthread.h>


#pragma mark Build Options
    

#define USE_DEBUG_BLOCKS
//#define USE_AURE_TESTS
    
#pragma mark Little Types
/*-------------------------------------------------------------------------------------------------*/
//
//      Little Types
//

typedef uint8_t au_byte;
typedef uint16_t au_ushort;
typedef uint32_t au_uint32;
typedef int au_bool;
typedef float au_scalar;
typedef double au_time;         //  au_time always represents time in seconds

typedef unsigned short int ushort;      //  DEPRECTATED


#define AU_FALSE 0
#define AU_TRUE 1

//
//      Little Type Utilities
//

au_time au_timeFromMillis(long millis);


#pragma mark Object Oriented C Macros
    
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



#pragma mark Enums

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
    
    AU_BAD_SOURCE_IMAGE_SIZE,
    AU_BAD_TARGET_IMAGE_SIZE,
    
    AU_INSUFFICIENT_SOURCE_IMAGE_MARGIN,
    AU_SOURCE_GEOMETRY_TOO_SMALL,
    
    AU_MEMORY_MANAGEMENT_OVER_RELEASE = 1100,
};
typedef enum _AU_ERROR AU_ERROR;

#pragma mark AuColor

/*-------------------------------------------------------------------------------------------------*/
//
//      AuColor
//

//  Class AuColor
//CLASS_DEF(AuColor);

typedef union {
    struct {
        au_scalar R, G, B;
    };
    au_scalar values[3];
} AuColor;
    
    
//
//          AuColor Methods
//

//  Use these together as a constructor.
int sizeof_AuColor();               //  Returns the size in bytes of an AuColor object.  (Use this for memory allocation).                                          
AuColor* init_AuColor(void* ptr, AU_ENUM format);    //  Initialize an AuColor object at the given location.
AuColor* new_AuColor(AU_ENUM format);                //  Call malloc and make a new AuColor instance

void AuColorSetBytes_RGB(AuColor*const color, const au_byte R, const au_byte G, const au_byte B);

//  WARNING: Curently works in RGB.  Future implementations of AuColor will fully be configurable.
au_byte AuColorGetByte_R(const AuColor*const col);
au_byte AuColorGetByte_G(const AuColor*const col);
au_byte AuColorGetByte_B(const AuColor*const col);


#pragma mark AuImage

/*-------------------------------------------------------------------------------------------------*/
//
//      AuImage
//

CLASS_DEF(AuImage)

//
//      AuImage Methods
//

//  Use these together as a constructor
int sizeof_AuImage(void);
AuImage* init_AuImage(void* ptr, AU_ENUM imageFormat);
AuImage* new_AuImage(AU_ENUM imageFormat);

//  The total size: img->data + size is a pointer to the memory immediatley after the image.
int AuImageGetSizeInBytes(const AuImage*const img);

//  Get a sub-image that knows its part of a bigger picture.
AU_ERROR AuImageGetSubImage(const AuImage*const img, const int startRow, const int startCol, const int rows, const int cols, AuImage*const subImage);

//  Get a sub-image that's unaware it's contained in anything larger.
AU_ERROR AuImageGetCroppedImage(const AuImage*const img, const int startRow, const int startCol, const int rows, const int cols, AuImage*const subImage);

//  Store a reference to your native image object type here, so you can refer to it in relinquishFrame(...)
void AuImageSetNativeImageObjectRef(AuImage*const img, const void*const ptr);
void* AuImageGetNativeImageObjectRef(const AuImage*const img);

//  Print a description of the image
void AuImageReport(const AuImage*const img);

#pragma mark AuVisionTask
/*-------------------------------------------------------------------------------------------------*/
//
//      AuVisionTask
//


CLASS_DEF(AuVisionTask)

    
    
#pragma mark -
#pragma mark Aure
/*-------------------------------------------------------------------------------------------------*/
//
//      Aure
//

//
//      Versioning
//

const char* auGetVersionString();


//
//      Initialization / Finalization
//

CLASS_DEF(AuDeviceCameraConfiguration)
CLASS_DEF(AuDeviceDisplayConfiguration)
CLASS_DEF(AuComputeConfiguration)
CLASS_DEF(AuPlatformConfiguration)

AU_ERROR auInitialize(AU_ENUM threadingMode, const AuPlatformConfiguration*const platformConfig);

AU_ERROR auFinalize(void);


//
//      Acquire/Relinquish frames
//

//  Specify a callback Aure will use to fetch new camera frames.
void auSetAcquireFrameMethod(AU_ERROR (*method)(AuImage** frame, au_time* time));

//  Specify a callback Aure will use to return new camera frames.
void auSetRelinquishFrameMethod(AU_ERROR (*method)(AuImage* frame));


//
//      Manage Vision Tasks
//

//  Add a new task
AU_ERROR auAddVisionTask(AuVisionTask* task);

//  Initialize the vision tasks
AU_ERROR auInitializeVisionTasks(au_time time);


//
//      Aure State Management
//

//  Push state changes into middle buffer so the next task uses them.
void auCommit(void);

//  Make most recent output data available to client query.
void auUpdate(void);


//
//      Work
//

//  Enter the vision loop (used only in multi-threading mode)
AU_ERROR auBegin(void);

//  Do a vision tastk.  (used only in single-threaded mode)
AU_ERROR auDoNextVisionTask(void);

//  Stop the vision thread from another thread (used in multi-threading mode).
AU_ERROR auStop(void);

//  Free memory etc.
AU_ERROR auQuit(void);

//  The number of vision tasks running currently.
int auGetNumOfActiveVisionTasks();


//
//      Retrieving Results
//

//  Specify a callback notifying the client of vision task completion
void auSetOnVisionTaskCompleteMethod(void (*method)(AuVisionTask* task, AuImage* img, au_time time, AU_ERROR err));

// Get FPS
au_scalar auGetTasksPerSecond(void);



//
//      IMU
//

CLASS_DEF(AuQMatrix);
CLASS_DEF(AuMatrix);
CLASS_DEF(AuCameraGeometry);

//  Feed the IMU with an external matrix
void auPutQMatrixAtTime(const AuQMatrix* matrix, au_time time);

//  Move the camera to the given coordinates.  
void auPutCameraPositionAtTime(au_scalar* xyz, au_time time);

//  Compute the curent camera position
void auGetCameraPosition(au_time time, au_scalar* xyz);

    
au_bool auGet3dFrom2dAs(AU_ENUM as, AU_ENUM matrixId, au_time time, au_scalar row, au_scalar col, au_scalar height, au_scalar* xyz);

void auGetScreenSpace3dFrom3dAs(AU_ENUM as, AU_ENUM matrixId, au_time time, const au_scalar* xyz, au_scalar* xyzOut);

//  Fetch the client opengl matrix in one go move.
void auGetOpenGLMatrix(au_time time, AuMatrix* matrixOut);

void auGetQMatrix(au_time time, AuQMatrix* matrixOut);

void auGetCameraMatrix(au_time, AuMatrix* matrixOut);

AuCameraGeometry* auGetCameraGeometry();

AuCameraGeometry* auGetDeviceCameraGeometry();
    
AuPlatformConfiguration* auGetPlatformConfiguration();


//
//      Logging
//

//  Specify a logging callback method
void auSetLoggingMethod(void (*auLogMethod)(char* message));

//  Log the message to output
void auLog(char* message);

//  "sprintf" style logging facility.
#define AU_LOG(args...)     { char AU_LOG_MSG[1000];    sprintf(AU_LOG_MSG, args);  auLog(AU_LOG_MSG);  }

//  Periodic logging: emit a log message every N times
#define AU_PLOG(N, args...) \
{   \
    static int AU_PLOG_COUNTER = 0; \
    AU_PLOG_COUNTER++;  \
    if (AU_PLOG_COUNTER == N)   \
    {   \
        char AU_LOG_MSG[1000];  \
        sprintf(AU_LOG_MSG, args);  \
        auLog(AU_LOG_MSG);  \
        AU_PLOG_COUNTER = 0;    \
    }   \
}


//  Log an error message
void auLogErr(AU_ERROR err, char* message);

#define AU_ERR(ERR, MSG)        \
{   \
    err = ERR;  \
    if (err != AU_SUCCESS) auLogErr(err, MSG);  \
}


//
//      Time
//

//  Tell Aure how to get the current time
void auSetGetTimeMethod(au_time (*auGetTimeMethod)(void));

//  Get the current time
au_time auGetTime(void);

    
//
//      Hardware Configuration
//

void auSetFocusCameraAtPointMethod(void (*auSetFocusCameraAtPointMethod)(const au_scalar row, const au_scalar col));

au_bool auSetCameraIsFocusingMethod(au_bool (*auCameraIsFocusingMethod)(void));
    
//
//      Macros
//

#define SQUARE(X) ((X)*(X))
#define CLAMP(X, A, B)      if (X < (A)) { X = A; } else if (X > (B)) { X = B; }


#pragma mark Debugging
//
//      Debugging
//

//  Lock the given mutex, but try it first and log failures.
void auDebugMutexLock(pthread_mutex_t* mutex, char* locationName);
int auDebugMutexUnlock(pthread_mutex_t* mutex);
    
#define DEBUG_MUTEX_LOCK(X, MSG)     auDebugMutexLock(X, MSG)

//  Thexe variants include curly brackets so you have to close them
//  and indention beahves correctly.
#define AU_MUTEX_LOCK(X, MSG)   { auDebugMutexLock(X, MSG);
#define AU_MUTEX_UNLOCK(X)      auDebugMutexUnlock(X); }
    
typedef enum AU_ASSERT_TAG_ENUM
{
    AU_DEFAULT_ASSERT_TAG = 0,
    AU_INTERFACE_ASSERT_TAG = 1,
    AU_PERF_CRIT_ASSERT_TAG = 2,
    
} AU_ASSERT_TAG;

void auAssert(int condition, AU_ASSERT_TAG tag, char* msg);


//  A General purpose assert macro
#define AU_ASSERT(CONDITION, args...)   \
{   \
    char AU_ASSERT_MSG[1000];   \
    sprintf(AU_ASSERT_MSG, args);   \
    auAssert(CONDITION, AU_DEFAULT_ASSERT_TAG, AU_ASSERT_MSG); \
}

//  An assert macro for interface code
#define AU_ASSERT_I(CONDITION, args...)   \
{   \
    char AU_ASSERT_MSG[1000];   \
    sprintf(AU_ASSERT_MSG, args);   \
    auAssert(CONDITION, AU_INTERFACE_ASSERT_TAG, AU_ASSERT_MSG); \
}

//  An assert macro reserved for performance critical code so it can be shut off independently
#define AU_ASSERT_P(CONDITION, args...)   \
{   \
    char AU_ASSERT_MSG[1000];   \
    sprintf(AU_ASSERT_MSG, args);   \
    auAssert(CONDITION, AU_PERF_CRIT_ASSERT_TAG, AU_ASSERT_MSG); \
}
    
//  Automatically fail
#define AU_FAIL(MSG)    AU_ASSERT(AU_FALSE, MSG)

//  Code blocks we can turn on/off
#ifdef USE_DEBUG_BLOCKS
#   define AU_DEBUG(CODE_BLOCK) { CODE_BLOCK }
#else
#   define AU_DEBUG(CODE_BLOCK)
#endif
    
//  Record where we've stopped work.
#define AU_STUB         //  An empty function
#define AU_CURSOR       //  Work is happening here
  
    
#pragma mark Exceptions

//
//      Faux Exceptions
//

//  Requires a mutable variable "err" of type AU_ERROR.

#define AU_TRY(TRY_ME)  \
{   \
    err = TRY_ME;   \
    switch (err)    \
    {                       \
        case AU_SUCCESS:    \
            break;          \

#define AU_RETHROW(DO_ME_FIRST_CODE)  \
        default:                \
            DO_ME_FIRST_CODE    \
            return err;         \
    }   \
}

#define AU_IGNORE_ERROR }}

#define AU_CATCH(AU_ERROR_ENUM_VALUE, CATCH_BLOCK_CODE)    \
        case AU_ERROR_ENUM_VALUE:       \
            CATCH_BLOCK_CODE            \
            break;                  \

#define AU_CATCH_ALL(CATCH_BLOCK_CODE)  \
        default:                \
            CATCH_BLOCK_CODE    \
    }   \
}

#define AU_TRY_OR_RETHROW(TRY_ME)  \
    AU_TRY(TRY_ME)      \
    AU_RETHROW()

    
    
    
//  Iteration.
    
#define FOR_EACH(ITEM_TYPE, VAR, ARRAY, LENGTH) \
    for (int VAR##_idx = 0; VAR##_idx < LENGTH; VAR##_idx++) {     \
    ITEM_TYPE VAR = ARRAY[ VAR##_idx ];
    
#define NEXT    }
    
    
    
#ifdef __cplusplus
}   //  End of extern "C"
#endif

        
#endif
























