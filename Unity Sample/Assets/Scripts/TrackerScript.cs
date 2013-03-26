using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class TrackerScript : MonoBehaviour
{
	/* Sphero object to get heading of 3d object */	
	Sphero sphero = null;
	
	/* Data that AR needs to draw a 3D character with the correct heading */
	private const SpheroDataStreamingMask kVisionMask = SpheroDataStreamingMask.IMUYawAngleFiltered;
	
	/* Data streaming back from sphero */
	float SpheroYaw = 0.0f;
	Quaternion SpheroQuaternion = Quaternion.identity;	
	
	//! @brief	number of vision puts before we switch to vision for heading
	public const int kPutCountForVisionHeading = 2;
	
	//! @brief	the current speed of sharky
	private float m_currentSpeed = 0.0f;

	//! @brief 	how much to scale the orientation from the old to the new each frame
	private float QuaternionScale = 0.5f;
	
	private bool TrackingSphero = false;
	
	//! @get and set the current speed of sharky
	public float Speed
	{
		get{ return m_currentSpeed; }
		set{ m_currentSpeed = value; }
	}

	/*!
	 * @brief	sets the perceived radius of the tracked object or set to null to use sphero's
	 *			radius according to vision. Always returns a non-null value
	 */
	private float? m_radius;
	public float? Radius
	{
		get
		{
			if (m_radius == null)
			{
#if !UNITY_EDITOR
				return (float?)ARUNBridge.SPHERO_RADIUS_IN_METERS;
#else
				return 0.0f;
#endif
			}
			else
			{
				return m_radius;
			}
		}
		set
		{
			m_radius = value;
		}
	}
	
	void Start()
	{				
		ARUNController controller = ARUNController.Instance;
		if (controller != null)
		{
			controller.OnVisionUpdate += OnVisionUpdate;
			
		}
		else
		{
			//Debug.LogWarning("AureController.Instance not set");
		}
#if !UNITY_EDITOR
		// Get first connected Sphero
		Sphero[] spheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
		if( spheros.Length > 0 ) {
			sphero = spheros[0];	
			sphero.AddDataStreamingMask((ulong)kVisionMask);
			SpheroDeviceMessenger.SharedInstance.AsyncDataReceived += ReceiveAsyncMessage;
		}
#endif
	}

	void OnDestroy()
	{
		ARUNController controller = ARUNController.Instance;
		if (controller != null)
		{
			controller.OnVisionUpdate -= OnVisionUpdate;
		}
		else
		{
			//Debug.LogWarning("AureController.Instance not set");
		}
#if !UNITY_EDITOR
		if( sphero != null ) {
			sphero.RemoveDataStreamingMask((ulong)kVisionMask);
			SpheroDeviceMessenger.SharedInstance.AsyncDataReceived -= ReceiveAsyncMessage;
		}
#endif
	}
	
	private void ReceiveAsyncMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		// Handler method for the streaming data. This code copies the data values
		SpheroDeviceSensorsAsyncData message = 
			(SpheroDeviceSensorsAsyncData)eventArgs.Message;
		SpheroDeviceSensorsData sensorsData = message.Frames[0];
		
		// There can be some discontinuation with vision data coming in, and what we requested,
		// so check if the data actually exists
		SpheroAttitudeData attitudeData = sensorsData.AttitudeData;
		if( attitudeData != null ) {
			SpheroYaw = sensorsData.AttitudeData.Yaw;
		}
		
		SpheroQuaternionData quaternionData = sensorsData.QuaternionData;
		if( quaternionData != null ) {
			SpheroQuaternion = new Quaternion(sensorsData.QuaternionData.Q1,
										  	  sensorsData.QuaternionData.Q2,
										      sensorsData.QuaternionData.Q3, 
										  	  sensorsData.QuaternionData.Q0);	
		}
	}
	
	// Update is called once per frame
	private void OnVisionUpdate ()
	{
		// Only display objects if we are tracking them
		if( ARUNBridge.CurrentARResult.SpheroTrackingState != ARUNBridge.FindSpheroState.Tracking &&
			ARUNBridge.CurrentARResult.SpheroTrackingState != ARUNBridge.FindSpheroState.TrackingPoorly ) {
			
			if( TrackingSphero ) {
				Component[] rendererComponents = GetComponentsInChildren(typeof(Renderer)) ;
				for(int i = 0; i < rendererComponents.Length; i++)
				{
				   	Renderer r = ((Renderer)rendererComponents[i]);
					r.enabled = false;
				}
			}
			TrackingSphero = false;
		}
		else {
			if( !TrackingSphero ) {
				Component[] rendererComponents = GetComponentsInChildren(typeof(Renderer)) ;
				for(int i = 0; i < rendererComponents.Length; i++)
				{
				   	Renderer r = ((Renderer)rendererComponents[i]);
					r.enabled = true;
				}
			}
			TrackingSphero = true;
		}
		
		Vector3 position;
		Quaternion orientation;
		GetSpheroTransform(out position, out orientation);

		transform.localRotation = Quaternion.Lerp(
			transform.localRotation,
			orientation,
			QuaternionScale);
		
		position.y -= ARUNBridge.SPHERO_RADIUS_IN_METERS;
		transform.localPosition = position;

		float dx = ARUNBridge.CurrentARResult.SpheroVelocity.x;
		float dy = ARUNBridge.CurrentARResult.SpheroVelocity.y;
		Speed = Mathf.Sqrt(dx*dx + dy*dy);
	}	
	
	/*!
	 * @brief	get's the transform to position sphero as \a position and \a orientation components
	 * @param	position Vector3 to store sphero's position
	 * @param	orientation Quaternion to store sphero's orientation
	 */
	private void GetSpheroTransform(out Vector3 position, out Quaternion orientation)
	{
		float yaw;
		if (ARUNBridge.CurrentARResult.CalibratingPutCount >= kPutCountForVisionHeading)
		{
			
			yaw = ComputeSpheroHeadingVision();
		}
		else
		{
			yaw = ComputeSpheroHeadingCalibration();
		}
		
		orientation = Quaternion.Euler(0.0f, -yaw, 0.0f);
		position = VisionUtils.s_spheroToUnityTransform * SpheroNativePosition();

		VisionUtils.AdjustSpheroTransformUsingHeadBob(ref position, ref orientation);
	}
	
	//! @return the sphero's heading according to vision
	private float ComputeSpheroHeadingVision()
	{
		return SpheroYaw - ARUNBridge.CurrentARResult.LocatorAlignmentAngle * Mathf.Rad2Deg + 180.0f;
	}
	
		//! @return the sphero's heading without relying on vision - before vision is primed this appears more correct
	private float ComputeSpheroHeadingCalibration()
	{
		const float angularCorrection = -90.0f;
		
		// get the sphero position data to work out the angle to the sphero
		Vector3 spheroPosition = VisionUtils.s_spheroToUnityTransform * SpheroNativePosition();
		float angleToSphero = Mathf.Atan2(spheroPosition.z, spheroPosition.x) * Mathf.Rad2Deg;

		return sphero.ApplyCalibrationToHeading(SpheroYaw) + angleToSphero + angularCorrection;
	}
	
	//! @return	sphero's position before the vision->unity transform
	private static Vector3 SpheroNativePosition()
	{
		ARUNBridge.ARUNVector3D nativePosition = ARUNBridge.CurrentARResult.SpheroPosition;
		return new Vector3(nativePosition.x, nativePosition.y, nativePosition.z);
	}
	
		//! @return 	sphero's orientation as a quaternion
	public Quaternion GetAbsoluteSpheroQuaternion()
	{
		Quaternion orientation = new Quaternion(-SpheroQuaternion.x, SpheroQuaternion.y, SpheroQuaternion.z, SpheroQuaternion.w);
		
		orientation = Quaternion.Euler(-90.0f, 0.0f, 0.0f)
			* orientation
			* Quaternion.Euler(90.0f, 0.0f, 0.0f)
			* Quaternion.Euler(0.0f, ARUNBridge.CurrentARResult.LocatorAlignmentAngle * Mathf.Rad2Deg + 45.0f, 0.0f);
		
		return orientation;
	}
}
