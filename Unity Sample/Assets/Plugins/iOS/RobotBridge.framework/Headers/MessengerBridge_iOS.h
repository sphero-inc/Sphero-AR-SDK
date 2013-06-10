//
//  MessengerBridge_iOS.h
//  C++ Messenger Bridge
//
//  Created by Mike DePhillips on 3/4/13.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

extern "C" {
    typedef void (*ReceiveDeviceMessageCallback)(const char *);
}


@interface MessengerBridge_iOS : NSObject {
    BOOL robotInitialized;
    BOOL robotOnline;
    BOOL controllerStreamingOn;
    uint64_t currentMask;
}

@property  ReceiveDeviceMessageCallback receiveDeviceMessageCallback;

+(MessengerBridge_iOS*)sharedBridge;

-(void)connectToRobot;
-(BOOL)isRobotOnline;
-(void)updateStreamingWithMask:(const char*)maskStr;

@end
