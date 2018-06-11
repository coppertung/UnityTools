#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityTools.Data.DataType;

namespace UnityTools.Data.Node {

	public enum DSFloatToIntType {
		Round = 0, Ceil = 1, Floor = 2
	}

	public class DSFloatToIntNode : DSNode {

		public DSFloatToIntType actionType;
		public Rect titleRect;
		public Rect extendedRect;

		public DSFloat target;
		public DSInt result;

		protected string targetString;
		protected string resultString;

		public DSFloatToIntNode(int id, Vector2 position, DataSimulator ds) : base(id, position, ds) {

			rect = new Rect (position.x, position.y, 250, 100);
			title = "Float To Int.";
			nodeType = DSNodeType.FloatToInt;
			inPoint = new DSConnectionPoint (id, DSConnectionPointType.In, ds);
			outPoint = new DSConnectionPoint (id, DSConnectionPointType.Out, ds);

		}

		public override void draw () {

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
			GUILayout.Label ("Type:", GUILayout.Width (60f));
			actionType = (DSFloatToIntType)EditorGUILayout.EnumPopup (actionType);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Target:", GUILayout.Width (60f));
			if (GUILayout.Button (targetString)) {
				chooseTargetWithDropDown ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Result:", GUILayout.Width (60f));
			if (GUILayout.Button (resultString)) {
				chooseResultWithDropDown ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			GUILayout.EndArea ();

		}

		public void chooseTargetWithDropDown() {

			GenericMenu dropDownMenu = new GenericMenu ();
			for (int i = 0; i < ds.datas.Count; i++) {
				for (int j = 0; j < ds.datas [i].fields.Count; j++) {
					if (ds.datas [i].fields [j].type == DSDataType.Float) {
						string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
						DSFloat item = (DSFloat)ds.datas [i].fields [j];
						dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
							target = item;
							targetString = itemName;
						});
					}
				}
			}
			dropDownMenu.ShowAsContext ();

		}

		public void chooseResultWithDropDown() {

			GenericMenu dropDownMenu = new GenericMenu ();
			for (int i = 0; i < ds.datas.Count; i++) {
				for (int j = 0; j < ds.datas [i].fields.Count; j++) {
					if (ds.datas [i].fields [j].type == DSDataType.Int) {
						string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
						DSInt item = (DSInt)ds.datas [i].fields [j];
						dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
							result = item;
							resultString = itemName;
						});
					}
				}
			}
			dropDownMenu.ShowAsContext ();

		}

		public override void execute () {

			switch (actionType) {
			case DSFloatToIntType.Round:
				result.value = Mathf.RoundToInt (target.value);
				break;
			case DSFloatToIntType.Ceil:
				result.value = Mathf.CeilToInt (target.value);
				break;
			case DSFloatToIntType.Floor:
				result.value = Mathf.FloorToInt (target.value);
				break;
			}

		}

		public override string save () {

			StringBuilder saveString = new StringBuilder ();
			saveString.Append (base.save ());
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append ((int)actionType);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (targetString);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (resultString);
			return saveString.ToString ();

		}

		public override void load (string save) {

			string[] saveStrings = save.Split (DataSimulator.DS_SAVELOAD_SEPERATOR);
			actionType = (DSFloatToIntType)int.Parse (saveStrings [4]);
			targetString = saveStrings [5];
			if (!string.IsNullOrEmpty (targetString)) {
				string[] splitTargetStrings = targetString.Split ('/');
				target = (DSFloat)ds.datas.Find (x => x.name.Equals (splitTargetStrings [0])).fields.Find (x => x.name.Equals (splitTargetStrings [1]));
			}
			resultString = saveStrings [6];
			if (!string.IsNullOrEmpty (resultString)) {
				string[] splitResultStrings = resultString.Split ('/');
				result = (DSInt)ds.datas.Find (x => x.name.Equals (splitResultStrings [0])).fields.Find (x => x.name.Equals (splitResultStrings [1]));
			}
		}

	}

}
#endif