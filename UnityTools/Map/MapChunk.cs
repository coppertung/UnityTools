using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Map {

	[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
	public class MapChunk : MonoBehaviour, IUpdateable {

		public int lod;
		public bool split = false;
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

		public Vector3 center {
			get;
			protected set;
		}

		public int priority {
			get;
			set;
		}

		public void updateEvent() {

			// split control
			if (MapGenerator.Instance.LODReferenceObject != null) {
				if (split) {
					if (lod <= 1 || countX * countY == 1
					    || Vector3.Distance (center, MapGenerator.Instance.LODReferenceObject.transform.position) > Mathf.Pow (MapGenerator.Instance.LODReferenceDistance, lod - 1)) {
						split = false;
						updateMesh ();
					}
				} else {
					if (lod > 1 && countX * countY != 1 &&
					    Vector3.Distance (center, MapGenerator.Instance.LODReferenceObject.transform.position) <= Mathf.Pow (MapGenerator.Instance.LODReferenceDistance, lod - 1)) {
						split = true;
						updateMesh ();
					}
				}
			} else {
				if (lod <= 1 || countX * countY == 1) {
					split = false;
				}
			}


		}

		void Awake() {

			GetComponent<MeshFilter> ().mesh = cellMesh = new UnityEngine.Mesh ();
			cellMesh.name = "Map Chunk Mesh";
			cellRenderer = GetComponent<MeshRenderer> ();
			cellCollider = gameObject.AddComponent<MeshCollider> ();
			vertices = new List<Vector3> ();
			triangles = new List<int> ();
			colors = new List<Color> ();

			childChunks = new MapChunk[4];
			UpdateManager.RegisterUpdate (this);

		}

		void OnDestroy() {

			UpdateManager.UnregisterUpdate (this);

		}

		public void init(int _lod, List<MapCell> _cells, int _countX, int _countY) {

			lod = _lod;
			cells = _cells;
			countX = _countX;
			countY = _countY;
			// adjust count x and count y
			if (cells.Count != countX * countY) {
				Vector2 refPos = cells [0].position;
				countX = cells.FindAll (x => x.position.y == refPos.y).Count;
				countY = cells.FindAll (x => x.position.x == refPos.x).Count;
			}
			if (cells.Count == 1) {
				center = cells [0].position;
			} else {
				Vector3 topLeft = cells [countY - 1].position + new Vector3 (-cells [countY - 1].size / 2, cells [countY - 1].size / 2, 0);
				Vector3 topRight = cells [countX * countY - 1].position + new Vector3 (cells [countX * countY - 1].size / 2, cells [countX * countY - 1].size / 2, 0);
				Vector3 bottomLeft = cells [0].position + new Vector3 (-cells [0].size / 2, -cells [0].size / 2, 0);
				Vector3 bottomRight = cells [countY * (countX - 1)].position + new Vector3 (cells [countY * (countX - 1)].size / 2, -cells [countY * (countX - 1)].size / 2, 0);
				center = (topLeft + topRight + bottomLeft + bottomRight) / 4;
			}

		}

		public MapCell getCellByPosition(Vector3 position) {

			for (int i = 0; i < cells.Count; i++) {
				if ((position.x >= cells [i].position.x - cells [i].size / 2 && position.x <= cells [i].position.x + cells [i].size / 2)
				   && (position.y >= cells [i].position.y - cells [i].size / 2 && position.y <= cells [i].position.y + cells [i].size / 2)) {
					return cells [i];
				}
			}
			return null;

		}

		public void updateMesh() {

			vertices.Clear ();
			triangles.Clear ();
			colors.Clear ();
			cellMesh.triangles = triangles.ToArray ();
			cellMesh.colors = colors.ToArray ();
			cellMesh.vertices = vertices.ToArray ();

			if (split) {
				splitChildChunk ();
			} else {
				createMesh ();
			}

		}

		public void splitChildChunk() {

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

			if (transform.childCount > 0) {
				// remove all childChunk
				for (int i = transform.childCount - 1; i >= 0; i--) {
					Destroy (transform.GetChild (i).gameObject);
					childChunks [i] = null;
				}
			}

			if (MapGenerator.Instance.reduceMeshByChunk) {
				triangulate (cells);
			} else {
				for (int i = 0; i < cells.Count; i++) {
					triangulate (cells [i]);
					cells [i].currentChunk = this;
				}
			}

			cellMesh.vertices = vertices.ToArray ();
			cellMesh.colors = colors.ToArray ();
			cellMesh.triangles = triangles.ToArray ();
			cellMesh.RecalculateNormals ();

			// initialize material color
			cellRenderer.material.color = Color.white;
			cellCollider.sharedMesh = cellMesh;

		}

		public void triangulate(List<MapCell> cells) {

			if (cells.Count == 1) {
				triangulate (cells [0]);
			} else {
				for (int i = 0; i < cells.Count; i++) {
					cells [i].currentChunk = this;
				}

				Vector3 topLeft = cells [countY - 1].position + new Vector3 (-cells [countY - 1].size / 2, cells [countY - 1].size / 2, 0);
				Vector3 topRight = cells [countX * countY - 1].position + new Vector3 (cells [countX * countY - 1].size / 2, cells [countX * countY - 1].size / 2, 0);
				Vector3 bottomLeft = cells [0].position + new Vector3 (-cells [0].size / 2, -cells [0].size / 2, 0);
				Vector3 bottomRight = cells [countY * (countX - 1)].position + new Vector3 (cells [countY * (countX - 1)].size / 2, -cells [countY * (countX - 1)].size / 2, 0);

				Vector4 avgColorVector = new Vector4 ();
				for (int i = 0; i < cells.Count; i++) {
					avgColorVector += (Vector4)cells [i].color;
				}
				avgColorVector /= cells.Count;
				Color avgColor = (Color)avgColorVector;

				addTriangle (bottomLeft, topLeft, center);
				addTriangleColor (avgColor, avgColor, avgColor);
				addTriangle (center, topLeft, topRight);
				addTriangleColor (avgColor, avgColor, avgColor);
				addTriangle (topRight, bottomRight, center);
				addTriangleColor (avgColor, avgColor, avgColor);
				addTriangle (center, bottomRight, bottomLeft);
				addTriangleColor (avgColor, avgColor, avgColor);
			}

		}

		public void triangulate(MapCell cell) {

			Vector3 topLeft = cell.position + new Vector3 (-cell.size / 2, cell.size / 2, 0);
			Vector3 topRight = cell.position + new Vector3 (cell.size / 2, cell.size / 2, 0);
			Vector3 bottomLeft = cell.position + new Vector3 (-cell.size / 2, -cell.size / 2, 0);
			Vector3 bottomRight = cell.position + new Vector3 (cell.size / 2, -cell.size / 2, 0);

			addTriangle (bottomLeft, topLeft, cell.position);
			addTriangleColor (cell.color, cell.color, cell.color);
			addTriangle (cell.position, topLeft, topRight);
			addTriangleColor (cell.color, cell.color, cell.color);
			addTriangle (topRight, bottomRight, cell.position);
			addTriangleColor (cell.color, cell.color, cell.color);
			addTriangle (cell.position, bottomRight, bottomLeft);
			addTriangleColor (cell.color, cell.color, cell.color);

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