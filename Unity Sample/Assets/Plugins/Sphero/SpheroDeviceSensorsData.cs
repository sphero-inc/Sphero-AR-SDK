using System;

public class SpheroDeviceSensorsData {
	private SpheroAccelerometerData accelerometerData;
	private SpheroAttitudeData		attitudeData;
	private SpheroQuaternionData	quaternionData;
	private SpheroBackEMFData		backEMFData;
	private SpheroLocatorData		locatorData;
	private SpheroGyroData			gyroData;
	
	
	public SpheroAccelerometerData AccelerometerData
	{
		get{ return accelerometerData; }
	}
	
	public SpheroAttitudeData AttitudeData
	{
		get{ return attitudeData; }
	}
	
	public SpheroQuaternionData QuaternionData
	{
		get{ return quaternionData; }
	}
	
	public SpheroBackEMFData BackEMFData
	{
		get{ return backEMFData; }
	}
	
	public SpheroLocatorData LocatorData
	{
		get{ return locatorData; }
	}
	
	public SpheroGyroData GyroData
	{
		get{ return gyroData; }
	}
	
	public SpheroDeviceSensorsData(SpheroDeviceMessageDecoder decoder)
	{
		accelerometerData = 
			(SpheroAccelerometerData)decoder.DecodeObject("accelerometerData");
		attitudeData = (SpheroAttitudeData)decoder.DecodeObject("attitudeData");
		quaternionData = (SpheroQuaternionData)decoder.DecodeObject("quaternionData");
		backEMFData = (SpheroBackEMFData)decoder.DecodeObject("backEMFData");
		locatorData = (SpheroLocatorData)decoder.DecodeObject("locatorData");
		gyroData = (SpheroGyroData)decoder.DecodeObject("gyroData");
	}
}