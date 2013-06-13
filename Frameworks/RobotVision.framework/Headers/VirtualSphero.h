//
//  VirtualSphero.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/12/13.
//  Copyright (c) 2013 Mike D. All rights reserved.
//

#ifndef __RobotVision__VirtualSphero__
#define __RobotVision__VirtualSphero__
#if defined(__cplusplus)

#include <iostream>
#include "Pose.h"
#include "ARStates.h"
#include "ARMemoryManaged.h"

namespace RobotVision {
    
    /*!
     *  This struct represents a 3D velocity of the Sphero in conjuction with Vision Engine
     */
    struct ARSpheroVelocity {
        float x;
        float y;
        float z;
    };
    
    /*!
     * @brief A class that represents the data from the results of the vision engine
     *        It describes the position orientation of the Sphero and tracking state
     */
    class VirtualSphero : public RobotVision::ARMemoryManaged {
        
    private:
        Pose* pose_;
        Pose* rawPose_;
        Pose* naturalPose_;
        ARTrackingState state_;
        ARSpheroVelocity velocity_;
        int calibrationPutCount_;
        float locatorAlignmentAngle_;
        double time_;
        
    public:
        
        // Deconstructor
        virtual ~VirtualSphero();
        
        // Constructors
        VirtualSphero(Pose* pose, Pose* rawPose, Pose* naturalPose, ARTrackingState trackingState,
                      ARSpheroVelocity velocity, float locatorAlignmentAngle, int calibrationPutCount) :
                      pose_(pose), rawPose_(rawPose), naturalPose_(naturalPose),
                      state_(trackingState), velocity_(velocity),
                      locatorAlignmentAngle_(locatorAlignmentAngle),
                      calibrationPutCount_(calibrationPutCount) {};
        
        VirtualSphero(const VirtualSphero& env);
        
        // Overloaded operator
        VirtualSphero& operator=(const VirtualSphero& env);
        
        /*!
         *  Returns either the raw pose or the natural pose depending on your preference
         */
        Pose* pose() const;
        /*!
         *  Returns the raw pose that has not been changed to coordinate with the other
         *  objects in the ARResult
         */
        Pose* rawPose() const;
        /*!
         *  Returns the raw pose that has been changed to coordinate with the other
         *  objects in the ARResult
         */
        Pose* naturalPose() const;
        /*!
         *  Returns the state of finding the Sphero
         */
        ARTrackingState trackingState() const;
        /*!
         *  Returns the velocity that the Sphero is traveling
         */
        ARSpheroVelocity velocity() const;
        /*
         * The suggest calibration angle that the 3D model should point
         * Determined through the locator position changing
         */
        float locatorAlignmentAngle() const;
        /*
         * The number of times the AREnging has a good estimate of calibration heading
         * If this is greater than 2, you could probably use locatorAlignmentAngle()
         */
        int calibrationPutCount() const;

        
    };  // class VirtualSphero
    
} // namespace RobotVision

#endif // C++
#endif /* defined(__RobotVision__VirtualSphero__) */
