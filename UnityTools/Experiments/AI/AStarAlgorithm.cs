using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AI {

	#region Delegate_Functions
	/// <summary>
	/// Definition of the delegate heuristic function.
	/// </summary>
	public delegate float heuristicFunctionEvent ();
	#endregion

	#region Interfaces
	/// <summary>
	/// Interface for the nodes that will use A* algorithm, define T as the type of the value variable.
	/// </summary>
	public interface IAStarable<T> {

		/// <summary>
		/// Identifier.
		/// </summary>
		int id {
			get;
			set;
		}
		/// <summary>
		/// List of neighbours.
		/// </summary>
		List<int> neighbours {
			get;
			set;
		}
		/// <summary>
		/// Some useful value.
		/// </summary>
		T value {
			get;
		}

		/// <summary>
		/// The heuristic function.
		/// </summary>
		heuristicFunctionEvent heuristicFunction {
			get;
			set;
		}

		/// <summary>
		/// Cost of doing something with the specified target.
		/// </summary>
		float cost (T target);

	}
	#endregion

	/// <summary>
	/// A* algorithm implemented with interface IAstarable.
	/// Referenced from https://en.wikipedia.org/wiki/A*_search_algorithm.
	/// </summary>
	public class AStarAlgorithm {

		#region Functions
		/// <summary>
		/// A Coroutine function that can be used to find the best path according the heuristic function of the nodes.
		/// Noted that T is the type of the value variable of the node.
		/// </summary>
		public static IEnumerator findPath<T>(List<IAStarable<T>> map, IAStarable<T> start, IAStarable<T> goal, Action<List<IAStarable<T>>> pathHandler) {

			// set of nodes already evaluated
			HashSet<IAStarable<T>> closeSet = new HashSet<IAStarable<T>> ();
			// set of nodes currently discovered and not yet evaluated
			HashSet<IAStarable<T>> openSet = new HashSet<IAStarable<T>>();
			// Initially, only the start node is known
			openSet.Add (start);

			// empty list to store the nodes that can be most effeciently reached from
			Dictionary<int, int> cameFrom = new Dictionary<int, int> ();

			// cost of getting from start node to that node
			float[] gScore = new float[map.Count];
			for (int i = 0; i < gScore.Length; i++) {
				gScore [i] = float.MaxValue;
			}
			// cost of start node to start node is 0
			gScore [start.id] = 0;

			// total cost of getting from start node to goal node
			float[] fScore = new float[map.Count];
			for (int i = 0; i < fScore.Length; i++) {
				fScore [i] = float.MaxValue;
			}
			fScore [start.id] = start.heuristicFunction.Invoke ();

			while (openSet.Count > 0) {
				IAStarable<T> current;
				float minFScore = float.MaxValue;
				int minNodeID = int.MaxValue;
				foreach (IAStarable<T> node in openSet) {
					if (fScore [node.id] < minFScore) {
						minFScore = fScore [node.id];
						minNodeID = node.id;
					}
				}
				current = map.Find (x => x.id == minNodeID);

				if (current == goal) {
					yield return reconstructPath<T> (map, cameFrom, current, pathHandler);
					break;
				}
				openSet.Remove (current);
				closeSet.Add (current);

				for (int i = 0; i < current.neighbours.Count; i++) {
					if (closeSet.Contains (map [current.neighbours [i]])) {
						continue;
					}
					if (!openSet.Contains (map [current.neighbours [i]])) {
						openSet.Add (map [current.neighbours [i]]);
					}
					// Score/Distance between current node and neighbour
					float tentativeGScore = gScore[current.id] + current.cost(map[current.neighbours[i]].value);
					if(tentativeGScore >= gScore[current.neighbours[i]]) {
						continue;
					}
					// record the best path until now
					cameFrom[current.neighbours[i]] = current.id;
					gScore [current.neighbours [i]] = tentativeGScore;
					fScore [current.neighbours [i]] = gScore [current.neighbours [i]] + map [current.neighbours [i]].heuristicFunction.Invoke ();
				}
				yield return null;
			}

			// fail to find a path
			pathHandler(null);

		}

		private static IEnumerator reconstructPath<T> (List<IAStarable<T>> map, Dictionary<int, int> cameFrom, IAStarable<T> current, Action<List<IAStarable<T>>> pathHandler) {

			List<IAStarable<T>> totalPath = new List<IAStarable<T>> ();
			totalPath.Add (current);
			int currentNode = current.id;
			while (cameFrom.ContainsKey (currentNode)) {
				int tempNode;
				cameFrom.TryGetValue (currentNode, out tempNode);
				currentNode = tempNode;
				totalPath.Insert (0, map [currentNode]);
				yield return null;
			}
			pathHandler (totalPath);

		}
		#endregion

	}

}