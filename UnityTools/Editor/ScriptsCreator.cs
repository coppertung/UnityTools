using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityTools {

	public class ScriptsCreator : EditorWindow {
		
		public const string scriptCreatorOptions = "UnityTools/Scripts/Scripts Creator";

		public string className = "CustomClass";

		public bool inheritDefault = false;
		public bool inheritMonoBehaviour = true;
		public bool inheritIUpdateable = false;
		public bool inheritIPoolObject = false;

		[MenuItem(scriptCreatorOptions)]
		public static void showWindow() {

			EditorWindow.GetWindow (typeof(ScriptsCreator));

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
				writer.WriteLine ();
				writer.Write ("public class " + className);
				if (inheritDefault) {
					if (inheritMonoBehaviour) {
						writer.Write (" : MonoBehaviour");
						if (inheritIPoolObject) {
							writer.Write (", IPoolObject");
						}
						if (inheritIUpdateable) {
							writer.Write (", IUpdateable");
						}
					} else if (inheritIPoolObject) {
						writer.Write (" : IPoolObject");
						if (inheritIUpdateable) {
							writer.Write (", IUpdateable");
						}
					} else if (inheritIUpdateable) {
						writer.Write (" : IUpdateable");
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
					if (inheritIUpdateable) {
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
						writer.WriteLine ("\tpublic void onEnable() {");
						writer.WriteLine ("\t\t// Enable call.");
						writer.WriteLine ("\t\t// NOT being called automatically.");
						writer.WriteLine ("\t}");
						writer.WriteLine ();
						writer.WriteLine ("\tpublic void onDisable() {");
						writer.WriteLine ("\t\t// Disable call.");
						writer.WriteLine ("\t\t// NOT being called automatically.");
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
			inheritMonoBehaviour = EditorGUILayout.Toggle ("MonoBehaviour", inheritMonoBehaviour);
			inheritIUpdateable = EditorGUILayout.Toggle ("IUpdateable", inheritIUpdateable);
			inheritIPoolObject = EditorGUILayout.Toggle ("IPoolObject", inheritIPoolObject);
			EditorGUILayout.EndToggleGroup ();

			if (GUILayout.Button ("Generate Script")) {
				createScript ();
			}

		}

	}

}