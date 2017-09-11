using System;
using UnityEngine;

namespace UnityTools.Patterns {

	/// <summary>
	/// Singleton Pattern, which is used to ensure there is only one specific class T at the scene.
	/// In addition, it can perform as a static class ( access by others using T.Instance.<non-static method>). 
	/// </summary>
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

		protected static T _instance;
		// The instance of the class
		public static T Instance {
			get {
				if (_instance == null) {
					if (FindObjectsOfType(typeof(T)).Length > 1) {
						throw new Exception ("There must not have more than one " + typeof(T).Name);
					}
					_instance = (T)FindObjectOfType(typeof(T));
				}
				return _instance;
			}
		}

	}

}