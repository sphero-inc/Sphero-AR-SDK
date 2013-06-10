using System;
using System.Collections.Generic;

public class SpheroDeviceNotification : SpheroDeviceMessage
{
	public enum SpheroNotificationType : int {
		CONNECTED         = 0,
		DISCONNECTED      = 1,
		CONNECTION_FAILED = 2
	}
	private SpheroNotificationType m_NotificationType;
	public SpheroNotificationType NotificationType {
		get { return m_NotificationType; }	
	}
	
	public SpheroDeviceNotification(SpheroDeviceMessageDecoder decoder) 
		: base(decoder)
	{
		// Do not change the "type" string since it is linked in RobotKit
		m_NotificationType = (SpheroNotificationType)decoder.DecodeInt32("type");
	}
}