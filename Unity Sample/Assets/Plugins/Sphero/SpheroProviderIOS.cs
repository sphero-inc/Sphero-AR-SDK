using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if UNITY_IPHONE

public class SpheroProviderIOS : SpheroProvider {
	
	/*
	 * Get the Robot Provider for Android 
	 */
	public SpheroProviderIOS() : base() {
		m_PairedSpheros = new Sphero[1];
		m_PairedSpheros[0] = new SpheroIOS();
		// DO NOT CHANGE THIS UNTIL MULTIPLE ROBOTS ARE USED ON iOS (if ever)
		m_PairedSpheros[0].DeviceInfo.UniqueId = "Robot";
		m_PairedSpheros[0].DeviceInfo.Name = "Robot";
		// Sign up for notifications on Sphero connections
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
	}
	
	/*
	 * Callback to receive connection notifications 
	 */
	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		SpheroDeviceNotification message = (SpheroDeviceNotification)eventArgs.Message;
		Sphero notifiedSphero = m_PairedSpheros[0];
		if( message.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTED ) {
			notifiedSphero.ConnectionState = Sphero.Connection_State.Connected;
			// Consider setting bluetooth device info here
		}
		else if( message.NotificationType == SpheroDeviceNotification.SpheroNotificationType.DISCONNECTED ) {
			notifiedSphero.ConnectionState = Sphero.Connection_State.Disconnected;
		}
		else if( message.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTION_FAILED ) {
			notifiedSphero.ConnectionState = Sphero.Connection_State.Failed;
		}
	}
	
	override public void DisconnectSpheros() {
		disconnectRobots();
		m_PairedSpheros[0].ConnectionState = Sphero.Connection_State.Disconnected;
	}
	
	override public void Connect(int index) {
		// Don't try to connect to multiple Spheros at once
		setupRobotConnection();
	}	
	
	override public Sphero GetSphero(string spheroId) {
		if( m_PairedSpheros.Length > 0 ) return m_PairedSpheros[0];
		return null; 
	}
	
	override public Sphero[] GetConnectedSpheros() {		
		if( m_PairedSpheros[0].ConnectionState == Sphero.Connection_State.Connected ) {
			Sphero[] connectedSpheros = new Sphero[1];
			connectedSpheros[0] = m_PairedSpheros[0];
			return connectedSpheros;
		}
		return new Sphero[0];
	}
	
	/* Need to call this to get the robot objects that are paired from Android */
	override public void FindRobots() {}
	/* Check if bluetooth is on */
	override public bool IsAdapterEnabled() { return true; }
	
	/* Native Bridge Functions from RKUNBridge.mm */
	[DllImport ("__Internal")]
	private static extern void setupRobotConnection();
	[DllImport ("__Internal")]
	private static extern bool isRobotConnected();
	[DllImport ("__Internal")]
	private static extern void disconnectRobots();	
}

#endif
