//
//  RKUNBridge.m
//  Unity-iPhone
//
//  Created by Jon Carroll on 6/4/12.
//  Modified by Mike DePhillips on 3/11/13.
//  Copyright (c) 2012 Orbotix, Inc. All rights reserved.
//

#import "RKUNBridge.h"
#import "ARUNBridge.h"
#import <RobotKit/RobotKit.h>
#import <RobotBridge/MessengerBridge_iOS.h>
#include <RobotBridge/SpheroDeviceMessenger.h>

static RKUNBridge *sharedBridge = nil;
extern void UnityPause(bool pause);
extern void UnitySendMessage(const char *, const char *, const char *);

void SpheroDataStreamingDelegate::onSerializedMessageDelivered(const char *message) {
    [[RKUNBridge sharedBridge] broadcastDeviceMessage:message];
}

@implementation RKUNBridge

@synthesize receiveDeviceMessageCallback;

-(id)init {
    self = [super init];
    
    robotOnline = NO;
    [ARUNBridge sharedBridge].robotOnline = NO;
    controllerStreamingOn = NO;
    
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(appWillEnterBackground) name:UIApplicationWillTerminateNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(appWillEnterBackground) name:UIApplicationWillResignActiveNotification object:nil];
    
    spheroDataStreamingDelegate = new SpheroDataStreamingDelegate();
    [MessengerBridge_iOS sharedBridge];
    
    return self;
}

+(RKUNBridge*)sharedBridge {
    if(sharedBridge==nil) {
        sharedBridge = [[RKUNBridge alloc] init];
    }
    return sharedBridge;
}

-(void)dealloc {
    delete spheroDataStreamingDelegate;
    [super dealloc];
}

-(BOOL)isRobotOnline {
    return robotOnline;
}

-(void)connectToRobot {
    /*Try to connect to the robot*/
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleRobotOnline) name:RKDeviceConnectionOnlineNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleDidGainControl:) name:RKRobotDidGainControlNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleRobotOffline) name:RKDeviceConnectionOfflineNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleRobotOffline) name:RKRobotDidLossControlNotification object:nil];
    robotInitialized = NO;
    if ([[RKRobotProvider sharedRobotProvider] isRobotUnderControl]) {
        [[RKRobotProvider sharedRobotProvider] openRobotConnection];        
    }
    robotInitialized = YES;
}

-(void)appWillEnterBackground {
    //[[RKRobotProvider sharedRobotProvider] closeRobotConnection];
}

- (void)handleRobotOnline {
    RKDeviceNotification* notification = [[RKDeviceNotification alloc]initWithNotificationType:RKDeviceNotificationTypeConnected];
    // Send serialized object to Unity
    if (receiveDeviceMessageCallback != NULL) {
        RKDeviceMessageEncoder *encoder = [RKDeviceMessageEncoder encodeWithRootObject:notification];
        receiveDeviceMessageCallback([[encoder stringRepresentation] UTF8String]);
    }
    robotOnline = YES;
    [ARUNBridge sharedBridge].robotOnline = YES;
    
}

- (void)handleRobotOffline {
    RKDeviceNotification* notification = [[RKDeviceNotification alloc]initWithNotificationType:RKDeviceNotificationTypeDisconnected];
    // Send serialized object to Unity
    if (receiveDeviceMessageCallback != NULL) {
        RKDeviceMessageEncoder *encoder = [RKDeviceMessageEncoder encodeWithRootObject:notification];
        receiveDeviceMessageCallback([[encoder stringRepresentation] UTF8String]);
    }
    robotOnline = NO;
    [ARUNBridge sharedBridge].robotOnline = NO;
}

-(void)handleDidGainControl:(NSNotification*)notification {
    if(!robotInitialized)return;
    [[RKRobotProvider sharedRobotProvider] openRobotConnection];
}

- (void)addDataStreamingWithMask:(uint64_t)mask
{
    if (!robotOnline) return;
    
    // Register data streaming
    RobotBridge::SpheroDeviceMessenger::sharedInstance()
        .addSpheroDeviceMessageDelegate(spheroDataStreamingDelegate, mask);
}

- (void)setDataStreamingWithSensorMask:(uint64_t)mask
{
    if (!robotOnline) return;
    
    [[RKDeviceMessenger sharedMessenger] addDataStreamingObserver:self
                                                         selector:@selector(handleDataStreaming:)
                                                             mask:mask];
}

-(void)enableControllerStreamingWithSampleRateDivisor:(uint16_t)divisor
                                         packetFrames:(uint16_t)frames
                                           sensorMask:(uint64_t)mask
 {
     if(controllerStreamingOn && !robotOnline) return;

     [RKStabilizationCommand sendCommandWithState:RKStabilizationStateOff];
     [RKBackLEDOutputCommand sendCommandWithBrightness:1.0];
     [[RKDeviceMessenger sharedMessenger] setMessageRateDivisor:divisor];
     [[RKDeviceMessenger sharedMessenger] addDataStreamingObserver:self
                                                          selector:@selector(handleDataStreaming:)
                                                              mask:mask];
     controllerStreamingOn = YES;
    
}

-(void)disableControllerStreaming {
    if (!controllerStreamingOn) return;
    
    [[RKDeviceMessenger sharedMessenger] removeDataStreamingObserver:self];
    
    [RKBackLEDOutputCommand sendCommandWithBrightness:0.0];
    [RKStabilizationCommand sendCommandWithState:RKStabilizationStateOn];
    controllerStreamingOn = NO;
    
}

-(void)broadcastDeviceMessage:(const char*)message {
    if (receiveDeviceMessageCallback != NULL) {
        receiveDeviceMessageCallback(message);
    }
}

- (void)handleDataStreaming:(RKDeviceAsyncData *)data
{
    // TODO: implement a way for data streaming to only come in once!
    if ([data isKindOfClass:[RKDeviceSensorsAsyncData class]]) {
        RKDeviceSensorsAsyncData *sensors_data = (RKDeviceSensorsAsyncData *)data;
        RKDeviceSensorsData *data = [sensors_data.dataFrames objectAtIndex:0];
        
        // Send serialized object to Unity
        if (receiveDeviceMessageCallback != NULL) {
            RKDeviceMessageEncoder *encoder = [RKDeviceMessageEncoder encodeWithRootObject:sensors_data];
            receiveDeviceMessageCallback([[encoder stringRepresentation] UTF8String]);
        }
    }
}

- (void)disconnectRobots {
    [[RKRobotProvider sharedRobotProvider]closeRobotConnection];
}

extern "C" {
    
    void setupRobotConnection() {
        [[RKUNBridge sharedBridge] connectToRobot];
    }
    
    bool isRobotConnected() {
        return [[RKUNBridge sharedBridge] isRobotOnline];
    }
    
    void setRGB(float red, float green, float blue) {
        [RKRGBLEDOutputCommand sendCommandWithRed:red green:green blue:blue];
    }
    
    void roll(int heading, float speed) {
        [RKRollCommand sendCommandWithHeading:heading velocity:speed];
    }

    void setRawMotorValues(
        RKRawMotorMode leftMode,
        RKRawMotorPower leftPower,
        RKRawMotorMode rightMode,
        RKRawMotorPower rightPower) {

        [RKRawMotorValuesCommand sendCommandWithLeftMode:leftMode
                                               leftPower:leftPower
                                               rightMode:rightMode
                                              rightPower:rightPower];
    }

    void sendMacroWithBytes(unsigned char* macro, int32_t length)
    {
        NSData* data = [NSData dataWithBytes:macro length:length];
        [RKSaveTemporaryMacroCommand sendCommandWithMacro:data flags:RKMacroFlagNone];
        [RKRunMacroCommand sendCommandWithId:255];
    }
    
    void addDataStreamingMask(uint64_t mask) {
        [[RKUNBridge sharedBridge] addDataStreamingWithMask:mask];
    }
    
    void setDataStreaming(uint16_t sampleRateDivisor, uint16_t sampleFrames,
    	 uint64_t sampleMask, uint8_t sampleCount)
    {
        [[RKUNBridge sharedBridge] setDataStreamingWithSensorMask:sampleMask];
    }
    
    void enableControllerStreaming(uint16_t divisor, uint16_t frames, uint64_t mask) {
        [[RKUNBridge sharedBridge] enableControllerStreamingWithSampleRateDivisor:divisor
                                                                     packetFrames:frames
                                                                       sensorMask:mask];
    }
    
    void disableControllerStreaming() {
        [[RKUNBridge sharedBridge] disableControllerStreaming];
    }
    
    void setHeading(int heading) {
        [RKCalibrateCommand sendCommandWithHeading:heading];
    }
    
    void setBackLED(float intensity) {
        [RKBackLEDOutputCommand sendCommandWithBrightness:intensity];
    }
    
	void _RegisterRecieveDeviceMessageCallback(ReceiveDeviceMessageCallback callback) {
        RKUNBridge *bridge = [RKUNBridge sharedBridge];
        bridge.receiveDeviceMessageCallback = callback;
    }
    
    void disconnectRobots() {
        [[RKUNBridge sharedBridge] disconnectRobots];
    }
}

@end

