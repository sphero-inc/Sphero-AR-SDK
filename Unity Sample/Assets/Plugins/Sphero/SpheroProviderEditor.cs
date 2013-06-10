using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if UNITY_EDITOR
public class SpheroProviderEditor : SpheroProvider {
	
	/*
	 * Get the Robot Provider for Android 
	 */
	public SpheroProviderEditor() : base() {}
	
	override public void DisconnectSpheros() {}
	override public void FindRobots() {}
	override public bool IsAdapterEnabled() { return true; }
	override public void Connect(int index) {}	
	override public Sphero GetSphero(string spheroId) { return null; }
	override public Sphero[] GetConnectedSpheros() { return new Sphero[0]; }
}
#endif