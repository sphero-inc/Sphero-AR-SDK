using System;
using System.Collections.Generic;

public class SpheroDeviceSensorsAsyncData : SpheroDeviceAsyncMessage
{
	private int frameCount;
	private ulong mask;
	private SpheroDeviceSensorsData[] frames;
	
	public int FrameCount { get{ return frameCount; } }
	public ulong Mask { get{ return mask; } }	
	public SpheroDeviceSensorsData[] Frames { get{ return frames; } }   
	
	public SpheroDeviceSensorsAsyncData(SpheroDeviceMessageDecoder decoder) 
		: base(decoder)
	{
		frameCount = decoder.DecodeInt32("frameCount");
		mask = decoder.DecodeUInt64("mask");
		
		
		// Decode the frames array and create the frames array converting the 
		// type of each object
		Object[] decodedArray = (Object[])decoder.DecodeObject("dataFrames");
		frames = new SpheroDeviceSensorsData[decodedArray.Length];
		int i = 0;
		foreach( Object decodedObject in decodedArray ) {
			frames[i++] = (SpheroDeviceSensorsData)decodedObject;
		}
	}
}