using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityTools.Data.DataType;
using UnityTools.Data.Node;

namespace UnityTools.Data {
	
	/// <summary>
	/// User Interface Panel of Data Simulator.
	/// Functions here should be used as display and accept events only.
	/// Reference from:
	/// http://gram.gs/gramlog/creating-node-based-editor-unity/
	/// </summary>
	public class DataSimulatorOptions : EditorWindow {

		public const string dataSimulatorOptions = "UnityTools/Data/Data Simulator";

		// save and load
		protected string dsFileSaveLocation;
		protected string dsFileName;
		protected string dsFileExtension = ".dsFile";

		// data field area
		protected Vector2 dataFieldScrollView;

		// simulation setting area
		protected Vector2 simulationScrollView;
		protected Vector2 offset;
		protected Vector2 drag;

		// handler
		protected DataSimulator ds;

		[MenuItem(dataSimulatorOptions, false, 30)]
		public static void showWindow() {

			EditorWindow.GetWindow (typeof(DataSimulatorOptions), false, "Simulator");

		}

		void OnEnable() {

			if (ds == null) {
				ds = new DataSimulator ();
			}

		}

		void OnGUI() {

			drawSaveAndLoadBar ();
			drawDataFieldArea ();
			drawSimulationArea ();

			if (GUI.changed) {
				Repaint ();
			}

		}

		private void drawSaveAndLoadBar() {

			GUILayout.BeginHorizontal();
			GUILayout.Label ("File Location:", GUILayout.Width (80f));
			GUILayout.Label (dsFileSaveLocation, GUILayout.MaxWidth (300f));
			if (GUILayout.Button ("...", GUILayout.Width(30f))) {
				dsFileSaveLocation = EditorUtility.SaveFolderPanel ("Choose Save Location", "", "");
			}
			GUILayout.Label ("File Name:", GUILayout.Width (60f));
			dsFileName = EditorGUILayout.TextField (dsFileName, GUILayout.Width (100f));
			if (GUILayout.Button ("Save", GUILayout.Width(60f))) {
				if (!string.IsNullOrEmpty (dsFileSaveLocation) && !string.IsNullOrEmpty (dsFileName)) {
					ds.Save (dsFileSaveLocation, dsFileName + dsFileExtension);
				}
			}
			if (GUILayout.Button ("Load", GUILayout.Width(60f))) {
				if (!string.IsNullOrEmpty (dsFileSaveLocation) && !string.IsNullOrEmpty (dsFileName)) {
					ds.Load (dsFileSaveLocation + "/" + dsFileName + dsFileExtension);
				}
			}
			GUILayout.EndHorizontal ();

		}

		private void drawDataFieldArea() {

			GUILayout.BeginHorizontal();
			GUILayout.Label ("Data", GUILayout.Width (40f));
			GUILayout.Space (position.width - 175f);
			if (GUILayout.Button ("Add", GUILayout.Width(60f))) {
				ds.addData ();
			}
			if (GUILayout.Button ("Clear", GUILayout.Width(60f))) {
				ds.clearData ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginVertical("Box");
			dataFieldScrollView = EditorGUILayout.BeginScrollView (dataFieldScrollView, GUILayout.Height (position.height * 0.3f));
			drawDataBoxes ();
			EditorGUILayout.EndScrollView ();
			GUILayout.EndVertical ();

		}

		private void drawDataBoxes() {

			GUILayout.BeginHorizontal();
			for (int i = 0; i < ds.datas.Count; i++) {
				try {
					GUILayout.BeginVertical ("Box");
					ds.datas [i].scrollView = EditorGUILayout.BeginScrollView (ds.datas [i].scrollView, GUILayout.Width (380f));
					GUILayout.BeginHorizontal ();
					ds.datas [i].name = GUILayout.TextField (ds.datas [i].name);
					if (GUILayout.Button ("x", GUILayout.Width (20))) {
						ds.removeData (i);
					}
					GUILayout.EndHorizontal ();
					drawDataField (ds.datas [i]);
					if (GUILayout.Button ("+", GUILayout.Width (20))) {
						ds.datas [i].addField ();
					}
					EditorGUILayout.EndScrollView ();
					GUILayout.EndVertical ();
				}
				catch(ArgumentOutOfRangeException ex) {
					// do nothing
				}
				catch(Exception ex) {
					Debug.Log (ex.Message);
				}
			}
			GUILayout.EndHorizontal ();

		}

		private void drawDataField(DSDataField data) {

			for (int i = 0; i < data.fields.Count; i++) {
				try {
					GUILayout.BeginHorizontal();
					data.fields [i].name = GUILayout.TextField (data.fields [i].name, GUILayout.Width(80f));
					DSDataType dataType = data.fields [i].type;
					dataType = (DSDataType)EditorGUILayout.EnumPopup (dataType, GUILayout.Width(50f));
					data.updateFieldType (i, dataType);
					switch(data.fields [i].type) {
					case DSDataType.Bool:
						GUILayout.Space(70f);
						DSBool boolField = (DSBool)data.fields[i];
						boolField.value = EditorGUILayout.Toggle(boolField.value);
						break;
					case DSDataType.Float:
						DSFloat floatField = (DSFloat)data.fields[i];
						GUILayout.Label("Random", GUILayout.Width(50f));
						floatField.isRandom = EditorGUILayout.Toggle(floatField.isRandom, GUILayout.Width(20f));
						if(floatField.isRandom) {
							floatField.minValue = EditorGUILayout.FloatField(floatField.minValue, GUILayout.Width(50f));
							GUILayout.Label(" -", GUILayout.Width(20f));
							floatField.maxValue = EditorGUILayout.FloatField(floatField.maxValue, GUILayout.Width(50f));
						}
						else {
							floatField.value = EditorGUILayout.FloatField(floatField.value, GUILayout.Width(50f));
						}
						break;
					case DSDataType.Int:
						DSInt intField = (DSInt)data.fields[i];
						GUILayout.Label("Random", GUILayout.Width(50f));
						intField.isRandom = EditorGUILayout.Toggle(intField.isRandom, GUILayout.Width(20f));
						if(intField.isRandom) {
							intField.minValue = EditorGUILayout.IntField(intField.minValue, GUILayout.Width(50f));
							GUILayout.Label(" -", GUILayout.Width(20f));
							intField.maxValue = EditorGUILayout.IntField(intField.maxValue, GUILayout.Width(50f));
						}
						else {
							intField.value = EditorGUILayout.IntField(intField.value, GUILayout.Width(50f));
						}
						break;
					case DSDataType.String:
						DSString stringField = (DSString)data.fields[i];
						if(string.IsNullOrEmpty(stringField.value)) {
							stringField.value = "";
						}
						stringField.value = GUILayout.TextField(stringField.value);
						break;
					}
					if (GUILayout.Button ("-", GUILayout.Width (20))) {
						data.removeField (i);
					}
					GUILayout.EndHorizontal ();
				}
				catch(ArgumentOutOfRangeException ex) {
					// do nothing
				}
				catch(Exception ex) {
					Debug.Log (ex.Message);
				}
			}

		}

		private void drawSimulationArea() {

			GUILayout.BeginHorizontal();
			GUILayout.Label ("Simulation", GUILayout.Width (80f));
			GUILayout.Space (position.width - 215f);
			if(GUILayout.Button("Simulate", GUILayout.Width(60f))) {
				ds.simulate ();
			}
			if(GUILayout.Button("Reset", GUILayout.Width(60f))) {
				ds.resetNode ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginVertical("Box");
			simulationScrollView = EditorGUILayout.BeginScrollView (simulationScrollView, GUILayout.Height (position.height * 0.5f));
			drawGrid (20f, 0.2f, Color.gray);
			drawGrid (100f, 0.4f, Color.gray);
			drawNodes ();
			drawConncetions ();
			drawUnfinishedConnections (Event.current);
			EditorGUILayout.EndScrollView ();
			GUILayout.EndVertical ();

			processSimulationAreaEvents (Event.current);

		}

		private void drawGrid(float gridSpacing, float gridOpacity, Color gridColor) {

			int width = Mathf.CeilToInt (position.width / gridSpacing);
			int height = Mathf.CeilToInt (position.height / gridSpacing);

			Handles.BeginGUI ();
			Handles.color = new Color (gridColor.r, gridColor.g, gridColor.b, gridOpacity);

			offset += drag * 0.5f;
			Vector3 newOffset = new Vector3 (offset.x % gridSpacing, offset.y % gridSpacing, 0);

			for (int i = 0; i < width; i++) {
				Handles.DrawLine (new Vector3 (gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3 (gridSpacing * i, position.height, 0) + newOffset);
			}
			for (int j = 0; j < height; j++) {
				Handles.DrawLine (new Vector3 (-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3 (position.width, gridSpacing * j, 0) + newOffset);
			}

			Handles.color = Color.white;
			Handles.EndGUI ();

		}

		private void drawNodes() {

			if (ds.nodes != null) {
				for (int i = 0; i < ds.nodes.Count; i++) {
					ds.nodes [i].draw ();
				}
			}

		}

		private void drawConncetions() {

			if (ds.connections != null) {
				for (int i = 0; i < ds.connections.Count; i++) {
					ds.connections [i].draw ();
				}
			}

		}

		private void drawUnfinishedConnections(Event e) {

			if (ds.selectedInPoint != null && ds.selectedOutPoint == null) {
				Handles.DrawBezier (
					ds.selectedInPoint.rect.center,
					e.mousePosition,
					ds.selectedInPoint.rect.center + Vector2.left * 50f,
					e.mousePosition - Vector2.left * 50f,
					Color.white,
					null,
					5f
				);
				GUI.changed = true;
			} else if (ds.selectedOutPoint != null && ds.selectedInPoint == null) {
				Handles.DrawBezier (
					ds.selectedOutPoint.rect.center,
					e.mousePosition,
					ds.selectedOutPoint.rect.center - Vector2.left * 50f,
					e.mousePosition + Vector2.left * 50f,
					Color.white,
					null,
					5f
				);
				GUI.changed = true;
			}

		}

		private void processSimulationAreaEvents(Event e) {

			drag = Vector2.zero;
			Rect eventArea = GUILayoutUtility.GetLastRect ();
			processNodeEvents (e, eventArea);
			if (eventArea.Contains (e.mousePosition)) {
				switch (e.type) {
				case EventType.MouseDown:
					if (e.button == 1) {
						showSimulationMenu (e.mousePosition - eventArea.position);
					}
					break;
				case EventType.MouseDrag:
					if (e.button == 0) {
						dragAll (e.delta);
					}
					break;
				}
			}

		}

		private void processNodeEvents(Event e, Rect eventArea) {

			if (ds.nodes != null) {
				for (int i = ds.nodes.Count - 1; i >= 0; i--) {
					bool guiChanged = ds.nodes [i].processEvent (e, eventArea, ds);
					if (guiChanged) {
						GUI.changed = true;
					}
				}
			}

		}

		private void dragAll(Vector2 delta) {

			drag = delta;
			if (ds.nodes != null) {
				for (int i = ds.nodes.Count - 1; i >= 0; i--) {
					ds.nodes [i].drag (delta);
				}
			}
			GUI.changed = true;

		}

		private void showSimulationMenu(Vector2 mousePosition) {

			GenericMenu customMenu = new GenericMenu ();
			customMenu.AddItem (new GUIContent ("Add node/Basic/Set Value"), false, () => ds.addNode (DSNodeType.SetValue, mousePosition));
			customMenu.AddItem (new GUIContent ("Add node/Basic/Output"), false, () => ds.addNode (DSNodeType.Output, mousePosition));
			customMenu.AddItem (new GUIContent ("Add node/Math/Int. Calculation"), false, () => ds.addNode (DSNodeType.IntCal, mousePosition));
			customMenu.AddItem (new GUIContent ("Add node/Math/Int to Float"), false, () => ds.addNode (DSNodeType.IntToFloat, mousePosition));
			customMenu.AddItem (new GUIContent ("Add node/Math/Float Calculation"), false, () => ds.addNode (DSNodeType.FloatCal, mousePosition));
			customMenu.AddItem (new GUIContent ("Add node/Math/Float to Int"), false, () => ds.addNode (DSNodeType.FloatToInt, mousePosition));
			customMenu.ShowAsContext ();

		}

	}

}