using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityTools.Data.DataType;

namespace UnityTools.Data.Node {

	public enum DSOutputType {
		print = 0
	}

	public class DSOutputNode : DSNode {

		public class DSOutputData {
			public string name;
			public IDSData data;
		}

		public DSOutputType actionType;
		public Rect titleRect;
		public Rect extendedRect;

		public List<DSOutputData> dataList;

		public DSOutputNode(int id, Vector2 position, DataSimulator ds) : base(id, position, ds) {

			dataList = new List<DSOutputData> ();
			rect = new Rect (position.x, position.y, 200, 80f);
			title = "Output";
			inPoint = new DSConnectionPoint (id, DSConnectionPointType.In, ds);
			outPoint = new DSConnectionPoint (id, DSConnectionPointType.Out, ds);

		}

		public override void draw () {

			rect.height = 25f + dataList.Count * 20f + 55f;
			drawInOutPoint ();
			titleRect = rect;
			titleRect.height = 20f;
			extendedRect = rect;
			extendedRect.y = rect.y + titleRect.height - 1f;
			extendedRect.height = rect.height - titleRect.height;
			GUILayout.BeginArea (titleRect, title, GUI.skin.box);
			GUILayout.EndArea ();
			GUILayout.BeginArea (extendedRect, GUI.skin.box);
			GUILayout.BeginVertical ();
			GUILayout.Space (5f);
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Type:", GUILayout.Width (50f));
			actionType = (DSOutputType)EditorGUILayout.EnumPopup (actionType);
			GUILayout.EndHorizontal ();
			for (int n = 0; n < dataList.Count; n++) {
				if (GUILayout.Button (dataList[n].name)) {
					DSOutputData data = dataList [n];
					GenericMenu dropDownMenu = new GenericMenu ();
					for (int i = 0; i < ds.datas.Count; i++) {
						for (int j = 0; j < ds.datas [i].fields.Count; j++) {
							string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
							IDSData item = ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								data.data = item;
								data.name = itemName;
							});
						}
					}
					dropDownMenu.ShowAsContext ();
				}
			}
			if (GUILayout.Button ("+", GUILayout.Width (20f))) {
				DSOutputData newData = new DSOutputData ();
				newData.data = null;
				newData.name = "";
				dataList.Add (newData);
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea ();

		}

		public string getValue(IDSData data) {

			switch (data.type) {
			case DSDataType.Bool:
				return ((DSBool)data).value.ToString ();
			case DSDataType.Int:
				return ((DSInt)data).value.ToString ();
			case DSDataType.Float:
				return ((DSFloat)data).value.ToString ();
			case DSDataType.String:
				return ((DSString)data).value;
			}
			return "";

		}

		public override void execute () {

			switch (actionType) {
			case DSOutputType.print:
				StringBuilder builder = new StringBuilder ();
				for (int i = 0; i < dataList.Count; i++) {
					builder.Append (getValue (dataList [i].data));
					if (i != dataList.Count - 1) {
						builder.Append (",");
					}
				}
				Debug.Log (builder.ToString ());
				break;
			}

		}

	}

}