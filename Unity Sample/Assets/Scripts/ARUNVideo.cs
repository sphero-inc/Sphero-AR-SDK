using UnityEngine;
using System.Collections;

public class ARUNVideo : MonoBehaviour
{
	public Color ClearColor = Color.green;	//!< @brief	the clear color for the video camera
	public float CameraDepth = -1;			//!< @brief	the depth (rendering order) of this camera
	public Material VideoMaterial;			//!< @brief	the material to apply to the video plane

	private GameObject m_plane;		//!< @brief	the plane to render video onto
	private GameObject m_camera;	//!< @breif	the camera rendering the video

	private bool m_videoGenerated = false;

	// Use this for initialization
	void Start ()
	{
		if( ARUNBridge._ARUNBridgeVisionIsInitialized() )
		{
			SetupARUNVideo();
		}
	}

	void Update()
	{
		if (!m_videoGenerated && ARUNBridge._ARUNBridgeVisionIsInitialized() )
		{
			SetupARUNVideo();
		}
	}

	/*!
	 * @brief	regenerates the camera and plane geometry from the given @a config struct
	 * @param	config the configuration for which to generate a camera and plane
	 */
	private void RegenerateGeometry(ref ARUNBridge.AuPlatformConfiguration config)
	{
		ConfigureCameraGeometry(m_camera.camera);
		ConfigurePlaneGeometry(ref config, m_plane.GetComponent<MeshFilter>().mesh);
	}

	/*!
	 * @brief	initializes the video rendering objects if the config is currently valid
	 */
	private void SetupARUNVideo()
	{
		ARUNBridge.AuPlatformConfiguration config;
		GetPlatformConfiguration(out config);

		if (ConfigurationValid(ref config))
		{
			m_camera = GenerateCamera();
			m_plane = GenerateVideoPlane(ref config);

			m_videoGenerated = true;
		}
	}

	/*!
	 * @brief	determines if the provided @a config structure is valid
	 * @param	config the config structure to check
	 * @return	true if @a config is true
	 */
	private bool ConfigurationValid(ref ARUNBridge.AuPlatformConfiguration config)
	{
		return
			config.projFrustLeft != 0
			&& config.projFrustTop != 0
			&& config.projFrustNear != 0
			&& config.vidRectLeft != 0
			&& config.vidRectTop != 0
			&& config.vidRectNear != 0;
 	}


#if UNITY_EDITOR
	static int queryCount = 0;	// editor only - number of times the GetPlatformConfiguration has been called
#endif

	/*!
	 * @brief	retrieves the AuPlatformConfiguration, setting @a config in the process
	 * @param	config the AuPlatformConfiguration struct to hold the current configuration
	 */
	private void GetPlatformConfiguration(out ARUNBridge.AuPlatformConfiguration config)
	{
#if UNITY_EDITOR
		// simulate waiting a few frames for the camera geometry
		if (queryCount > 5)
		{
			config = new ARUNBridge.AuPlatformConfiguration();
			config.projFrustLeft = -0.03f;
			config.projFrustTop = -0.05325f;
			config.projFrustNear = 0.1f;

			config.vidRectLeft = -360.0f;
			config.vidRectTop = -640.0f;
			config.vidRectNear = 1200.0f;
		}
		else
		{
			config = new ARUNBridge.AuPlatformConfiguration();
			config.projFrustLeft = 0.0f;
			config.projFrustTop = 0.0f;
			config.projFrustNear = 0.0f;

			config.vidRectLeft = 0.0f;
			config.vidRectTop = 0.0f;
			config.vidRectNear = 0.0f;
		}
		queryCount ++;
#else
		config = ARUNBridge.CurrentARResult.CameraConfigurationSettings;
#endif
	}


	/*!
	 * @brief	configures the camera's geometry based on the @a config struct provided
	 * @param	cam the camera being configured
	 */
	private void ConfigureCameraGeometry(Camera cam)
	{
		// cam.aspect = 1f;
		cam.orthographic = true;
		cam.orthographicSize = 1f;
		cam.farClipPlane = 1f;	// we apparently still need to set this for clipping...
		cam.nearClipPlane = 0f;
	}

	/*!
	 * @brief	Creates a camera based on the provided @a config struct
	 * @return	the GameObject representing the camera
	 */
	private GameObject GenerateCamera()
	{
		GameObject camObj = new GameObject();
		camObj.transform.parent = transform;
		camObj.transform.localPosition = Vector3.zero;
		camObj.name = name + "_Camera";
		Camera cam = camObj.AddComponent<Camera>();
		ConfigureCameraGeometry(cam);
		cam.cullingMask = 1 << gameObject.layer;
		cam.backgroundColor = ClearColor;
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.depth = CameraDepth;

		return camObj;
	}

	Vector2 OffsetsFromConfiguration(ref ARUNBridge.AuPlatformConfiguration config)
	{
		float cols = ARUNBridge.CurrentARResult.BackgroundVideoSize.width;
		float rows = ARUNBridge.CurrentARResult.BackgroundVideoSize.height;

		Vector2 result;
		if (cols > rows)
		{
			result.x = cols / rows;
			result.y = 1f;
		}
		else
		{
			result.x = 1f;
			result.y = rows / cols;
		}
		return result;
	}

	/*!
	 * @brief	configures the video plane geometry (vertex positions)
	 * @param	config the AuPlatformConfiguration describing the plane geometry
	 * @param	targetMesh the mesh to write into
	 */
	private void ConfigurePlaneGeometry(ref ARUNBridge.AuPlatformConfiguration config, Mesh targetMesh)
	{
		Vector2 offsets = OffsetsFromConfiguration(ref config);
		Vector3[] positions = {
			new Vector3(-offsets.x, -offsets.y, 0.5f),	// 0
			new Vector3(-offsets.x,  offsets.y, 0.5f),	// 1
			new Vector3( offsets.x,  offsets.y, 0.5f),	// 2
			new Vector3( offsets.x, -offsets.y, 0.5f)	// 3
		};

		targetMesh.vertices = positions;
	}

	/*!
	 * @brief	generates the video plane from a configuration struct
	 * @param	config the AuPlatformConfiguration describing the configuration
	 * @return	the generated video plane
	 */
	private GameObject GenerateVideoPlane(ref ARUNBridge.AuPlatformConfiguration config)
	{
		Vector2[] uvs = {
			new Vector2(0,1),
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(1,1)
		};

		int[] indices = {
			0, 1, 2,
			0, 2, 3
		};

		GameObject videoObj = new GameObject();
		videoObj.transform.parent = transform;
		videoObj.transform.localPosition = Vector3.zero;
		videoObj.layer = gameObject.layer;
		videoObj.name = name + "_Plane";

		MeshFilter vidMeshFilter = videoObj.AddComponent<MeshFilter>();
		Mesh vidMesh = vidMeshFilter.mesh;
		ConfigurePlaneGeometry(ref config, vidMesh);
		vidMesh.uv = uvs;
		vidMesh.triangles = indices;

		MeshRenderer vidRenderer = videoObj.AddComponent<MeshRenderer>();
		vidRenderer.material = VideoMaterial;
		
#if !UNITY_EDITOR
		ARUNVideoTexture auVidTexture = videoObj.AddComponent<ARUNVideoTexture>();
#endif

		videoObj.transform.eulerAngles = new Vector3(0f, 0f, -90f);
		
		return videoObj;
	}
}
