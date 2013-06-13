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
#include "ARMacros.h"
#include "ARMemoryManaged.h"

namespace RobotVision {
    
    class ARResult : public RobotVision::ARMemoryManaged {
        
    private:

        DISALLOW_COPY_AND_ASSIGN(ARResult);
        VirtualEnvironment* environment_;
        VirtualSphero* sphero_;
        VirtualCamera* camera_;
        AREngineState engineState_;
        double time_;
        
        ARResult() {};
        
    public:
        
        // Deconstructor
        virtual ~ARResult();
        
        // Constructors
        ARResult(VirtualCamera* camera,
                 VirtualEnvironment* environment,
                 VirtualSphero* sphero,
                 AREngineState engineState,
                 double time);
        
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
         *  The time stamp associated with this data.
         */
        double time() const;

    };  // class ARResult
    
} // namespace RobotVision

#endif // C++
#endif /* defined(__RobotVision__ARResult__) */
