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
		/// **ENHANCEMENT REQUIRE** currently O(n^3)
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
			Vector3 normal = Vector3.zero;
			result = new int[triangles.Count * 3];
			for (i = 0; i < triangles.Count; i++) {
				triangle = triangles [i];
				if (i == 0) {
					// finding the normal of the surface by finding the normal, take reference with the first triangle
					normal = Vector3.Cross (vertices [triangle [1]] - vertices [triangle [0]], vertices [triangle [2]] - vertices [triangle [0]]).normalized;
				}
				if (Vector3.Cross (vertices [triangle [1]] - vertices [triangle [0]], vertices [triangle [2]] - vertices [triangle [0]]).normalized == normal) {
					// clockwise
					result [i * 3] = triangle [0];
					result [i * 3 + 1] = triangle [1];
					result [i * 3 + 2] = triangle [2];
				} else {
					// anti-clockwise
					result [i * 3] = triangle [0];
					result [i * 3 + 1] = triangle [2];
					result [i * 3 + 2] = triangle [1];
				}
				Debug.Log (triangle [0] + ", " + triangle [1] + ", " + triangle [2]);
			}

			return result;

		}

		private static bool delaunayCondition(int[] triangle, Vector3[] vertices) {

			/*
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
					Debug.Log (triangle [0] + ", " + triangle [1] + ", " + triangle [2] + "[" + circumsphereRadius + "] : " + i + "[" + Vector3.Distance (circumsphereCenter, vertices [i]) + "]");
					return false;
				}
			}
			return true;
			*/
			// 3 points of the triangle
			Vector3 ptA = vertices [triangle [0]];
			Vector3 ptB = vertices [triangle [1]];
			Vector3 ptC = vertices [triangle [2]];

			// circumcenter in 3D
			Vector3 a = ptC - ptB;
			Vector3 b = ptC - ptA;
			Vector3 c = ptB - ptA;
			// vector from a to the circumcenter center
			Vector3 circumsphereCenter = 
				(a.sqrMagnitude * (b.sqrMagnitude + c.sqrMagnitude - a.sqrMagnitude) * ptA + b.sqrMagnitude * (c.sqrMagnitude + a.sqrMagnitude - b.sqrMagnitude) * ptB +
				c.sqrMagnitude * (a.sqrMagnitude + b.sqrMagnitude - c.sqrMagnitude) * ptC) / (a.sqrMagnitude * (b.sqrMagnitude + c.sqrMagnitude - a.sqrMagnitude) +
				b.sqrMagnitude * (c.sqrMagnitude + a.sqrMagnitude - b.sqrMagnitude) + c.sqrMagnitude * (a.sqrMagnitude + b.sqrMagnitude - c.sqrMagnitude));
			// Vector3 toCircumsphereCenter = (Vector3.Cross (abXac, ab) * ac.sqrMagnitude + Vector3.Cross (abXac, ac) * ab.sqrMagnitude) / (2f * abXac.sqrMagnitude);
			float circumsphereRadius = Vector3.Distance(circumsphereCenter, ptA);
			// check if there is any vertices exists in the circumsphere, by definition, the circumsphere must not contains any vertices other than the defined three
			for (int i = 0; i < vertices.Length; i++) {
				if (i != triangle [0] && i != triangle [1] && i != triangle [2] && Vector3.Distance(circumsphereCenter, vertices[i]) <= circumsphereRadius) {
					Debug.Log (triangle [0] + ", " + triangle [1] + ", " + triangle [2] + "[" + circumsphereRadius + "] : " + i + "[" + Vector3.Distance (circumsphereCenter, vertices [i]) + "]");
					return false;
				}
			}
			return true;

		}

	}

}