using System;
using System.IO;
using SharpUnit;

namespace SharpUnit {
	public class SpheroDeviceSensorsAsyncDataTests : TestCase
	{
		
		[UnitTest]
		public void TestDecode()
		{
			string encodedString = File.ReadAllText("/Users/brian/Documents/code/development/unity-plugin/ExampleProject/SensorStreaming/Assets/Plugins/SpheroTests/DataStreamingExample.json");
			
			// Test that a message is created.
			SpheroDeviceMessage message = 
				SpheroDeviceMessageDecoder.messageFromEncodedString(encodedString);
			Assert.NotNull(message);
			Assert.True(message is SpheroDeviceSensorsAsyncData);
			Assert.Equal(123456, message.TimeStamp);
			
			// Specific test for SpheroDeviceSensorsAsyncData 
			SpheroDeviceSensorsAsyncData sensorsAsyncData = 
				(SpheroDeviceSensorsAsyncData)message;
				
			Assert.Equal(2, sensorsAsyncData.FrameCount);
			Assert.Equal(0xF0000000E067E060, sensorsAsyncData.Mask);		
			Assert.NotNull(sensorsAsyncData.Frames);
			Assert.True(sensorsAsyncData.Frames.Length > 1);
			
			// Check values for a DeviceSensorsData object
			SpheroDeviceSensorsData sensorsData = sensorsAsyncData.Frames[0];
			
			// Accelerometer
			Assert.Equal(1.23f, sensorsData.AccelerometerData.Normalized.X);
			Assert.Equal(1.23f, sensorsData.AccelerometerData.Normalized.Y);
			Assert.Equal(1.23f, sensorsData.AccelerometerData.Normalized.Z); 
			Assert.Equal(4096, sensorsData.AccelerometerData.Raw.x);
			Assert.Equal(4096, sensorsData.AccelerometerData.Raw.y);
			Assert.Equal(4096, sensorsData.AccelerometerData.Raw.z);
			
			// Attitude 
			Assert.Equal(45.0f, sensorsData.AttitudeData.Pitch);
			Assert.Equal(180.0f, sensorsData.AttitudeData.Roll);
			Assert.Equal(270.0f, sensorsData.AttitudeData.Yaw);
			
			// Quaternion
			Assert.Equal(0.3f, sensorsData.QuaternionData.Q0);
			Assert.Equal(0.7f, sensorsData.QuaternionData.Q1);
			Assert.Equal(0.3f, sensorsData.QuaternionData.Q2);
			Assert.Equal(1.0f, sensorsData.QuaternionData.Q3);
			
			// back EMF
			Assert.Equal(200, sensorsData.BackEMFData.Filtered.right);
			Assert.Equal(200, sensorsData.BackEMFData.Filtered.left);
			Assert.Equal(200, sensorsData.BackEMFData.Raw.right);
			Assert.Equal(200, sensorsData.BackEMFData.Raw.left);
			
			// locator
			Assert.Equal(190.2f, sensorsData.LocatorData.Position.x);
			Assert.Equal(85.6f, sensorsData.LocatorData.Position.y);
			Assert.True(Math.Abs(208.5746868630036f - sensorsData.LocatorData.Position.Magnitude()) < 0.001);
			Assert.Equal(9.99f, sensorsData.LocatorData.Velocity.x);
			Assert.Equal(86.4f, sensorsData.LocatorData.Velocity.y);
			
			// gyro
			Assert.Equal(300, sensorsData.GyroData.RotationRate.x);
			Assert.Equal(300, sensorsData.GyroData.RotationRate.y);
			Assert.Equal(300, sensorsData.GyroData.RotationRate.z);
			Assert.Equal(300, sensorsData.GyroData.RotationRateRaw.x);
			Assert.Equal(300, sensorsData.GyroData.RotationRateRaw.y);
			Assert.Equal(300, sensorsData.GyroData.RotationRateRaw.z);
		}
	}	
}