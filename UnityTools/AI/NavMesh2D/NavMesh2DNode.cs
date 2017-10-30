using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityTools.AI.NavMesh2D {

	#region Node
	/// <summary>
	/// Nodes that will be used in the Navigation Mesh 2D system, which inherited the IAstarable interface in order to use the A* Algorithm.
	/// </summary>
	[System.Serializable]
    public class NavMesh2DNode : IAStarable<Vector3> {

		[SerializeField]
		private int _id;
		[SerializeField]
		private Vector3 _position;
		[SerializeField]
		private List<int> _neighbours;

		/// <summary>
		/// The identifier of the node.
		/// </summary>
		public int id {
			get {
				return _id;
			}
			set {
				_id = value;
			}
		}
		/// <summary>
		/// The position information of the node.
		/// </summary>
		public Vector3 position {
			get {
				return _position;
			}
			set {
				_position = value;
			}
		}
		/// <summary>
		/// Mainly used for makesure some useful values can be used even if it is casted as IAStarable.
		/// This return the value of the position.
		/// </summary>
		public Vector3 value {
			get {
				return position;
			}
		}
		/// <summary>
		/// The neighbour nodes.
		/// </summary>
		public List<int> neighbours {
			get {
				return _neighbours;
			}
			set {
				_neighbours = value;
			}
		}

		/// <summary>
		/// The heuristic function.
		/// </summary>
		public heuristicFunctionEvent heuristicFunction {
			get;
			set;
		}

		/// <summary>
		/// Required (estimated) cost to travel to the specified target position.
		/// </summary>
		public float cost (Vector3 target) {

			return Vector3.Distance (position, target);

		}

    }
	#endregion

	/// <summary>
	/// The class stored the nodes of the Navigation Mesh 2D system.
	/// </summary>
	[System.Serializable]
	public class NavMesh2DNodeList {

		#region Fields_And_Properties
		/// <summary>
		/// Acceptable error of the unit length, in order to find out the nodes that is very close (within the unit error distance) to obstacles.
		/// This variable must prevent from having large value.
		/// Recommanded to set a value that is smaller that 1/4 of the unit length.
		/// Defined by the baker.
		/// </summary>
		public float unitError;
		/// <summary>
		/// The list of the nodes.
		/// </summary>
		public List<NavMesh2DNode> nodes;
		#endregion

		#region Functions
		/// <summary>
		/// Find the nearest node from the specified position.
		/// </summary>
		public NavMesh2DNode findNearestNode(Vector3 position) {

			if (nodes.Count > 0) {
				float minDistance = float.MaxValue;
				int closestPointID = -1;
				for (int i = 0; i < nodes.Count; i++) {
					if (position != nodes [i].position) {
						float distance = Vector3.Distance (position, nodes [i].position);
						if (distance < minDistance) {
							minDistance = distance;
							closestPointID = i;
						}
					} else {
						return nodes [i];
					}
				}
				return nodes [closestPointID];
			} else {
				throw new NullReferenceException ();
			}

		}

		#if UNITY_EDITOR
		/// <summary>
		/// Save the nodes into specified path with specified filename as a json file (Editor only).
		/// </summary>
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
		#endif

		/// <summary>
		/// Read the specified path with specified filename in order to get a list of nodes of the Navigation Mesh 2D system.
		/// </summary>
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
		#endregion

	}

}