using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools {

	#region Interfaces
    /// <summary>
    /// This is the interface for the pooled objects in Object Pooler.
    /// </summary>
    public interface IPoolObject {

		/// <summary>
		/// The identifier of the pool object.
		/// </summary>
		int id {
			get;
			set;
		}
		/// <summary>
		/// Is the pool object active in the scene?
		/// </summary>
		bool isActive {
			get;
			set;
		}

		/// <summary>
		/// Initialize the existed instance.
		/// </summary>
		void init (Vector3 position, Quaternion rotation);
		/// <summary>
		/// Initialize the new instance.
		/// </summary>
		void init (int id, Vector3 position, Quaternion rotation);

	}
	#endregion

	/// <summary>
	/// Object Pooler is used to manage the pooled objects where using activate and deactivate objects instead of instantiate and destroy in order to save the memory.
	/// Usually used for the objects which always requiring instantiate and destroy.
	/// Noted that only support the gameobjects which attached with script that inherited IPoolObject.
	/// </summary>
	public class ObjectPooler {

		#region Fields_And_Properties
		/// <summary>
		/// List which store the pooled objects (script).
		/// </summary>
		public static List<IPoolObject> pooledObjectScriptList {
			get;
			private set;
		}
		/// <summary>
		/// List which store the pooled objects (gameobject).
		/// </summary>
		public static List<GameObject> pooledObjectList {
			get;
			private set;
		}

		/// <summary>
		/// The GameObject that store all the pooled objects in the hierarchy.
		/// </summary>
		public static GameObject pooledObjectsParent;

		/// <summary>
		/// count of pooled objects.
		/// </summary>
		public static int Count {
			get;
			private set;
		}
		#endregion

		#region Functions
		/// <summary>
		/// Get a pooled object from the stored list.
		/// If there is no suitable pooled object, new pool object will be instantiated and stored in the list.
		/// </summary>
		public static GameObject GetPoolObject(string name, GameObject prefabModel, Vector3 position, Quaternion rotation) {

			if (pooledObjectList == null) {
				pooledObjectList = new List<GameObject> ();
			}
			if (pooledObjectScriptList == null) {
				pooledObjectScriptList = new List<IPoolObject> ();
			}
			for (int i = 0; i < Count; i++) {
				if (pooledObjectList[i].name.Equals(name) && !pooledObjectScriptList[i].isActive) {
					pooledObjectScriptList[i].init(position, rotation);
					return pooledObjectList[i];
				}
			}
			// cannot find a suitable object
			GameObject newObject = GameObject.Instantiate (prefabModel);
			if (pooledObjectsParent == null) {
				pooledObjectsParent = new GameObject ("Object Pool");
			}
			newObject.transform.SetParent (pooledObjectsParent.transform);
			newObject.name = name;
			IPoolObject newScript = newObject.GetComponent<IPoolObject>();
			if (newScript == null) {
				GameObject.Destroy (newObject);
				throw new NotSupportedException();
			}
			else {
				newScript.init (Count, position, rotation);
				pooledObjectList.Add(newObject);
				pooledObjectScriptList.Add(newScript);
				Count++;
				return newObject;
			}

		}

		/// <summary>
		/// Disable the pool object by deactivate it.
		/// </summary>
		public static void DisablePoolObject(int id) {

			if (pooledObjectList [id].activeInHierarchy) {
				pooledObjectList [id].SetActive (false);
			}
			if (pooledObjectScriptList [id].isActive) {
				pooledObjectScriptList [id].isActive = false;
			}

		}

		/// <summary>
		/// Clear the pooled objects to release the memory.
		/// </summary>
		public static void ClearPoolObject() {

			if (pooledObjectScriptList != null) {
				pooledObjectScriptList.Clear ();
			}
			if (pooledObjectList != null) {
				for (int i = 0; i < Count; i++) {
					// always remove the first pooled object
					GameObject obj = pooledObjectList [0];
					pooledObjectList.Remove (obj);
					GameObject.Destroy (obj);
				}
			}
			Count = 0;

		}
		#endregion

	}

}