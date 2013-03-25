# Augmented Reality Unity Sample

## Overview

Unity is a powerful rendering engine to create interactive 3D content.  You can create an Augmented Reality game using these tools quicker than using a native platform with OpenGL.  Hence, this sample shows the basics of setting up a Unity scene to render a 3D character on top of Sphero.  

## The Importance of a Home Screen

It is very important that you create a home scene that then turns on AR after some user interaction.  This gurentees that Unity can finish setting up before we initialize or AREngine.  If you do not do this and start the vision engine on start up, you will get frequent, completely random, EXEC_BAD_ACCESs errors.  

	Known Unity Bug: 
	When you are deploying to your iOS device, you will occasionally get a EXEC_BAD_ACCESS in CoreLocations that will not occur once the app is on your device.

## ARUNController

The ARUNController manages the results of the AREngine and making sure the vision update delegates know about new ARResults.  

	You must add this script to your scene to get vision updates to the TackerScript and CameraScript

## Background Video

The virtual world we create does not mean anything without the actual camera image.  We need to draw the camera image to a 2D texture.  The ARUNVideo script, with the help of ARUNVideoTexture and ARUNVideoTextureManager, controls creating a 2D texture and then sending the texture id down into native code to perform an OpenGL call to draw the camera frame.  This is the most effecient way to draw the background video.  

	To display the camera image as a 2D background, simply drag and drop the ARUNVideo script onto an object in your scene.

The ARUNVideo script uses a custom shader that ignores lighting and draws the raw texture.  You can find the BasicShader in the Assets/Shaders directory. 

## Tracker Script

TrackerScript models the position and orientation of the Sphero in the world. Sphero is assumed to have started at the origin of the world, and we use the inbuilt location data to track how it has moved in real world meters. The orientation uses Spheros internal heading information and correlates it with a learning algorithm built into the SDK that tracks the camera’s position around Sphero to display an accurate heading after a brief learning phase.  

	To apply a 3D character over Sphero, simply drag the TrackerScript onto a model.

## Camera Script

CameraScript models the position, angle, and view metrics of the device camera in the context of the Unity scene. The camera is internally modeled about sphero and the entire camera/sphero system traverses the Unity game world as location data is streamed in from the ball. CameraScript, in addition to keeping track of a position and orientation, also uses measured metrics from the device camera to match the perspective projection parameters of the Unity camera with the incoming video feed.

	To apply the camera script to a Unity camera, simply drag and drop it onto the Unity camera object.
	
## Automatic Building on iOS

---
When building for iOS, XCode requires a few extra frameworks to work with our `RobotKit.framework` (iOS Native Sphero SDK).  So, we wrote a post process build script that you should include in your Unity project if you want to be able to hit "Build And Run…" and not have to add the RobotKit.framework, ExternalAccessory.framework, and the supported external accessory protocol of "com.orbotix.robotprotocol" in the info.plist.

Simply drag the Editor folder found in the Assets folder of any of the examples in the Sphero-Unity-Plugin into your project's Assets directory.  Also, RobotKit.framework will be missing, unless you create the iOS build folder in Unity in the same directory as the Assets folder.

## License

---
The Sphero Unity Plugin is distributed under the Orbotix Source Code License. 

## Community and Help

---

* [StackOverflow](http://stackoverflow.com/questions/tagged/sphero-api?sort=newest) - Share your project, get help and talk to the Sphero developers!

---
