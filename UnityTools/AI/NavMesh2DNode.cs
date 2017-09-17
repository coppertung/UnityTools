using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityTools.AI {

	[System.Serializable]
    public class NavMesh2DNode {
        
		public int id;
        public Vector3 position;
        public List<int> neighbours;

    }

	[System.Serializable]
	public class NavMesh2DNodeList {

		public List<NavMesh2DNode> nodes;

		public void save(string filename) {

			string jsonString = null;

			if (nodes != null)
				jsonString = JsonUtility.ToJson (this);
			else
				throw new NullReferenceException ();

			string fullpath = Application.persistentDataPath + "/" + filename;
			if (File.Exists (fullpath)) {
				File.Delete (fullpath);
			}
			StreamWriter writer = new StreamWriter (fullpath, false);
			writer.WriteLine (jsonString);
			writer.Close ();
				
		}

		public static NavMesh2DNodeList read(string filename) {

			string jsonString = null;
			string fullpath = Application.persistentDataPath + "/" + filename;

			if (File.Exists (fullpath)) {
				StreamReader reader = new StreamReader (fullpath);
				jsonString = reader.ReadLine ();
				Debug.Log (jsonString);
				return JsonUtility.FromJson<NavMesh2DNodeList>(jsonString);
			} else {
				return null;
			}

		}

	}

}