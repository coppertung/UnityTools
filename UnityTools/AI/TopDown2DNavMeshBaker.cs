using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTools.AI {

    [ExecuteInEditMode]
    public class TopDown2DNavMeshBaker : MonoBehaviour {

		// saved file name
		public const string NAVMESH_NODES = "NavMeshNodes";
		public const string OBSTACLE_NODES = "ObstacleNodes";
		// basic setting
		public float unitError;
        public float unitLength;
		// display setting
		public bool showNode;

        public static NavMesh2DNodeList navMeshNodes;
		public static NavMesh2DNodeList obstacleNodes;

		// GOT PROBLEMS
		public void init() {

			if (navMeshNodes == null) {
				navMeshNodes = NavMesh2DNodeList.read (NAVMESH_NODES);
				if (navMeshNodes == null) {
					navMeshNodes = new NavMesh2DNodeList ();
					navMeshNodes.nodes = new List<NavMesh2DNode> ();
				}
			}
			if (obstacleNodes == null) {
				obstacleNodes =NavMesh2DNodeList.read (NAVMESH_NODES);
				if (obstacleNodes == null) {
					obstacleNodes = new NavMesh2DNodeList ();
					obstacleNodes.nodes = new List<NavMesh2DNode> ();
				}
			}

		}

        public void bake() {

            // initialization
			clear();

            bakeInTriangle();

        }

        private void bakeInTriangle() {

            // generate nav mesh nodes
			GameObject[] gameObjects = (GameObject[])FindObjectsOfType(typeof(GameObject));
			int count = 0;

            for (int n = 0; n < gameObjects.Length; n++) {
                if (gameObjects[n].isStatic && gameObjects[n].GetComponent<NavMesh2DObstacle>() == null)
                {
                    Vector3 gameObjectSize = gameObjects[n].GetComponent<SpriteRenderer>().bounds.size;
                    Vector3 gameObjectCenter = gameObjects[n].transform.position;

                    int row = (int)(gameObjectSize.y / (unitLength / 2));
                    int maxCol = (int)(gameObjectSize.x / unitLength);
                    // Debug.Log("row = " + row + ", col = " + maxCol);

                    Vector3 startPoint = Vector3.zero;

                    for (int i = 0; i <= row; i++)
                    {
                        startPoint.x = gameObjectCenter.x - gameObjectSize.x / 2 + ((i % 2 == 0) ? 0 : unitLength / 2);
                        startPoint.y = gameObjectCenter.y + gameObjectSize.y / 2;
                        for (int j = 0; j <= maxCol; j++)
                        {
                            NavMesh2DNode newNode = new NavMesh2DNode();
							newNode.id = count;
                            newNode.position = new Vector3(startPoint.x + j * unitLength, startPoint.y - i * unitLength / 2, 0);
							newNode.neighbours = new List<int> ();
                            // check if there is obstacles exist at the node
							if(Physics2D.OverlapPoint(new Vector2(newNode.position.x, newNode.position.y)) || Physics2D.OverlapPoint(new Vector2(newNode.position.x + unitError, newNode.position.y + unitError))
								|| Physics2D.OverlapPoint(new Vector2(newNode.position.x + unitError, newNode.position.y)) || Physics2D.OverlapPoint(new Vector2(newNode.position.x - unitError, newNode.position.y))
								|| Physics2D.OverlapPoint(new Vector2(newNode.position.x, newNode.position.y + unitError)) || Physics2D.OverlapPoint(new Vector2(newNode.position.x, newNode.position.y - unitError))
								|| Physics2D.OverlapPoint(new Vector2(newNode.position.x - unitError, newNode.position.y + unitError)) || Physics2D.OverlapPoint(new Vector2(newNode.position.x + unitError, newNode.position.y - unitError))
								|| newNode.position.x < gameObjectCenter.x - gameObjectSize.x / 2 || newNode.position.x > gameObjectCenter.x + gameObjectSize.x / 2
								|| newNode.position.y < gameObjectCenter.y - gameObjectSize.y / 2 || newNode.position.y > gameObjectCenter.y + gameObjectSize.y / 2) {
								// there is obstacle or out of bound
								// Debug.Log("Found obstacle: " + newNode.position.x + ", " + newNode.position.y);
								obstacleNodes.nodes.Add(newNode);
							}
							if (!navMeshNodes.nodes.Contains(newNode) && !obstacleNodes.nodes.Contains(newNode)) {
                                // Debug.Log("Add new node: " + newNode.position.x + ", " + newNode.position.y);
								navMeshNodes.nodes.Add(newNode);
								count += 1;
								// find neighbours
								for (int k = 0; k < navMeshNodes.nodes.Count - 1; k++) {
									float distance = Vector3.Distance (navMeshNodes.nodes [k].position, navMeshNodes.nodes [navMeshNodes.nodes.Count - 1].position);
									if (navMeshNodes.nodes [k].neighbours.Count < 8 && distance <= unitLength) {
										Vector2 origin = new Vector2 (navMeshNodes.nodes [k].position.x, navMeshNodes.nodes [k].position.y);
										Vector2 dest = new Vector2 (navMeshNodes.nodes [navMeshNodes.nodes.Count - 1].position.x, navMeshNodes.nodes [navMeshNodes.nodes.Count - 1].position.y);
										RaycastHit2D hit = Physics2D.Linecast (origin, dest);
										if (hit.transform == null) {
											navMeshNodes.nodes [k].neighbours.Add (navMeshNodes.nodes [navMeshNodes.nodes.Count - 1].id);
											navMeshNodes.nodes [navMeshNodes.nodes.Count - 1].neighbours.Add (navMeshNodes.nodes [k].id);
										}
									}
								}
                            }
                        }
                    }
                }
            }
			Debug.Log ("Finish Baking. [NavMesh Points: " + navMeshNodes.nodes.Count + " | Obstacle Points: " + obstacleNodes.nodes.Count + "]");
			navMeshNodes.save (NAVMESH_NODES);
			obstacleNodes.save (OBSTACLE_NODES);

        }

        public void clear() {

			if (navMeshNodes != null && navMeshNodes.nodes != null)
				navMeshNodes.nodes.Clear ();
			else {
				navMeshNodes = new NavMesh2DNodeList ();
				navMeshNodes.nodes = new List<NavMesh2DNode> ();
			}
			if (obstacleNodes != null && obstacleNodes.nodes != null)
				obstacleNodes.nodes.Clear ();
			else {
				obstacleNodes = new NavMesh2DNodeList ();
				obstacleNodes.nodes = new List<NavMesh2DNode> ();
			}
			if (File.Exists (Application.persistentDataPath + "/" + NAVMESH_NODES)) {
				File.Delete (Application.persistentDataPath + "/" + NAVMESH_NODES);
			}
			if (File.Exists (Application.persistentDataPath + "/" + OBSTACLE_NODES)) {
				File.Delete (Application.persistentDataPath + "/" + OBSTACLE_NODES);
			}

        }

        void OnDrawGizmosSelected()
        {

			//initialization
			if (navMeshNodes == null || obstacleNodes == null) {
				init ();
				Debug.Log (navMeshNodes);
				Debug.Log (obstacleNodes);
			}
            // draw all nav mesh nodes on scene
			if (navMeshNodes != null && navMeshNodes.nodes.Count > 0)
            {
				for (int i = 0; i < navMeshNodes.nodes.Count; i++)
                {
					if (showNode) {
						Gizmos.color = Color.gray;
						Gizmos.DrawSphere (new Vector3 (navMeshNodes.nodes [i].position.x, navMeshNodes.nodes [i].position.y, 0), 0.1f);
					}
					Gizmos.color = Color.green;
					for (int j = 0; j < navMeshNodes.nodes [i].neighbours.Count; j++) {
						if (navMeshNodes.nodes [i].neighbours [j] > navMeshNodes.nodes [i].id) {
							Gizmos.DrawLine (navMeshNodes.nodes [i].position, navMeshNodes.nodes [navMeshNodes.nodes [i].neighbours [j]].position);
						}
					}
                }
                // Debug.Log("Drawed " + navMeshNodes.Count + " Nodes.");
            }
			if (showNode) {
				// draw all obstacle nodes on scene
				if (obstacleNodes != null && obstacleNodes.nodes.Count > 0) {
					for (int i = 0; i < obstacleNodes.nodes.Count; i++) {
						Gizmos.color = Color.red;
						Gizmos.DrawSphere (new Vector3 (obstacleNodes.nodes [i].position.x, obstacleNodes.nodes [i].position.y, 0), 0.1f);
					}
					// Debug.Log("Drawed " + obstacleNodes.Count + " Nodes.");
				}
			}

        }

    }

    [CustomEditor(typeof(TopDown2DNavMeshBaker))]
    public class TopDown2DNavMeshBakerEditor : Editor {

        TopDown2DNavMeshBaker script;

        void OnEnable() {

            if (script == null)
                script = (TopDown2DNavMeshBaker)target;

        }

        void OnDisable() {

        }

        public override void OnInspectorGUI() {
            
			// variables
			GUILayout.Label("Settings", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Unit Length: ");
			script.unitLength = EditorGUILayout.FloatField (script.unitLength);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Unit Error (suggested that not more than 1/4 unit length): ");
			script.unitError = EditorGUILayout.FloatField (script.unitError);
			GUILayout.EndHorizontal();
			GUILayout.Label("Displays", EditorStyles.boldLabel);
			script.showNode = GUILayout.Toggle (script.showNode, "Show Nodes");
            // Buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Bake")) {
                script.bake();
            }
            if (GUILayout.Button("Clear")) {
                script.clear();
            }
            GUILayout.EndHorizontal();
            if (GUI.changed)
                EditorUtility.SetDirty(script);

        }
    }

}