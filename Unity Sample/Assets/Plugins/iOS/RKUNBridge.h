//
//  RKUNBridge.h
//  Unity-iPhone
//
//  Created by Jon Carroll on 6/4/12.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#include <RobotBridge/SpheroDeviceMessage.h>
#include <RobotBridge/SpheroDeviceMessageDelegate.h>

extern "C" {
    typedef void (*ReceiveDeviceMessageCallback)(const char *);
}

class SpheroDataStreamingDelegate : public RobotBridge::SpheroDeviceMessageDelegate {
public:
    
    SpheroDataStreamingDelegate() {}
    virtual ~SpheroDataStreamingDelegate() {}
    void onMessageDelivered(RobotBridge::SpheroDeviceMessage* message) {}
    void onSerializedMessageDelivered(const char* message);
};


@interface RKUNBridge : NSObject {
    BOOL robotInitialized;
    BOOL robotOnline;
    BOOL controllerStreamingOn;
    SpheroDataStreamingDelegate* spheroDataStreamingDelegate;
}

@property  ReceiveDeviceMessageCallback receiveDeviceMessageCallback;

+(RKUNBridge*)sharedBridge;

-(void)connectToRobot;
-(BOOL)isRobotOnline;

-(void)broadcastDeviceMessage:(const char*)message;
- (void)setDataStreamingWithSampleRateDivisor:(uint16_t)divisor
                                 packetFrames:(uint16_t)frames
                                   sensorMask:(uint64_t)mask
                                  packetCount:(uint8_t)count;
-(void)enableControllerStreamingWithSampleRateDivisor:(uint16_t)divisor
                                         packetFrames:(uint16_t)frames
                                           sensorMask:(uint64_t)mask;
-(void)disableCotrollerStreaming;

@end
