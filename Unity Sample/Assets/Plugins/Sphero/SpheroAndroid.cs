using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_ANDROID

public class SpheroAndroid : Sphero {
	
	private AndroidJavaObject m_AndroidJavaSphero;
	public AndroidJavaObject AndroidJavaSphero
    {
		get{ return this.m_AndroidJavaSphero; }
    }
	
	private AndroidJavaObject m_UnityBridge = (new AndroidJavaClass("orbotix.unity.UnityBridge")).CallStatic<AndroidJavaObject>("sharedBridge");
	
	// Cached Java Classes for efficient calls
	private AndroidJavaClass m_RGBLEDOutput = new AndroidJavaClass("orbotix.robot.base.RGBLEDOutputCommand");
	private AndroidJavaClass m_RollCommand = new AndroidJavaClass("orbotix.robot.base.RollCommand");
	private AndroidJavaClass m_SetHeadingCommand = new AndroidJavaClass("orbotix.robot.base.SetHeadingCommand");
	private AndroidJavaClass m_BackLEDOutputCommand = new AndroidJavaClass("orbotix.robot.base.BackLEDOutputCommand");
	private AndroidJavaClass m_RawMotorCommand = new AndroidJavaClass("orbotix.robot.base.RawMotorCommand");
	private AndroidJavaClass m_SaveTemporaryMacroCommand = new AndroidJavaClass("orbotix.robot.base.SaveTemporaryMacroCommand");
	private AndroidJavaClass m_RunMacroCommand = new AndroidJavaClass("orbotix.robot.base.RunMacroCommand");
	
	/* More detailed constructor used for Android */ 
	public SpheroAndroid(AndroidJavaObject sphero, string bt_name, string bt_address) : base() {		
		m_AndroidJavaSphero = sphero;
		m_DeviceInfo = new BluetoothDeviceInfo(bt_name, bt_address);
	}
	
	override public void SetRGBLED(float red, float green, float blue) {
		m_RGBLEDOutput.CallStatic("sendCommand",m_AndroidJavaSphero,(int)(red*255),(int)(green*255),(int)(blue*255));
		m_RGBLEDColor = new Color(red, green, blue, 1.0f);
	}

	override public void EnableControllerStreaming(ushort divisor, ushort packetFrames, SpheroDataStreamingMask sensorMask) {			
		m_UnityBridge.Call("enableControllerStreaming",m_AndroidJavaSphero,(int)divisor,(int)packetFrames,(long)sensorMask);
	}

	override public void DisableControllerStreaming() {
		m_UnityBridge.Call("disableControllerStreaming",m_AndroidJavaSphero);
	}
	
	override public void SetDataStreaming(ushort divisor, ushort packetFrames, SpheroDataStreamingMask sensorMask, ushort packetCount) {
		m_UnityBridge.Call("setDataStreaming",m_AndroidJavaSphero, divisor, packetFrames, sensorMask, packetCount);
	}
	
	override public void AddDataStreamingMaskAndSendCommand(ulong mask) {
		// TODO: implement on Android
	}
	
	override public void Roll(int heading, float speed) {
		m_RollCommand.CallStatic("sendCommand",m_AndroidJavaSphero,(float)heading,speed);
	}
	
	override public void SetHeading(int heading) {
		m_SetHeadingCommand.CallStatic("sendCommand",m_AndroidJavaSphero,(float)heading);
	}
	
	override public void SetBackLED(float intensity) {
		m_BackLEDOutputCommand.CallStatic("sendCommand",m_AndroidJavaSphero,intensity);
	}

	override public void SetRawMotorValues(
		SpheroRawMotorMode leftMode,
		float leftPower,
		SpheroRawMotorMode rightMode,
		float rightPower) {
		m_RawMotorCommand.CallStatic(
			"sendCommand",
			m_AndroidJavaSphero,
			(int)leftMode,
			(int)(leftPower * 255f),
			(int)rightMode,
			(int)(rightPower * 255f));
	}

	override public void SendMacroWithBytes(byte[] macro)
	{
		m_SaveTemporaryMacroCommand.CallStatic(
			"sendCommand",
			m_AndroidJavaSphero,
			(byte)0,
			macro);
		m_RunMacroCommand.CallStatic("sendCommand", m_AndroidJavaSphero, (byte)255);
	}
}

#endif