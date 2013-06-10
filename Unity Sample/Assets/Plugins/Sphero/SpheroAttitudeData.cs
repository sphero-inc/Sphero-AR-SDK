using System;

public class SpheroAttitudeData : SpheroSensorData {
	private float pitch;
	private float roll;
	private float yaw;
	
	public float Pitch {get{ return pitch;}}
	public float Roll {get{ return roll;}}
	public float Yaw {get{ return yaw;}}
	
	public SpheroAttitudeData(SpheroDeviceMessageDecoder decoder) : base(decoder)
	{
		pitch = decoder.DecodeFloat("pitch");
		roll = decoder.DecodeFloat("roll");
		yaw = decoder.DecodeFloat("yaw");
	}
}