/*
 * AureunVideoTexture
 * Attach this script to a plane in your scene to have the current video
 * frame rendered to it on each update.
 * Part of the Aure-Unity bridge
 * 
 * Created on 6/1/2012 by Jon Carroll
 * Copyright Orbotix, Inc. All rights reserved.
 * 
 */

using UnityEngine;
using System;
using System.Collections;

public class ARUNVideoTexture : MonoBehaviour
{
	private TextureType m_textureType = TextureType.None;

	private Material m_material = null;
	private GUITexture m_guiTexture = null;

	enum TextureType
	{
		None,
		Renderer,
		GUITexture
	}
	
	//initialization
	void Start ()
	{
		// Assign texture to the renderer
        if (renderer != null)
        {
        	m_textureType = TextureType.Renderer;
        	m_material = renderer.material;
            m_material.mainTexture = VideoTextureManager.GetInstance().GetTexture();
        	m_material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		else if (guiTexture != null)
		{
			// or gui texture
			m_textureType = TextureType.GUITexture;
			m_guiTexture = guiTexture;
            m_guiTexture.texture = VideoTextureManager.GetInstance().GetTexture();
            m_guiTexture.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
        	m_textureType = TextureType.None;
			//Debug.Log("Game object has no renderer or gui texture to assign the generated texture to!");
        }
	}

	void OnWillRenderObject()
	{
		switch(m_textureType)
		{
			case TextureType.Renderer:
				m_material.mainTexture = VideoTextureManager.GetInstance().GetTexture();
				break;
			case TextureType.GUITexture:
				m_guiTexture.texture = VideoTextureManager.GetInstance().GetTexture();
				break;
			default:
				break;
		}
	}
}
