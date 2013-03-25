using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_IPHONE

public class SpheroIOS : Sphero {
	
	/*
	 * Default constructor used for iOS 
	 */ 
	public SpheroIOS() : base() {
		m_DeviceInfo = new BluetoothDeviceInfo("", "");
	}
	
	override public void SetRGBLED(float red, float green, float blue) {
		setRGB(red,green,blue);
		m_RGBLEDColor = new Color(red, green, blue, 1.0f);
	}
	
	override public void EnableControllerStreaming(ushort divisor, ushort packetFrames, SpheroDataStreamingMask sensorMask) {		
		enableControllerStreaming(divisor, packetFrames, sensorMask);
	}
	
	override public void DisableControllerStreaming() {
		disableControllerStreaming();
	}
	
	override public void SetDataStreaming(ushort divisor, ushort packetFrames, SpheroDataStreamingMask sensorMask, ushort packetCount) {
		setDataStreaming(divisor, packetFrames, sensorMask, (byte)packetCount);
	}
	
	override public void AddDataStreamingMaskAndSendCommand(ulong mask) {
		addDataStreamingMask(mask);
	}
	
	override public void Roll(int heading, float speed) {
		roll(heading,speed);	
	}
	
	override public void SetHeading(int heading) {
		setHeading(heading);
	}
	
	override public void SetBackLED(float intensity) {
		setBackLED(intensity);	
	}

	override public void SetRawMotorValues(
		SpheroRawMotorMode leftMode,
		float leftPower,
		SpheroRawMotorMode rightMode,
		float rightPower) {
		
		setRawMotorValues(leftMode, (int)(leftPower * 255f), rightMode, (int)(rightPower * 255f));
	}

	override public void SendMacroWithBytes(byte[] macro) {
		sendMacroWithBytes(macro, macro.Length);
	}
	
	/* Native Bridge Functions from RKUNBridge.mm */
	[DllImport ("__Internal")]
	private static extern void setRGB(float red, float green, float blue);
	[DllImport ("__Internal")]
	private static extern void roll(int heading, float speed);
	[DllImport ("__Internal")]
	private static extern void setRawMotorValues(
		SpheroRawMotorMode leftMode,
		int leftPower,
		SpheroRawMotorMode rightMode,
		int rightPower);
	[DllImport ("__Internal")]
	private static extern void sendMacroWithBytes(byte[] bytes, int size);
	[DllImport ("__Internal")]
	private static extern void setHeading(int heading);
	[DllImport ("__Internal")]
	private static extern void setBackLED(float intensity);
	[DllImport ("__Internal")]
	private static extern void addDataStreamingMask(ulong mask);
	[DllImport ("__Internal")]
	private static extern void setDataStreaming(ushort sampleRateDivisor, 
		ushort sampleFrames, SpheroDataStreamingMask sampleMask, byte sampleCount);
	[DllImport ("__Internal")]
	private static extern void enableControllerStreaming(ushort sampleRateDivisor,
		ushort sampleFrames, SpheroDataStreamingMask sampleMask);
	[DllImport ("__Internal")]
	private static extern void disableControllerStreaming();
}

#endif