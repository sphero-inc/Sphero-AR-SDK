using UnityEngine;
using System.Collections;

public class SingletonBehaviour<T> : MonoBehaviour where T: MonoBehaviour
{
	private static T s_instance = null;
	public static T Instance
	{
		get{return s_instance;}
	}

	public virtual void Awake()
	{
		if (s_instance != null)
		{
			Debug.LogError("Singleton<" + typeof(T) +"> already exists!");
		}
		s_instance = this as T;
	}

	public virtual void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}
}
