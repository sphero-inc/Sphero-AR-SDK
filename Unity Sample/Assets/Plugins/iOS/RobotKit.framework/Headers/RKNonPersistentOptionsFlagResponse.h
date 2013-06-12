//
//  RKNonPersistentOptionsFlagResponse.h
//  RobotKit
//
//  Created by wes felteau on 4/16/13.
//  Copyright (c) 2013 Orbotix Inc. All rights reserved.
//

/*! @file */

#import <RobotKit/RobotKit.h>

typedef enum RKNonPersistentOptionsFlagMask : uint32_t {
   StopOnDisconnect = 0x00000001
} RKNonPersistentOptionsFlagMask;


/*! */
@interface RKNonPersistentOptionsFlagResponse : RKDeviceResponse

/*! The current option flags on the device */
@property ( nonatomic, readonly ) RKNonPersistentOptionsFlagMask mask;

/*! */
- (BOOL) isStopOnDisconnect;

@end
