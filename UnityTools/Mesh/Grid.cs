using System.Collections;
using UnityEngine;

namespace UnityTools.Mesh {

	/// <summary>
	/// Reference from:
	/// http://catlikecoding.com/unity/tutorials/procedural-grid/
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class Grid : MonoBehaviour {

		public float unitLength;
		public int xSize, ySize;

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
			GetComponent<MeshFilter>().mesh = mesh = new UnityEngine.Mesh();
			mesh.name = "Procedural Grid";

			// define vertices
			Vector3 startPosition = transform.position - new Vector3((float)(xSize - 1) / 2 * unitLength, (float)(ySize - 1) / 2 * unitLength, 0);
			vertices = new Vector3[(xSize + 1) * (ySize + 1)];
			int index = 0;
			for (int y = 0; y <= ySize; y++) {
				for (int x = 0; x <= xSize; x++) {
					vertices [index++] = startPosition + new Vector3 (x * unitLength, y * unitLength);
					yield return wait;	// unnecessary, just slow down the process
				}
			}
			mesh.vertices = vertices;
			yield return null;

			// draw the surface in triangles (by assigning the vertex index)
			int[] triangles = new int[xSize * ySize * 6];
			int ti = 0;		// triangle index
			int vi = 0;		// vertex index
			for (int y = 0; y < ySize; y++, vi++) {
				for (int x = 0; x < xSize; x++, ti += 6, vi++) {
					// use two triangles to form a square
					triangles [ti] = vi;
					triangles [ti + 3] = triangles [ti + 2] = vi + 1;
					triangles [ti + 4] = triangles [ti + 1] = vi + xSize + 1;
					triangles [ti + 5] = vi + xSize + 2;
					yield return wait;	// unnecessary, just slow down the process
				}
			}
			mesh.triangles = triangles;
			yield return null;

			// recalculate the normals
			mesh.RecalculateNormals ();

		}

	}

}