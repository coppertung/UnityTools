using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityTools {
	
	public class CustomDefinesOptions : EditorWindow {

		public const string customDefinesOptions = "UnityTools/Defines/Custom Defines";

		public string newDefine = "";

		[MenuItem(customDefinesOptions, false, 20)]
		public static void showWindow() {

			EditorWindow.GetWindow (typeof(CustomDefinesOptions), false, "Custom Defines");

		}

		public void deleteDefine(string definesName) {

			string[] defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup).Split (';');
			// clear plugins defines
			for (int i = 0; i < defineSymbols.Length; i++) {
				// example
				if (defineSymbols [i].Equals (definesName)) {
					defineSymbols [i] = "";
				}
			}
			StringBuilder defineSymbolsStringBuilder = new StringBuilder ();
			for (int i = 0; i < defineSymbols.Length; i++) {
				if (!string.IsNullOrEmpty(defineSymbols [i])) {
					if (defineSymbolsStringBuilder.Length > 0) {
						defineSymbolsStringBuilder.Append (";");
					}
					defineSymbolsStringBuilder.Append (defineSymbols [i]);
				}
			}
			PlayerSettings.SetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbolsStringBuilder.ToString ());

		}

		public void addDefine(string definesName) {

			if (string.IsNullOrEmpty (definesName))
				throw new NullReferenceException ("Define name can not be null!");
			string[] definesSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup).Split (';');
			bool defineExists = false;
			for (int i = 0; i < definesSymbols.Length; i++) {
				if (definesSymbols [i].Equals (definesName)) {
					defineExists = true;
					break;
				}
			}
			if (defineExists)
				throw new InvalidOperationException ("Define already exists!");
			else
				PlayerSettings.SetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup,
				PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup) + ";" + definesName);

		}

		void OnGUI() {

			GUILayout.Label ("Custom Defines Editor", EditorStyles.boldLabel);
			GUILayout.Label ("Defines List", EditorStyles.boldLabel);
			string[] definesSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup).Split (';');
			for (int i = 0; i < definesSymbols.Length; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (definesSymbols [i]);
				if (GUILayout.Button ("Delete")) {
					deleteDefine (definesSymbols [i]);
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.Label ("Add Define", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("New Define Name: ");
			newDefine = GUILayout.TextField (newDefine);
			if (GUILayout.Button ("Add")) {
				addDefine (newDefine);
			}
			GUILayout.EndHorizontal ();

		}

	}

}