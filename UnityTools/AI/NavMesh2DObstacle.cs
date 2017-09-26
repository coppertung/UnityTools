using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityTools.AI {

	/// <summary>
	/// Used to define the gameobject as the obstacles in the Navigation Mesh 2D system.
	/// </summary>
    public class NavMesh2DObstacle : MonoBehaviour {

		/// <summary>
		/// The custom bound offset used to adjust its position.
		/// Default is set as 0.
		/// Noted that this custom bound is in circle shape.
		/// </summary>
		public float customBoundOffset = 0;

		private Collider2D col2D;

		/// <summary>
		/// Is the spectified position collided with the box?
		/// </summary>
		public bool isCollided(Vector3 position) {

			if (col2D == null) {
				col2D = GetComponent<Collider2D> ();
			}
			if (customBoundOffset > 0) {
				return Vector3.Distance (position, transform.position) <= customBoundOffset;
			} else {
				return col2D.bounds.Contains (position);
			}

		}

		#if UNITY_EDITOR
		void OnDrawGizmosSelected() {

			// view the custom bounding of the agent
			if (customBoundOffset > 0) {
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere (transform.position, customBoundOffset);
			}

		}
		#endif

    }

}
