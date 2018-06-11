#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityTools.Data.DataType;

namespace UnityTools.Data.Node {

	public class DSSetValueNode : DSNode {

		public DSDataType actionType;
		public Rect titleRect;
		public Rect extendedRect;

		public bool assignValue;

		public DSInt intTargetA;
		public DSFloat floatTargetA;
		public DSBool boolTargetA;
		public DSString stringTargetA;

		public DSInt intTargetB;
		public DSFloat floatTargetB;
		public DSBool boolTargetB;
		public DSString stringTargetB;

		public int intInput;
		public float floatInput;
		public bool boolInput;
		public string stringInput;

		protected string targetAString;
		protected string targetBString;

		public DSSetValueNode(int id, Vector2 position, DataSimulator ds) : base(id, position, ds) {

			rect = new Rect (position.x, position.y, 250, 110);
			title = "Set Value";
			nodeType = DSNodeType.SetValue;
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
			GUILayout.Label (assignValue ? "Target:" : "Target A:", GUILayout.Width (60f));
			if (GUILayout.Button (targetAString)) {
				chooseTargetAWithDropDown ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Assign Value:", GUILayout.Width (100f));
			assignValue = EditorGUILayout.Toggle (assignValue);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label (assignValue ? "Value:" : "Target B:", GUILayout.Width (60f));
			switch (actionType) {
			case DSDataType.Int:
				if (assignValue) {
					intInput = EditorGUILayout.IntField (intInput);
				} else {
					if (GUILayout.Button (targetBString)) {
						GenericMenu dropDownMenu = new GenericMenu ();
						for (int i = 0; i < ds.datas.Count; i++) {
							for (int j = 0; j < ds.datas [i].fields.Count; j++) {
								if (ds.datas [i].fields [j].type == DSDataType.Int) {
									string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
									DSInt item = (DSInt)ds.datas [i].fields [j];
									dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
										intTargetB = item;
										targetBString = itemName;
									});
								}
							}
						}
						dropDownMenu.ShowAsContext ();
					}
				}
				break;
			case DSDataType.Float:
				if (assignValue) {
					floatInput = EditorGUILayout.FloatField (floatInput);
				} else {
					if (GUILayout.Button (targetBString)) {
						GenericMenu dropDownMenu = new GenericMenu ();
						for (int i = 0; i < ds.datas.Count; i++) {
							for (int j = 0; j < ds.datas [i].fields.Count; j++) {
								if (ds.datas [i].fields [j].type == DSDataType.Float) {
									string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
									DSFloat item = (DSFloat)ds.datas [i].fields [j];
									dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
										floatTargetB = item;
										targetBString = itemName;
									});
								}
							}
						}
						dropDownMenu.ShowAsContext ();
					}
				}
				break;
			case DSDataType.Bool:
				if (assignValue) {
					boolInput = EditorGUILayout.Toggle (boolInput);
				} else {
					if (GUILayout.Button (targetBString)) {
						GenericMenu dropDownMenu = new GenericMenu ();
						for (int i = 0; i < ds.datas.Count; i++) {
							for (int j = 0; j < ds.datas [i].fields.Count; j++) {
								if (ds.datas [i].fields [j].type == DSDataType.Bool) {
									string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
									DSBool item = (DSBool)ds.datas [i].fields [j];
									dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
										boolTargetB = item;
										targetBString = itemName;
									});
								}
							}
						}
						dropDownMenu.ShowAsContext ();
					}
				}
				break;
			case DSDataType.String:
				if (assignValue) {
					stringInput = EditorGUILayout.TextField (stringInput);
				} else {
					if (GUILayout.Button (targetBString)) {
						GenericMenu dropDownMenu = new GenericMenu ();
						for (int i = 0; i < ds.datas.Count; i++) {
							for (int j = 0; j < ds.datas [i].fields.Count; j++) {
								if (ds.datas [i].fields [j].type == DSDataType.String) {
									string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
									DSString item = (DSString)ds.datas [i].fields [j];
									dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
										stringTargetB = item;
										targetBString = itemName;
									});
								}
							}
						}
						dropDownMenu.ShowAsContext ();
					}
				}
				break;
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			GUILayout.EndArea ();

		}

		public void chooseTargetAWithDropDown() {

			GenericMenu dropDownMenu = new GenericMenu ();
			switch(actionType) {
			case DSDataType.Int:
				for (int i = 0; i < ds.datas.Count; i++) {
					for (int j = 0; j < ds.datas [i].fields.Count; j++) {
						if (ds.datas [i].fields [j].type == DSDataType.Int) {
							string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
							DSInt item = (DSInt)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								intTargetA = item;
								targetAString = itemName;
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
								floatTargetA = item;
								targetAString = itemName;
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
								boolTargetA = item;
								targetAString = itemName;
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
								stringTargetA = item;
								targetAString = itemName;
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
				if (assignValue) {
					intTargetA.value = intInput;
				} else {
					intTargetA.value = intTargetB.value;
				}
				break;
			case DSDataType.Float:
				if (assignValue) {
					floatTargetA.value = floatInput;
				} else {
					floatTargetA.value = floatTargetB.value;
				}				break;
			case DSDataType.Bool:
				if (assignValue) {
					boolTargetA.value = boolInput;
				} else {
					boolTargetA.value = boolTargetB.value;
				}				break;
			case DSDataType.String:
				if (assignValue) {
					stringTargetA.value = stringInput;
				} else {
					stringTargetA.value = stringTargetB.value;
				}				break;
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
			saveString.Append (assignValue);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			if (assignValue) {
				switch (actionType) {
				case DSDataType.Int:
					saveString.Append (intInput);
					break;
				case DSDataType.Float:
					saveString.Append (floatInput);
					break;
				case DSDataType.Bool:
					saveString.Append (boolInput);
					break;
				case DSDataType.String:
					saveString.Append (stringInput);
					break;
				}
			} else {
				saveString.Append (targetBString);
			}
			return saveString.ToString ();

		}

		public override void load (string save) {

			string[] saveStrings = save.Split (DataSimulator.DS_SAVELOAD_SEPERATOR);
			actionType = (DSDataType)int.Parse (saveStrings [4]);
			targetAString = saveStrings [5];
			assignValue = bool.Parse (saveStrings [6]);
			if (!string.IsNullOrEmpty (targetAString)) {
				string[] splitTargetAStrings = targetAString.Split ('/');
				string[] splitTargetBStrings = null;
				switch(actionType) {
				case DSDataType.Int:
					intTargetA = (DSInt)ds.datas.Find (x => x.name.Equals (splitTargetAStrings [0])).fields.Find (x => x.name.Equals (splitTargetAStrings [1]));
					if (assignValue) {
						intInput = int.Parse (saveStrings [7]);
					} else {
						targetBString = saveStrings [7];
						splitTargetBStrings = targetBString.Split ('/');
						intTargetB = (DSInt)ds.datas.Find (x => x.name.Equals (splitTargetBStrings [0])).fields.Find (x => x.name.Equals (splitTargetBStrings [1]));
					}
					break;
				case DSDataType.Float:
					floatTargetA = (DSFloat)ds.datas.Find (x => x.name.Equals (splitTargetAStrings [0])).fields.Find (x => x.name.Equals (splitTargetAStrings [1]));
					if (assignValue) {
						floatInput = float.Parse (saveStrings [7]);
					} else {
						targetBString = saveStrings [7];
						splitTargetBStrings = targetBString.Split ('/');
						floatTargetB = (DSFloat)ds.datas.Find (x => x.name.Equals (splitTargetBStrings [0])).fields.Find (x => x.name.Equals (splitTargetBStrings [1]));
					}					break;
				case DSDataType.Bool:
					boolTargetA = (DSBool)ds.datas.Find (x => x.name.Equals (splitTargetAStrings [0])).fields.Find (x => x.name.Equals (splitTargetAStrings [1]));
					if (assignValue) {
						boolInput = bool.Parse (saveStrings [7]);
					} else {
						targetBString = saveStrings [7];
						splitTargetBStrings = targetBString.Split ('/');
						boolTargetB = (DSBool)ds.datas.Find (x => x.name.Equals (splitTargetBStrings [0])).fields.Find (x => x.name.Equals (splitTargetBStrings [1]));
					}						break;
				case DSDataType.String:
					stringTargetA = (DSString)ds.datas.Find (x => x.name.Equals (splitTargetAStrings [0])).fields.Find (x => x.name.Equals (splitTargetAStrings [1]));
					if (assignValue) {
						stringInput = saveStrings [7];
					} else {
						targetBString = saveStrings [7];
						splitTargetBStrings = targetBString.Split ('/');
						stringTargetB = (DSString)ds.datas.Find (x => x.name.Equals (splitTargetBStrings [0])).fields.Find (x => x.name.Equals (splitTargetBStrings [1]));
					}
					break;
				}
			}
		}

	}

}
#endif