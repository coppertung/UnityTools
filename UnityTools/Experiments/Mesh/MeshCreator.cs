#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Mesh {

	/// <summary>
	/// Reference from:
	/// http://catlikecoding.com/unity/tutorials/procedural-grid/
	/// http://catlikecoding.com/unity/tutorials/rounded-cube/
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)), ExecuteInEditMode]
	public class MeshCreator : MonoBehaviour {

		public class MeshLevel {

			public bool foldout = true;
			public List<Vector3> vertices;
			public float radius;

			public void clear() {

				vertices.Clear ();

			}

		}

		public int level = 1;
		public List<MeshLevel> levels;

		private UnityEngine.Mesh mesh;
		private List<Vector3> vertices;
		private bool generated = false;

		void OnDrawGizmos() {

			if (vertices == null || vertices.Count == 0) {
				return;
			}
			// draw vertices
			Gizmos.color = Color.black;
			for (int i = 0; i < vertices.Count; i++) {
				Gizmos.DrawSphere (vertices [i], 0.1f);
			}

		}

		public IEnumerator generate() {

			// create mesh
			GetComponent<MeshFilter>().mesh = mesh = new UnityEngine.Mesh();
			mesh.name = "Procedural Mesh";

			// doing stuffs
			for (int i = 0; i < level; i++) {
				initLevel (i);
			}
			updateMesh ();
			generated = true;

			yield return null;

		}

		public void clear() {

			generated = false;
			vertices.Clear ();
			GetComponent<MeshFilter>().mesh = mesh = null;

		}

		public void initLevel(int level) {

			if (levels == null) {
				levels = new List<MeshLevel> ();
			}
			if (levels.Count == level) {
				MeshLevel newLevel = new MeshLevel ();
				newLevel.vertices = new List<Vector3> ();
				newLevel.radius = 1;
				levels.Add (newLevel);
				updateLevel (level, 3);
			}

		}

		public void updateLevel(int level, int numOfVertices) {

			if (numOfVertices < 3) {
				numOfVertices = 3;
			}
			levels [level].clear ();
			levels [level].vertices.Add (transform.position);
			for (int i = 0; i < numOfVertices; i++) {
				Vector3 newVertex = transform.position + levels [level].radius * new Vector3 (Mathf.Sin (360f / numOfVertices * i * Mathf.Deg2Rad), Mathf.Cos (360f / numOfVertices * i * Mathf.Deg2Rad));
				levels [level].vertices.Add (newVertex);
			}
			if (generated) {
				updateMesh ();
			}

		}

		public void updateMesh() {

			mesh.Clear ();
			if (vertices == null) {
				vertices = new List<Vector3> ();
			} else if (vertices.Count > 0) {
				vertices.Clear ();
			}
			for (int i = 0; i < levels.Count; i++) {
				for (int j = 0; j < levels [i].vertices.Count; j++) {
					vertices.Add (levels [i].vertices [j]);
				}
			}
			mesh.vertices = vertices.ToArray ();
			mesh.triangles = DelanunayTriangulation.Calculate (mesh.vertices);

		}

	}

	[CustomEditor(typeof(MeshCreator))]
	public class MeshCreatorEditor : Editor {

		MeshCreator script;

		void OnEnable() {

			if (script == null)
				script = (MeshCreator)target;

		}

		void OnDisable() {

		}

		public override void OnInspectorGUI() {

			GUILayout.Label ("Settings", EditorStyles.boldLabel);
			// variables
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Total Level: ");
			script.level = EditorGUILayout.IntField (script.level);
			GUILayout.EndHorizontal ();
			for (int i = 0; i < script.level; i++) {
				if (script.levels == null || script.levels.Count == i) {
					script.initLevel (i);
				}
				script.levels [i].foldout = EditorGUILayout.Foldout (script.levels [i].foldout, "Level " + i);
				if (script.levels [i].foldout) {
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Radius: ");
					script.levels [i].radius = EditorGUILayout.FloatField (script.levels [i].radius);
					GUILayout.EndHorizontal ();
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Verices Number ");
					if (GUILayout.Button ("-")) {
						script.updateLevel (i, script.levels [i].vertices.Count - 2);
					}
					GUILayout.Label (script.levels [i].vertices.Count.ToString ());
					if (GUILayout.Button ("+")) {
						script.updateLevel (i, script.levels [i].vertices.Count);
					}
					GUILayout.EndHorizontal ();
				}
			}
			// Buttons
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Generate")) {
				script.StartCoroutine (script.generate ());
			}
			if (GUILayout.Button ("Clear")) {
				script.clear ();
			}
			GUILayout.EndHorizontal ();
			if (GUI.changed)
				EditorUtility.SetDirty (script);

		}

	}

}
#endif