using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools.Patterns;

namespace UnityTools.Map {

	public class MapGenerator : Singleton<MapGenerator> {

		public int generateCellAmountPerFrame;
		public Vector2 maxGridSize;
		public float squareSize;
		public CellType cellType;

		public string seed;
		public bool useRandomSeed;

		public Material defaultMaterial;
		public Color defaultColor;
		public Color selectedColor;

		public List<MapCell> cells;

		void Start () {

			generateMap ();

		}

		public void generateMap() {

			cells = new List<MapCell> ();
			StartCoroutine (createMapCells ());

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
					newCell.init (x * countY + y, coord, pos, squareSize, defaultColor);
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
			for (int i = 1; i < cells.Count; i++) {
				for (int j = 0; j < i; j++) {
					if (Vector3.Distance (cells [i].coordinate, cells [j].coordinate) < 2) {
						cells [i].neighbours.Add (cells [j].id);
						cells [j].neighbours.Add (cells [i].id);
						countGeneratedCells += 1;
						if (countGeneratedCells >= generateCellAmountPerFrame) {
							countGeneratedCells = 0;
							yield return null;
						}
					}
				}
			}

		}

	}

}