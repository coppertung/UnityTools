using System.Collections;
using UnityEngine;

namespace UnityTools.Mesh {

	/// <summary>
	/// Reference from:
	/// http://catlikecoding.com/unity/tutorials/rounded-cube/
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class Cube : MonoBehaviour {

		public float unitLength;
		public int width;		// x
		public int height;		// y
		public int length;		// z

		private UnityEngine.Mesh mesh;
		private Vector3[] vertices;

		void Awake() {

			StartCoroutine (generate ());

		}

		void OnDrawGizmos() {

			if (vertices == null) {
				return;
			}
			// draw vertices
			Gizmos.color = Color.black;
			for (int i = 0; i < vertices.Length; i++) {
				Gizmos.DrawSphere (vertices [i], 0.1f);
			}

		}

		private IEnumerator generate() {

			WaitForSeconds wait = new WaitForSeconds (0.05f);
			// create mesh
			GetComponent<MeshFilter> ().mesh = mesh = new UnityEngine.Mesh ();
			mesh.name = "Procedural Cube";

			// define vertices
			yield return generateVertices ();

			// draw the surface in triangles (by assigning the vertex index)
			yield return generateTriangles ();

		}

		private IEnumerator generateVertices() {

			int cornerVertices = 8;
			int edgeVertices = (width + height + length - 3) * 4;
			int faceVertices = (
				(width - 1) * (length - 1) +
				(width - 1) * (height - 1) + 
				(length - 1) * (height - 1)
			) * 2;
			vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

			// Vector3 startPosition = transform.position - new Vector3 ((float)(width - 1) / 2 * unitLength, (float)(height - 1) / 2 * unitLength, (float)(length - 1) / 2 * unitLength);
			Vector3 startPosition = transform.position;
			int v = 0;
			// four faces
			for (int y = 0; y <= height; y++) {
				for (int x = 0; x <= width; x++) {
					vertices [v++] = startPosition + new Vector3 (x * unitLength, y * unitLength, 0);
				}
				for (int z = 1; z <= length; z++) {
					vertices [v++] = startPosition + new Vector3 (width * unitLength, y * unitLength, z * unitLength);
				}
				for (int x = width - 1; x >= 0; x--) {
					vertices [v++] = startPosition + new Vector3 (x * unitLength, y * unitLength, length * unitLength);
				}
				for (int z = length - 1; z > 0; z--) {
					vertices [v++] = startPosition + new Vector3 (0, y * unitLength, z * unitLength);
				}
			}
			// top and bottom
			for (int z = 1; z < length; z++) {
				for (int x = 1; x < width; x++) {
					vertices [v++] = startPosition + new Vector3 (x * unitLength, height * unitLength, z * unitLength);
				}
			}
			for (int z = 1; z < length; z++) {
				for (int x = 1; x < width; x++) {
					vertices [v++] = startPosition + new Vector3 (x * unitLength, 0, z * unitLength);
				}
			}
			mesh.vertices = vertices;
			yield return null;

		}

		private IEnumerator generateTriangles() {

			int quads = (width * height + width * length + length * height) * 2;
			int[] triangles = new int[quads * 6];
			int ring = (width + length) * 2;
			int t = 0;		// triangle indices
			int v = 0;		// vertices indices

			for (int y = 0; y < height; y++, v++) {
				// last quad has to generate seperately
				for (int q = 0; q < ring - 1; q++, v++) {
					t = setQuad (triangles, t, v, v + 1, v + ring, v + ring + 1);
				}
				t = setQuad (triangles, t, v, v - ring + 1, v + ring, v + 1);
			}
			// top and bottom
			t = CreateTopFace (triangles, t, ring);

			mesh.triangles = triangles;
			yield return null;

		}

		private int CreateTopFace(int[] triangles, int t, int ring) {

			int v = ring * height;

			// first row
			for (int x = 0; x < width - 1; x++, v++) {
				t = setQuad (triangles, t, v, v + 1, v - ring + 1, v + ring);
			}
			t = setQuad (triangles, t, v, v + 1, v - ring + 1, v + 2);

			// middle rows
			int vMin = ring * (height + 1) - 1;
			int vMid = vMin + 1;
			int vMax = v + 2;
			for (int z = 1; z < length - 1; z++, vMin--, vMid++, vMax++) {
				t = setQuad (triangles, t, vMin, vMid, vMin - 1, vMid + width - 1);
				for (int x = 1; x < width - 1; x++, vMid++) {
					t = setQuad (triangles, t, vMid, vMid + 1, vMid + width - 1, vMid + width);
				}
				t = setQuad (triangles, t, vMid, vMax, vMid + width - 1, vMax + 1);
			}

			// last row
			int vTop = vMin - 2;
			t = setQuad (triangles, t, vMin, vMid, vTop + 1, vTop);
			for (int x = 1; x < width - 1; x++, vTop--, vMid++) {
				t = setQuad (triangles, t, vMid, vMid + 1, vTop, vTop - 1);
			}
			t = setQuad (triangles, t, vMid, vTop - 2, vTop, vTop - 1);

			return t;

		}

		private int setQuad(int[] triangles, int startIndex, int leftBottom, int rightBottom, int leftTop, int rightTop) {

			// the drawing order of vertices of triangle is same as 2D
			triangles [startIndex] = leftBottom;
			triangles [startIndex + 1] = triangles [startIndex + 4] = leftTop;
			triangles [startIndex + 2] = triangles [startIndex + 3] = rightBottom;
			triangles [startIndex + 5] = rightTop;
			return startIndex + 6;

		}

	}

}