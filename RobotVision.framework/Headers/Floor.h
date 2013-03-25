//
//  Floor.h
//  TestApp
//
//  Created by Michael DePhillips on 2/12/13.
//  Copyright (c) 2013 Orbotix, Inc All rights reserved.
//

#ifndef __RobotVision__Floor__
#define __RobotVision__Floor__

#include <iostream>

namespace RobotVision {
    
    class Floor {
        
    private:
        int color_;
        
    public:
        
        // Deconstructor
        virtual ~Floor();
        
        // Constructors
        Floor(): color_(0) {};
        Floor(const int color): color_(color) {};
        Floor(const Floor&);
        
        // Overloaded operator
        Floor& operator=(const Floor& floor);
        
        // Getters
        int color() const;
        
    };  // class Floor
    
} // namespace RobotVision

#endif /* defined(__RobotVision__Floor__) */
