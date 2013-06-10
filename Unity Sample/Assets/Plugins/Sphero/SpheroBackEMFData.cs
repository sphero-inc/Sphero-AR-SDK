using System;

public class SpheroBackEMFData : SpheroSensorData {
	public struct Motors {
		public int right;
		public int left;		
	}
	
	private Motors raw;
	private Motors filtered;
	
	public Motors Filtered { get{ return filtered; } }
	public Motors Raw { get{ return raw; } }
	
	public SpheroBackEMFData(SpheroDeviceMessageDecoder decoder) : base(decoder)
	{
		filtered.right = decoder.DecodeInt32("filtered.rightMotor");
		filtered.left = decoder.DecodeInt32("filtered.leftMotor");
		raw.right = decoder.DecodeInt32("raw.rightMotor");
		raw.left = decoder.DecodeInt32("raw.leftMotor");
	}

}