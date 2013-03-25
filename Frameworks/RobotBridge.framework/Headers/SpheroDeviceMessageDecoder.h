//
//  SpheroDeviceMessageDecoder.h
//  RobotBridge
//
//  Created by Michael DePhillips on 2/21/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef __RobotBridge__SpheroDeviceMessageDecoder__
#define __RobotBridge__SpheroDeviceMessageDecoder__

#include <iostream>
#include <string>
#include <map>
#include <set>
#include "json.h"
#include "SpheroDeviceMessage.h"
#include "DecodableObject.h"

namespace RobotBridge {
    
    class SpheroDeviceMessageDecoder {
        
        public:
            static SpheroDeviceMessage* messageFromEncodedString(const char* msg);
        
            SpheroDeviceMessageDecoder();
            SpheroDeviceMessageDecoder(const char* msg);
            virtual ~SpheroDeviceMessageDecoder();
        
            SpheroDeviceMessageDecoder(const SpheroDeviceMessageDecoder& decoder);
            SpheroDeviceMessageDecoder& operator=(const SpheroDeviceMessageDecoder& decoder);
        
            SpheroDeviceMessage* createDeviceMessage();
            DecodableObject* createDecodableObject();
        
            // Decode functions
            uint64_t                    decodeUInt64(std::string key);
            int64_t                     decodeInt64(std::string key);
            int                         decodeInt(std::string key);
            int32_t                     decodeInt32(std::string key);
            float                       decodeFloat(std::string key);
            void                        decodeString(std::string key, std::string& retVal);
            DecodableObject*            decodeObject(std::string key);
            std::set<DecodableObject*>  decodeObjectSet(std::string key);
        
        private:
            Json::Value jsonObject;
            std::map<std::string,SpheroDeviceMessage*> deviceMessageClasses_;
            std::map<std::string,DecodableObject*> decodableMessageClasses_;
            void initializeMaps();
        
            SpheroDeviceMessageDecoder(Json::Value value);
    };
};

#endif /* defined(__RobotBridge__SpheroDeviceMessageDecoder__) */
