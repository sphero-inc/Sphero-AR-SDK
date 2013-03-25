//
//  Light.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/12/13.
//  Copyright (c) 2013 Orbotix, Inc All rights reserved.
//

#ifndef __RobotVision__Light__
#define __RobotVision__Light__

#include <iostream>

namespace RobotVision {
    
    class Light {
        
    private:
        int color_;
        int direction_;  // so far unimplemented
        
    public:
        
        // Deconstructor
        virtual ~Light();
        
        // Constructors
        Light(): color_(0), direction_(0) {};
        Light(const int color): color_(color), direction_(0) {};
        Light(const Light& light);
        
        // Overloaded operator
        Light& operator=(const Light& light);
        
        // Getters
        int color() const;
        
    };  // class Light
    
} // namespace RobotVision

#endif /* defined(__RobotVision__Light__) */
