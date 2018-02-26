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
			nodeType = DSNodeType.Output;
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

		public override string save () {

			StringBuilder saveString = new StringBuilder ();
			saveString.Append (base.save ());
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append ((int)actionType);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (Data.DataSimulator.DS_SAVELOAD_CHILD_START);
			for (int i = 0; i < dataList.Count; i++) {
				if (i > 0) {
					saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
				}
				saveString.Append (dataList[i].name);
			}
			saveString.Append (Data.DataSimulator.DS_SAVELOAD_CHILD_END);
			return saveString.ToString ();

		}

		public override void load (string save) {

			List<string> nodeStrings = new List<string> ();
			StringBuilder buffer = new StringBuilder ();
			int level = 0;
			for (int i = 0; i < save.Length; i++) {
				if (level > 0) {
					buffer.Append (save [i]);
				} else {
					if (save [i] == DataSimulator.DS_SAVELOAD_SEPERATOR && buffer.Length > 0) {
						nodeStrings.Add (buffer.ToString ());
						buffer.Length = 0;
						buffer.Capacity = 0;
					} else {
						buffer.Append (save [i]);
					}
				}
				if (save [i] == DataSimulator.DS_SAVELOAD_CHILD_END) {
					level -= 1;
					if (level == 0) {
						nodeStrings.Add (buffer.ToString ());
						buffer.Length = 0;
						buffer.Capacity = 0;
					}
				}
				if (save [i] == DataSimulator.DS_SAVELOAD_CHILD_START) {
					level += 1;
				} 
			}

			actionType = (DSOutputType)int.Parse (nodeStrings [4]);
			parseNodeString (nodeStrings [5]);

		}
			
		public void parseNodeString(string save) {

			string[] saveStrings = save.Replace (DataSimulator.DS_SAVELOAD_CHILD_START.ToString (), "")
				.Replace (DataSimulator.DS_SAVELOAD_CHILD_END.ToString (), "")
				.Split (DataSimulator.DS_SAVELOAD_SEPERATOR);
			for (int i = 0; i < saveStrings.Length; i++) {
				DSOutputData newData = new DSOutputData ();
				newData.name = saveStrings [i];
				string[] splitDataName = newData.name.Split ('/');
				newData.data = ds.datas.Find (x => x.name.Equals (splitDataName [0])).fields.Find (x => x.name.Equals (splitDataName [1]));
				dataList.Add (newData);
			}

		}

	}

}