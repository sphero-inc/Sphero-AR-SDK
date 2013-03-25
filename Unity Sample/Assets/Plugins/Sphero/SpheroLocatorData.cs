using System;

public class SpheroLocatorData : SpheroSensorData {
	public struct Value {
		public float x;
		public float y;
		
		public float Magnitude() {
			return (float)Math.Sqrt(x * x + y * y);
		}
	}
	
	private Value position;
	private Value velocity;
	
	public Value Position { get{ return position; } }
	public Value Velocity { get{ return velocity; } }
	
	public SpheroLocatorData( SpheroDeviceMessageDecoder decoder ) : base(decoder)
	{
		position.x = decoder.DecodeFloat("position.x");
		position.y = decoder.DecodeFloat("position.y");
		velocity.x = decoder.DecodeFloat("velocity.x");
		velocity.y = decoder.DecodeFloat("velocity.y");
	}
}