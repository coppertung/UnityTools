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
			generateVertices ();
			yield return null;

			// draw the surface in triangles (by assigning the vertex index)
			generateTriangles ();
			yield return null;

		}

		private void generateVertices() {

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

		}

		private void generateTriangles() {
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