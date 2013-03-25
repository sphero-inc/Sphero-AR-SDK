using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class VideoTextureManager : MonoBehaviour
{
	private const int kTextureCount = 2;
	private const TextureFormat kTextureFormat = TextureFormat.ARGB32;

	private static VideoTextureManager s_instance = null;

	private Texture2D m_texture;
	private List<Texture2D> m_aTextures = new List<Texture2D>(kTextureCount);
	private int m_currentTextureIndex = 0;

	private bool m_flipped = false;

	private bool m_texturesGenerated = false;
	private int m_generatedWidth = 0;
	private int m_generatedHeight = 0;

	//! @return	the static instance of the VideoTextureManager (lazily initialized)
	public static VideoTextureManager GetInstance()
	{
		if (s_instance == null)
		{
			GameObject go = new GameObject();
			s_instance = go.AddComponent<VideoTextureManager>();
			go.name = typeof(VideoTextureManager).FullName;
		}

		return s_instance;
	}

	private void GenerateTextures(int width, int height)
	{
		m_aTextures.Clear();
		if (width == 0 || height == 0)
		{
			m_texturesGenerated = false;
			return;
		}
		for (int i = 0; i < kTextureCount; i++)
		{
			Texture2D tex = new Texture2D(width, height, kTextureFormat, false);
			tex.Apply();
			m_aTextures.Add(tex);
		}
		m_generatedWidth = width;
		m_generatedHeight = height;
		m_texturesGenerated = true;
	}

	/*!
	 * @brief	gets the current video frame, updating the cached texture if necessary
	 * @return	the current video frame
	 */
	public Texture2D GetTexture()
	{
		if (!m_flipped)
		{
			Flip();
		}

		// flip generates textures
		if (!m_texturesGenerated)
		{
			return null;
		}
		return m_aTextures[m_currentTextureIndex];
	}

	/*!
	 * @brief	flips the current frame for one of the cached ones
	 */
	private void Flip()
	{
#if !UNITY_EDITOR
		m_currentTextureIndex = (m_currentTextureIndex + 1) % kTextureCount;

		int textureWidth = ARUNBridge.CurrentARResult.BackgroundVideoSize.width;
		int textureHeight = ARUNBridge.CurrentARResult.BackgroundVideoSize.height;
		if (ShouldGenerateTextures(textureWidth, textureHeight))
		{
			GenerateTextures(textureWidth, textureHeight);
		}
		if (m_texturesGenerated)
		{
			_ARUNBridgeUpdateVideoTexture(m_aTextures[m_currentTextureIndex].GetNativeTextureID());
		}
		else
		{
			_ARUNBridgeUpdateVideoTexture(0);
		}
#endif
		m_flipped = true;
		StartCoroutine(ResetFlip());
	}

	private bool ShouldGenerateTextures(float width, float height)
	{
		return width != 0
			&& height != 0
			&& ((!m_texturesGenerated)
				|| (width != m_generatedWidth)
				|| (height != m_generatedHeight));
	}

	/*!
	 * should be called after the scene is rendered, prepares the camera to receive a new texture
	 */
	IEnumerator ResetFlip()
	{
		yield return new WaitForEndOfFrame();
		m_flipped = false;
	}

	void OnDestroy()
	{
		s_instance = null;
	}
	
	//Register the call to the plugin to send the texture to the GPU
	[DllImport ("__Internal")]
	private static extern void _ARUNBridgeUpdateVideoTexture(int textureID);
}
