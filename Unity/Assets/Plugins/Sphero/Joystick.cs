using UnityEngine;
using System.Collections;

public class Joystick : MonoBehaviour {
	
	struct Boundary {
		public Vector2 min;
		public Vector2 max;
	};

	private int lastFingerId = -1;						// Finger last used for this joystick
	private bool firstTouch = true;

	public float velocityScale = 0.6f;					// The Max velocity the joystick can make Sphero go
	public GUITexture puck;								// Joystick puck graphic
	public GUITexture background;						// Joystick background graphic
	public float joystickScale;
	private Rect defaultRect;							// Default position / extents of the joystick graphic
	private Rect defaultBackRect;						// Default Background Rect
	private Boundary guiBoundary;						// Boundary for joystick graphic
	private Vector2 guiTouchOffset;						// Offset to apply to touch input
	private Vector2 guiCenter;							// Center of joystick

	private bool stopped = true;						// Whether Sphero is stopped or not
	private float lastHeading = 0.0f;					// The last heading Sphero traveled in
	private Vector2 startScreenSize;					// The size of the screen on start
	private Vector3 originalTransformPos;				// The original transform position
	
	/* List of connected Spheros */
	Sphero[] m_Spheros;
	
	// Use this for initialization
	void Start () {
		
		m_Spheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
		
		startScreenSize = new Vector2(Camera.main.pixelRect.width,Camera.main.pixelRect.height);
		originalTransformPos = transform.position;
		SetJoystickSize();
	}
	
	void Disable()
	{
		gameObject.active = false;
	}
	
	void ResetJoystick()
	{
		// Release the finger control and set the joystick back to the default position
		puck.pixelInset = defaultRect;
		background.pixelInset = defaultBackRect;
		lastFingerId = -1;
	}
	
	void SetJoystickSize() {
		transform.position = originalTransformPos;
		// Make the joystick scale a certain (joystickScale)% of the limiting dimension
		float limitingScaledDimension = Mathf.Min(startScreenSize.x,startScreenSize.y)*joystickScale;
		float puckScale = 0.32f;
		
		Rect backgroundRect = new Rect(-limitingScaledDimension*0.5f, -limitingScaledDimension*0.5f,
									   limitingScaledDimension, limitingScaledDimension);
		background.pixelInset = backgroundRect;
		Rect puckRect = new Rect(-limitingScaledDimension*0.5f*puckScale, -limitingScaledDimension*0.5f*puckScale,
								 limitingScaledDimension*puckScale, limitingScaledDimension*puckScale);
		puck.pixelInset = puckRect;
		
		// Store the default rect for the gui, so we can snap back to it
		defaultRect = puck.pixelInset;	
		defaultRect.x += transform.position.x * startScreenSize.x;
		defaultRect.y += transform.position.y * startScreenSize.y;
		
		defaultBackRect = background.pixelInset;
		defaultBackRect.x += transform.position.x * startScreenSize.x;
		defaultBackRect.y += transform.position.y * startScreenSize.y;
		transform.position = new Vector3(0.0f, 0.0f, 0.0f);
					
		// This is an offset for touch input to match with the top left
		// corner of the GUI
		guiTouchOffset.x = defaultRect.width * 0.5f;
		guiTouchOffset.y = defaultRect.height * 0.5f;
		
		// Cache the center of the GUI, since it doesn't change
		guiCenter.x = defaultRect.x + guiTouchOffset.x;
		guiCenter.y = defaultRect.y + guiTouchOffset.y;
		
		// Let's build the GUI boundary, so we can clamp joystick movement
		guiBoundary.min.x = defaultBackRect.x;
		guiBoundary.max.x = defaultBackRect.x + defaultBackRect.width;
		guiBoundary.min.y = defaultBackRect.y;
		guiBoundary.max.y = defaultBackRect.y + defaultBackRect.height;
	}
	
	bool IsFingerDown()
	{
		return (lastFingerId != -1);
	}
		
	void LatchedFinger(int fingerId)
	{
		// If another joystick has latched this finger, then we must release it
		if ( lastFingerId == fingerId )
			ResetJoystick();
	}
	
	// Update is called once per frame
	void Update () {
		
		// Check for screen size changes
		if( startScreenSize.x != Camera.main.pixelRect.width || startScreenSize.y != Camera.main.pixelRect.height ) {
			startScreenSize = new Vector2(Camera.main.pixelRect.width,Camera.main.pixelRect.height);
			SetJoystickSize();
		}
			
		int count = Input.touchCount;
		float headingRad = 0;
		float touchRadius = 0;
		float radius = (guiBoundary.max.y - guiBoundary.min.y)*0.5f;
		
		if ( count == 0 ) {
			if(stopped == false) {
				stopped = true;
				foreach( Sphero sphero in m_Spheros ) {
					sphero.Roll((int)sphero.ApplyCalibrationToHeading(lastHeading), 0.0f);	
				}
			}
			firstTouch = true;
			ResetJoystick();
		}
		else
		{
			for(int i =0; i < count; i++)
			{
				Touch touch = Input.GetTouch(i);	
		
				bool shouldLatchFinger = false;
				if ( puck.HitTest( touch.position ) && firstTouch )
				{
					shouldLatchFinger = true;
				}		
		
				// Latch the finger if this is a new touch
				if ( shouldLatchFinger && ( lastFingerId == -1 || lastFingerId != touch.fingerId ) )
				{
					lastFingerId = touch.fingerId;					
				}				
		
				if ( lastFingerId == touch.fingerId )
				{	
					// Change the location of the joystick graphic to match where the touch is
					Vector2 clampedPosition = touch.position - guiTouchOffset;
					
					headingRad = Mathf.Atan2(touch.position.y-guiCenter.y, touch.position.x-guiCenter.x);
					touchRadius = (touch.position-guiCenter).magnitude;
					
					// Clamp circular boundaries	
					if( (touch.position-guiCenter).magnitude > radius ) {
						Vector2 offset = new Vector2(radius*Mathf.Cos(headingRad),radius*Mathf.Sin(headingRad));
						clampedPosition = guiCenter+offset-guiTouchOffset; 
					}
						
					puck.pixelInset = new Rect(clampedPosition.x, clampedPosition.y, puck.pixelInset.width, puck.pixelInset.height);
					
					// Convert to Sphero heading
					headingRad = (headingRad + Mathf.PI*2.0f) % (Mathf.PI*2.0f);
					headingRad -= Mathf.PI * 0.5f;
					if( headingRad < 0 )
					{
						headingRad += Mathf.PI * 2.0f;
					}
				
					float degrees = 360 - (Mathf.Rad2Deg * headingRad);
					float velocity = touchRadius / radius;
					velocity = velocity * velocityScale;
					if(velocity > velocityScale) {
						velocity = velocityScale;
					}
					if(velocity > 0.0f) {					
						foreach( Sphero sphero in m_Spheros ) {
							sphero.Roll(
								(int)sphero.ApplyCalibrationToHeading(degrees),
								velocity);
						}
						stopped = false;
						lastHeading = degrees;
					}
					
					if ( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled )
						ResetJoystick();					
				}	
				
				firstTouch = false;
			}
		}		
	}
}
