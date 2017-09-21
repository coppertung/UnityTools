using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools;

namespace UnityTools.AI {

	public class TopDown2DNavMeshAgent : MonoBehaviour, IFixedUpdateable {

		// The update call will be called in prior if the priority is larger.
		public int priority {
			get;
			set;
		}
			
		public float speed;
		public float errorDistance;
		[HideInInspector]
		public Vector3 target {
			get;
			private set;
		}
		[HideInInspector]
		public List<Vector3> path {
			get;
			private set;
		}

		// private Vector3 speedInVec3;
		private Collider2D col2D;

		void Awake() {

			// speedInVec3 = new Vector3 (speed, speed, 0);
			col2D = GetComponent<Collider2D>();
			if (col2D == null) {
				throw new NullReferenceException ("The GameObject must be attached with at least one 2D collider!");
			}

		}

		public void setTarget(Vector3 target) {

			this.target = target;
			target.z = 0;
			// find the path
			float minStartDistance = float.MaxValue;
			NavMesh2DNode closestStart = null;
			float minGoalDistance = float.MaxValue;
			NavMesh2DNode closestGoal = null;
			if (TopDown2DNavMeshBaker.navMeshNodes == null) {
				TopDown2DNavMeshBaker.init ();
			}
			for (int i = 0; i < TopDown2DNavMeshBaker.navMeshNodes.nodes.Count; i++) {
				// find start
				if (transform.position != TopDown2DNavMeshBaker.navMeshNodes.nodes [i].position) {
					float distance = Vector3.Distance (transform.position, TopDown2DNavMeshBaker.navMeshNodes.nodes [i].position);
					if (distance < minStartDistance) {
						minStartDistance = distance;
						closestStart = TopDown2DNavMeshBaker.navMeshNodes.nodes [i];
					}
				}
				// find goal
				if (target != TopDown2DNavMeshBaker.navMeshNodes.nodes [i].position) {
					float distance = Vector3.Distance (target, TopDown2DNavMeshBaker.navMeshNodes.nodes [i].position);
					if (distance < minGoalDistance) {
						minGoalDistance = distance;
						closestGoal = TopDown2DNavMeshBaker.navMeshNodes.nodes [i];
					}
				}
			}
			List<IAStarable<Vector3>> map = new List<IAStarable<Vector3>> ();
			for (int i = 0; i < TopDown2DNavMeshBaker.navMeshNodes.nodes.Count; i++) {
				IAStarable<Vector3> newNode = (IAStarable<Vector3>)TopDown2DNavMeshBaker.navMeshNodes.nodes [i];
				newNode.heuristicFunction = () => {
					return Vector3.Distance (newNode.value, closestGoal.position);
				};
				map.Add (newNode);
			}
			StartCoroutine (AStarAlgorithm.findPath<Vector3> (map, (IAStarable<Vector3>)closestStart, (IAStarable<Vector3>)closestGoal, pathHandler));

		}

		public void pathHandler(List<IAStarable<Vector3>> result) {

			if (result != null) {
				List<NavMesh2DNode> newPath = new List<NavMesh2DNode> ();
				if (path == null) {
					path = new List<Vector3> ();
				} else if (path.Count > 0) {
					path.Clear ();
				}
				for (int i = 0; i < result.Count; i++) {
					NavMesh2DNode newNode = (NavMesh2DNode)result [i];
					newNode.heuristicFunction = null;
					newPath.Add (newNode);
				}
				Vector3 curPosition = transform.position;
				curPosition.z = 0;
				this.path.Add (curPosition);
				// do linear optimization to the path
				Vector3 lastStartPoint = curPosition;
				Vector3 lastEndPoint = newPath[0].position;
				bool canSkip = true;
				for (int i = 1; i < newPath.Count; i++) {
					canSkip = true;
					RaycastHit2D[] targetPoint = Physics2D.LinecastAll (lastStartPoint, newPath [i].position);
					for (int j = 0; j < targetPoint.Length; j++) {
						if (targetPoint[j].transform != null && targetPoint[j].transform.gameObject.isStatic) {
							canSkip = false;
						}
					}
					if (canSkip) {
						lastEndPoint = newPath [i].position;
					} else {
						this.path.Add (lastEndPoint);
						lastStartPoint = newPath [i].position;
					}
					// Debug.Log ("Path point " + i + " is (" + newPath [i].position.x + ", " + newPath [i].position.y + ").");
				}
				this.path.Add (target);
				if (!UpdateManager.IsRegistered (this)) {
					UpdateManager.RegisterFixedUpdate (this);
				}
			}

		}

		public void fixedUpdateEvent() {
			// Used to replace the Update().
			// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
			if (path.Count > 0) {
				// showing line, delete soon
				for (int i = 1; i < path.Count; i++) {
					Debug.DrawLine (path [i - 1], path[i], Color.white);
				}
				Vector3 curPosition = transform.position;
				curPosition.z = 0;
				// Debug.Log ("Current Position is (" + curPosition.x + ", " + curPosition.y + "), Next point is (" + path [0].x + ", " + path [0].y + "). | distance = " + Vector3.Distance(curPosition, path [0]));
				if (Vector3.Distance(curPosition, path [0]) <= errorDistance) {
					path.Remove (path [0]);
				} 
				if (path.Count > 0) {
					// move and rotate
					// transform.LookAt (path [0]);
					transform.Translate ((path [0] - curPosition).normalized * speed * Time.deltaTime);
				}
			} else {
				UpdateManager.UnregisterFixedUpdate (this);
			}
		}

	}

}