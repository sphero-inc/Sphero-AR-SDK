using UnityEngine;
using System.Collections;

[AddComponentMenu("RobotVision/AureController")]

/*!
 * @brief	handles synchronizing calls with vision by providing messages that execute before, during, and after positioning of
 *			vision elements (tracker/camera)
 */
public class ARUNController : SingletonBehaviour<ARUNController>
{
	//! @brief	callback when vision is about to update
	public delegate void VisionWillUpdate();

	//! @brief	callback when vision is updating
	public delegate void VisionUpdate();

	//! @brief	callback immediately after vision updates
	public delegate void VisionUpdateComplete();

	/*!
	 * @brief	callback when the tracking state changes
	 * @param	newState the new state
	 */
	public delegate void TrackingStateChanged(ARUNBridge.FindSpheroState newState);

	public VisionWillUpdate OnVisionWillUpdate;			//!< @brief	callback for vision about to update
	public VisionUpdate OnVisionUpdate;					//!< @brief	callback for vision updating
	public VisionUpdateComplete OnVisionUpdateComplete;	//!< @brief	callback for vision done updating
	public TrackingStateChanged OnTrackingStateChanged;	//!< @brief	callback for the tracking state changed

	private ARUNBridge.FindSpheroState m_lastTrackingState = ARUNBridge.FindSpheroState.NoInformation;

	// Update is called once per frame
	void Update ()
	{
		if( !ARUNBridge._ARUNBridgeVisionIsInitialized() )
		{
			return;
		}
		
		ARUNBridge.UpdateArResults();
		
		ARUNBridge.FindSpheroState newState = ARUNBridge.CurrentARResult.SpheroTrackingState;
		if (m_lastTrackingState != newState)
		{
			if (OnTrackingStateChanged != null)
			{
				OnTrackingStateChanged(newState);
			}
			m_lastTrackingState = newState;
		}
		
		if (OnVisionWillUpdate != null)
		{
			OnVisionWillUpdate();
		}

		if (OnVisionUpdate != null)
		{
			OnVisionUpdate();
		}

		if (OnVisionUpdateComplete != null)
		{
			OnVisionUpdateComplete();
		}
	}
}
	