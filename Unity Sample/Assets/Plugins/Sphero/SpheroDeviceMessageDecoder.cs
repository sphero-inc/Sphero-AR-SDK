using System;
using System.Collections.Generic;
using JsonFx.Json;

public class SpheroDeviceMessageDecoder {
	Dictionary<string, object> dictionaryRepresentation = null;
	
	public static SpheroDeviceMessage messageFromEncodedString(string encodedMessage)
	{
		SpheroDeviceMessageDecoder decoder = 
			new SpheroDeviceMessageDecoder(encodedMessage);		
		return (SpheroDeviceMessage)decoder.CreateObject();
	}

	public SpheroDeviceMessageDecoder(string encodedMessage) 
	{
		JsonReader jsonReader = new JsonReader(encodedMessage);
		dictionaryRepresentation = jsonReader.Deserialize< Dictionary<string,object> >();
	}	
	
	private SpheroDeviceMessageDecoder(Dictionary<string,object> encodedDictionary) {
		dictionaryRepresentation = encodedDictionary;
	}
	
	private object CreateObject()
	{
		// Get the class from the decoder to make an object. "Sphero" is added
		// for namespacing.
		string className = "Sphero" +  DecodeString("class");
		
		// Create an instance from this class name
		Type messageType = Type.GetType(className);
		Object[] parameters = new Object[] {this};
		return Activator.CreateInstance(messageType, parameters);
	}
	
	public object DecodeObject(string key)
	{
		object value = null;
		dictionaryRepresentation.TryGetValue(key, out value);
		
		if (value is Array) {
			// need to decode the objects in the array
			Dictionary<string,object>[] encodedArray = (Dictionary<string,object>[])value;
			Object[] decodedArray = new Object[encodedArray.Length];
			int index = 0;
			foreach(Dictionary<string,object> encodedDictionary in encodedArray)
			{
				SpheroDeviceMessageDecoder itemDecoder = 
					new SpheroDeviceMessageDecoder(encodedDictionary);
				object decodedItem = itemDecoder.CreateObject();
				decodedArray[index++] = decodedItem;
			}
			value = decodedArray;
		} else if (value is Dictionary<string, object>) {
			SpheroDeviceMessageDecoder decoder = 
				new SpheroDeviceMessageDecoder((Dictionary<string,object>)value);
			value = decoder.CreateObject();
		}
		
		return value;
	}
		
	public string DecodeString(string key)
	{
		return (string)DecodeObject(key);
	}
	
	public int DecodeInt32(string key)
	{
		return Convert.ToInt32(DecodeObject(key));
	}
	
	public long DecodeInt64(string key)
	{
		return Convert.ToInt64(DecodeObject(key));
	}
	
	public ushort DecodeUInt16(string key)
	{
		// To be compatible with Java unsigned numbers are transmitted as hex strings
		string hexString = DecodeString(key);
		return Convert.ToUInt16(hexString, 16);
	}
	
	public ulong DecodeUInt64(string key)
	{
		// To be compatible with Java unsigned numbers are transmitted as hex strings
		string hexString = DecodeString(key);
		return Convert.ToUInt64(hexString, 16);
	}
	
	public float DecodeFloat(string key)
	{
		return Convert.ToSingle(DecodeObject(key));
	}
}