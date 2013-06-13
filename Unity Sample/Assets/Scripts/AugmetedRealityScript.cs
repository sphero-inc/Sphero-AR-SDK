using UnityEngine;
using System.Collections;

public class AugmetedRealityScript : MonoBehaviour {

	[SerializeField]
	private bool m_ignoreConnectionSceneInEditor = true;

	private static bool s_visionInitialized = false;

	/* Use this for initialization */
	void ViewSetup()
	{
#if UNITY_EDITOR
		if (!m_ignoreConnectionSceneInEditor)
#endif
		{
			// Get Connected Sphero
			Sphero[] spheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
			if( spheros.Length == 0 ) {
				Application.LoadLevel("SpheroConnectionScene");
				return;
			}
			else 
			{
				spheros[0].SetRGBLED(1,1,1);
				spheros[0].Roll(0, 0f);
				//spheros[0].SetHeading(0);
			}
		}

		if( !ARUNBridge._ARUNBridgeStartVisionEngine() ) {
			Debug.LogError("Vision Failed to Start :(");
		}
	}
	
	/* This is called when the application returns from background or entered from NoSpheroConnectionScene */
	void OnApplicationPause(bool pause) 
	{
#if !UNITY_EDITOR
		if( pause )
		{
			ARUNBridge._ARUNBridgePauseVisionEngine();
		}
		else
		{			
			ARUNBridge._ARUNBridgeStartVisionEngine();	
			SpheroProvider.GetSharedProvider().Connect(0);
			StartCoroutine(DelaySpheroConnection());
		}
#endif
	}
	
	IEnumerator DelaySpheroConnection()
	{
		yield return new WaitForSeconds(1.0f); 
		Sphero[] spheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
		spheros[0].SetRGBLED(1,1,1);
	}
	
	// Use this for initialization
	void Start () {
		if (!s_visionInitialized)
		{
			ARUNBridge._ARUNBridgeInitializeVisionEngine(ARUNBridge.CameraMotionMode.StaticHeight);
			s_visionInitialized = true;
		}
		ViewSetup();
	}
}
