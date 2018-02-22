using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityTools {

	public class ScriptsCreatorOptions : EditorWindow {
		
		public const string scriptCreatorOptions = "UnityTools/Scripts/Scripts Creator";

		public string className = "CustomClass";

		public bool inheritDefault = false;
		public bool inheritMonoBehaviour = true;
		public bool inheritSingleton = false;
        public bool inheritIUpdateable = false;
        public bool inheritIFixedUpdateable = false;
        public bool inheritILateUpdateable = false;
        public bool inheritIPoolObject = false;

		[MenuItem(scriptCreatorOptions, false, 40)]
		public static void showWindow() {

			EditorWindow.GetWindow (typeof(ScriptsCreatorOptions), false, "Script Creator");

		}

		public void createScript() {

			// remove space and transform minus to underscore
			className.Replace (" ", "");
			className.Replace ("-", "_");

			string filepath = Application.dataPath + "/" + className + ".cs";
			// prevent from overriding
			if (!File.Exists (filepath)) {
				StreamWriter writer = new StreamWriter (filepath);
				writer.WriteLine ("using System.Collections;");
				writer.WriteLine ("using System.Collections.Generic;");
				writer.WriteLine ("using UnityEngine;");
				if (inheritDefault && (inheritIPoolObject || inheritIUpdateable)) {
					writer.WriteLine ("using UnityTools;");
				}
				if (inheritDefault && inheritMonoBehaviour && inheritSingleton) {
					writer.WriteLine ("using UnityTools.Patterns;");
				}
				writer.WriteLine ();
				writer.Write ("public class " + className);
				if (inheritDefault) {
					if (inheritMonoBehaviour) {
						if (inheritSingleton) {
							writer.Write (" : Singleton<" + className + ">");
						} else {
							writer.Write (" : MonoBehaviour");
						}
						if (inheritIPoolObject) {
							writer.Write (", IPoolObject");
						}
						if (inheritIUpdateable) {
							writer.Write (", IUpdateable");
                        }
                        if (inheritIFixedUpdateable) {
                            writer.Write(", IFixedUpdateable");
                        }
                        if (inheritILateUpdateable) {
                            writer.Write(", ILateUpdateable");
                        }
                    } else if (inheritIPoolObject) {
						writer.Write (" : IPoolObject");
						if (inheritIUpdateable) {
							writer.Write (", IUpdateable");
                        }
                        if (inheritIFixedUpdateable) {
                            writer.Write(", IFixedUpdateable");
                        }
                        if (inheritILateUpdateable) {
                            writer.Write(", ILateUpdateable");
                        }
                    } else if (inheritIUpdateable) {
						writer.Write (" : IUpdateable");
                        if (inheritIFixedUpdateable) {
                            writer.Write(", IFixedUpdateable");
                        }
                        if (inheritILateUpdateable) {
                            writer.Write(", ILateUpdateable");
                        }
                    } else if (inheritIFixedUpdateable) {
                        writer.Write(" : IFixedUpdateable");
                        if (inheritILateUpdateable) {
                            writer.Write(", ILateUpdateable");
                        }
                    }
                    else if (inheritILateUpdateable) {
                        writer.Write(" : ILateUpdateable");
                    }
                }
				writer.WriteLine (" {");
				writer.WriteLine ();
				if (inheritDefault) {
					// parameters
					if (inheritIPoolObject) {
						writer.WriteLine ("\t// The identifier of the pool object.");
						writer.WriteLine ("\tpublic int id {");
						writer.WriteLine ("\t\tget;");
						writer.WriteLine ("\t\tset;");
						writer.WriteLine ("\t}");
						writer.WriteLine ();
						writer.WriteLine ("\t// Is the pool object active in the scene?");
						writer.WriteLine ("\tpublic bool isActive {");
						writer.WriteLine ("\t\tget;");
						writer.WriteLine ("\t\tset;");
						writer.WriteLine ("\t}");
						writer.WriteLine ();
					}
					if (inheritIUpdateable || inheritIFixedUpdateable || inheritILateUpdateable) {
						writer.WriteLine ("\t// The update call will be called in prior if the priority is larger.");
						writer.WriteLine ("\tpublic int priority {");
						writer.WriteLine ("\t\tget;");
						writer.WriteLine ("\t\tset;");
						writer.WriteLine ("\t}");
						writer.WriteLine ();
					}
					// Methods
					if (inheritIPoolObject) {
						writer.WriteLine ("\tpublic void init (Vector3 position, Quaternion rotation) {");
						writer.WriteLine ("\t\t// Initialize the existed instance.");
						writer.WriteLine ("\t}");
						writer.WriteLine ();
						writer.WriteLine ("\tpublic void init (int id, Vector3 position, Quaternion rotation) {");
						writer.WriteLine ("\t\t// Initialize the new instance.");
						writer.WriteLine ("\t}");
						writer.WriteLine ();
					}
					if (inheritIUpdateable) {
						writer.WriteLine ("\tpublic void updateEvent() {");
						writer.WriteLine ("\t\t// Used to replace the Update().");
						writer.WriteLine ("\t\t// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.");
						writer.WriteLine ("\t}");
						writer.WriteLine ();
                    }
                    if (inheritIFixedUpdateable)
                    {
                        writer.WriteLine("\tpublic void fixedUpdateEvent() {");
                        writer.WriteLine("\t\t// Used to replace the FixedUpdate().");
                        writer.WriteLine("\t\t// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.");
                        writer.WriteLine("\t}");
                        writer.WriteLine();
                    }
                    if (inheritILateUpdateable)
                    {
                        writer.WriteLine("\tpublic void lateUpdateEvent() {");
                        writer.WriteLine("\t\t// Used to replace the LateUpdate().");
                        writer.WriteLine("\t\t// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.");
                        writer.WriteLine("\t}");
                        writer.WriteLine();
                    }
                }
				writer.WriteLine ("}");
				writer.Close ();
			} else {
				throw new Exception ("File already existed!");
			}
			AssetDatabase.Refresh ();

		}

		void OnGUI() {

			GUILayout.Label ("Setting", EditorStyles.boldLabel);
			className = EditorGUILayout.TextField ("Class Name", className);

			inheritDefault = EditorGUILayout.BeginToggleGroup ("Inherit default classes", inheritDefault);
			inheritMonoBehaviour = EditorGUILayout.BeginToggleGroup ("InheritMonoBehaviour", inheritMonoBehaviour);
			inheritSingleton = EditorGUILayout.Toggle ("Singleton", inheritSingleton);
			EditorGUILayout.EndToggleGroup ();
			inheritIUpdateable = EditorGUILayout.Toggle ("IUpdateable", inheritIUpdateable);
            inheritIFixedUpdateable = EditorGUILayout.Toggle("IFixedUpdateable", inheritIFixedUpdateable);
            inheritILateUpdateable = EditorGUILayout.Toggle("ILateUpdateable", inheritILateUpdateable);
            inheritIPoolObject = EditorGUILayout.Toggle ("IPoolObject", inheritIPoolObject);
			EditorGUILayout.EndToggleGroup ();

			if (GUILayout.Button ("Generate Script")) {
				createScript ();
			}

		}

	}

}