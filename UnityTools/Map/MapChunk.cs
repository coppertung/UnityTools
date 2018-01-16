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
		public Color avgColor {
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
				Vector2 refPos = new Vector2 (MapGenerator.Instance.LODReferenceObject.transform.position.x, MapGenerator.Instance.LODReferenceObject.transform.position.y);
				Vector2 centerPos = new Vector2 (center.x, center.y);
				if (split) {
					if (lod <= 1 || countX * countY == 1
						|| Vector2.Distance (centerPos, refPos) > Mathf.Pow (MapGenerator.Instance.LODReferenceDistance, lod - 1)) {
						split = false;
						updateMesh ();
					}
				} else {
					if (lod > 1 && countX * countY != 1 &&
						Vector3.Distance (centerPos, refPos) <= Mathf.Pow (MapGenerator.Instance.LODReferenceDistance, lod - 1)) {
						split = true;
						updateMesh ();
					}
				}
			} else {
				if (lod <= 1 || countX * countY == 1) {
					split = false;
					updateMesh ();
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

			cellCollider.enabled = false;

			// clear original memory
			vertices.Clear ();
			triangles.Clear ();
			colors.Clear ();
			cellMesh.triangles = null;
			cellMesh.colors = null;
			cellMesh.vertices = null;

			// update color of chunk
			Vector4 avgColorVector = new Vector4 ();
			for (int i = 0; i < cells.Count; i++) {
				avgColorVector += (Vector4)cells [i].color;
			}
			avgColorVector /= cells.Count;
			avgColor = (Color)avgColorVector;

			// generate chunk by spliting chunk or create mesh
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

			// update cells' current chunk variable
			for (int i = 0; i < cells.Count; i++) {
				cells [i].currentChunk = this;
			}

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
				}
			}

			cellMesh.vertices = vertices.ToArray ();
			cellMesh.colors = colors.ToArray ();
			cellMesh.triangles = triangles.ToArray ();
			cellMesh.RecalculateNormals ();

			// initialize material color
			cellRenderer.material.color = Color.white;
			cellCollider.sharedMesh = cellMesh;
			cellCollider.enabled = true;

		}

		public void triangulate(List<MapCell> cells) {

			if (cells.Count == 1) {
				triangulate (cells [0]);
			} else {
				Vector3 topLeft = cells [countY - 1].position + new Vector3 (-cells [countY - 1].size / 2, cells [countY - 1].size / 2, 0);
				Vector3 topRight = cells [countX * countY - 1].position + new Vector3 (cells [countX * countY - 1].size / 2, cells [countX * countY - 1].size / 2, 0);
				Vector3 bottomLeft = cells [0].position + new Vector3 (-cells [0].size / 2, -cells [0].size / 2, 0);
				Vector3 bottomRight = cells [countY * (countX - 1)].position + new Vector3 (cells [countY * (countX - 1)].size / 2, -cells [countY * (countX - 1)].size / 2, 0);

				List<Vector3> outerTop = getChunkOuterVertices (cells, CellDirection.Top);
				List<Vector3> outerBottom = getChunkOuterVertices (cells, CellDirection.Bottom);
				List<Vector3> outerLeft = getChunkOuterVertices (cells, CellDirection.Left);
				List<Vector3> outerRight = getChunkOuterVertices (cells, CellDirection.Right);

				// base squares
				addComplexQuad (center, bottomLeft, topLeft, topRight, bottomRight);
				addComplexQuadColor (avgColor, avgColor, avgColor, avgColor, avgColor);

			}

		}

		public void triangulate(MapCell cell) {

			// solid square vertices
			Vector3 topLeft = cell.position + new Vector3 (-cell.size / 2, cell.size / 2, 0) * MapGenerator.Instance.solidColorFactor;
			Vector3 topRight = cell.position + new Vector3 (cell.size / 2, cell.size / 2, 0) * MapGenerator.Instance.solidColorFactor;
			Vector3 bottomLeft = cell.position + new Vector3 (-cell.size / 2, -cell.size / 2, 0) * MapGenerator.Instance.solidColorFactor;
			Vector3 bottomRight = cell.position + new Vector3 (cell.size / 2, -cell.size / 2, 0) * MapGenerator.Instance.solidColorFactor;

			// blend color part vertices
			Vector3 outerTopLeft = cell.position + new Vector3 (-cell.size / 2, cell.size / 2, 0);
			Vector3 outerTopRight = cell.position + new Vector3 (cell.size / 2, cell.size / 2, 0);
			Vector3 outerBottomLeft = cell.position + new Vector3 (-cell.size / 2, -cell.size / 2, 0);
			Vector3 outerBottomRight = cell.position + new Vector3 (cell.size / 2, -cell.size / 2, 0);

			// colors
			Color topLeftColor = cell.neighbours [(int)CellDirection.TopLeft] >= 0 ? MapGenerator.Instance.cells [cell.neighbours [(int)CellDirection.TopLeft]].currentChunk.avgColor : cell.color;
			Color topRightColor = cell.neighbours [(int)CellDirection.TopRight] >= 0 ? MapGenerator.Instance.cells [cell.neighbours [(int)CellDirection.TopRight]].currentChunk.avgColor : cell.color;
			Color bottomLeftColor = cell.neighbours [(int)CellDirection.BottomLeft] >= 0 ? MapGenerator.Instance.cells [cell.neighbours [(int)CellDirection.BottomLeft]].currentChunk.avgColor : cell.color;
			Color bottomRightColor = cell.neighbours [(int)CellDirection.BottomRight] >= 0 ? MapGenerator.Instance.cells [cell.neighbours [(int)CellDirection.BottomRight]].currentChunk.avgColor : cell.color;
			Color leftColor = cell.neighbours [(int)CellDirection.Left] >= 0 ? MapGenerator.Instance.cells [cell.neighbours [(int)CellDirection.Left]].currentChunk.avgColor : cell.color;
			Color rightColor = cell.neighbours [(int)CellDirection.Right] >= 0 ? MapGenerator.Instance.cells [cell.neighbours [(int)CellDirection.Right]].currentChunk.avgColor : cell.color;
			Color topColor = cell.neighbours [(int)CellDirection.Top] >= 0 ? MapGenerator.Instance.cells [cell.neighbours [(int)CellDirection.Top]].currentChunk.avgColor : cell.color;
			Color bottomColor = cell.neighbours [(int)CellDirection.Bottom] >= 0 ? MapGenerator.Instance.cells [cell.neighbours [(int)CellDirection.Bottom]].currentChunk.avgColor : cell.color;

			// base square
			addComplexQuad (cell.position, bottomLeft, topLeft, topRight, bottomRight);
			addComplexQuadColor (cell.color, cell.color, cell.color, cell.color, cell.color);

			// blend color part
			addBlendRegionSide (CellDirection.Left, outerBottomLeft, outerTopLeft, topLeft, bottomLeft, cell.color, bottomColor, bottomLeftColor, leftColor, topLeftColor, topColor);
			addBlendRegionSide (CellDirection.Top, outerTopLeft, outerTopRight, topRight, topLeft, cell.color, leftColor, topLeftColor, topColor, topRightColor, rightColor);
			addBlendRegionSide (CellDirection.Right, outerTopRight, outerBottomRight, bottomRight, topRight, cell.color, topColor, topRightColor, rightColor, bottomRightColor, bottomColor);
			addBlendRegionSide (CellDirection.Bottom, outerBottomRight, outerBottomLeft, bottomLeft, bottomRight, cell.color, rightColor, bottomRightColor, bottomColor, bottomLeftColor, leftColor);

		}

		private List<Vector3> getChunkOuterVertices(List<MapCell> cells, CellDirection direction) {

			List<Vector3> outerVertices = new List<Vector3> ();

			Vector3 topRightCell = cells [countX * countY - 1].position;
			Vector3 bottomLeftCell = cells [0].position;

			for (int i = 0; i < cells.Count; i++) {
				if (direction == CellDirection.Left && Mathf.Approximately (cells [i].position.y, bottomLeftCell.y)) {
					// left
					Vector3 bottomLeft = cells [i].position + new Vector3 (-cells [i].size / 2, -cells [i].size / 2, 0);
					Vector3 topLeft = cells [i].position + new Vector3 (-cells [i].size / 2, cells [i].size / 2, 0);
					if (!outerVertices.Contains (bottomLeft)) {
						outerVertices.Add (bottomLeft);
					}
					if (!outerVertices.Contains (topLeft)) {
						outerVertices.Add (topLeft);
					}
				} else if (direction == CellDirection.Top && Mathf.Approximately (cells [i].position.x, topRightCell.x)) {
					// top
					Vector3 topLeft = cells [i].position + new Vector3 (-cells [i].size / 2, cells [i].size / 2, 0);
					Vector3 topRight = cells [i].position + new Vector3 (cells [i].size / 2, cells [i].size / 2, 0);
					if (!outerVertices.Contains (topLeft)) {
						outerVertices.Add (topLeft);
					}
					if (!outerVertices.Contains (topRight)) {
						outerVertices.Add (topRight);
					}
				} else if (direction == CellDirection.Right && Mathf.Approximately (cells [i].position.y, topRightCell.y)) {
					// right
					Vector3 topRight = cells [i].position + new Vector3 (cells [i].size / 2, cells [i].size / 2, 0);
					Vector3 bottomRight = cells [i].position + new Vector3 (cells [i].size / 2, -cells [i].size / 2, 0);
					if (!outerVertices.Contains (topRight)) {
						outerVertices.Add (topRight);
					}
					if (!outerVertices.Contains (bottomRight)) {
						outerVertices.Add (bottomRight);
					}
				} else if (direction == CellDirection.Bottom && Mathf.Approximately (cells [i].position.x, bottomLeftCell.x)) {
					// bottom
					Vector3 bottomLeft = cells [i].position + new Vector3 (-cells [i].size / 2, -cells [i].size / 2, 0);
					Vector3 bottomRight = cells [i].position + new Vector3 (cells [i].size / 2, -cells [i].size / 2, 0);
					if (!outerVertices.Contains (bottomLeft)) {
						outerVertices.Add (bottomLeft);
					}
					if (!outerVertices.Contains (bottomRight)) {
						outerVertices.Add (bottomRight);
					}
				}
			}

			return outerVertices;

		}

		private List<Color> getChunkNeighbourColor(List<MapCell> cells, CellDirection direction) {



		}

		private void addBlendRegionSide(CellDirection direction, Vector3 outerLeft, Vector3 outerRight, Vector3 innerRight, Vector3 innerLeft, Color self, Color neighbourLeft, Color neighbourLeftCorner, Color neighbour, Color neighbourRightCorner, Color neighbourRight) {

			Vector3 cornerLeft = Vector3.zero;
			Vector3 cornerRight = Vector3.zero;

			switch (direction) {
			case CellDirection.Left:
				cornerLeft = new Vector3 (outerLeft.x, innerLeft.y, outerLeft.z);
				cornerRight = new Vector3 (outerRight.x, innerRight.y, innerRight.z);
				break;
			case CellDirection.Top:
				cornerLeft = new Vector3 (innerLeft.x, outerLeft.y, outerLeft.z);
				cornerRight = new Vector3 (innerRight.x, outerRight.y, innerRight.z);
				break;
			case CellDirection.Right:
				cornerLeft = new Vector3 (outerLeft.x, innerLeft.y, outerLeft.z);
				cornerRight = new Vector3 (outerRight.x, innerRight.y, outerLeft.z);
				break;
			case CellDirection.Bottom:
				cornerLeft = new Vector3 (innerLeft.x, outerLeft.y, outerLeft.z);
				cornerRight = new Vector3 (innerRight.x, outerRight.y, innerRight.z);
				break;
			default:
				break;
			}
			addTriangle (outerLeft, cornerLeft, innerLeft);
			addTriangleColor ((neighbour + neighbourLeft + neighbourLeftCorner + self) / 4, (neighbour + self) / 2, self);
			addSimpleQuad (innerLeft, cornerLeft, cornerRight, innerRight);
			addSimpleQuadColor (self, (neighbour + self) / 2, (neighbour + self) / 2, self); 
			addTriangle (innerRight, cornerRight, outerRight);
			addTriangleColor (self, (neighbour + self) / 2, (neighbour + neighbourRight + neighbourRightCorner + self) / 4);

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

		private void addSimpleQuad(Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight) {

			addTriangle (bottomLeft, topLeft, bottomRight);
			addTriangle (bottomRight, topLeft, topRight);

		}

		private void addSimpleQuadColor(Color bottomLeft, Color topLeft, Color topRight, Color bottomRight) {

			addTriangleColor (bottomLeft, topLeft, bottomRight);
			addTriangleColor (bottomRight, topLeft, topRight);

		}

		private void addComplexQuad(Vector3 center, Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight) {

			addTriangle (bottomLeft, topLeft, center);
			addTriangle (center, topLeft, topRight);
			addTriangle (topRight, bottomRight, center);
			addTriangle (center, bottomRight, bottomLeft);

		}

		private void addComplexQuadColor(Color center, Color bottomLeft, Color topLeft, Color topRight, Color bottomRight) {

			addTriangleColor (bottomLeft, topLeft, center);
			addTriangleColor (center, topLeft, topRight);
			addTriangleColor (topRight, bottomRight, center);
			addTriangleColor (center, bottomRight, bottomLeft);

		}

	}

}