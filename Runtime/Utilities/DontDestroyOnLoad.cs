using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagement
{
	/// <summary>
	/// This sets this gameObject to not be destroyed when loading a new scene.
	/// </summary>
	public class DontDestroyOnLoad : MonoBehaviour
	{
		void Awake()
		{
			transform.SetParent(null);
			DontDestroyOnLoad(gameObject);
		}
	}
}