using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Map {

	public enum CellDirection {
		BottomLeft = 0, Left = 1, TopLeft = 2, Top = 3,
		TopRight = 4, Right = 5, BottomRight = 6, Bottom = 7
	}

	[System.Serializable]
	public class MapCell {

		public int id;
		public int[] neighbours;
		public Vector3 coordinate;
		public Vector3 position;
		public Color color;

		public float size;

		public MapChunk currentChunk;

		public void init(int _id, Vector3 _coordinate, Vector3 _position, float _size, Color _color) {

			id = _id;
			coordinate = _coordinate;
			position = _position;
			size = _size;
			color = _color;

			neighbours = new int[8];
			for (int i = 0; i < neighbours.Length; i++) {
				neighbours [i] = -1;
			}

		}

		public static CellDirection OppositeDirection(CellDirection direction) {

			int dir = (int)direction;
			if (dir > 3)
				dir -= 4;
			else
				dir += 4;
			return (CellDirection)dir;

		}

	}

}