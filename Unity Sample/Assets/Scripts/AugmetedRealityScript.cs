using UnityEngine;
using System.Collections;

public class AugmetedRealityScript : MonoBehaviour {
	
	private string ButtonText = "Turn Vision Off";

	[SerializeField]
	private bool m_ignoreConnectionSceneInEditor = true;
	
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
			else spheros[0].SetHeading(0);
		}

		if( ARUNBridge._ARUNBridgeStartVisionEngine() ) {
			//Debug.Log("Vision Started!");
		}
		else {
			//Debug.Log("Vision Failed to Start :(");
		}
	}
	
	/* This is called when the application returns from background or entered from NoSpheroConnectionScene */
	void OnApplicationPause(bool pause) {	
		if( pause ) {
			// Initialize the device messenger which sets up the callback
			SpheroProvider.GetSharedProvider().DisconnectSpheros();
			ARUNBridge._ARUNBridgeQuitVisionEngine();
		}
		else {
			ViewSetup();
		}
	}

	// Use this for initialization
	void Start () {
		ViewSetup();
	}
}
