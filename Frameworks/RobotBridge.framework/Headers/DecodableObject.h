//
//  SerializableObject.h
//  RobotVision
//
//  Created by Michael DePhillips on 2/25/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_SerializableObject_h
#define RobotVision_SerializableObject_h

namespace RobotBridge {

    class SpheroDeviceMessageDecoder;
    
    class DecodableObject {
    public:
        DecodableObject() {}
        virtual ~DecodableObject() {}
        virtual DecodableObject* decode(SpheroDeviceMessageDecoder* decoder) = 0;
        
    }; // class DecodableObject
}; // namespace RobotBridge

#endif
