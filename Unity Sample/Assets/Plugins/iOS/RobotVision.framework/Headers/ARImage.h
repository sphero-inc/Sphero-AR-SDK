//
//  ARImage.h
//  RobotVision
//
//  Created by Michael DePhillips on 3/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotVision__ARImage__
#define __RobotVision__ARImage__

#if defined(__cplusplus)

#include <iostream>
#include "AureDef.h"
#include "ARMacros.h"
#include "ARMemoryManaged.h"

namespace RobotVision {
    
    /*!
     * @brief A class that represents the camera frame of the ARResult
     *        It contains all the information you need to do an OpenGL call to
     *        draw the image as a 2D texture.
     */
    class ARImage : public RobotVision::ARMemoryManaged {
        
    private:
        DISALLOW_COPY_AND_ASSIGN(ARImage);
        AuImage* auImage_;
        
    public:
        
        // Deconstructor
        virtual ~ARImage();
                
        // Constructors
        ARImage(AuImage* image);
        
        /*!
         *  Returns the width of the image
         */
        int width() const;
        /*!
         *  Returns the height of the image
         */
        int height() const;
        /*!
         *  Returns the stride of the image (bytes per row of data)
         */
        int stride() const;
        /*!
         *  Returns the bytes of data of the image
         */
        unsigned char* data() const;
        
    };  // class ARImage
    
} // namespace RobotVision

#endif // C++
#endif /* defined(__RobotVision__ARImage__) */
