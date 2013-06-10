using System;

public class SpheroAccelerometerData : SpheroSensorData
{
	public class Acceleration {
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		
		public Acceleration() 
		{
			this.X = 0.0f;
			this.Y = 0.0f;
			this.Z = 0.0f;
		}
	}
	
	private Acceleration normalized = new Acceleration();
	private ThreeAxisSensor raw;
	
	public Acceleration Normalized 
	{
		get{ return normalized; }
	}
	
	public ThreeAxisSensor Raw
	{
		get{ return raw; }
	}
	
	public SpheroAccelerometerData(SpheroDeviceMessageDecoder decoder) : base(decoder)
	{
		normalized.X = decoder.DecodeFloat("normalized.x");
		normalized.Y = decoder.DecodeFloat("normalized.y");
		normalized.Z = decoder.DecodeFloat("normalized.z");
		raw.x = decoder.DecodeInt32("accelerationRaw.x");
		raw.y = decoder.DecodeInt32("accelerationRaw.y");
		raw.z = decoder.DecodeInt32("accelerationRaw.z");
	}
}