#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Data.Node {

	public enum DSNodeType {
		Start = 0,
		SetValue = 6, Output = 5,
		IntCal = 1, FloatCal = 2, FloatToInt = 3, IntToFloat = 4,		// Math functions
		IfStatement = 7
	}

	public class DSNode {

		public DSNodeType nodeType;

		public int id;

		public Rect rect;
		public string title;
		public bool isDragged;

		public DSConnectionPoint inPoint;
		public DSConnectionPoint outPoint;

		public bool isSelectionNode = false;

		protected DataSimulator ds;

		public DSNode() {}

		public DSNode(int id, Vector2 position, DataSimulator ds) {

			this.ds = ds;
			this.id = id;
			rect = new Rect (position.x, position.y, 50, 25);
			title = "Start";
			nodeType = DSNodeType.Start;
			outPoint = new DSConnectionPoint (id, DSConnectionPointType.Out, ds);
				
		}

		public void drag(Vector2 delta) {

			rect.position += delta;

		}

		public virtual void draw() {

			drawInOutPoint ();
			GUILayout.BeginArea (rect, title, GUI.skin.box);
			GUILayout.EndArea ();

		}

		public virtual void drawInOutPoint() {

			if (inPoint != null) {
				inPoint.draw ();
			}
			if (outPoint != null) {
				outPoint.draw ();
			}

		}

		public bool processEvent(Event e, Rect eventArea, DataSimulator ds) {

			switch (e.type) {
			case EventType.MouseDown:
				if (e.button == 0) {
					if (rect.Contains (e.mousePosition - eventArea.position)) {
						isDragged = true;
					}
					GUI.changed = true;
				} else if (e.button == 1 && rect.Contains (e.mousePosition - eventArea.position)) {
					showNodeMenu (ds, e.mousePosition - eventArea.position);
					e.Use ();
				}
				break;
			case EventType.MouseUp:
				isDragged = false;
				break;
			case EventType.MouseDrag:
				if (e.button == 0 && isDragged) {
					drag (e.delta);
					e.Use ();
					return true;
				}
				break;
			}

			return false;

		}

		private void showNodeMenu(DataSimulator ds, Vector2 mousePosition) {

			GenericMenu customMenu = new GenericMenu ();
			customMenu.AddItem (new GUIContent ("Remove"), false, () => ds.removeNode (this));
			customMenu.ShowAsContext ();

		}

		public virtual void execute() {
		}

		public virtual string save() {

			StringBuilder saveString = new StringBuilder ();
			saveString.Append (nodeType);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (id);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (rect.position.x);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (rect.position.y);
			return saveString.ToString ();

		}

		public virtual void load(string save) {
		}

	}

}
#endif