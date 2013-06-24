using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ARUNBridge {
	
	public static readonly float SPHERO_RADIUS_IN_METERS = 0.0365f;
	
	public static ARUNResult CurrentARResult;
	
	/*
	 * A struct that encapsulates the C++ class ARResult 
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct ARUNResult
	{
		public float LocatorAlignmentAngle;
		public int CalibratingPutCount;
		public ARUNSize BackgroundVideoSize;
		public FindSpheroState SpheroTrackingState;
		public ARUNVector3D SpheroPosition;
		public ARUNVector2D SpheroVelocity;
		public ARUNGLMatrix CameraMatrix;
		public AuPlatformConfiguration CameraConfigurationSettings;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AuDeviceCameraConfiguration
	{
		public float near, width, height;
		public int rows, cols;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct ARUNVector3D
	{
		public float x, y, z;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct ARUNVector2D
	{
		public float x, y;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct ARUNSize
	{
		public int width, height;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct ARUNGLMatrix
	{
		public float m11, m12, m13, m14;
		public float m21, m22, m23, m24;
		public float m31, m32, m33, m34;
		public float m41, m42, m43, m44;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct ARUNQuaternion
	{		
		public float w, x, y, z;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AuDeviceDisplayConfiguration
	{
		public int rows, cols;         //  Resolution of the actual output
		public float width, height;    //  Ratio should match ratio of rows/cols
		public float near;             //  Distance to the near plane (in pixels)
		public float widthInMeters;    //  Width of the actual physical display (width <-> cols)
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AuPlatformConfiguration
	{
		//  Hardware Configuration
		public AuDeviceCameraConfiguration camera;
		public AuDeviceDisplayConfiguration display;

		//  Derived Data
		//  Video render rect
		public float vidRectTop, vidRectLeft, vidRectNear;
		public float projFrustTop, projFrustLeft, projFrustNear;

		public bool matchedWidth;                                       //  true->cropped height, false->cropped width
		public float pixelsToNearPlaneMetersConversionFactor;
		
		public bool shouldUseManualFocusControl;
		public int numberOfCores;
		public float cpuPerformanceRatio;
	}

	public enum FindSpheroState
	{
		NoInformation,
		Lost,
		Confused,
		Tracking,
		TrackingPoorly
	}
	
	public enum CameraMotionMode
	{
		AllowVerticalMovement, // This is best for investigating the model like in Sharky.  Allows height to change.
		StaticHeight		   // This is best for AR with other models in the scene, like in Rolling Dead.  Locks user's height.
	}
	
	public static void UpdateArResults() {
		CurrentARResult = _ARUNBridgeGetCurrentResult();
	}

#if UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern bool _ARUNHasNewFrame();
#else
	public static bool _ARUNHasNewFrame()
	{
		// TODO: implement me!
		return true;
	}
#endif

	public static float GetDeviceYaw() {
		return Input.gyro.attitude.eulerAngles.z;	
	}
	
#if UNITY_EDITOR
	public static ARUNResult _ARUNBridgeGetCurrentResult()
	{
		return new ARUNResult();
	}
#else
#	if UNITY_IPHONE
	[DllImport ("__Internal")]
#	endif
	public static extern ARUNResult _ARUNBridgeGetCurrentResult();
#endif

#if UNITY_EDITOR
	public static void _ARUNBridgeInitializeVisionEngine(CameraMotionMode mode) {}
#else
#	if UNITY_IPHONE
	[DllImport ("__Internal")]
#	endif
	public static extern void _ARUNBridgeInitializeVisionEngine(CameraMotionMode mode);
#endif
	
#if UNITY_EDITOR
	public static bool _ARUNBridgeStartVisionEngine() { return false; }
#else
#	if UNITY_IPHONE
		[DllImport ("__Internal")]
#	endif
		public static extern bool _ARUNBridgeStartVisionEngine();
#endif

#if UNITY_EDITOR
	public static bool _ARUNBridgeQuitVisionEngine() { return false; }
#else
#	if UNITY_IPHONE
	[DllImport ("__Internal")]
#	endif
	public static extern bool _ARUNBridgeQuitVisionEngine();
#endif
	
#if UNITY_EDITOR
	public static bool _ARUNBridgePauseVisionEngine() { return false; }
#else
#	if UNITY_IPHONE
	[DllImport ("__Internal")]
#	endif
	public static extern bool _ARUNBridgePauseVisionEngine();
#endif
	
#if UNITY_EDITOR
	public static bool _ARUNBridgeVisionIsInitialized() { return false; }
#else
#	if UNITY_IPHONE
		[DllImport ("__Internal")]
#	endif
		public static extern bool _ARUNBridgeVisionIsInitialized();
#endif
	
#if !UNITY_EDITOR
#	if UNITY_IOS
	[DllImport ("__Internal")]
#	endif
	public static extern string _GetVersionString();
#else
	public static string _GetVersionString()
	{
		return "N/A";
	}
#endif
}
