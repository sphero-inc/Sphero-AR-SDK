using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
//using ARUNBridge;

[AddComponentMenu("RobotVision/CameraScript")]

public class CameraScript : MonoBehaviour
{
	private static float kFov = 16.7f;

	private float m_lastLeft = 0.0f;
	private float m_lastTop = 0.0f;
	
	// Use this for initialization
	void Start ()
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;

		ARUNController controller = ARUNController.Instance;
		if (controller != null)
		{
			controller.OnVisionUpdate += OnVisionUpdate;
		}
		else
		{
			Debug.LogError("AureController.Instance not set");
		}
		
		Camera[] cameras = GetComponentsInChildren<Camera>();
		foreach(Camera cam in cameras)
		{
			cam.fieldOfView = kFov * 2f;
			cam.near = 0.05f;
			cam.far = 1000f;
		}
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
	}
	
	// OnPreCull is called just before rendering begins
	private void OnVisionUpdate () {
#if !UNITY_EDITOR
		Quaternion orientation;
		Vector3 position;
		VisionUtils.GetCameraTransform(out position, out orientation);
		transform.localRotation = orientation;
		transform.localPosition = position;

		RegenerateProjectionIfNecessary();
#else
#endif
	}

	/*!
	 * @brief	regenerates the projection matrix if necessary
	 */
	private void RegenerateProjectionIfNecessary()
	{
		ARUNBridge.AuPlatformConfiguration config = ARUNBridge.CurrentARResult.CameraConfigurationSettings;
		if (ConfigurationNeedsUpdating(ref config))
		{
			Camera[] cameras = GetComponentsInChildren<Camera>();
			foreach(Camera cam in cameras)
			{
				SetCameraToConfig(ref config, cam);
			}

			m_lastTop = config.projFrustTop;
			m_lastLeft = config.projFrustLeft;
		}
	}

	/*!
	 * @brief	sets the camera to match the given configuration
	 * @param	config the configuration to match
	 * @param	cam the camera whose projection paramters will be modified
	 */
	private void SetCameraToConfig(ref ARUNBridge.AuPlatformConfiguration config, Camera cam)
	{
		cam.orthographic = false;
		cam.nearClipPlane = config.projFrustNear;
		cam.projectionMatrix = ARUNMath.PerspectiveProjectionFromCameraGeometry(
			ref config,
			cam.farClipPlane);
	}
	
	/*!
	 * @brief	determines if the camera geometry must be updated from @a config
	 * @param	config configuration struct describing the current camera gometry
	 * @return 	true if the camera proje tion needs updating
	 */
	private bool ConfigurationNeedsUpdating(ref ARUNBridge.AuPlatformConfiguration config)
	{
		return config.projFrustTop != m_lastTop || config.projFrustLeft != m_lastLeft;
	}
}
