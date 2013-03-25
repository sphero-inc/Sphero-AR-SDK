//
//  Aux.h
//  DroneBattle2
//
//  Created by Fabrizio Polo on 3/5/12.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#ifndef DroneBattle2_Aux_h
#define DroneBattle2_Aux_h

#ifdef __cplusplus
extern "C" {
#endif

#include "Aure.h"

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


AU_ERROR auxInitAureWithAuFST3(AU_ENUM threadingMode,
                               au_time (*auGetTimeMethod)(void),
                               void (*requestSpherColorChangeMethod)(const au_scalar*const rgb),
                               AuPlatformConfiguration*const platform,
                               AUX_CAMERA_MOTION_MODE cameraMotionMode);
    
void auxInitAureImuToLookAtGround(void);

void auxInitCameraGeometryWithDefaults(void);

//////////////////////////////////////////////////////////////////////////////////////////////////////

//
//  Getters
//
    
void auxGetQMatrix(au_time time, AuQMatrix*const qmOut);
void auxGetCameraMatrix(au_time time, AuMatrix* matrixOut);     //  Same thing as prev but with square matrix

au_scalar auxGetSphero_x(au_time time);
au_scalar auxGetSphero_y(au_time time);
au_scalar auxGetSphero_z(au_time time);

au_scalar auxGetSphero_relx(au_time time);
au_scalar auxGetSphero_rely(au_time time);
au_scalar auxGetSphero_relz(au_time time);

au_scalar auxGetSphero_row(au_time time);
au_scalar auxGetSphero_col(au_time time);

//void (*auxGetSphero_rowCol)(au_time time, au_scalar* row, au_scalar* col);
//void (*auxGetSphero_rowColScale)(au_time time, au_scalar* row, au_scalar* col, au_scalar* scale);
//au_bool (*auxSpheroFinderIsLost)(void);
//void (*auxGetDebugImage)(AuImage*);

AUX_FIND_SPHERO_STATE auxGetTrackingState(void);

au_scalar auxGetSpheroDerivative_x(au_time time);
au_scalar auxGetSpheroDerivative_y(au_time time);


void auxGetSpheroLocatorPosition(au_time time, au_scalar*const xyzOut);
void auxGetSpheroLocatorVelocity(au_time time, au_scalar*const xyzOut);
au_scalar auxGetLocatorAlignmentAngle();
    
//au_scalar (*auxGetSpheroHeadingTare)(void);
float auxGetOdometerCalibrationError(void);

//void (*auxSetOdometerCalibrationGain)(float gain);
//void (*auxGetOdometerCalibrationValues)(float* angle, float* scale);
//void (*auxGetHeavilyFilteredCameraVelocity)(float* xyOut);

au_scalar auxGetDebugScalar(void);
    
AuColor auxGetLightColor(void);
    
au_scalar auxGetTrueRadiusOfSphero(void);
    
int auxGetLocatorCalibratingPutCount(void);
    
#pragma Settings

//
//      Settings
//

void auxSetShouldLockCamera(au_bool should);
void auxSetShouldLockCameraHeight(au_bool should);
void auxSetShouldForceLockCameraHeight(au_bool should);
void auxFlushLocatorCalibration();
void auxSetTrackingActive(au_bool shouldTrack);


//
//  Putters
//

void auxPutAllStateData(au_scalar* odometerPosition, au_scalar* odometerVelocity, au_scalar* deviceAccel, au_time time);


//////////////////////////////////////////////////////////////////////////////////////////////////////



//////////////////////////////////////////////////////////////////////////////////////////////////////

    
void auxReportColorOfSphero(au_scalar red, au_scalar green, au_scalar blue);

void auxPutSpheroPositionHint2d(const au_scalar row, const au_scalar col);

void auxEnterInspectionMode();
    
void auxLeaveInspectionMode();

    
#pragma mark Debug
    
void auxSetDebugVector(au_scalar*const data, int vectorLength);

    
    
#ifdef __cplusplus
}   //  End of extern "C"
#endif


#endif
