using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Data.Node {

	public class DSConnection {

		public DSConnectionPoint inPoint;
		public DSConnectionPoint outPoint;
		private DataSimulator ds;

		public Rect removeButtonRect;

		public DSConnection(DataSimulator ds) {

			this.ds = ds;
			removeButtonRect = new Rect (0f, 0f, 20f, 20f);

		}

		public DSConnection(DSConnectionPoint inPoint, DSConnectionPoint outPoint, DataSimulator ds) {

			this.ds = ds;
			this.inPoint = inPoint;
			this.outPoint = outPoint;
			removeButtonRect = new Rect (0f, 0f, 20f, 20f);

		}

		public void draw() {

			Handles.DrawBezier (
				inPoint.rect.center,
				outPoint.rect.center,
				inPoint.rect.center + Vector2.left * 50f,
				outPoint.rect.center - Vector2.left * 50f,
				Color.white,
				null,
				5f
			);
			Handles.Button ((inPoint.rect.center + outPoint.rect.center) / 2, Quaternion.identity, 10f, 20f, Handles.RectangleHandleCap);
			// require a GUI Button to replace the function of Handles Button as Handles throws null exception in editorWindow while handling the events
			// reference from: https://answers.unity.com/questions/1276985/handlesbutton-unresponsive-in-custom-editor-after.html
			removeButtonRect.x = (inPoint.rect.center.x + outPoint.rect.center.x) / 2 - removeButtonRect.width / 2;
			removeButtonRect.y = (inPoint.rect.center.y + outPoint.rect.center.y) / 2 - removeButtonRect.height / 2;
			if (GUI.Button (removeButtonRect, GUIContent.none, GUIStyle.none)) {
				ds.removeConnection (this);
			}

		}

		public string save() {

			StringBuilder saveString = new StringBuilder ();
			saveString.Append ((int)inPoint.type);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (inPoint.nodeID);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append ((int)outPoint.type);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (outPoint.nodeID);
			return saveString.ToString ();

		}

		public void load(string save) {

			string[] splited = save.Split (DataSimulator.DS_SAVELOAD_SEPERATOR);
			DSConnectionPointType inType = (DSConnectionPointType)int.Parse (splited [0]);
			DSNode inNode = ds.nodes.Find (x => x.id == int.Parse (splited [1]));
			DSConnectionPointType outType = (DSConnectionPointType)int.Parse (splited [2]);
			DSNode outNode = ds.nodes.Find (x => x.id == int.Parse (splited [3]));

			switch (inType) {
			case DSConnectionPointType.In:
				inPoint = inNode.inPoint;
				break;
			default:
				break;
			}

			switch (outType) {
			case DSConnectionPointType.Out:
				outPoint = outNode.outPoint;
				break;
			case DSConnectionPointType.TrueOut:
				outPoint = ((DSSelectionNode)outNode).trueOutPoint;
				break;
			case DSConnectionPointType.FalseOut:
				outPoint = ((DSSelectionNode)outNode).falseOutPoint;
				break;
			default:
				break;
			}

		}

	}

}