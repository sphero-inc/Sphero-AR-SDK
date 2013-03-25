//
//  ARResult.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/12/13.
//  Copyright (c) 2013 Mike D. All rights reserved.
//

#ifndef __RobotVision__ARResult__
#define __RobotVision__ARResult__

#if defined(__cplusplus)

#include "VirtualCamera.h"
#include "VirtualEnvironment.h"
#include "VirtualSphero.h"
#include "AuImage.h"
#include "ARMacros.h"

namespace RobotVision {
    
    class ARResult {
        
    private:

        DISALLOW_COPY_AND_ASSIGN(ARResult);
        VirtualEnvironment* environment_;
        VirtualSphero* sphero_;
        VirtualCamera* camera_;
        AREngineState engineState_;
        int refCount_;
        
    public:
        
        // Deconstructor
        virtual ~ARResult();
        
        // Constructors
        ARResult(): camera_(new VirtualCamera()),
                    environment_(new VirtualEnvironment()),
                    sphero_(new VirtualSphero()) {};
        
        ARResult(VirtualCamera* camera,
                 VirtualEnvironment* environment,
                 VirtualSphero* sphero,
                 AREngineState engineState);
        
        /*!
         *  Returns the data as result of the vision engine tracking relating to the camera
         */
        VirtualCamera* virtualCamera() const;
        /*!
         *  Returns the data as result of the vision engine tracking relating to
         *  the environment
         */
        VirtualEnvironment* virtualEnvironment() const;
        /*!
         *  Returns the data as result of the vision engine tracking relating to the
         *  Sphero's state
         */
        VirtualSphero* virtualSphero() const;
        /*!
         *  The state of the engine
         */
        AREngineState engineState() const;
        
        /*!
         *  Returns the reference count of this result, this object is deleted
         *  when it gets to zero
         */
        int refCount() {return refCount_; }
        
        /*!
         *  Increments the reference count, which retains the data in this class, 
         *  be sure to call this when you are passed this class and don't want its 
         *  memory to be deleted
         */
        void retain();
        /*!
         *  Decrements the reference count of this object. Be sure to call this method,
         *  when you are done using this object, or else you will get memory leaks and
         *  problems with camera frames.
         */
        void release();
        
    };  // class ARResult
    
} // namespace RobotVision

#endif // C++
#endif /* defined(__RobotVision__ARResult__) */
