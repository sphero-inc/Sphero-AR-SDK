using System;

[Flags]
public enum SpheroDataStreamingMask : ulong {
	/*! Turns off all data streaming. */
	Off                          = 0x0000000000000000,
	/*! Mask to register for left motor back EMF filtered data */
	LeftMotorBackEMFFiltered     = 0x0000000000000020,
	/*! Mask to register for right motor back EMF filtered data */
	RightMotorBackEMFFiltered    = 0x0000000000000040,
	/*! Mask to register for magnetometer z-axis filtered data */
	MagnetometerZFiltered        = 0x0000000000000080,
	/*! Mask to register for magnetometer y-axis filtered data */
	MagnetometerYFiltered        = 0x0000000000000100,
	/*! Mask to register for magnetometer x-axis filtered data */
	MagnetometerXFiltered        = 0x0000000000000200,
	/*! Mask to register for gyro z-axis filtered data */
	GyroZFiltered                = 0x0000000000000400,
	/*! Mask to register for gyro y-axis filtered data */
	GyroYFiltered                = 0x0000000000000800,
	/*! Mask to register for gyro x-axis filtered data */
	GyroXFiltered                = 0x0000000000001000,
	/*! Mask to register for accelerometer z-axis filtered data */
	AccelerometerZFiltered       = 0x0000000000002000,
	/*! Mask to register for accelerometer y-axis filtered data */
	AccelerometerYFiltered       = 0x0000000000004000,
	/*! Mask to register for accelerometer x-axis filtered data */
	AccelerometerXFiltered       = 0x0000000000008000,
	/*! Mask to register for IMU gyro yaw (heading) angle filtered data */
	IMUYawAngleFiltered          = 0x0000000000010000,
	/*! Mask to register for IMU gyro roll angle filtered data */
	IMURollAngleFiltered         = 0x0000000000020000,
	/*! Mask to register for IMU gyro pitch angle filtered data */
	IMUPitchAngleFiltered        = 0x0000000000040000,
	/*! Mask to register for left motor back EMF raw data */
	LeftMotorBackEMFRaw          = 0x0000000000200000,
	/*! Mask to register for right motor back EMF raw data */
	RightMotorBackEMFRaw         = 0x0000000000400000,
	/*! Mask to register for magnetometer z-axis raw data */
	MagnetometerZRaw             = 0x0000000000800000,
	/*! Mask to register for magnetometer y-axis raw data */
	MagnetometerYRaw             = 0x0000000001000000,
	/*! Mask to register for magnetometer x-axis raw data */
	MagnetometerXRaw             = 0x0000000002000000,
	/*! Mask to register for gyro z-axis raw data */
	GyroZRaw                     = 0x0000000004000000,
	/*! Mask to register for gyro y-axis raw data */
	GyroYRaw                     = 0x0000000008000000,
	/*! Mask to register for gyro x-axis raw data */
	GyroXRaw                     = 0x0000000010000000,
	/*! Mask to register for accelerometer z-axis raw data */
	AccelerometerZRaw            = 0x0000000020000000,
	/*! Mask to register for accelerometer y-axis raw data */
	AccelerometerYRaw            = 0x0000000040000000,
	/*! Mask to register for accelerometer x-axis raw data */
	AccelerometerXRaw            = 0x0000000080000000,

	/*! Mask to register for quaternion0 data - ONLY FOR FIRMWARE 1.17 OR GREATER */
	Quaternion0                 = 0x8000000000000000,
	/*! Mask to register for quaternion1 data - ONLY FOR FIRMWARE 1.17 OR GREATER */
	Quaternion1                 = 0x4000000000000000,
	/*! Mask to register for quaternion2 data - ONLY FOR FIRMWARE 1.17 OR GREATER */
	Quaternion2                 = 0x2000000000000000,
	/*! Mask to register for quaternion3 data - ONLY FOR FIRMWARE 1.17 OR GREATER */
	Quaternion3                 = 0x1000000000000000,
	/*! Mask to register for locator streaming of x position - ONLY FOR FIRMWARE 1.17 OR GREATER */
	LocatorX                    = 0x0800000000000000,
	/*! Mask to register for locator streaming of y position - ONLY FOR FIRMWARE 1.17 OR GREATER */
	LocatorY                    = 0x0400000000000000,
	
	// TODO:
	//AccelOne                  = 0x0200000000000000,
	
	/*! Mask to register for locator streaming of x velocity - ONLY FOR FIRMWARE 1.17 OR GREATER */
	VelocityX                   = 0x0100000000000000,
	/*! Mask to register for locator streaming of y velocity - ONLY FOR FIRMWARE 1.17 OR GREATER */
	VelocityY                   = 0x0080000000000000,
	
	/*! Convenience mask to register for all IMU gyro angle filtered data (roll, pitch, yaw) */
	IMUAnglesFilteredAll        = 0x0000000000070000,
	/*! Convenience mask to register for all filtered accelerometer data (x-axis, y-axis, z-axis) */
	AccelerometerFilteredAll    = 0x000000000000E000,
	/*! Convenience mask to register for locator streaming of all data (position and velocity)
	 * ONLY FOR FIRMWARE 1.17 OR GREATER */
	LocatorAll                  = 0x0D80000000000000,
	/*! Convenience mask to register for locator streaming of all data (position and velocity)
	 * ONLY FOR FIRMWARE 1.17 OR GREATER */
	QuaternionAll               = 0xF000000000000000

};
