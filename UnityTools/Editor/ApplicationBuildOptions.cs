using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityTools;

namespace UnityTools.Build {

	public class ApplicationBuildOptions : EditorWindow {

		public const string buildApplicationOptions = "UnityTools/Build/Application Builder";
		public const string applicationBuildDragIdentifier = "Application Build Drag";

		public string applicationName = "Application";
		public string savePath;
		public Vector2 scenePanelScroll;
		public List<string> sceneList;

		public bool isDevelopmentBuild;
		public bool isOutputScript;

		[MenuItem(buildApplicationOptions, false, 100)]
		public static void showWindow() {

			ApplicationBuildOptions applicationBuildWindow = (ApplicationBuildOptions)EditorWindow.GetWindow (typeof(ApplicationBuildOptions), false, "Builder");
			applicationBuildWindow.init ();

		}

		void OnGUI() {

			GUILayout.Label ("Application Builder", EditorStyles.boldLabel);

			GUILayout.Label ("Basic Information", EditorStyles.boldLabel);
			applicationName = EditorGUILayout.TextField ("Application Name", applicationName);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Save Position");
			GUILayout.Label (savePath, GUILayout.MaxWidth (position.width * 0.6f));
			if (GUILayout.Button ("Choose", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				savePath = EditorUtility.SaveFolderPanel ("Choose Save Location", "", "");
			}
			EditorGUILayout.EndHorizontal ();

			GUILayout.Label ("Scenes", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("Scenes List");
			if (GUILayout.Button ("From Setting", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				foreach (string scenePath in EditorBuildSettings.scenes.Where (s => s.enabled).Select (s => s.path)) {
					addScene (scenePath);
				}
			}
			if (GUILayout.Button ("Add", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				string scenePath = EditorUtility.OpenFilePanel ("Choose Scene To Add", "", "unity");
				if (!string.IsNullOrEmpty (scenePath)) {
					addScene (scenePath);
				}
			}
			if (GUILayout.Button ("Clear", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				sceneList.Clear ();
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginVertical ("Box");
			scenePanelScroll = EditorGUILayout.BeginScrollView (scenePanelScroll, GUILayout.MaxHeight(300f), GUILayout.MinHeight(100f));
			for (int i = 0; i < sceneList.Count; i++) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label (sceneList [i]);
				if (GUILayout.Button ("Up", GUILayout.Width (45f))) {
					if (i != 0) {
						string temp = sceneList [i];
						sceneList [i] = sceneList [i - 1];
						sceneList [i - 1] = temp;
					}
				}
				if (GUILayout.Button ("Down", GUILayout.Width (45f))) {
					if (i != sceneList.Count - 1) {
						string temp = sceneList [i];
						sceneList [i] = sceneList [i + 1];
						sceneList [i + 1] = temp;
					}
				}
				if (GUILayout.Button ("Remove", GUILayout.Width (60f))) {
					sceneList.Remove (sceneList [i]);
				}
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.EndScrollView ();
			EditorGUILayout.EndVertical ();
			dragEvent (GUILayoutUtility.GetLastRect ());

			GUILayout.Label ("Settings", EditorStyles.boldLabel);
			isDevelopmentBuild = EditorGUILayout.Toggle ("Development Mode", isDevelopmentBuild, GUILayout.ExpandWidth (true));
			isOutputScript = EditorGUILayout.Toggle ("Output Script (Mobile only)", isOutputScript, GUILayout.ExpandWidth (true));

			GUILayout.Label ("Build", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Windows");
			if (GUILayout.Button ("32-bit", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				Debug.Log ("Build Windows 32-bit Application.");
				buildApplication (BuildTarget.StandaloneWindows);
			}
			if (GUILayout.Button ("64-bit", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				Debug.Log ("Build Windows 64-bit Application.");
				buildApplication (BuildTarget.StandaloneWindows64);
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Mac");
			if (GUILayout.Button ("Intel 32-bit", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				Debug.Log ("Build MacOS Intel 32-bit Application.");
				buildApplication (BuildTarget.StandaloneOSXIntel);
			}
			if (GUILayout.Button ("Intel 64-bit", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				Debug.Log ("Build MacOS Intel 64-bit Application.");
				buildApplication (BuildTarget.StandaloneOSXIntel64);
			}
			if (GUILayout.Button ("Universal", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				Debug.Log ("Build MacOS Universal Application.");
				buildApplication (BuildTarget.StandaloneOSXUniversal);
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Mobile");
			if (GUILayout.Button ("Android", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				Debug.Log ("Build Android Application.");
				buildApplication (BuildTarget.Android);
			}
			if (GUILayout.Button ("iOS", GUILayout.MaxWidth (100f), GUILayout.MinWidth (60f))) {
				Debug.Log ("Build iOS Application.");
				buildApplication (BuildTarget.iOS);
			}
			EditorGUILayout.EndHorizontal ();

		}

		public void init() {

			if (sceneList == null) {
				sceneList = new List<string> ();
			}

		}

		public void addScene(string path) {

			string newPath = path.Substring (path.IndexOf ("Assets"));
			if (!sceneList.Contains (newPath)) {
				sceneList.Add (newPath);
			}

		}

		public void dragEvent(Rect dropArea) {

			Event currentEvent = Event.current;
			EventType currentEventType = currentEvent.type;
			List<string> scenes;

			if (currentEventType == EventType.DragExited) {
				DragAndDrop.PrepareStartDrag ();
			}
			if (!dropArea.Contains (currentEvent.mousePosition)) {
				return;
			}

			switch (currentEventType) {
			case EventType.MouseDown:
				DragAndDrop.PrepareStartDrag ();
				DragAndDrop.SetGenericData (applicationBuildDragIdentifier, sceneList);
				currentEvent.Use ();
				break;
			case EventType.MouseDrag:
				scenes = DragAndDrop.GetGenericData (applicationBuildDragIdentifier) as List<string>;
				if (scenes != null) {
					DragAndDrop.StartDrag ("Start dragging");
					currentEvent.Use ();
				}
				break;
			case EventType.DragUpdated:
				try {
					for (int i = 0; i < DragAndDrop.objectReferences.Length; i++) {
						SceneAsset temp = (SceneAsset)DragAndDrop.objectReferences [i];
						if (temp != null) {
							DragAndDrop.visualMode = DragAndDropVisualMode.Link;
						} else {
							DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
						}
					}
				}
				catch(Exception ex) {
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
				break;
			case EventType.DragPerform:
				DragAndDrop.AcceptDrag ();
				for (int i = 0; i < DragAndDrop.objectReferences.Length; i++) {
					SceneAsset temp = (SceneAsset)DragAndDrop.objectReferences [i];
					if (temp != null) {
						addScene (DragAndDrop.paths [i]);
					}
				}
				currentEvent.Use ();
				break;
			case EventType.mouseUp:
				DragAndDrop.PrepareStartDrag ();
				break;
			}

		}

		public void buildApplication(BuildTarget targetPlatform) {


			BuildOptions options = BuildOptions.ShowBuiltPlayer;
			if (isDevelopmentBuild) {
				options |= BuildOptions.Development;
			}
			if (isOutputScript && (targetPlatform == BuildTarget.Android || targetPlatform == BuildTarget.iOS)) {
				options |= BuildOptions.AcceptExternalModificationsToPlayer;
			}

			// NOT SURE START FROM THIS PART
			// TODO: Test and Improve and codes below
			string outputname = savePath + "/" + Utils.buildPlatform + "/" + applicationName;
			switch (targetPlatform) {
			case BuildTarget.Android:
				outputname += ".apk";
				break;
			case BuildTarget.iOS:
				// outputname += ".xcodeproj";
				break;
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				outputname += ".exe";
				break;
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				outputname += ".app";
				break;
			}

			BuildTarget currentPlatform = EditorUserBuildSettings.activeBuildTarget;
			if (currentPlatform != targetPlatform) {
				EditorUserBuildSettings.SwitchActiveBuildTarget (targetPlatform);
			}

			BuildPipeline.BuildPlayer (
				sceneList.ToArray (),
				outputname,
				targetPlatform,
				options
			);

			if (EditorUserBuildSettings.activeBuildTarget != currentPlatform) {
				EditorUserBuildSettings.SwitchActiveBuildTarget (currentPlatform);
			}

		}

	}

}