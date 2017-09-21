using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityTools.AI {

	[System.Serializable]
    public class NavMesh2DNode : IAStarable<Vector3> {
		
		[SerializeField]
		private int _id;
		[SerializeField]
		private Vector3 _position;
		[SerializeField]
		private List<int> _neighbours;

		public int id {
			get {
				return _id;
			}
			set {
				_id = value;
			}
		}
		public Vector3 position {
			get {
				return _position;
			}
			set {
				_position = value;
			}
		}
		public Vector3 value {
			get {
				return position;
			}
		}
		public List<int> neighbours {
			get {
				return _neighbours;
			}
			set {
				_neighbours = value;
			}
		}

		public heuristicFunctionEvent heuristicFunction {
			get;
			set;
		}

		public float cost (Vector3 target) {

			return Vector3.Distance (position, target);

		}

    }

	[System.Serializable]
	public class NavMesh2DNodeList {

		public List<NavMesh2DNode> nodes;

		[Conditional("UNITY_EDITOR")]
		public void save(string path, string filename) {

			string fileExtension = ".json";
			string jsonString = null;

			if (nodes != null)
				jsonString = JsonUtility.ToJson (this);
			else
				throw new NullReferenceException ();

			string fullpath = Application.dataPath + "/Resources/" + path + "/" + filename + fileExtension;
			if (!Directory.Exists (Application.dataPath + "/" + "Resources/" + path)) {
				Directory.CreateDirectory (Application.dataPath + "/" + "Resources/" + path);
			}
			if (File.Exists (fullpath)) {
				File.Delete (fullpath);
			}
			StreamWriter writer = new StreamWriter (fullpath, false);
			writer.Write (jsonString);
			writer.Close ();
			AssetDatabase.Refresh ();
				
		}

		public static NavMesh2DNodeList read(string path, string filename) {

			string jsonString = null;
			string fullpath = path + "/" + filename;

			try {
				TextAsset asset = Resources.Load<TextAsset> (fullpath);
				UnityEngine.Debug.Log (asset);
				jsonString = asset.text;
				UnityEngine.Debug.Log (jsonString);
				return JsonUtility.FromJson<NavMesh2DNodeList>(jsonString);
			}
			catch(Exception ex) {
				UnityEngine.Debug.Log (ex.Message);
				return null;
			}

		}

	}

}