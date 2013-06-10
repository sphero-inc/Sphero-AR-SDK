//
//  Pose.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/6/13.
//  Copyright (c) 2013 Orbotix, Inc All rights reserved.
//

#ifndef __RobotVision__Pose__
#define __RobotVision__Pose__

#include "AureDef.h"

namespace RobotVision {
    
    /*!
     *  This struct represents a 3D position of an object
     */
    struct ARPosition {
        float x;
        float y;
        float z;
    };
    
    /*!
     *  This struct represents a an orientation of an object by a quaternion
     */
    struct AROrientation {
        float w;
        float x;
        float y;
        float z;
    };
    
    /*!
     *  This struct represents the position and orientation of the object in one struct
     */
    struct ARQMatrix {
        ARPosition position;
        AROrientation orientation;
    };
    
    /*!
     *  This struct represents a 4x4 OpenGL matrix that represents both
     *  position and orientation.
     */
    struct ARMatrix {
        float m11;
        float m12;
        float m13;
        float m14;
        float m21;
        float m22;
        float m23;
        float m24;
        float m31;
        float m32;
        float m33;
        float m34;
        float m41;
        float m42;
        float m43;
        float m44;
    };
    
    /*!
     * @brief A class that represents the position and orientation of an object
     */
    class Pose {
        
    private:
        ARPosition position_;
        AROrientation orientation_;
        ARMatrix matrix_;
        ARQMatrix qMatrix_;
        
    public:
        
        // Deconstructor
        virtual ~Pose();
        
        // Constructors
        Pose();
        Pose(const AuQMatrix quaternion, const AuMatrix matrix);
        Pose(const Pose&);
        Pose(const ARPosition&);

        // Overloaded operator
        Pose& operator=(const Pose& pose);
        
        /*!
         *  Returns the position of the object this is a child of
         */
        ARPosition    position()   const;
        /*!
         *  Returns the quaternion of the object this is a child of
         */
        AROrientation quaternion() const;
        /*!
         *  Returns the position and orientation of the pose in one structure
         */
        ARQMatrix     qMatrix()    const;
        /*!
         *  Returns the OpenGL drawing matrix of the object this is a child of
         */
        ARMatrix      matrix()     const;
        
    };  // class Pose
    
}; // namespace RobotVision

#endif // defined(__RobotVision__Pose__)
