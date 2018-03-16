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
		public int width;
		public int height;

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
			Vector3 startPosition = transform.position - new Vector3((float)(width - 1) / 2 * unitLength, (float)(height - 1) / 2 * unitLength, 0);
			vertices = new Vector3[(width + 1) * (height + 1)];
			// The texture coordinates, this is for us to mapping the texture
			Vector2[] uv = new Vector2[vertices.Length];
			// The tangents of each vertices, this is use for us to define normal map
			Vector4[] tangents = new Vector4[vertices.Length];
			// tangent point to the right
			Vector4 tangent = new Vector4 (1f, 0f, 0f, -1f);

			int index = 0;
			for (int y = 0; y <= height; y++) {
				for (int x = 0; x <= width; x++, index++) {
					vertices [index] = startPosition + new Vector3 (x * unitLength, y * unitLength);
					uv [index] = new Vector2 ((float)x / width, (float)y / height);
					tangents [index] = tangent;
					// yield return wait;	// unnecessary, just slow down the process
				}
			}
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.tangents = tangents;
			yield return null;

			// draw the surface in triangles (by assigning the vertex index)
			int[] triangles = new int[width * height * 6];
			int ti = 0;		// triangle index
			int vi = 0;		// vertex index
			for (int y = 0; y < height; y++, vi++) {
				for (int x = 0; x < width; x++, ti += 6, vi++) {
					// use two triangles to form a square
					triangles [ti] = vi;
					triangles [ti + 3] = triangles [ti + 2] = vi + 1;
					triangles [ti + 4] = triangles [ti + 1] = vi + width + 1;
					triangles [ti + 5] = vi + width + 2;
				}
			}
			mesh.triangles = triangles;
			yield return null;

			// recalculate the normals
			mesh.RecalculateNormals ();

		}

	}

}