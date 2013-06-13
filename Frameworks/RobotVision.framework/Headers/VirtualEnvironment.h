//
//  VirtualEnvironment.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/12/13.
//  Copyright (c) 2013 Orbotix, Inc All rights reserved.
//

#ifndef __RobotVision__VirtualEnvironment__
#define __RobotVision__VirtualEnvironment__

#if defined(__cplusplus)

#include <iostream>
#include "Floor.h"
#include "Light.h"
#include "ARMemoryManaged.h"

namespace RobotVision {
    
    /*!
     * @brief A class that represents the data from the results of the vision engine
     *        It describes the environment of the ARResult.  For example, the color
     *        of the lighting, or the color of the floor.
     */
    class VirtualEnvironment : public ARMemoryManaged {
        
    private:
        Floor* floor_;
        Light* light_;

    public:
        
        // Deconstructor
        virtual ~VirtualEnvironment();
        
        // Constructors
        VirtualEnvironment(Floor* floor, Light* light):
                           floor_(floor), light_(light) {};
        
        VirtualEnvironment(const VirtualEnvironment& env);
        VirtualEnvironment& operator=(const VirtualEnvironment& env);
        
        /*!
         * NOT IMPLEMENTED YET
         * Returns the Floor object that represents color for the floor
         */
        Floor* floor() const;
        /*!
         * NOT IMPLEMENTED YET
         * Returns the Light object that represents color for the floor
         */
        Light* light() const;
        
    };  // class VirtualEnvironment
    
} // namespace RobotVision

#endif // C++
#endif /* defined(__RobotVision__VirtualEnvironment__) */
