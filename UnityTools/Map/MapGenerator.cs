using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools.Patterns;

namespace UnityTools.Map {

	public class MapGenerator : Singleton<MapGenerator> {

		[System.Serializable]
		public class NamedColor {
			public string name;
			public Color color;
		}

		// Basic Information
		public int generateCellAmountPerFrame;
		public Vector2 maxGridSize;
		public float squareSize;
		public int maxLOD;
		[Range(0, 1)]
		public float solidColorFactor = 0.75f;
		public bool smoothColor;
		[Range(1, 8)]
		public int maxNeighbourToSmoothColor = 4;
		[Range(1, 100)]
		public int smoothColorChance = 50;
		public bool reduceMeshByChunk;
		[HideInInspector]
		public bool generatedCell = false;
		[HideInInspector]
		public bool generatedColor = false;
		[HideInInspector]
		public bool generateFinish = false;

		// Random seed
		public int seed = 0;
		public bool useRandomSeed;

		// Materials and Colors
		public Material defaultMaterial;
		public NamedColor[] colorList;

		// chunk control
		public GameObject LODReferenceObject;
		public float LODReferenceDistance;

		// Datas
		public List<MapCell> cells;
		public List<MapChunk> chunks;

		void Start () {

			// generateMap ();
			if (maxLOD == 0) {
				maxLOD = 1;
			}
			LODReferenceDistance = Mathf.Max (LODReferenceDistance, 2);

		}

		public void generateMap() {

			if (useRandomSeed || seed == 0) {
				seed = Environment.TickCount & Int32.MaxValue;
				Debug.Log (seed);
			}
			UnityEngine.Random.InitState (seed);
			if (cells == null) {
				cells = new List<MapCell> ();
			}
			if (chunks == null) {
				chunks = new List<MapChunk> ();
			}
			StartCoroutine (createMapCells ());

		}

		public void clearMap () {

			StartCoroutine (deleteMapCells ());
			generatedCell = false;
			generatedColor = false;
			generateFinish = false;

		}

		public IEnumerator createMapCells() {

			int countX = (int)maxGridSize.x;
			int countY = (int)maxGridSize.y;
			float mapWidth = (countX + 1) * squareSize;
			float mapLength = (countY + 1) * squareSize;

			// create all map cells
			for (int x = 0; x < countX; x++) {
				for (int y = 0; y < countY; y++) {
					Vector3 coord = new Vector3 (-maxGridSize.x / 2 + x + 1, -maxGridSize.y / 2 + y + 1, 0);
					Vector3 pos = new Vector3 (-mapWidth / 2 + x * squareSize + squareSize / 2, -mapLength / 2 + y * squareSize + squareSize / 2, 0);
					// GameObject newCellObject = new GameObject ("Cell_" + coord.x + "_" + coord.y + "_" + coord.z);
					// newCellObject.transform.SetParent (transform);
					// MapCell newCell = newCellObject.AddComponent<MapCell> ();
					MapCell newCell = new MapCell();
					newCell.init (x * countY + y, coord, pos, squareSize, colorList [UnityEngine.Random.Range (0, colorList.Length)].color);
					// newCell.cellRenderer.material = defaultMaterial;
					// newCell.createMesh ();
					cells.Add (newCell);
				}
			}

			// find all neighbours
			for (int x = 0; x < countX; x++) {
				for (int y = 0; y < countY; y++) {
					int i = x * countY + y;
					int neighbour;
					if (x > 0) {
						neighbour = (x - 1) * countY + y;
						cells [i].neighbours [(int)CellDirection.Left] = cells [neighbour].id;
						cells [neighbour].neighbours [(int)MapCell.OppositeDirection (CellDirection.Left)] = i;
						if (y > 0) {
							neighbour = (x - 1) * countY + (y - 1);
							cells [i].neighbours [(int)CellDirection.BottomLeft] = cells [neighbour].id;
							cells [neighbour].neighbours [(int)MapCell.OppositeDirection (CellDirection.BottomLeft)] = i;
						}
					}
					if (y > 0) {
						neighbour = x * countY + (y - 1);
						cells [i].neighbours [(int)CellDirection.Bottom] = cells [neighbour].id;
						cells [neighbour].neighbours [(int)MapCell.OppositeDirection (CellDirection.Bottom)] = i;
						if (x < countX - 1) {
							neighbour = (x + 1) * countY + (y - 1);
							cells [i].neighbours [(int)CellDirection.BottomRight] = cells [neighbour].id;
							cells [neighbour].neighbours [(int)MapCell.OppositeDirection (CellDirection.BottomRight)] = i;
						}
					}
				}
			}

			generatedCell = true;
			yield return smoothingColor ();

		}

		public IEnumerator smoothingColor() {

			yield return null;
			if (smoothColor) {
				for (int i = 0; i < cells.Count; i++) {
					int maxColor = -1;
					int[] colorCount = new int[colorList.Length];
					for (int j = 0; j < cells [i].neighbours.Length; j++) {
						if (maxColor >= 0) {
							break;
						}
						if (cells [i].neighbours [j] < 0) {
							continue;
						}
						for (int k = 0; k < colorList.Length; k++) {
							if (colorList [k].color == cells [cells [i].neighbours [j]].color) {
								colorCount [k] += 1;
								if (colorCount [k] > maxNeighbourToSmoothColor) {
									maxColor = k;
								}
								break;
							}
						}
					}
					if (maxColor < 0) {
						maxColor = 0;
						for (int j = 1; j < colorCount.Length; j++) {
							if (colorCount [j] > colorCount [maxColor]) {
								maxColor = j;
							}
						}
					}
					if (colorList [maxColor].color != cells [i].color && UnityEngine.Random.Range (1, 101) <= smoothColorChance) {
						cells [i].color = colorList [maxColor].color;
					}
				}
			}

			generatedColor = true;
			yield return createChunks ();

		}

		public IEnumerator createChunks() {

			yield return null;

			int countX = (int)maxGridSize.x;
			int countY = (int)maxGridSize.y;

			int numOfChunkX = countX / (int)Mathf.Pow (2, maxLOD - 1);
			int numOfChunkY = countY / (int)Mathf.Pow (2, maxLOD - 1);

			List<MapCell>[,] cellsForChunks = new List<MapCell>[numOfChunkX + 1, numOfChunkY + 1];

			for (int i = 0; i < numOfChunkX + 1; i++) {
				for (int j = 0; j < numOfChunkY + 1; j++) {
					cellsForChunks [i, j] = new List<MapCell> ();
				}
			}
			for (int x = 0; x < countX; x++) {
				int chunkNumX = x / (int)Mathf.Pow (2, maxLOD - 1);
				for (int y = 0; y < countY; y++) {
					int chunkNumY = y / (int)Mathf.Pow (2, maxLOD - 1);
					cellsForChunks [chunkNumX, chunkNumY].Add (cells [x * countY + y]);
				}
			}
			for (int i = 0; i < numOfChunkX + 1; i++) {
				for (int j = 0; j < numOfChunkY + 1; j++) {
					if (cellsForChunks [i, j].Count > 0) {
						GameObject newChunkObject = new GameObject ("Cell Chunk Level " + maxLOD);
						newChunkObject.transform.SetParent (transform);
						MapChunk newChunk = newChunkObject.AddComponent<MapChunk> ();
						int numOfChunk = (int)Mathf.Pow (2, maxLOD - 1);
						newChunk.init (maxLOD, cellsForChunks [i, j], numOfChunk, numOfChunk);
						newChunk.cellRenderer.material = defaultMaterial;
						newChunk.updateMesh ();
						chunks.Add (newChunk);
					}
				}
			}

			generateFinish = true;
			yield return null;

		}

		public IEnumerator deleteMapCells() {

			MapChunk deletedChunk;
			// delete all cells
			for (int i = chunks.Count - 1; i >= 0; i--) {
				deletedChunk = chunks [i];
				chunks.Remove (deletedChunk);
				Destroy (deletedChunk.gameObject);
			}
			yield return null;

		}

	}

}