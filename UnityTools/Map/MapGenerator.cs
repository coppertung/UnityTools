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
		public CellType cellType;
		public bool smoothColor;
		[HideInInspector]
		public bool generatedMesh = false;
		[HideInInspector]
		public bool generateFinish = false;

		// Random seed
		public int seed = 0;
		public bool useRandomSeed;

		// Materials and Colors
		public Material defaultMaterial;
		public NamedColor[] colorList;

		// Datas
		public List<MapCell> cells;

		void Start () {

			// generateMap ();

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
			StartCoroutine (createMapCells ());

		}

		public void clearMap () {

			StartCoroutine (deleteMapCells ());
			generatedMesh = false;
			generateFinish = false;

		}

		public IEnumerator createMapCells() {

			int countX = (int)maxGridSize.x;
			int countY = (int)maxGridSize.y;
			float mapWidth = (countX + 1) * squareSize;
			float mapLength = (countY + 1) * squareSize;

			int countGeneratedCells = 0;

			// create all map cells
			for (int x = 0; x < countX; x++) {
				for (int y = 0; y < countY; y++) {
					Vector3 coord = new Vector3 (-maxGridSize.x / 2 + x + 1, -maxGridSize.y / 2 + y + 1, 0);
					Vector3 pos = new Vector3 (-mapWidth / 2 + x * squareSize + squareSize / 2, -mapLength / 2 + y * squareSize + squareSize / 2, 0);
					GameObject newCellObject = new GameObject ("Cell_" + coord.x + "_" + coord.y + "_" + coord.z);
					newCellObject.transform.SetParent (transform);
					MapCell newCell = newCellObject.AddComponent<MapCell> ();
					newCell.init (x * countY + y, coord, pos, squareSize, colorList [UnityEngine.Random.Range (0, colorList.Length)].color);
					newCell.cellRenderer.material = defaultMaterial;
					newCell.cellType = cellType;
					newCell.createMesh ();
					cells.Add (newCell);
					countGeneratedCells += 1;
					if (countGeneratedCells >= generateCellAmountPerFrame) {
						countGeneratedCells = 0;
						yield return null;
					}
				}
			}

			countGeneratedCells = 0;
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
						cells [i].neighbours [(int)CellDirection.Top] = cells [neighbour].id;
						cells [neighbour].neighbours [(int)MapCell.OppositeDirection (CellDirection.Top)] = i;
						if (x < countX - 1) {
							neighbour = (x + 1) * countY + (y - 1);
							cells [i].neighbours [(int)CellDirection.BottomRight] = cells [neighbour].id;
							cells [neighbour].neighbours [(int)MapCell.OppositeDirection (CellDirection.BottomRight)] = i;
						}
					}
				}
			}

			generatedMesh = true;
			yield return smoothingColor ();

		}

		public IEnumerator smoothingColor() {

			if (smoothColor) {
				int countSmoothedCells = 0;
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
								if (colorCount [k] > cells [i].neighbours.Length / 2) {
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
					if (colorList [maxColor].color != cells [i].color) {
						cells [i].color = colorList [maxColor].color;
						cells [i].createMesh ();
					}
					countSmoothedCells += 1;
					if (countSmoothedCells >= generateCellAmountPerFrame) {
						countSmoothedCells = 0;
						yield return null;
					}
				}
			}
			generateFinish = true;

		}

		public IEnumerator deleteMapCells() {

			int countDestroyCells = 0;
			MapCell deletedCell;
			// delete all cells
			for (int i = cells.Count - 1; i >= 0; i--) {
				deletedCell = cells [i];
				cells.Remove (deletedCell);
				Destroy (deletedCell.gameObject);
				if (countDestroyCells >= generateCellAmountPerFrame) {
					countDestroyCells = 0;
					yield return null;
				}
			}

		}

	}

}