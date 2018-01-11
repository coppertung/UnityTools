using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Map {

	[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
	public class MapChunk : MonoBehaviour {

		public int lod;
		public bool split = true;
		public int countX;
		public int countY;

		public List<MapCell> cells;

		[HideInInspector]
		public MapChunk[] childChunks;

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
			cellMesh.name = "Map Chunk Mesh";
			cellRenderer = GetComponent<MeshRenderer> ();
			cellCollider = gameObject.AddComponent<MeshCollider> ();
			vertices = new List<Vector3> ();
			triangles = new List<int> ();
			colors = new List<Color> ();

			childChunks = new MapChunk[4];

		}

		public void init(int _lod, List<MapCell> _cells, int _countX, int _countY) {

			lod = _lod;
			cells = _cells;
			countX = _countX;
			countY = _countY;
			if (lod <= 1) {
				split = false;
			}

		}

		public void updateMesh() {

			if (split) {
				splitChildChunk ();
			} else {
				createMesh ();
			}

		}

		public void splitChildChunk() {

			vertices.Clear ();
			triangles.Clear ();
			colors.Clear ();

			List<MapCell>[] cellLists = new List<MapCell>[4];
			for (int i = 0; i < cellLists.Length; i++) {
				cellLists [i] = new List<MapCell> ();
			}

			for (int x = 0; x < countX; x++) {
				for (int y = 0; y < countY; y++) {
					int i = x * (int)countY + y;
					if (i < cells.Count) {
						if (x < countX / 2) {
							if (y < countY / 2) {
								cellLists[0].Add (cells [i]);	// bottomLeft
							} else {
								cellLists[1].Add (cells [i]);	// topLeft
							}
						} else {
							if (y < countY / 2) {
								cellLists[3].Add (cells [i]);	// bottomRight
							} else {
								cellLists[2].Add (cells [i]);	// topRight
							}
						}
					}
				}
			}

			for (int i = 0; i < 4; i++) {
				if (cellLists [i].Count > 0) {
					GameObject newChunkObject = new GameObject ("Cell Chunk Level " + (lod - 1));
					newChunkObject.transform.SetParent (transform);
					MapChunk newChunk = newChunkObject.AddComponent<MapChunk> ();
					newChunk.init ((lod - 1), cellLists [i], countX / 2, countY / 2);
					newChunk.cellRenderer.material = cellRenderer.material;
					newChunk.updateMesh ();
					childChunks [i] = newChunk;
				}
			}

		}

		public void createMesh() {

			vertices.Clear ();
			triangles.Clear ();
			colors.Clear ();

			for (int i = 0; i < cells.Count; i++) {
				triangulate (cells[i]);
				cells [i].currentChunk = this;
			}

			cellMesh.vertices = vertices.ToArray ();
			cellMesh.colors = colors.ToArray ();
			cellMesh.triangles = triangles.ToArray ();
			cellMesh.RecalculateNormals ();

			// initialize material color
			cellRenderer.material.color = Color.white;
			cellCollider.sharedMesh = cellMesh;

		}

		public void triangulate(MapCell cell) {

			Vector3 topLeft = cell.position + new Vector3 (-cell.size / 2, cell.size / 2, 0);
			Vector3 topRight = cell.position + new Vector3 (cell.size / 2, cell.size / 2, 0);
			Vector3 bottomLeft = cell.position + new Vector3 (-cell.size / 2, -cell.size / 2, 0);
			Vector3 bottomRight = cell.position + new Vector3 (cell.size / 2, -cell.size / 2, 0);

			switch (cell.cellType) {
			case CellType.TwoTrianglesSquare:
				addTriangle (bottomLeft, topLeft, bottomRight);
				addTriangleColor (cell.color, cell.color, cell.color);
				addTriangle (bottomRight, topLeft, topRight);
				addTriangleColor (cell.color, cell.color, cell.color);
				break;
			case CellType.FourTrianglesSquare:
				addTriangle (bottomLeft, topLeft, cell.position);
				addTriangleColor (cell.color, cell.color, cell.color);
				addTriangle (cell.position, topLeft, topRight);
				addTriangleColor (cell.color, cell.color, cell.color);
				addTriangle (topRight, bottomRight, cell.position);
				addTriangleColor (cell.color, cell.color, cell.color);
				addTriangle (cell.position, bottomRight, bottomLeft);
				addTriangleColor (cell.color, cell.color, cell.color);
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

		private void addTriangleColor(Color c1, Color c2, Color c3) {

			colors.Add (c1);
			colors.Add (c2);
			colors.Add (c3);

		}


	}

}