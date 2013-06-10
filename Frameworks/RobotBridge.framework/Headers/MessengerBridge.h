//
//  MessengerBridge.h
//  RobotBridge
//
//  Created by Brian Smith on 2/22/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotBridge_MessengerBridge_H
#define RobotBridge_MessengerBridge_H

#include <map>
#include "SpheroDeviceMessenger.h"

extern "C" {
	typedef void (*ReceivedMessageCallback)(const char *);
	typedef void (*SendMessageCallback)(const char *);
    typedef void (*UpdateMaskCallback)(const char *);
    extern void RegisterReceivedMessageCallback(ReceivedMessageCallback callback, const char* mask);
    extern void SetUpdateMaskCallback(const char *);
}

namespace RobotBridge {

class MessengerBridge {

public:
	static MessengerBridge& sharedInstance();

	void registerReceivedMessageCallback(
         ReceivedMessageCallback callback, unsigned long long mask);
	void setSendMessageCallback(SendMessageCallback callback);
    void setUpdateMaskCallback(UpdateMaskCallback callback);

	void broadcastReceivedMessage(const char* serializedMessage);
    void broadcastReceivedMessage(SpheroDeviceMessage* message);
	void sendMessage(const char* serializedMessage);
    
    void updateDataStreamingMask(const char* maskStr);

private:
    std::map<ReceivedMessageCallback, unsigned long long> receivedMessageCallbacks;
	SendMessageCallback sendMessageCallback;
    UpdateMaskCallback updateMaskCallback;
    
	static MessengerBridge instance;

	MessengerBridge();
	~MessengerBridge();

	MessengerBridge(const MessengerBridge&);
	MessengerBridge& operator=(const MessengerBridge&);
    
}; // class MessengerBridge

#endif // RobotBridge_MessengerBridge_H

}