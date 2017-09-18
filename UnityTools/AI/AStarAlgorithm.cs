using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AI {
	
	public delegate float heuristicFunctionEvent ();

	public interface IAStarable<T> {

		int id {
			get;
			set;
		}
		List<int> neighbours {
			get;
			set;
		}
		T value {
			get;
		}

		heuristicFunctionEvent heuristicFunction {
			get;
			set;
		}

		float cost (T target);

	}

	/// <summary>
	/// A* algorithm implemented with interface IAstarable.
	/// Referenced from https://en.wikipedia.org/wiki/A*_search_algorithm.
	/// </summary>
	public class AStarAlgorithm {
		
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

		public static IEnumerator reconstructPath<T> (List<IAStarable<T>> map, Dictionary<int, int> cameFrom, IAStarable<T> current, Action<List<IAStarable<T>>> pathHandler) {

			List<IAStarable<T>> totalPath = new List<IAStarable<T>> ();
			totalPath.Add (current);
			int currentNode = current.id;
			while (cameFrom.ContainsKey (currentNode)) {
				int tempNode;
				cameFrom.TryGetValue (currentNode, out tempNode);
				currentNode = tempNode;
				totalPath.Add (map [currentNode]);
				yield return null;
			}
			pathHandler (totalPath);

		}

	}

}