using System;

public class SpheroQuaternionData : SpheroSensorData {
	private float q0;
	private float q1;
	private float q2;
	private float q3;
	
	public float Q0 { get { return q0; } }
	public float Q1 { get { return q1; } }
	public float Q2 { get { return q2; } }
	public float Q3 { get { return q3; } }
	
	public SpheroQuaternionData( SpheroDeviceMessageDecoder decoder ) : base(decoder)
	{
		q0 = decoder.DecodeFloat("quaternions.q0");
		q1 = decoder.DecodeFloat("quaternions.q1");
		q2 = decoder.DecodeFloat("quaternions.q2");
		q3 = decoder.DecodeFloat("quaternions.q3");
	}
}