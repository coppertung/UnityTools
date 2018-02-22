﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityTools.Data.DataType;

namespace UnityTools.Data.Node {

	public enum DSFloatCalType {
		Add = 0, Subtract = 1, Multiply = 2, Divide = 3, Absolute = 4, Log = 5, Max = 6, Min = 7,
		Mod = 8, Power = 9, Random = 10, SqaureRoot = 11
	}

	public class DSFloatCalNode : DSNode {

		public DSFloatCalType actionType;
		public Rect titleRect;
		public Rect extendedRect;

		public DSFloat targetA;
		public DSFloat targetB;
		public DSFloat result;

		protected string targetAString;
		protected string targetBString;
		protected string resultString;

		public DSFloatCalNode(int id, Vector2 position, DataSimulator ds) : base(id, position, ds) {

			rect = new Rect (position.x, position.y, 250, 110);
			title = "Float Calculation";
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
			actionType = (DSFloatCalType)EditorGUILayout.EnumPopup (actionType);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Target A:", GUILayout.Width (60f));
			if (GUILayout.Button (targetAString)) {
				chooseTargetAWithDropDown ();
			}
			GUILayout.EndHorizontal ();
			switch (actionType) {
			case DSFloatCalType.Add:
			case DSFloatCalType.Subtract:
			case DSFloatCalType.Multiply:
			case DSFloatCalType.Divide:
			case DSFloatCalType.Mod:
			case DSFloatCalType.Power:
			case DSFloatCalType.Max:
			case DSFloatCalType.Min:
			case DSFloatCalType.Log:
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Target B:", GUILayout.Width (60f));
				if (GUILayout.Button (targetBString)) {
					chooseTargetBWithDropDown ();
				}
				GUILayout.EndHorizontal ();
				break;
			}
			switch (actionType) {
			case DSFloatCalType.Add:
			case DSFloatCalType.Subtract:
			case DSFloatCalType.Multiply:
			case DSFloatCalType.Divide:
			case DSFloatCalType.Mod:
			case DSFloatCalType.Power:
			case DSFloatCalType.Max:
			case DSFloatCalType.Min:
			case DSFloatCalType.Absolute:
			case DSFloatCalType.Log:
			case DSFloatCalType.SqaureRoot:
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
					if (ds.datas [i].fields [j].type == DSDataType.Float) {
						string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
						DSFloat item = (DSFloat)ds.datas [i].fields [j];
						if (actionType == DSFloatCalType.Random ? item.isRandom : true) {
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
					if (ds.datas [i].fields [j].type == DSDataType.Float) {
						string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
						DSFloat item = (DSFloat)ds.datas [i].fields [j];
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
					if (ds.datas [i].fields [j].type == DSDataType.Float) {
						string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
						DSFloat item = (DSFloat)ds.datas [i].fields [j];
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
			case DSFloatCalType.Add:
				result.value = targetA.value + targetB.value;
				break;
			case DSFloatCalType.Subtract:
				result.value = targetA.value - targetB.value;
				break;
			case DSFloatCalType.Multiply:
				result.value = targetA.value * targetB.value;
				break;
			case DSFloatCalType.Divide:
				result.value = targetA.value / targetB.value;
				break;
			case DSFloatCalType.Mod:
				result.value = targetA.value % targetB.value;
				break;
			case DSFloatCalType.Power:
				result.value = (int)Mathf.Pow (targetA.value, targetB.value);
				break;
			case DSFloatCalType.Max:
				result.value = Mathf.Max (targetA.value, targetB.value);
				break;
			case DSFloatCalType.Min:
				result.value = Mathf.Min (targetA.value, targetB.value);
				break;
			case DSFloatCalType.Absolute:
				result.value = Mathf.Abs (targetA.value);
				break;
			case DSFloatCalType.Random:
				targetA.genRandomValue ();
				break;
			case DSFloatCalType.Log:
				result.value = Mathf.Log (targetA.value, targetB.value);
				break;
			case DSFloatCalType.SqaureRoot:
				result.value = Mathf.Sqrt (targetA.value);
				break;
			}

		}

	}

}