//
//  SpheroDeviceSensorsData.h
//  RobotBridge
//
//  Created by Michael DePhillips on 2/21/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotVision__SpheroDeviceSensorsData__
#define __RobotVision__SpheroDeviceSensorsData__

#include <iostream>
#include "SpheroDeviceMessage.h"
#include "json.h"
#include "DecodableObject.h"
#include "SpheroAccelerometerData.h"
#include "SpheroAttitudeData.h"
#include "SpheroBackEMFData.h"
#include "SpheroGyroData.h"
#include "SpheroLocatorData.h"
#include "SpheroQuaternionData.h"

namespace RobotBridge {
    
    class SpheroDeviceSensorsData : public DecodableObject {
    public:
        
        SpheroDeviceSensorsData() : accelerometerData_  (new SpheroAccelerometerData()),
                                    attitudeData_       (new SpheroAttitudeData()),
                                    backEMFData_        (new SpheroBackEMFData()),
                                    gyroData_           (new SpheroGyroData()),
                                    locatorData_        (new SpheroLocatorData()),
                                    quaternionData_     (new SpheroQuaternionData()) {};
        
        SpheroDeviceSensorsData(SpheroAccelerometerData* accelerometerData,
                                SpheroAttitudeData*      attitudeData,
                                SpheroBackEMFData*       backEMFData,
                                SpheroGyroData*          gyroData,
                                SpheroLocatorData*       locatorData,
                                SpheroQuaternionData*    quaternionData) :
                                accelerometerData_       (accelerometerData),
                                attitudeData_            (attitudeData),
                                backEMFData_             (backEMFData),
                                gyroData_                (gyroData),
                                locatorData_             (locatorData),
                                quaternionData_          (quaternionData) {};
        
        virtual ~SpheroDeviceSensorsData();
        
        SpheroDeviceSensorsData(const SpheroDeviceSensorsData& data);
        SpheroDeviceSensorsData& operator=(const SpheroDeviceSensorsData& data);
        
        DecodableObject* decode(SpheroDeviceMessageDecoder* decoder);
        
        SpheroAccelerometerData* accelerometerData();
        SpheroAttitudeData*      attitudeData();
        SpheroBackEMFData*       backEMFData();
        SpheroGyroData*          gyroData();
        SpheroLocatorData*       locatorData();
        SpheroQuaternionData*    quaternionData();
        
    private:
        SpheroAccelerometerData* accelerometerData_;
        SpheroAttitudeData*      attitudeData_;
        SpheroBackEMFData*       backEMFData_;
        SpheroGyroData*          gyroData_;
        SpheroLocatorData*       locatorData_;
        SpheroQuaternionData*    quaternionData_;
                                                    
    };
};

#endif /* defined(__RobotVision__SpheroDeviceSensorsData__) */
