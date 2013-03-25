using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public abstract class SpheroProvider {
	
	/* Shared instance of Sphero Provider */
	static SpheroProvider sharedProvider = null;
	
	/* Robots */
	protected Sphero[] m_PairedSpheros;
	public Sphero[] PairedSpheros {
		get{ return this.m_PairedSpheros; }	
		set{ this.m_PairedSpheros = value; }
	}
	
	/*
	 * Default Constructor
	 */
	public SpheroProvider() {}
	
	/* Get the shared RobotProvider instance */
	public static SpheroProvider GetSharedProvider() {
		if( sharedProvider == null ) {
			#if UNITY_EDITOR
				sharedProvider = new SpheroProviderEditor();
				sharedProvider.m_PairedSpheros = new Sphero[0];
			#elif UNITY_ANDROID			
				sharedProvider = new SpheroProviderAndroid();
			#elif UNITY_IPHONE
				sharedProvider = new SpheroProviderIOS();
			#endif			
		}
		return sharedProvider;
	}

	/* Grab the connecting Robot */
	public Sphero GetConnectingSphero() {
		foreach( Sphero sphero in m_PairedSpheros ) {
			if( sphero.ConnectionState == Sphero.Connection_State.Connecting ) {
				return sphero;
			}	
		}
		return null;
	}
	
	/* Grab the robot names from Java array */
	public string[] GetRobotNames() {
		Debug.Log(m_PairedSpheros);
		// Store the robots that are paired into an array
		string[] robotNames = new string[m_PairedSpheros.Length];	
		for( int i = 0; i < m_PairedSpheros.Length; i++ ) {
			robotNames[i] = m_PairedSpheros[i].DeviceInfo.Name;
		}		
		return robotNames;
	}
	
	/* Get an array of Connected Spheros */
	abstract public Sphero[] GetConnectedSpheros();
	/*
	 * Call to properly disconnect Spheros.  Call in OnApplicationPause 
	 */
	abstract public void DisconnectSpheros();
	/* Need to call this to get the robot objects that are paired from Android */
	abstract public void FindRobots();
	/* Check if bluetooth is on */
	abstract public bool IsAdapterEnabled();
	/* Connect to a robot at index */
	abstract public void Connect(int index);	
	/*
	 * Get a Sphero object from the unique Sphero id 
	 * Returns nulls if no Spheros were found with that particular id
	 */
	abstract public Sphero GetSphero(string spheroId);
}
