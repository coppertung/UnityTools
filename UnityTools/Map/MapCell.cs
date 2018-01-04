using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Map {

	public enum CellType {
		TwoTrianglesSquare = 1,
		FourTrianglesSquare = 2
	}

	[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
	public class MapCell : MonoBehaviour {

		public int id;
		public List<int> neighbours;
		public Vector3 coordinate;
		public Vector3 position;
		public Color color;

		public CellType cellType;
		public float size;

		[HideInInspector]
		public UnityEngine.Mesh cellMesh;
		[HideInInspector]
		public MeshRenderer cellRenderer;
		[HideInInspector]
		public MeshCollider cellCollider;
		private List<Vector3> vertices;
		private List<int> triangles;
		private List<Color> colors;

		void Awake() {

			GetComponent<MeshFilter> ().mesh = cellMesh = new UnityEngine.Mesh ();
			cellMesh.name = "Map Cell Mesh";
			cellRenderer = GetComponent<MeshRenderer> ();
			cellCollider = gameObject.AddComponent<MeshCollider> ();
			vertices = new List<Vector3> ();
			triangles = new List<int> ();
			colors = new List<Color> ();

			neighbours = new List<int> ();

		}

		public void init(int _id, Vector3 _coordinate, Vector3 _position, float _size, Color _color) {

			id = _id;
			coordinate = _coordinate;
			position = _position;
			size = _size;
			color = _color;

		}

		public void createMesh() {

			triangulate ();

			cellMesh.vertices = vertices.ToArray ();
			cellMesh.colors = colors.ToArray ();
			cellMesh.triangles = triangles.ToArray ();
			cellMesh.RecalculateNormals ();

			cellCollider.sharedMesh = cellMesh;

		}

		public void triangulate() {

			vertices.Clear ();
			triangles.Clear ();
			colors.Clear ();

			Vector3 topLeft = position + new Vector3 (-size / 2, size / 2, 0);
			Vector3 topRight = position + new Vector3 (size / 2, size / 2, 0);
			Vector3 bottomLeft = position + new Vector3 (-size / 2, -size / 2, 0);
			Vector3 bottomRight = position + new Vector3 (size / 2, -size / 2, 0);

			switch (cellType) {
			case CellType.TwoTrianglesSquare:
				addTriangle (bottomLeft, topLeft, bottomRight);
				addTriangleColor ();
				addTriangle (bottomRight, topLeft, topRight);
				addTriangleColor ();
				break;
			case CellType.FourTrianglesSquare:
				addTriangle (bottomLeft, topLeft, position);
				addTriangleColor ();
				addTriangle (position, topLeft, topRight);
				addTriangleColor ();
				addTriangle (topRight, bottomRight, position);
				addTriangleColor ();
				addTriangle (position, bottomRight, bottomLeft);
				addTriangleColor ();
				break;
			}

		}

		private void addTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {

			int verticesIndex = vertices.Count;
			vertices.Add (v1);
			vertices.Add (v2);
			vertices.Add (v3);
			triangles.Add (verticesIndex);
			triangles.Add (verticesIndex + 1);
			triangles.Add (verticesIndex + 2);

		}

		private void addTriangleColor() {

			colors.Add (color);
			colors.Add (color);
			colors.Add (color);

		}

	}

}