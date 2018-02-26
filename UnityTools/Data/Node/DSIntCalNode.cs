using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityTools.Data.DataType;

namespace UnityTools.Data.Node {

	public enum DSIntCalType {
		Add = 0, Subtract = 1, Multiply = 2, Divide = 3, Absolute = 4, Max = 5, Min = 6, Mod = 7,
		Power = 8, Random = 9
	}

	public class DSIntCalNode : DSNode {

		public DSIntCalType actionType;
		public Rect titleRect;
		public Rect extendedRect;

		public DSInt targetA;
		public DSInt targetB;
		public DSInt result;

		protected string targetAString;
		protected string targetBString;
		protected string resultString;

		public DSIntCalNode(int id, Vector2 position, DataSimulator ds) : base(id, position, ds) {

			rect = new Rect (position.x, position.y, 250, 110);
			title = "Int. Calculation";
			nodeType = DSNodeType.IntCal;
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
			actionType = (DSIntCalType)EditorGUILayout.EnumPopup (actionType);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Target A:", GUILayout.Width (60f));
			if (GUILayout.Button (targetAString)) {
				chooseTargetAWithDropDown ();
			}
			GUILayout.EndHorizontal ();
			switch (actionType) {
			case DSIntCalType.Add:
			case DSIntCalType.Subtract:
			case DSIntCalType.Multiply:
			case DSIntCalType.Divide:
			case DSIntCalType.Mod:
			case DSIntCalType.Power:
			case DSIntCalType.Max:
			case DSIntCalType.Min:
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Target B:", GUILayout.Width (60f));
				if (GUILayout.Button (targetBString)) {
					chooseTargetBWithDropDown ();
				}
				GUILayout.EndHorizontal ();
				break;
			}
			switch (actionType) {
			case DSIntCalType.Add:
			case DSIntCalType.Subtract:
			case DSIntCalType.Multiply:
			case DSIntCalType.Divide:
			case DSIntCalType.Mod:
			case DSIntCalType.Power:
			case DSIntCalType.Max:
			case DSIntCalType.Min:
			case DSIntCalType.Absolute:
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Result:", GUILayout.Width (60f));
				if (GUILayout.Button (resultString)) {
					chooseResultWithDropDown ();
				}
				GUILayout.EndHorizontal ();
				break;
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea ();

		}

		public void chooseTargetAWithDropDown() {

			GenericMenu dropDownMenu = new GenericMenu ();
			for (int i = 0; i < ds.datas.Count; i++) {
				for (int j = 0; j < ds.datas [i].fields.Count; j++) {
					if (ds.datas [i].fields [j].type == DSDataType.Int) {
						string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
						DSInt item = (DSInt)ds.datas [i].fields [j];
						if (actionType == DSIntCalType.Random ? item.isRandom : true) {
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								targetA = item;
								targetAString = itemName;
							});
						}
					}
				}
			}
			dropDownMenu.ShowAsContext ();

		}

		public void chooseTargetBWithDropDown() {

			GenericMenu dropDownMenu = new GenericMenu ();
			for (int i = 0; i < ds.datas.Count; i++) {
				for (int j = 0; j < ds.datas [i].fields.Count; j++) {
					if (ds.datas [i].fields [j].type == DSDataType.Int) {
						string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
						DSInt item = (DSInt)ds.datas [i].fields [j];
						dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
							targetB = item;
							targetBString = itemName;
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
			case DSIntCalType.Add:
				result.value = targetA.value + targetB.value;
				break;
			case DSIntCalType.Subtract:
				result.value = targetA.value - targetB.value;
				break;
			case DSIntCalType.Multiply:
				result.value = targetA.value * targetB.value;
				break;
			case DSIntCalType.Divide:
				result.value = targetA.value / targetB.value;
				break;
			case DSIntCalType.Mod:
				result.value = targetA.value % targetB.value;
				break;
			case DSIntCalType.Power:
				result.value = (int)Mathf.Pow (targetA.value, targetB.value);
				break;
			case DSIntCalType.Max:
				result.value = Mathf.Max (targetA.value, targetB.value);
				break;
			case DSIntCalType.Min:
				result.value = Mathf.Min (targetA.value, targetB.value);
				break;
			case DSIntCalType.Absolute:
				result.value = Mathf.Abs (targetA.value);
				break;
			case DSIntCalType.Random:
				targetA.genRandomValue ();
				break;
			}

		}

		public override string save () {

			StringBuilder saveString = new StringBuilder ();
			saveString.Append (base.save ());
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append ((int)actionType);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (targetAString);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (targetBString);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (resultString);
			return saveString.ToString ();

		}

		public override void load (string save) {

			string[] saveStrings = save.Split (DataSimulator.DS_SAVELOAD_SEPERATOR);
			actionType = (DSIntCalType)int.Parse (saveStrings [4]);
			targetAString = saveStrings [5];
			if (!string.IsNullOrEmpty (targetAString)) {
				string[] splitTargetAStrings = targetAString.Split ('/');
				targetA = (DSInt)ds.datas.Find (x => x.name.Equals (splitTargetAStrings [0])).fields.Find (x => x.name.Equals (splitTargetAStrings [1]));
			}
			targetBString = saveStrings [6];
			if (!string.IsNullOrEmpty (targetBString)) {
				string[] splitTargetBStrings = targetBString.Split ('/');
				targetB = (DSInt)ds.datas.Find (x => x.name.Equals (splitTargetBStrings [0])).fields.Find (x => x.name.Equals (splitTargetBStrings [1]));
			}
			resultString = saveStrings [7];
			if (!string.IsNullOrEmpty (resultString)) {
				string[] splitResultStrings = resultString.Split ('/');
				result = (DSInt)ds.datas.Find (x => x.name.Equals (splitResultStrings [0])).fields.Find (x => x.name.Equals (splitResultStrings [1]));
			}
		}

	}

}