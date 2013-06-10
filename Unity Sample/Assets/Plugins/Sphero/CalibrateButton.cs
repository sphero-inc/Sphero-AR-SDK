using UnityEngine;
using System.Collections;

public class CalibrateButton : MonoBehaviour {
	
	private int lastFingerId = -1;						// Finger last used for this joystick
	private bool firstTouch = true;
	private bool isCalibrating = false;
	private int currentHeading = 0;

	/* Size of the button as a percentage of the screen */
	public float buttonScale = 0.1f;
	public float ringScale = 0.7f;
	
	/* Graphics that are grabbed from the game object */
	public GUITexture buttonBackground;					// Button Background Texture
	public GUITexture buttonForeground;					// Button Foreground Texture
	public GUITexture innerRing;						// Overlay image inner ring
	public GUITexture middleRing;						// Overlay image middle ring
	public GUITexture outerRing;						// Overlay image outer ring
	
	private Rect ringRect;								// Rect of the biggest ring (absolute coords)
	private Rect guiOuterRingRect;						// Rect of the outer ring (gui absolute coords)
	private Rect guiInnerRingRect;						// Rect of the inner ring (gui absolute coords)
	private Vector2 ringCenter;							// Center of rings (absolute coords)
	private Vector2 guiRingCenter;						// Center of rings (gui absolute coords)
	private Rect buttonRect;							// Rect of the calibrate button (absolute coords)
	
	private Vector2 startScreenSize;					// The size of the screen on start
	private Vector3 originalTransformPos;				// The original transform position
	
	/* List of connected Spheros */
	Sphero[] m_Spheros;
	
	// Use this for initialization
	void Start () {
		m_Spheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
		
		startScreenSize = new Vector2(Screen.width,Screen.height);
		originalTransformPos = transform.position;
		SetCalibrateButtonSize();
	}
	
	void Disable()
	{
		gameObject.active = false;
	}
	
	void SetCalibrateButtonSize() {
		
		// Go to absolute coordinates to deal with touch events
		transform.position = new Vector3(0.0f, 0.0f, 0.0f);
		
		// Make Rings invisible
		SetRingsVisibility(false);
		innerRing.gameObject.active = false;	
		outerRing.gameObject.active = false;
		
		// Limiting size that is reference for the calibrate button textures
		float limitingSize = Mathf.Min(Screen.width,Screen.height);
		
		// Set the button sizes
		float buttonSize = limitingSize*buttonScale;
		Vector2 buttonMid = new Vector2(originalTransformPos.x*Screen.width,originalTransformPos.y*Screen.height);
		Rect buttonRect = new Rect(buttonMid.x-buttonSize*0.5f,buttonMid.y-buttonSize*0.5f, buttonSize, buttonSize);
		buttonBackground.pixelInset = buttonRect;
		buttonForeground.pixelInset = buttonRect;
		
		// Set the Rings size
		float ringSize = limitingSize*ringScale;
		Rect ringRect = new Rect(buttonMid.x-ringSize*0.5f,buttonRect.y,ringSize,ringSize);
		outerRing.pixelInset = ringRect;  
		guiOuterRingRect = new Rect(buttonMid.x-ringSize*0.5f,(Screen.height - buttonRect.y)-ringSize, ringSize,ringSize);
		float midSize = ringSize*0.88235f;
		middleRing.pixelInset = new Rect(buttonMid.x-midSize*0.5f,buttonRect.y+(ringSize-midSize)*0.5f,midSize,midSize);
		float innerSize = ringSize*0.9f;
		innerRing.pixelInset = new Rect(buttonMid.x-innerSize*0.5f,buttonRect.y+(ringSize-innerSize)*0.5f,innerSize,innerSize);
		guiInnerRingRect = new Rect(buttonMid.x-innerSize*0.5f,Screen.height - (buttonRect.y+(ringSize-innerSize)*0.5f) - innerSize,innerSize,innerSize);
		
		// Store the center of these elements for future access
		ringCenter = new Vector2(ringRect.x+ringRect.width*0.5f,ringRect.y+ringRect.height*0.5f);
		guiRingCenter = new Vector2(ringRect.x+ringRect.width*0.5f, (Screen.height - ringRect.y) - ringRect.height*0.5f);
	}
	
	/* Set the GUI rings' visibility */
	void SetRingsVisibility(bool visible) {
		middleRing.gameObject.active = visible;
	}
	
	bool IsFingerDown()
	{
		return (lastFingerId != -1);
	}
	
	// Update is called once per frame
	void Update () {
		
		// Check for screen size changes
		if( Screen.width != startScreenSize.x || Screen.height != startScreenSize.y ) {
			SetCalibrateButtonSize();
			startScreenSize = new Vector2(Screen.width,Screen.height);
		}
			
		int count = Input.touchCount;
		
		if ( count == 0 ) {
			// Stop Calibrating Spheros
			if( isCalibrating ) {
				foreach( Sphero sphero in m_Spheros ) {
					sphero.SetBackLED(0.0f);
					ApplyHeading(currentHeading);
					//sphero.SetHeading(0);					
				}
			}
			isCalibrating = false;
			firstTouch = true;
			SetRingsVisibility(false);
		}
		else
		{
			for(int i =0; i < count; i++)
			{
				Touch touch = Input.GetTouch(i);	
		
				bool shouldLatchFinger = false;
				if ( buttonBackground.HitTest( touch.position ) && firstTouch )
				{
					isCalibrating = true;
					// Start calibrating Sphero and turn on BackLED
					foreach( Sphero sphero in m_Spheros ) {
						sphero.SetBackLED(1.0f);							
						//sphero.SetHeading(0);
					}
					SetRingsVisibility(true);
					shouldLatchFinger = true;
				}		
		
				// Latch the finger if this is a new touch
				if ( shouldLatchFinger && ( lastFingerId == -1 || lastFingerId != touch.fingerId ) )
				{
					lastFingerId = touch.fingerId;					
				}				
		
				if ( lastFingerId == touch.fingerId && isCalibrating )
				{	
					// Find the new angle of the touch in reference to the center of the rings
					Vector2 localTouch = touch.position - ringCenter;
					float headingRad = Mathf.Atan2(localTouch.y, localTouch.x);
					
					// Convert to Sphero heading (0 degrees on -y axis)
					headingRad = (headingRad + Mathf.PI*2.5f) % (Mathf.PI*2.0f);
					float degrees = 360 - (Mathf.Rad2Deg * headingRad);
					
					// Turn Sphero
					foreach( Sphero sphero in m_Spheros ) {		
						currentHeading = (int)CapAngle(sphero.CalibrationOffset+degrees);
						sphero.Roll(currentHeading,0.0f);	
					}				
				}	
				
				firstTouch = false;
			}
		}
	}
	
	/*!
	 * @brief	applies the given heading
	 * @param	headingDegrees the heading to apply (for RobotVision only)
	 */
	private void ApplyHeading(float headingDegrees)
	{		
		foreach( Sphero sphero in m_Spheros ) {
			sphero.CalibrationOffset = CapAngle(headingDegrees);
			sphero.CalibrationGyroOffset = Input.gyro.attitude.eulerAngles.z;
		}
	}
	
	private float CapAngle(float angleDegrees)
	{
		float outAngle = angleDegrees % 360.0f;
		if (outAngle < 0.0f)
		{
			outAngle += 360.0f;
		}
		return outAngle;
	}
	
	void OnGUI() {
		// If we are calibrating, darken the screen and show rotating calibration widget
		if( isCalibrating ) {
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"");
	        Matrix4x4 matrixBackup = GUI.matrix;
	        GUIUtility.RotateAroundPivot(currentHeading, guiRingCenter);
	        GUI.DrawTexture(guiOuterRingRect, outerRing.texture);
	        GUI.matrix = matrixBackup;
			GUIUtility.RotateAroundPivot(-currentHeading, guiRingCenter);
	        GUI.DrawTexture(guiInnerRingRect, innerRing.texture);
		}
	}
}
