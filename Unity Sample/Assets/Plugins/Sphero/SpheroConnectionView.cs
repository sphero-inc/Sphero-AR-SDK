using UnityEngine;
using System.Collections;

public class SpheroConnectionView : MonoBehaviour {
	
	// Next Level to load after the balls connect
	public string m_NextLevel;
	
	// Controls how many Spheros you can connect to (Android only)
	public bool m_MultipleSpheros;
	
	// Controls the look and feel of the Connection Scene
	public GUISkin m_SpheroConnectionSkin;	
	
	// Loading image
	public Texture2D m_Spinner;
	Vector2 m_SpinnerSize = new Vector2(128, 128);
	float m_SpinnerAngle = 0;
	Vector2 m_SpinnerPosition = new Vector2(0, 0);
	Vector2 m_SpinnerPivotPos = new Vector2(0, 0);
	Rect m_SpinnerRect;
	
	// Other icons
	public Texture2D m_CheckMark;
	public Texture2D m_RedX;
	public Texture2D m_SplashScreen;   // Used only when iOS is connecting
	
	// UI Padding Variables
	int m_ViewPadding = 20;
	int m_ElementPadding = 10;
	
	// Button Size Variables
	int m_ButtonWidth = 200;
	int m_ButtonHeight = 55;
	
	// Title Variables
	int m_TitleHeight = 40;
	string m_Title;
	
	// Sphero Name Label Variable
	int m_SpheroLabelWidth = 250;
	int m_SpheroLabelHeight = 60;
	int m_SpheroLabelSelected = -1;
	
	// Sphero Provider
	SpheroProvider m_SpheroProvider;
	// Paired Sphero Info
	string[] m_RobotNames;
	string m_ConnectingRobotName;
	
    // Internal variables for managing touches and drags
	private int selected = -1;
	private float scrollVelocity = 0f;
	private float timeTouchPhaseEnded = 0f;
	private const float inertiaDuration = 0.5f;
	
    Vector2 scrollPosition;

	// size of the window and scrollable list
    Vector2 windowMargin = new Vector2(0,0);
    Vector2 listMargin = new Vector2(40,40);
    private Rect windowRect;
	
	/* Use this to initialize the view */
	private void ViewSetup() {
		m_SpheroProvider = SpheroProvider.GetSharedProvider();
		#if UNITY_ANDROID
			SetupAndroid();
		#elif UNITY_IPHONE
			SetupIOS();
		#else
			// Display that it doesn't work with these platforms?
		#endif
	}
	
	/* Use these for initialization */
	void Start () {	
		ViewSetup();
	}
	
	/* This is called when the application returns from background or entered from NoSpheroConnectionScene */
	void OnApplicationPause(bool pause) {
		if( pause ) {
			// Initialize the device messenger which sets up the callback
			SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
			m_SpheroProvider.DisconnectSpheros();
		}
		else {
			ViewSetup();
		}
	}
	
	/*
	 * Called if the OS is iOS to immediately try to connect to the robot
	 */
	void SetupIOS() {
		m_MultipleSpheros = false;
		// Initialize the device messenger which sets up the callback
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
		SpheroProvider.GetSharedProvider().Connect(0);
		Invoke("CheckForSpheroConnection",1.5f);
	}
	
	/*
	 * Check if a Sphero has connected or not.  If not, load the No Sphero Connected Scene 
	 */
	void CheckForSpheroConnection() {
		if( m_SpheroProvider.GetConnectedSpheros().Length == 0 ) {
			Application.LoadLevel("NoSpheroConnectedScene");	
		}
	}

	/*
	 * Called if the OS is Android to show the Connection Scene
	 */
	void SetupAndroid() {
		
		// initialize the Sphero Provider (Cannot call in the initialization of member variables or you will get a crash!)
		m_SpheroProvider = SpheroProvider.GetSharedProvider();
		
		// Search for paired robots
		if( !m_SpheroProvider.IsAdapterEnabled() ) {
			m_Title = "Bluetooth Not Enabled";
			m_RobotNames = new string[0];
		}
		else {
			m_Title = "Connect to a Sphero";
			// Refreshes the list of robots
			m_SpheroProvider.FindRobots();
			
			// Initialize the device messenger which sets up the callback
			SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
	
			// Make the spinner smaller to appear next to the clickable list
			if( m_MultipleSpheros ) {
				m_SpinnerSize = new Vector2(m_SpheroLabelHeight-10, m_SpheroLabelHeight-10);	
			}	
			m_RobotNames = m_SpheroProvider.GetRobotNames();
			// Sphero Provider will try and connect to the first robot, so show that progress
			if( m_RobotNames.Length == 1 ) {
				m_SpheroProvider.PairedSpheros[0].ConnectionState = Sphero.Connection_State.Connecting;
				m_SpheroLabelSelected = 0;
				m_ConnectingRobotName = m_SpheroProvider.PairedSpheros[0].DeviceInfo.Name;
			}
		}
	}
	
	/*
	 * Callback to receive connection notifications 
	 */
	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		SpheroDeviceNotification message = (SpheroDeviceNotification)eventArgs.Message;
		if( message.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTED ) {
			
			// Connect to the robot and move to the next scene designated by the developer
			if( !m_MultipleSpheros ) {
				m_Title = "Connection Success";
				SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
				Application.LoadLevel(m_NextLevel); 
			}
		}
		else if( message.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTION_FAILED ) {
			Sphero notifiedSphero = m_SpheroProvider.GetSphero(message.RobotID);
			// Connection only has failed if we are trying to connect to that robot the notification belongs to
			if( m_ConnectingRobotName.Equals(notifiedSphero.DeviceInfo.Name)) {
				m_Title = "Connection Failed";
			}
		}
	}
	
	// Update is called once per frame
 	void Update()
    {
#if UNITY_ANDROID
		if (Input.touchCount != 1)
		{
			selected = -1;

			if ( scrollVelocity != 0.0f )
			{
				// slow down over time
				float t = (Time.time - timeTouchPhaseEnded) / inertiaDuration;
				float frameVelocity = Mathf.Lerp(scrollVelocity, 0, t);
				scrollPosition.y += frameVelocity * Time.deltaTime;
				
				// after N seconds, we've stopped
				if (t >= inertiaDuration) scrollVelocity = 0.0f;
			}
			return;
		}
		
		Touch touch = Input.touches[0];
		if (touch.phase == TouchPhase.Began)
		{
			selected = TouchToRowIndex(touch.position);
			scrollVelocity = 0.0f;
		}
		else if (touch.phase == TouchPhase.Canceled)
		{
			selected = -1;
		}
		else if (touch.phase == TouchPhase.Moved)
		{
			// dragging
			selected = -1;
			scrollPosition.y += touch.deltaPosition.y;
		}
		else if (touch.phase == TouchPhase.Ended)
		{
            // Was it a tap, or a drag-release?
            if ( selected > -1 && selected < m_RobotNames.Length)
            {
	            Debug.Log("Player selected row " + selected);
				// Sweet!
				if( m_MultipleSpheros ) {
					ConnectSphero(selected);
				}
            }
			else
			{
				// impart momentum, using last delta as the starting velocity
				// ignore delta < 10; precision issues can cause ultra-high velocity
				if (Mathf.Abs(touch.deltaPosition.y) >= 10) 
					scrollVelocity = (int)(touch.deltaPosition.y / touch.deltaTime);
				timeTouchPhaseEnded = Time.time;
			}
		}
#endif		
	}
	
	/*
	 * Attempt to connect to a Sphero 
	 */
	void ConnectSphero(int row) {
		// Don't connect to more than one at a time
		if( m_SpheroProvider.GetConnectingSphero() != null ||
			m_SpheroProvider.PairedSpheros[row].ConnectionState == Sphero.Connection_State.Connected ) return;
		
		m_SpheroProvider.Connect(row);
		// Adjust title info
		m_SpheroLabelSelected = row;
		m_Title = "Connecting to " + m_RobotNames[m_SpheroLabelSelected];	
		m_ConnectingRobotName = m_RobotNames[m_SpheroLabelSelected];
	}
	
	// Called when the GUI should update
	void OnGUI() {
#if UNITY_ANDROID
		
		if( m_RobotNames == null ) return;
		GUI.skin = m_SpheroConnectionSkin;
		
		// Draw a title lable
		GUI.Label(new Rect(m_ViewPadding,m_ViewPadding,Screen.width-(m_ViewPadding*2),m_TitleHeight), m_Title, "label");
		
		// Disable interface if we are trying to connect to a robot
		if( m_SpheroProvider.GetConnectingSphero() != null ) GUI.enabled = false;
		
		// Set up the scroll view that holds all the Sphero names
		int scrollY = m_ViewPadding + m_TitleHeight + m_ElementPadding*2;
        windowMargin = new Vector2(m_ViewPadding,scrollY);
        windowRect = new Rect(windowMargin.x, windowMargin.y-15, 
        				 Screen.width - (2*windowMargin.x), Screen.height - (2*windowMargin.y));
        GUI.Window(0, windowRect, (GUI.WindowFunction)DoWindow, "");
		
		// Set up the Connect or Done Button
		string buttonLabel = "Connect";
		if( m_MultipleSpheros ) {
			buttonLabel = "Done";	
		}
		int connectBtnX = (Screen.width/2)-(m_ButtonWidth/2);
		int connectBtnY = Screen.height-m_ViewPadding-m_ButtonHeight;
		
		// Check if Spheros are connected (only on multiple Sphero mode)
		if( m_MultipleSpheros && m_SpheroProvider.GetConnectedSpheros().Length == 0 ) {
			GUI.enabled = false;
		}
		// Draw Button at the bottom
		if( GUI.Button(new Rect(connectBtnX,connectBtnY,m_ButtonWidth,m_ButtonHeight), buttonLabel )) {
			// Check if we are done adding robots
			if( buttonLabel.Equals("Done") ){
				SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
				Application.LoadLevel(m_NextLevel); 	
			}
			// Check if we have a Sphero connected
			else if( m_SpheroLabelSelected >= 0 ) {
				ConnectSphero(m_SpheroLabelSelected);	
			}
		}			
		GUI.enabled = true;
		
		// Only show the connection dialog if we are connecting to a robot
		if( !m_MultipleSpheros && m_SpheroProvider.GetConnectingSphero() != null ) {
			m_SpinnerPosition.x = Screen.width/2;
			m_SpinnerPosition.y = Screen.height/2;
			// Rotate the object
			m_SpinnerRect = new Rect(m_SpinnerPosition.x - m_SpinnerSize.x * 0.5f, m_SpinnerPosition.y - m_SpinnerSize.y * 0.5f, m_SpinnerSize.x, m_SpinnerSize.y);
        	m_SpinnerPivotPos = new Vector2(m_SpinnerRect.xMin + m_SpinnerRect.width * 0.5f, m_SpinnerRect.yMin + m_SpinnerRect.height * 0.5f);
			
			GUI.Box(m_SpinnerRect,"");
			
			// Draw the new image
	        Matrix4x4 matrixBackup = GUI.matrix;
	        GUIUtility.RotateAroundPivot(m_SpinnerAngle, m_SpinnerPivotPos);
	        GUI.DrawTexture(m_SpinnerRect, m_Spinner);
	        GUI.matrix = matrixBackup;
			m_SpinnerAngle+=3;
		}
#else
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),m_SplashScreen);
#endif
	}

	void DoWindow (int windowID) 
	{
		Vector2 listSize = new Vector2(windowRect.width - 2*listMargin.x,
									   windowRect.height - 2*listMargin.y);

		Rect rScrollFrame = new Rect(listMargin.x, listMargin.y, listSize.x, listSize.y);
		Rect rList = new Rect(0, 0, m_SpheroLabelWidth, m_RobotNames.Length*m_SpheroLabelHeight);
		
        scrollPosition = GUI.BeginScrollView (rScrollFrame, scrollPosition, rList, false, false);
            
		// Show a list of Spheros that you can connect to
		if( m_MultipleSpheros ) {
			// Create rows of spinners
			for( int i = 0; i < m_RobotNames.Length; i++ ) {
				m_SpinnerPosition.x = (m_SpinnerSize.x/2);
				m_SpinnerPosition.y = (i*m_SpheroLabelHeight)+(m_SpinnerSize.y/2);
				// Rotate the object
				m_SpinnerRect = new Rect(m_SpinnerPosition.x - m_SpinnerSize.x * 0.5f, m_SpinnerPosition.y - m_SpinnerSize.y * 0.5f, m_SpinnerSize.x, m_SpinnerSize.y);
	        	m_SpinnerPivotPos = new Vector2(m_SpinnerRect.xMin + m_SpinnerRect.width * 0.5f, m_SpinnerRect.yMin + m_SpinnerRect.height * 0.5f);
				
				Sphero sphero = m_SpheroProvider.PairedSpheros[i];
				// Draw the spinner rotating if it is connecting
				if( sphero.ConnectionState == Sphero.Connection_State.Connecting ) {
			        Matrix4x4 matrixBackup = GUI.matrix;
			        GUIUtility.RotateAroundPivot(m_SpinnerAngle, m_SpinnerPivotPos);
			        GUI.DrawTexture(m_SpinnerRect, m_Spinner);
			        GUI.matrix = matrixBackup;
					m_SpinnerAngle+=3;
				}
				else if( sphero.ConnectionState == Sphero.Connection_State.Connected ) {
					GUI.DrawTexture(m_SpinnerRect, m_CheckMark);
				}
				else if( sphero.ConnectionState == Sphero.Connection_State.Failed ) {
					GUI.DrawTexture(m_SpinnerRect, m_RedX);
				}
				// Otherwise draw it normally
				else {
					GUI.DrawTexture(m_SpinnerRect, m_Spinner);
				}

				// Draw the Sphero Label
				GUI.Label(new Rect(m_SpinnerSize.x, i*m_SpheroLabelHeight, m_SpheroLabelWidth, m_SpheroLabelHeight), m_RobotNames[i]);
			}
		}
		// Show a grid of potential Spheros to connect to (only can connect to one)
		else {
 			m_SpheroLabelSelected = GUI.SelectionGrid(new Rect(0,0,m_SpheroLabelWidth,m_SpheroLabelHeight*m_RobotNames.Length),m_SpheroLabelSelected,m_RobotNames,1,"toggle");
		}
		
        GUI.EndScrollView();
	}
	
    private int TouchToRowIndex(Vector2 touchPos)
    {
		float y = Screen.height - touchPos.y;  // invert coordinates
		y += scrollPosition.y;  // adjust for scroll position
		y -= windowMargin.y;    // adjust for window y offset
		y -= listMargin.y;      // adjust for scrolling list offset within the window
		int irow = (int)(y / m_SpheroLabelHeight);
		
		irow = Mathf.Min(irow, m_RobotNames.Length);  // they might have touched beyond last row
		return irow;
    }
}
