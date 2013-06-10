using System;

public class SpheroSensorData {

	public struct ThreeAxisSensor {
		public int x;
		public int y;
		public int z;
	}
	
	private long timeStamp;
	
	public long TimeStamp
	{
		get{ return timeStamp; }
	}
	
	public SpheroSensorData(SpheroDeviceMessageDecoder decoder)
	{
		
	}
}