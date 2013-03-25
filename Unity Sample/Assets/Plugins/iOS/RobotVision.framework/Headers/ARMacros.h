//
//  ARMacros.h
//  RobotVision
//
//  Created by Brandon Dorris on 3/1/13.
//  Copyright (c) 2013 Orbotix, Inc. All rights reserved.
//

#ifndef RobotVision_ARMacros_h
#define RobotVision_ARMacros_h

#define DISALLOW_COPY_AND_ASSIGN(TypeName) \
TypeName(const TypeName&);\
void operator=(const TypeName&)

#define SAFE_DELETE(x) {delete x; x = NULL;}

#endif
