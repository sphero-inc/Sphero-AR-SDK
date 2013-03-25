using UnityEngine;
using System.Collections;

public class NoSpheroConnectedView : MonoBehaviour {
	
	// Scene to load after iOS connection success
	public string m_NextLevel;
	
	// Controls the look and feel of the Connection Scene
	public GUISkin m_GetASpheroButtonSkin;	
	public GUISkin m_ConnectButtonSkin;	
	
	// NoSpheroConnected Image
	public Texture2D m_Background;
	public string m_GetASpheroURL = "http://gosphero.com";
	public Texture2D m_GetASpheroTexture;
	public Texture2D m_ConnectTexture;
	float m_BackgroundAspectRatio;
	
	// Loading image
	public Texture2D m_Spinner;
	private Vector2 m_SpinnerSize = new Vector2(128, 128);
	private float m_SpinnerAngle = 0;
	private Vector2 m_SpinnerPosition = new Vector2(0, 0);
	private Vector2 m_SpinnerPivotPos = new Vector2(0, 0);
	private Rect m_SpinnerRect;
	
	// UI Padding Variables
	int m_ViewPadding = 20;
	
	void Start () {	
		ViewSetup();
	}
	
	/* Use this for initialization */
	void ViewSetup() {
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
		#if UNITY_IPHONE
			// Connect first Sphero available on iOS
			SpheroProvider.GetSharedProvider().Connect(0);	
		#endif
	}
	
	/* This is called when the application returns from or enters background */
	void OnApplicationPause(bool pause) {
		if( pause ) {
			SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
		}
		else {
			ViewSetup();
		}
	}
	
	/*
	 * Callback to receive connection notifications 
	 */
	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		SpheroDeviceNotification message = (SpheroDeviceNotification)eventArgs.Message;
		if( message.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTED ) {
			// Go to the desired scene
			Application.LoadLevel (m_NextLevel); 
		}
	}
	
	// Update is called once per frame
 	void Update() {}
	
	// Called when the GUI should update
	void OnGUI() {
		
		GUI.skin = m_GetASpheroButtonSkin;
		
		// Draw No Sphero Connected Background
		int backgroundWidth = 0;
		int backgroundHeight = 0;
		m_BackgroundAspectRatio = (float)m_Background.height / (float)m_Background.width;
		// height is the limiting dimension
		if( m_BackgroundAspectRatio > ((float)Screen.height / (float)Screen.width) ) {
			backgroundHeight = Screen.height - (m_ViewPadding*2);
			backgroundWidth = (int)(((float)backgroundHeight / (float)m_Background.height) * m_Background.width);
		}
		// Width is the limiting dimension
		else {
			backgroundWidth = Screen.width - (m_ViewPadding*2);
			backgroundHeight = (int)(((float)backgroundWidth / (float)m_Background.width) * m_Background.height);
		}
		int backgroundX = (Screen.width/2)-(backgroundWidth/2);
		int backgroundY = (Screen.height/2)-(backgroundHeight/2);
		Rect backgroundRect = new Rect(backgroundX,backgroundY,backgroundWidth,backgroundHeight);
		GUI.DrawTexture(backgroundRect, m_Background);
		
		// Draw the Get A Sphero Button
		float buttonWidth = backgroundWidth*0.3f;
		float buttonHeight = ((backgroundWidth*0.25f)/(float)m_GetASpheroTexture.width)*m_GetASpheroTexture.height;
		float getASpheroButtonX = backgroundX+(backgroundWidth*0.605f);
		float getASpheroButtonY = backgroundY+(backgroundHeight*0.85f) - (buttonHeight/2);
		// If the get a Sphero button is clicked
		if( GUI.Button (new Rect(getASpheroButtonX, getASpheroButtonY,buttonWidth,buttonHeight), "") ) {
			Application.OpenURL(m_GetASpheroURL);
		}
		
#if UNITY_IPHONE		
		// iOS has a spinner, and Android has an extra button
		int spinnerDim = (int)(backgroundWidth*0.07);
		m_SpinnerSize = new Vector2(spinnerDim,spinnerDim);
		m_SpinnerPosition.x = backgroundX+(backgroundWidth*0.115f);
		m_SpinnerPosition.y = backgroundY+(backgroundHeight*0.85f);
		// Rotate the object
		m_SpinnerRect = new Rect(m_SpinnerPosition.x - m_SpinnerSize.x * 0.5f, m_SpinnerPosition.y - m_SpinnerSize.y * 0.5f, m_SpinnerSize.x, m_SpinnerSize.y);
    	m_SpinnerPivotPos = new Vector2(m_SpinnerRect.xMin + m_SpinnerRect.width * 0.5f, m_SpinnerRect.yMin + m_SpinnerRect.height * 0.5f);
		
		// Draw the new image
        Matrix4x4 matrixBackup = GUI.matrix;
        GUIUtility.RotateAroundPivot(m_SpinnerAngle, m_SpinnerPivotPos);
        GUI.DrawTexture(m_SpinnerRect, m_Spinner);
        GUI.matrix = matrixBackup;
		m_SpinnerAngle = (m_SpinnerAngle + 3) % 360;
#else
		GUI.skin = m_ConnectButtonSkin;
		// Draw the Get A Sphero Button
		buttonWidth = backgroundWidth*0.3f;
		buttonHeight = ((backgroundWidth*0.25f)/(float)m_GetASpheroTexture.width)*m_GetASpheroTexture.height;
		getASpheroButtonX = backgroundX+(backgroundWidth*0.1f);
		getASpheroButtonY = backgroundY+(backgroundHeight*0.85f) - (buttonHeight/2);
		// If the get a Sphero button is clicked
		if( GUI.Button (new Rect(getASpheroButtonX, getASpheroButtonY,buttonWidth,buttonHeight), "") ) {
			Application.LoadLevel("SpheroConnectionScene");
		}
#endif		
	}
}
