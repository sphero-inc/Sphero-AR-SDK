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
			if( spheros.Length == 0 ) Application.LoadLevel("SpheroConnectionScene");
			else spheros[0].SetHeading(0);
		}

		if( ARUNBridge._ARUNBridgeStartVisionEngine() ) {
			//Debug.Log("Vision Started!");
		}
		else {
			//Debug.Log("Vision Failed to Start :(");
		}
	}
	
	void OnDestroy()
	{
#if !UNITY_EDITOR
		SpheroProvider.GetSharedProvider().DisconnectSpheros();
#endif
	}

	// Use this for initialization
	void Start () {
		ViewSetup();
	}
}
