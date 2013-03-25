using System;

public class SpheroGyroData : SpheroSensorData {
	private ThreeAxisSensor rotationRate;
	private ThreeAxisSensor rotationRateRaw;
	
	public ThreeAxisSensor RotationRate { get{ return rotationRate; } }
	public ThreeAxisSensor RotationRateRaw { get{ return rotationRateRaw; } }
	
	public SpheroGyroData( SpheroDeviceMessageDecoder decoder ) : base(decoder)
	{
		rotationRate.x = decoder.DecodeInt32("rotationRate.x");
		rotationRate.y = decoder.DecodeInt32("rotationRate.y");
		rotationRate.z = decoder.DecodeInt32("rotationRate.z");
		rotationRateRaw.x = decoder.DecodeInt32("rotationRateRaw.x");
		rotationRateRaw.y = decoder.DecodeInt32("rotationRateRaw.y");
		rotationRateRaw.z = decoder.DecodeInt32("rotationRateRaw.z");
	}
}