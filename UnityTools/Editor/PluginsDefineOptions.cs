using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityTools {

	public class PluginsDefineOptions : MonoBehaviour {

		public const string addPluginsDefineOptions = "UnityTools/Plugins/Add Plugins Define";
		public const string clearPluginsDefineOptions = "UnityTools/Plugins/Clear Plugins Define";

		private static string pluginsPath = Application.dataPath + "/Plugins/";
		private static string[] includedFileExtension = { ".js", ".cs", ".dll", ".jslib" };
		private static string[] excludedDirectory = { "UnityTools" };

		// taking websockersharp as an example
		private static bool websocketsharpExists = false;
		private static string websocketsharpDefine = "UNITYTOOLS_FOUND_WEBSOCKETSHARP";

		[MenuItem(addPluginsDefineOptions, false, 20)]
		public static void checkPluginsExists() {

			// example
			#if UNITYTOOLS_FOUND_WEBSOCKETSHARP
			Debug.Log("websocket found!");
			#endif
			scanAllFiles (pluginsPath);
			addPluginsDefine ();
			Debug.Log ("Plugins Defines were being added.");

		}

		[MenuItem(clearPluginsDefineOptions, false, 21)]
		public static void clearPlayerDefine() {

			string[] defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup).Split (';');
			// clear plugins defines
			for (int i = 0; i < defineSymbols.Length; i++) {
				// example
				if (defineSymbols [i].Equals (websocketsharpDefine)) {
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
			Debug.Log ("Plugins Defines were being cleared.");

		}

		private static void scanAllFiles(string path) {

			string[] directorylist = Directory.GetDirectories (path);
			string[] filelist = Directory.GetFiles (path);
			bool excluded = false;
			for (int i = 0; i < directorylist.Length; i++) {
				excluded = false;
				for (int j = 0; j < excludedDirectory.Length; j++) {
					if (directorylist [i].Contains (excludedDirectory [j])) {
						excluded = true;
					}
				}
				if (!excluded) {
					scanAllFiles (directorylist [i]);
				}
			}
			for (int i = 0; i < filelist.Length; i++) {
				excluded = true;
				for (int j = 0; j < includedFileExtension.Length; j++) {
					if (Path.GetExtension (filelist [i]).Equals (includedFileExtension [j])) {
						excluded = false;
						break;
					}
				}
				if (!excluded) {
					pluginIdentifier (filelist [i]);
				}
			}
		}

		private static void pluginIdentifier(string file) {

			// Handle the plugins here
			// example
			if (file.Contains ("websocket-sharp.dll")) {
				websocketsharpExists = true;
			} else {
				Debug.Log (file);
			}

		}

		private static void addPluginsDefine() {
			
			// example
			if (websocketsharpExists && !PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup).Contains (websocketsharpDefine))
				PlayerSettings.SetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup,
					PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup) + ";" + websocketsharpDefine);

		}

	}

}