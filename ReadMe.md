# Sphero Augmented Reality SDK

## Overview

The Sphero Augmented Reality (AR) SDK is used to overlay a live view of the real world with virtual content.  For gaming applications in particular, the goal is to render 3D content into the live scene so that it appears to be realistic and authentic. That’s where Sphero rolls in.  Augmented reality gameplay with Sphero is simple and seamless for two reasons – Sphero is a robot, and Sphero is round. This allows us to employ a robotic fiducial, which provides never-before-seen freedom in characterizing the visual environment (since by definition a robot is a reprogrammable machine). With no auxiliary fiducial markers, Sphero can be effectively found and tracked at a distance, at any angle, and when partially obscured. A ball is the same from every angle, after all.

Sphero AR also has the ability to impact the real world, whereas traditional augmented reality is limited to the screen. Not only can we make Sphero interact with virtual objects (collect coins, run over monsters, etc.), but we can also make Sphero respond to these objects (bumping into invisible walls, hitting fictional oil slicks). Just wait till you’re holding Sphero in your hand, looking at an augmented reality character through the screen of your device. It’s pretty awesome.

At the end of the day, putting together all the sensor data from Sphero and your mobile device, analyzing the video feed, setting up the lighting, camera, and action takes some heavy-duty math.  Let’s break it down.

1.  Data starts out in Sphero’s accelerometer and gyroscope (get the low-down on Sphero’s inner robot here) and is piped into a state of the art, sensor-fusion algorithm. This generates Sphero’s sense of orientation. The combination of theses sensors and software behaves very much like Sphero’s “inner ear”.  It allows Sphero to know which way is up (literally) and which way it’s facing. You may get the spins on a merry-go-round, but Sphero can handle hundreds of RPM without getting dizzy.

2.  Next, the camera feed from your mobile device is captured and combined with the device’s own sense of orientation. This gives us a rough understanding of the contents of the picture.  Are we looking at the ground, or at the sky?

3.  Here comes more math. Custom vision algorithms tear the image apart, identifying Sphero, identifying the floor, analyzing the color of the lights, the ground, and any other data that might impact feedback.

4.  Data from the motors is then combined with Sphero’s sense of orientation to generate a sense of position. Just like you can navigate parts of your house with your eyes closed, Sphero can combine its sensor data to figure out where it is.  And you might be surprised how good this sense is. You can tell Sphero to drive a twisting path through a large room, and it still knows how to drive back to within inches of its starting location.  I challenge you to try that blindfolded!

5.  All of the sensory information is combined using a lot of geometry, statistics, and duct-tape to figure out the real-world location of Sphero, the height of the player, the position of virtual objects, and the direction and color of lights.

In the end, the fluid, responsive augmented reality experience with Sphero boils down to the accuracy of about a dozen numbers describing the real and virtual scene. Even with carefully optimized code, it takes hundreds of millions of calculations per second to compute these.  If that seems like a lot of effort for a dozen numbers, just imagine what’s going on in the visual cortex of your own brain. A not-so-simple process, resulting in seamless and intuitive gameplay with endless possibility.

## Using the SDK

	Note: this release only works with iOS™ with Android™ coming soon

The Sphero AR SDK consists of three iOS frameworks, and you should already be familiar with the first one, RobotKit.framework.  The other two frameworks are RobotVision.framework and RobotBridge.framework.  The RobotVision.framework contains all the code that has to do with AR.  The RobotBridge.framework is a set of C++ classes that can manage Sphero data streaming requests from multiple access points.  Whenever you create a project, you will need these three frameworks, along with these linker flags in the xcodeproject build settings:

	-lstdc++
    -all_load
    -ObjC
    -lsqlite3
    
We compile our xcodeproject with Apple LLVM Compiler 4.2 for C/C++/Objective-C or for GnuC++11.  Our C++ code does not use boost, but it does use the std library.
    
The easiest way to get started with our AR SDK is to open the Augmented Reality Sample in Unity.  The sample will demonstrate how to use our engine, and we highly suggest starting here.  In particular, the ARUNBridge.mm file will show you how to set up and interface with the AREngine class, and the Unity scripts will demonstrate how to make meaning out of the data acquired from the AREngine.  

However, if you want to be a renegade, continue reading and attempt to make a native iOS application with OpenGL… we dare you!

### Initializing the AREngine

AREngine is a singleton class that sets up the AR Vision library, feeds in the necessary data, and handles delegation of the results.  On iOS, you set up the engine as follows (in a .mm, objective-c++) file:

    // Set up AREngine
    platformParams.mode = AUX_SHARKY_CAMERA_MOTION_MODE;
    
    motionManager = [[CMMotionManager alloc] init];
	motionManager.deviceMotionUpdateInterval = 1.0/60.0;
    platformParams.motionManager = motionManager;
    
    arEngine = &RobotVision::AREngine::sharedInstance();
    arEngine->initializeEngine(platformParams);
    
The platform parameters are variables specific to iOS you will need to provide the AREngine.  In particular, you need to provide the engine with a CMMotionManger class (used to obtain device sensor data). Apple does not want the developer to make multiple instances of the CMMotionManager, so if you already have one in your application, then just pass that instance to the engine.  

Also, since AREngine is a singleton, we are storing it locally for future reference, and then initializing it with the platform parameters.  

This will not start the AREngine, it only prepares it to start. To start the engine, simply call:

	arEngine->startVision();	

### Receiving ARResults

ARResult is a class that encapsulates everything useful that the AR Vision SDK produces after analysis of a camera frame.  To receive results, you must register a C++ class ARResultsDelegate, as the results delegate for the AREngine.  In the header of your file, create a C++ subclass that inherits from ARResultsDelegate, like this:

	class ARUNBridgeResultDelegate : public RobotVision::ARResultsDelegate {
	public:
    	ARUNBridgeResultDelegate() {}
    	virtual ~ARUNBridgeResultDelegate() {}
    	void onNewResultReady(RobotVision::ARResult* result);
	};
	
And then register the results delegate like this:

    resultsDelegate = new ARUNBridgeResultDelegate();
    arEngine->setResultsDelegate(resultsDelegate);

Then, the only callback you need to receive is from onNewResultReady(RobotVision::ARResult* result).  In your Objective-C++ file you can send this result data to an Objective-C class, or use OpenGL calls on the data.  It is important to note that you must retain and release the ARResult that you get from the onNewResultReady function.  Here is an example of how you would use the the result:

	void ARUNBridgeResultDelegate::onNewResultReady(RobotVision::ARResult *result) {
    	pthread_mutex_lock(&queuedResultLock_);
    	{	
        	if( queuedArResult != NULL ) {
            	queuedArResult->release();
        	}
        	queuedArResult = result;
        	queuedArResult->retain();
    		}
    	pthread_mutex_unlock(&queuedResultLock_);
	}
	
This code will keep the new result in a variable called queuedArResult.  This is coming in on a different thread, so you want to perform a mutex lock when swapping it. 

The ARResult class has three important variables.  VirtualSphero contains information about the Sphero's tracking state, position, and orientation in 3D space.  These are encapsulated in a Pose.  A pose contains a 4x4 OpenGL matrix, a float array struct with a size of 3 for position, and a float array struct with a size of 4 for orientation (as a quaternion).  

Now, a VirtualCamera has a Pose for the camera, and contains the camera frame image that was analyzed to produce this result.  Here, the camera frame is an AuImage class.  To draw the image to the background, you would do this OpenGL call:

		//Get the latest image data to draw
		AuImage* resultImage = (queuedArResult->virtualCamera())->cameraFrame();
		
		unsigned char* imgdata = resultImage->data;
		int rows = resultImage->rows;
		int cols = resultImage->cols;
		int stride = resultImage->bytesPerRow;
		
		//Don't do anything if there isn't a frame to draw
		if(imgdata!=NULL)
		{
			//Send the camera frame to the GPU
			glBindTexture(GL_TEXTURE_2D, textureID);
		
			// unfortunately we can't set stride in gles, adjust the rows instead
			glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE );
			glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE );
			glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, stride / 4, rows, 0, GL_BGRA, GL_UNSIGNED_BYTE, imgdata);
		};

Last, there is the VirtualEnvironment.  Unfortunately, it is not implemented yet; however, it will contain information about the world, such as floor color, lighting direction, etc.

In our sample, we send these ARResult values from native code into Unity, and then Unity uses these to draw the 3D object over Sphero and to generate and position the camera projection correctly.  Hence, with OpenGL magic and the ARResult, you should be able to create a virtual world within your application.

## Data Streaming

Since the RobotVision.framework requires Sphero data streaming as well as our application, we need to make sure that we never disable data streaming, or request different data than the AR Vision SDK needs.  The way to do this is use the RobotBridge.framework, which is a cross-platform asynchronous message library.  To use this, you initialize the iOS version with one line of code:

	[MessengerBridge_iOS sharedBridge];
	
Which will register a callback that allows C++ to call data streaming commands.  Then, you simply register your C++ SpheroDeviceMessageDelegate class with with SpheroDeviceMessenger, like this:

	class SpheroDataStreamingDelegate : public RobotBridge::SpheroDeviceMessageDelegate {

		public:    
    		SpheroDataStreamingDelegate() {}
    		virtual ~SpheroDataStreamingDelegate() {}
    		void onMessageDelivered(RobotBridge::SpheroDeviceMessage* message) {}
    		void onSerializedMessageDelivered(const char* message);
	};
	
Then, you need to tell SpheroDeviceMessenger which data you would like to receive.  These mask values are available in RobotKit.framework under RKSetDataStreamingCommand class.  The AR Vision SDK requests Sphero locator values like so:

	unsigned long long VISION_DATA_STREAMING_MASK = 0x0D80000000000000;

    RobotBridge::SpheroDeviceMessenger::sharedInstance()
        .addSpheroDeviceMessageDelegate(this, VISION_DATA_STREAMING_MASK); 
        
And receives the message by implementing this function:

	void SpheroSensors::onMessageDelivered(RobotBridge::SpheroDeviceMessage* message) {
        // Check for type of device message
        if( strcmp(message->className().c_str(),"SpheroDeviceSensorsAsyncData" ) == 0) {
            RobotBridge::SpheroDeviceSensorsAsyncData* asyncData =
                (RobotBridge::SpheroDeviceSensorsAsyncData*)message;
            
            std::set<RobotBridge::SpheroDeviceSensorsData*> frames = asyncData->frames();
            if( frames.size() > 0 ) {
                RobotBridge::SpheroDeviceSensorsData* frame = *(frames.begin());
                
                // Get the locator data
                RobotBridge::SpheroLocatorData* locatorData = frame->locatorData();
                locatorPosition_ = locatorData->position();
                locatorVelocity_ = locatorData->velocity();
            }
        }
    }
    
The only difference between this and the iOS version in RobotKit, is now the lower-level classes manage who wants which data, and therefore can maintain everyone getting the data they want from Sphero.    
	        

## Automatic Unity Project Building for iOS

---
When building for iOS, XCode requires a few extra frameworks to work with our `RobotVision.framework`.  So, we wrote a post process build script that you should include in your Unity project if you want to be able to hit "Build And Run…" and not have to add the RobotKit.framework, RobotVision.framework, RobotBridge.framework, ExternalAccessory.framework, the supported external accessory protocol of "com.orbotix.robotprotocol" in the info.plist, and linker flags.

Simply drag the Editor folder found in the Assets folder of any of the examples in the Sphero-Unity-Plugin into your project's Assets directory.  Also, RobotKit.framework, RobotBridge.framework, and RobotVision.framework will be missing, unless you create the iOS build folder in Unity in the same directory as the Assets folder.

## License

---
The Sphero Unity Plugin is distributed under the Orbotix Source Code License. 

## Community and Help

---

* [StackOverflow](http://stackoverflow.com/questions/tagged/sphero-api?sort=newest) - Share your project, get help and talk to the Sphero developers!

---

*iOS™ is a licensed trademark of Apple*
*Android™ is a licensed trademark of Google*