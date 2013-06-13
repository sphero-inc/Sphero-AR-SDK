//
//  VirtualCamera.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/12/13.
//  Copyright (c) 2013 Michael DePhillips All rights reserved.
//

#ifndef __RobotVision__VirtualCamera__
#define __RobotVision__VirtualCamera__

#if defined(__cplusplus)

#include <iostream>
#include "ARImage.h"
#include "Pose.h"
#include "ARMacros.h"
#include "ARMemoryManaged.h"

namespace RobotVision {
    
    /*!
     * @brief A class that represents the data from the results of the vision engine
     *        It describes the position orientation of the device camera and the camera
     *        frame that was analyzed to produce this result.
     */
    class VirtualCamera : public RobotVision::ARMemoryManaged {
        
    private:
        DISALLOW_COPY_AND_ASSIGN(VirtualCamera);
        Pose* pose_;
        Pose* rawPose_;
        Pose* naturalPose_;
        ARImage* cameraFrame_;
        
    public:
        
        // Deconstructor
        virtual ~VirtualCamera();
        
        // Constructors
        VirtualCamera(Pose* pose, Pose* rawPose, Pose* naturalPose, ARImage* cameraFrame);
        
        // I don't like this one, commenting out to see if anything needs it
//        VirtualCamera(ARImage* cameraFrame):cameraFrame_(cameraFrame),
//                      pose_(), rawPose_(), naturalPose_(){};
        
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
         *  Returns the pose that has been changing to coordinate with the other objects
         *  in the ARResult
         */
        Pose* naturalPose() const;
        
        /*!
         *  Returns the image class that represents the image header, format, and
         *  actual pixel data of the camera frame
         */
        ARImage* cameraFrame();
        
    };  // class VirtualCamera
    
} // namespace RobotVision

#endif // C++
#endif /* defined(__RobotVision__VirtualCamera__) */
