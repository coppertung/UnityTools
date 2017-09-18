using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools;

namespace UnityTools.AI {

	public class TopDown2DNavMeshAgent : MonoBehaviour, IUpdateable {

		// The update call will be called in prior if the priority is larger.
		public int priority {
			get;
			set;
		}
			
		public Vector3 target;
		public List<Vector3> path;

		public void setTarget(Vector3 target) {

			this.target = target;
			// find the path
			float minStartDistance = float.MaxValue;
			NavMesh2DNode closestStart = null;
			float minGoalDistance = float.MaxValue;
			NavMesh2DNode closestGoal = null;
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
			}
			StartCoroutine (AStarAlgorithm.findPath<Vector3> (map, (IAStarable<Vector3>)closestStart, (IAStarable<Vector3>)closestGoal, pathHandler));

		}

		public void pathHandler(List<IAStarable<Vector3>> result) {

			List<NavMesh2DNode> path = new List<NavMesh2DNode> ();
			// add true start position
			// do optimization if it is need
			for (int i = 0; i < result.Count; i++) {
				NavMesh2DNode newNode = (NavMesh2DNode)result [i];
				newNode.heuristicFunction = null;
				path.Add (newNode);
			}
			// add true goal position
			for (int i = 0; i < path.Count; i++) {
				this.path.Add (path [i].position);
			}

		}

		public void updateEvent() {
			// Used to replace the Update().
			// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
		}

	}

}