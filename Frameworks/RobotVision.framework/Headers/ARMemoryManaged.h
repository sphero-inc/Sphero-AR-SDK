//
//  ARMemoryManaged.h
//  RobotVision
//
//  Created by Brandon Dorris on 5/27/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_ARMemoryManaged_h
#define RobotVision_ARMemoryManaged_h

namespace RobotVision {

class ARMemoryManaged {
    int refCount_;
    
public:
    ARMemoryManaged() : refCount_(1) {}
    virtual ~ARMemoryManaged() {}
    
    /*!
     *  Increments the reference count, which retains the data in this class,
     *  be sure to call this when you are passed this class and don't want its
     *  memory to be deleted
     */
    inline void retain() { refCount_++; }
    
    /*!
     *  Decrements the reference count of this object. Be sure to call this method,
     *  when you are done using this object, or else you will get memory leaks and
     *  problems with camera frames.
     */
    inline void release() {
        refCount_--;
        // == here so it doesn't happen twice
        if (refCount_ == 0) {
            delete(this);
        }
    }
    
    /*!
     *  Returns the reference count of this result, this object is deleted
     *  when it gets to zero
     */
    inline int refCount() { return refCount_; }
};
    
} // namespace RobotVision

#endif
