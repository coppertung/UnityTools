using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Mesh {

	/// <summary>
	/// 3D Delaunay Triangulation Algorithm.
	/// Reference from:
	/// http://answers.unity3d.com/questions/307544/mesh-from-vertices-polygon-from-spline.html
	/// http://gamedev.stackexchange.com/questions/60630/how-do-i-find-the-circumcenter-of-a-triangle-in-3d
	/// </summary>
	public class DelanunayTriangulation {

		/// <summary>
		/// Calculate the triangles which satisfied the rules of Delanunay Triangulation.
		/// More information can be checked in:
		/// https://en.wikipedia.org/wiki/Delaunay_triangulation
		/// </summary>
		public static int[] Calculate(Vector3[] vertices) {

			if (vertices.Length == 0) {
				return new int[1];
			}

			int[] result;
			int i, j, k;

			if (vertices.Length <= 3) {
				result = new int[vertices.Length];
				for (i = 0; i < vertices.Length; i++) {
					result [i] = i;
				}
				return result;
			}

			List<int[]> triangles = new List<int[]> ();
			int[] triangle;

			// find all combinations of triangles without repetition
			for (i = 0; i < vertices.Length - 2; i++) {
				for (j = i + 1; j < vertices.Length - 1; j++) {
					for (k = j + 1; k < vertices.Length; k++) {
						triangle = new int[] { i, j, k };
						triangles.Add (triangle);
					}
				}
			}
			Debug.Log ("Before Delaunay Conditioning: " + triangles.Count);
			for (i = triangles.Count - 1; i >= 0; i--) {
				if (!delaunayCondition (triangles [i], vertices)) {
					triangles.RemoveAt (i);
				}
			}
			Debug.Log ("After Delaunay Conditioning: " + triangles.Count);
			result = new int[triangles.Count * 3];
			for (i = 0; i < triangles.Count; i++) {
				triangle = triangles [i];
				if (i != triangles.Count - 1) {
					result [i * 3] = triangle [0];
					result [i * 3 + 1] = triangle [1];
					result [i * 3 + 2] = triangle [2];
				} else {
					result [i * 3] = triangle [1];
					result [i * 3 + 1] = triangle [2];
					result [i * 3 + 2] = triangle [0];
				}
			}

			return result;

		}

		private static bool delaunayCondition(int[] triangle, Vector3[] vertices) {

			// 3 points of the triangle
			Vector3 a = vertices [triangle [0]];
			Vector3 b = vertices [triangle [1]];
			Vector3 c = vertices [triangle [2]];

			// circumcenter in 3D
			Vector3 ac = c - a;
			Vector3 ab = b - a;
			Vector3 abXac = Vector3.Cross (ab, ac);
			// vector from a to the circumcenter center
			Vector3 toCircumsphereCenter = (Vector3.Cross (abXac, ab) * ac.sqrMagnitude + Vector3.Cross (abXac, ac) * ab.sqrMagnitude) / (2f * abXac.sqrMagnitude);
			Vector3 circumsphereCenter = a + toCircumsphereCenter;
			float circumsphereRadius = toCircumsphereCenter.magnitude;
			// check if there is any vertices exists in the circumsphere, by definition, the circumsphere must not contains any vertices other than the defined three
			for (int i = 0; i < vertices.Length; i++) {
				if (i != triangle [0] && i != triangle [1] && i != triangle [2] && Vector3.Distance(circumsphereCenter, vertices[i]) <= circumsphereRadius) {
					return false;
				}
			}
			return true;

		}

	}

}