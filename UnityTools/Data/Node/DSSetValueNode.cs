using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityTools.Data.DataType;

namespace UnityTools.Data.Node {

	public class DSSetValueNode : DSNode {

		public DSDataType actionType;
		public Rect titleRect;
		public Rect extendedRect;

		public DSInt intTarget;
		public DSFloat floatTarget;
		public DSBool boolTarget;
		public DSString stringTarget;

		public int intInput;
		public float floatInput;
		public bool boolInput;
		public string stringInput;

		protected string targetString;

		public DSSetValueNode(int id, Vector2 position, DataSimulator ds) : base(id, position, ds) {

			rect = new Rect (position.x, position.y, 250, 100);
			title = "Set Value";
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
			GUILayout.Label ("Type:", GUILayout.Width (50f));
			actionType = (DSDataType)EditorGUILayout.EnumPopup (actionType);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Target:", GUILayout.Width (60f));
			if (GUILayout.Button (targetString)) {
				chooseTargetWithDropDown ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Value:", GUILayout.Width (60f));
			switch (actionType) {
			case DSDataType.Int:
				intInput = EditorGUILayout.IntField (intInput);
				break;
			case DSDataType.Float:
				floatInput = EditorGUILayout.FloatField (floatInput);
				break;
			case DSDataType.Bool:
				boolInput = EditorGUILayout.Toggle (boolInput);
				break;
			case DSDataType.String:
				stringInput = EditorGUILayout.TextField (stringInput);
				break;
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			GUILayout.EndArea ();

		}

		public void chooseTargetWithDropDown() {

			GenericMenu dropDownMenu = new GenericMenu ();
			switch(actionType) {
			case DSDataType.Int:
				for (int i = 0; i < ds.datas.Count; i++) {
					for (int j = 0; j < ds.datas [i].fields.Count; j++) {
						if (ds.datas [i].fields [j].type == DSDataType.Int) {
							string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
							DSInt item = (DSInt)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								intTarget = item;
								targetString = itemName;
							});
						}
					}
				}
				break;
			case DSDataType.Float:
				for (int i = 0; i < ds.datas.Count; i++) {
					for (int j = 0; j < ds.datas [i].fields.Count; j++) {
						if (ds.datas [i].fields [j].type == DSDataType.Float) {
							string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
							DSFloat item = (DSFloat)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								floatTarget = item;
								targetString = itemName;
							});
						}
					}
				}
				break;
			case DSDataType.Bool:
				for (int i = 0; i < ds.datas.Count; i++) {
					for (int j = 0; j < ds.datas [i].fields.Count; j++) {
						if (ds.datas [i].fields [j].type == DSDataType.Bool) {
							string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
							DSBool item = (DSBool)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								boolTarget = item;
								targetString = itemName;
							});
						}
					}
				}
				break;
			case DSDataType.String:
				for (int i = 0; i < ds.datas.Count; i++) {
					for (int j = 0; j < ds.datas [i].fields.Count; j++) {
						if (ds.datas [i].fields [j].type == DSDataType.String) {
							string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
							DSString item = (DSString)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								stringTarget = item;
								targetString = itemName;
							});
						}
					}
				}
				break;
			}
			dropDownMenu.ShowAsContext ();

		}

		public override void execute () {

			switch (actionType) {
			case DSDataType.Int:
				intTarget.value = intInput;
				break;
			case DSDataType.Float:
				floatTarget.value = floatInput;
				break;
			case DSDataType.Bool:
				boolTarget.value = boolInput;
				break;
			case DSDataType.String:
				stringTarget.value = stringInput;
				break;
			}

		}

	}

}