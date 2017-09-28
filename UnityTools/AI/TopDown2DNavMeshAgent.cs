using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityTools;

namespace UnityTools.AI {

	public class TopDown2DNavMeshAgent : MonoBehaviour, IFixedUpdateable {

		// The update call will be called in prior if the priority is larger.
		public int priority {
			get;
			set;
		}
			
		/// <summary>
		/// Speed of agent travel through the path.
		/// </summary>
		public float speed;
		/// <summary>
		/// Acceptable error distance between the nodes and the agent.
		/// </summary>
		public float errorDistance;
		/// <summary>
		/// The custom bound offset used to adjust its position.
		/// Default is set as 0.
		/// Noted that this custom bound is in circle shape.
		/// </summary>
		public float customBoundOffset = 0;

		/// <summary>
		/// Target position the agent want to travel.
		/// </summary>
		[HideInInspector]
		public Vector3 target {
			get;
			private set;
		}
		/// <summary>
		/// The path used to travel to the target position.
		/// </summary>
		[HideInInspector]
		public List<Vector3> path {
			get;
			private set;
		}

		private Collider2D col2D;

		void Awake() {

			col2D = GetComponent<Collider2D>();
			if (col2D == null) {
				throw new NullReferenceException ("The GameObject must be attached with at least one 2D collider!");
			}

		}

		/// <summary>
		/// Set the target position of the agent.
		/// </summary>
		public void setTarget(Vector3 target) {

			this.target = target;
			target.z = 0;
			// find the path
			if (TopDown2DNavMeshBaker.navMeshNodes == null) {
				TopDown2DNavMeshBaker.init ();
			}
			NavMesh2DNode closestStart = TopDown2DNavMeshBaker.navMeshNodes.findNearestNode (transform.position);
			NavMesh2DNode closestGoal = TopDown2DNavMeshBaker.navMeshNodes.findNearestNode (target);
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

		/// <summary>
		/// Handle the recieved path.
		/// </summary>
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
				path.Add (curPosition);
				// do linear optimization to the path
				Vector3 lastStartPoint = curPosition;
				Vector3 lastEndPoint = newPath[0].position;
				bool canSkip = true;
				for (int i = 1; i < newPath.Count; i++) {
					canSkip = true;
					RaycastHit2D hit = Physics2D.Linecast (lastStartPoint + (newPath [i].position - lastStartPoint).normalized * customBoundOffset, newPath [i].position);
					if (hit.transform != null) {
						if (hit.transform.gameObject == gameObject) {
							RaycastHit2D[] targetPoint = Physics2D.LinecastAll (lastStartPoint + (newPath [i].position - lastStartPoint).normalized * customBoundOffset, newPath [i].position);
							int j = 0;
							for (j = 0; j < targetPoint.Length; j++) {
								if (targetPoint [j].transform != null && targetPoint [j].transform.gameObject.GetComponent<NavMesh2DObstacle> () != null) {
									canSkip = false;
									break;
								}
							}
						} else if (hit.transform.gameObject.GetComponent<NavMesh2DObstacle> () != null) {
							canSkip = false;
						}
					}
					if (canSkip) {
						lastEndPoint = newPath [i].position;
					} else {
						path.Add (lastEndPoint);
						lastStartPoint = newPath [i].position;
					}
					// Debug.Log ("Path point " + i + " is (" + newPath [i].position.x + ", " + newPath [i].position.y + ").");
				}
				if (!path.Contains (lastEndPoint))
					path.Add (lastEndPoint);
				path.Add (target);
				if (!UpdateManager.IsRegistered (this)) {
					UpdateManager.RegisterFixedUpdate (this);
				}
			}

		}

		public void fixedUpdateEvent() {
			// Used to replace the FixedUpdate().
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
					if (customBoundOffset > 0) {
						Vector3 nearestObstaclePoint = TopDown2DNavMeshBaker.obstacleNodes.findNearestNode (transform.position).position;
						Collider2D[] colliders = new Collider2D[9];
						// center
						colliders[0] = getFirstNonSelfOverlapPoint(new Vector2(nearestObstaclePoint.x, nearestObstaclePoint.y));
						// top
						colliders[1] = getFirstNonSelfOverlapPoint (new Vector2 (nearestObstaclePoint.x, nearestObstaclePoint.y + TopDown2DNavMeshBaker.obstacleNodes.unitError));
						// bottom
						colliders[2] = getFirstNonSelfOverlapPoint (new Vector2 (nearestObstaclePoint.x, nearestObstaclePoint.y - TopDown2DNavMeshBaker.obstacleNodes.unitError));
						// left
						colliders[3] = getFirstNonSelfOverlapPoint (new Vector2 (nearestObstaclePoint.x - TopDown2DNavMeshBaker.obstacleNodes.unitError, nearestObstaclePoint.y));
						// right
						colliders[4] = getFirstNonSelfOverlapPoint (new Vector2 (nearestObstaclePoint.x + TopDown2DNavMeshBaker.obstacleNodes.unitError, nearestObstaclePoint.y));
						// topLeft
						colliders[5] = getFirstNonSelfOverlapPoint (new Vector2 (nearestObstaclePoint.x - TopDown2DNavMeshBaker.obstacleNodes.unitError, nearestObstaclePoint.y + TopDown2DNavMeshBaker.obstacleNodes.unitError));
						// topRight
						colliders[6] = getFirstNonSelfOverlapPoint (new Vector2 (nearestObstaclePoint.x + TopDown2DNavMeshBaker.obstacleNodes.unitError, nearestObstaclePoint.y + TopDown2DNavMeshBaker.obstacleNodes.unitError));
						// bottomLeft
						colliders[7] = getFirstNonSelfOverlapPoint (new Vector2 (nearestObstaclePoint.x - TopDown2DNavMeshBaker.obstacleNodes.unitError, nearestObstaclePoint.y - TopDown2DNavMeshBaker.obstacleNodes.unitError));
						// bottomRight
						colliders[8] = getFirstNonSelfOverlapPoint (new Vector2 (nearestObstaclePoint.x + TopDown2DNavMeshBaker.obstacleNodes.unitError, nearestObstaclePoint.y - TopDown2DNavMeshBaker.obstacleNodes.unitError));
						for (int i = 0; i < colliders.Length; i++) {
							if (colliders[i] != null && colliders[i].gameObject.GetComponent<NavMesh2DObstacle> () != null) {
								Vector3 closestPoint = colliders [i].bounds.ClosestPoint (transform.position);
								if (Vector3.Distance (transform.position, closestPoint) <= customBoundOffset) {
									transform.position += (transform.position - closestPoint).normalized * customBoundOffset;
									break;
								}
							}
						}
					}
				}
			} else {
				UpdateManager.UnregisterFixedUpdate (this);
			}
		}

		private Collider2D getFirstNonSelfOverlapPoint(Vector2 position) {

			Collider2D[] col = Physics2D.OverlapPointAll (position);
			for (int i = 0; i < col.Length; i++) {
				if (col [i].gameObject != gameObject) {
					return col [i];
				}
			}
			return null;

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