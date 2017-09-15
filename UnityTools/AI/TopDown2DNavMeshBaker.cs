using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTools.AI {

    [ExecuteInEditMode]
    public class TopDown2DNavMeshBaker : MonoBehaviour {
		
		public float unitError;
        public float unitLength;

        public List<NavMesh2DNode> navMeshNodes;
        public List<NavMesh2DNode> obstacleNodes;

        public void bake() {

            // initialization
            if (navMeshNodes == null)
                navMeshNodes = new List<NavMesh2DNode>();
            else
                clear();
            if (obstacleNodes == null)
                obstacleNodes = new List<NavMesh2DNode>();
            else
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
                    int maxCol = (int)(gameObjectSize.x / (unitLength / 2));
                    // Debug.Log("row = " + row + ", col = " + maxCol);

                    Vector3 startPoint = Vector3.zero;

                    for (int i = 0; i <= row; i++)
                    {
                        startPoint.x = gameObjectCenter.x - gameObjectSize.x / 2 + ((i % 2 == 0) ? 0 : unitLength / 2);
                        startPoint.y = gameObjectCenter.y + gameObjectSize.y / 2;
                        for (int j = 0; j <= maxCol; j++)
                        {
                            if (j % 2 == 0) {
                                NavMesh2DNode newNode = new NavMesh2DNode();
								newNode.id = count;
                                newNode.position = new Vector3(startPoint.x + j * unitLength / 2, startPoint.y - i * unitLength / 2, 0);
								newNode.neighbours = new List<NavMesh2DNode> ();
                                NavMesh2DObstacle[] obstacles = (NavMesh2DObstacle[])FindObjectsOfType(typeof(NavMesh2DObstacle));
                                // check if there is obstacles exist at the node
                                for(int k = 0; k < obstacles.Length; k++) {
                                    Vector3 obstacleSize = obstacles[k].GetComponent<SpriteRenderer>().bounds.size;
                                    obstacleSize.z = 0;
                                    if ((Mathf.Abs(obstacles[k].transform.position.x - newNode.position.x) <= obstacleSize.x / 2 + unitError)
                                        && (Mathf.Abs(obstacles[k].transform.position.y - newNode.position.y) <= obstacleSize.y / 2 + unitError)
                                        || newNode.position.x < gameObjectCenter.x - gameObjectSize.x / 2 || newNode.position.x > gameObjectCenter.x + gameObjectSize.x / 2
                                        || newNode.position.y < gameObjectCenter.y - gameObjectSize.y / 2 || newNode.position.y > gameObjectCenter.y + gameObjectSize.y / 2) {
                                        // there is obstacle or out of bound
                                        // Debug.Log("Found obstacle: " + newNode.position.x + ", " + newNode.position.y);
                                        obstacleNodes.Add(newNode);
                                    }
                                }
								if (!navMeshNodes.Contains(newNode) && !obstacleNodes.Contains(newNode)) {
                                    // Debug.Log("Add new node: " + newNode.position.x + ", " + newNode.position.y);
                                    navMeshNodes.Add(newNode);
									count += 1;
									// find neighbours
									for (int k = 0; k < navMeshNodes.Count - 1; k++) {
										if (navMeshNodes [k].neighbours.Count < 8 && Vector3.Distance (navMeshNodes [k].position, navMeshNodes [navMeshNodes.Count - 1].position) <= unitLength) {
											navMeshNodes [k].neighbours.Add (navMeshNodes [navMeshNodes.Count - 1]);
											navMeshNodes [navMeshNodes.Count - 1].neighbours.Add (navMeshNodes [k]);
										}
									}
                                }
                            }
                        }
                    }
                }
            }
            Debug.Log("Finish Baking.");

        }

        public void clear() {

            navMeshNodes.Clear();
            obstacleNodes.Clear();

        }

        private void OnDrawGizmosSelected()
        {

            // draw all nav mesh nodes on scene
            if (navMeshNodes != null && navMeshNodes.Count > 0)
            {
                for (int i = 0; i < navMeshNodes.Count; i++)
                {
                    Gizmos.color = Color.gray;
					Gizmos.DrawSphere(new Vector3(navMeshNodes[i].position.x, navMeshNodes[i].position.y, 0), 0.1f);
					Gizmos.color = Color.green;
					for (int j = 0; j < navMeshNodes [i].neighbours.Count; j++) {
						if (navMeshNodes [i].neighbours [j].id > navMeshNodes [i].id) {
							Gizmos.DrawLine (navMeshNodes [i].position, navMeshNodes [i].neighbours [j].position);
						}
					}
                }
                // Debug.Log("Drawed " + navMeshNodes.Count + " Nodes.");
            }
            // draw all obstacle nodes on scene
            if (obstacleNodes != null && obstacleNodes.Count > 0)
            {
                for (int i = 0; i < obstacleNodes.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(new Vector3(obstacleNodes[i].position.x, obstacleNodes[i].position.y, 0), 0.1f);
                }
                // Debug.Log("Drawed " + obstacleNodes.Count + " Nodes.");
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
            GUILayout.BeginHorizontal();
            GUILayout.Label("Unit Length: ");
			script.unitLength = EditorGUILayout.FloatField (script.unitLength);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Unit Error (suggested that not more than 1/4 unit length): ");
			script.unitError = EditorGUILayout.FloatField (script.unitError);
			GUILayout.EndHorizontal();
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
        /*
        void OnSceneGUI() {

            if(script.navMeshNodes != null && script.navMeshNodes.Count > 0) {
                for(int i = 0; i < script.navMeshNodes.Count; i++) {
                    Handles.color = new Color(0.5f, 0.5f, 0.5f);
                    Handles.DotHandleCap(
                        0,
                        script.transform.position + new Vector3(script.navMeshNodes[i].position.x, script.navMeshNodes[i].position.y, 0),
                        script.transform.rotation,
                        1f,
                        EventType.Repaint
                        );
                }
                Debug.Log("Drawed " + script.navMeshNodes.Count + "Nodes.");
            }
            if (GUI.changed)
                EditorUtility.SetDirty(script);

        }
        */
    }

}