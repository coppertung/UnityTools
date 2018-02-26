using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityTools.Data.DataType;

namespace UnityTools.Data.Node {
	
	public enum DSCompareType {
		EqualTo = 0, NotEqualTo = 1, GreaterThan = 2, GreaterThanOrEqualTo = 3, LessThan = 4, LessThanOrEqualTo = 5
	}

	public class DSIfStatement {

		public bool compareValue;
		public DSCompareType type;
		public DSDataType targetType;

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

		public string targetAString;
		public string targetBString;

	}

	public class DSIfNode : DSSelectionNode {
		
		public Rect titleRect;
		public Rect extendedRect;

		public List<DSIfStatement> ifStatements;
		public List<DSLogicGateType> logicOperators;

		public DSIfNode(int id, Vector2 position, DataSimulator ds) : base(id, position, ds) {

			rect = new Rect (position.x, position.y, 260, 110);
			title = "If Statement";
			nodeType = DSNodeType.IfStatement;
			ifStatements = new List<DSIfStatement> ();
			logicOperators = new List<DSLogicGateType> ();

		}

		public override void draw () {
			
			rect.height = 25f + ifStatements.Count * 100f + logicOperators.Count * 25f + 30f;
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
			for (int i = 0; i < ifStatements.Count; i++) {
				if (i > 0) {
					logicOperators [i - 1] = (DSLogicGateType)EditorGUILayout.EnumPopup (logicOperators [i - 1]);
				}
				drawIfStatementBox (ifStatements [i]);
			}
			if(GUILayout.Button("+", GUILayout.Width(20f))) {
				if (ifStatements.Count > 0) {
					DSLogicGateType newOperator = DSLogicGateType.AND;
					logicOperators.Add (newOperator);
				}
				DSIfStatement newIf = new DSIfStatement ();
				ifStatements.Add (newIf);
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea ();

		}

		public void drawIfStatementBox(DSIfStatement statement) {

			float lableSize = 110f;
			GUILayout.BeginVertical ("Box");
			GUILayout.BeginHorizontal ();
			statement.type = (DSCompareType)EditorGUILayout.EnumPopup (statement.type);
			switch(statement.targetType) {
			case DSDataType.Int:
			case DSDataType.Float:
				break;
			case DSDataType.Bool:
			case DSDataType.String:
				if ((int)statement.type > 1) {
					statement.type = DSCompareType.NotEqualTo;
				}
				break;
			}
			if (GUILayout.Button ("-", GUILayout.Width (20f))) {
				int index = ifStatements.IndexOf (statement);
				if (index > 0) {
					logicOperators.RemoveAt (index - 1);
				}
				if (index < logicOperators.Count - 2) {
					logicOperators.RemoveAt (index + 1);
				}
				ifStatements.Remove (statement);
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Target type:", GUILayout.Width(lableSize));
			statement.targetType = (DSDataType)EditorGUILayout.EnumPopup (statement.targetType);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Compare Value:", GUILayout.Width(lableSize));
			statement.compareValue = EditorGUILayout.Toggle (statement.compareValue);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label (statement.compareValue ? "Target:" : "Target A:", GUILayout.Width (lableSize));
			if (GUILayout.Button (statement.targetAString)) {
				chooseTargetA (statement);
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			if (statement.compareValue) {
				GUILayout.Label ("Value:", GUILayout.Width (lableSize));
				switch (statement.targetType) {
				case DSDataType.Int:
					statement.intInput = EditorGUILayout.IntField (statement.intInput);
					break;
				case DSDataType.Float:
					statement.floatInput = EditorGUILayout.FloatField (statement.floatInput);
					break;
				case DSDataType.Bool:
					statement.boolInput = EditorGUILayout.Toggle (statement.boolInput);
					break;
				case DSDataType.String:
					statement.stringInput = EditorGUILayout.TextField (statement.stringInput);
					break;
				}
			} else {
				GUILayout.Label ("Target B", GUILayout.Width (lableSize));
				if (GUILayout.Button (statement.targetBString)) {
					chooseTargetB (statement);
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();

		}

		public void chooseTargetA(DSIfStatement statement) {

			GenericMenu dropDownMenu = new GenericMenu ();
			for (int i = 0; i < ds.datas.Count; i++) {
				for (int j = 0; j < ds.datas [i].fields.Count; j++) {
					if (ds.datas [i].fields [j].type == statement.targetType) {
						string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
						switch (statement.targetType) {
						case DSDataType.Int:
							DSInt intItem = (DSInt)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								statement.intTargetA = intItem;
								statement.targetAString = itemName;
							});
							break;
						case DSDataType.Float:
							DSFloat floatItem = (DSFloat)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								statement.floatTargetA = floatItem;
								statement.targetAString = itemName;
							});
							break;
						case DSDataType.Bool:
							DSBool boolItem = (DSBool)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								statement.boolTargetA = boolItem;
								statement.targetAString = itemName;
							});
							break;
						case DSDataType.String:
							DSString stringItem = (DSString)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								statement.stringTargetA = stringItem;
								statement.targetAString = itemName;
							});
							break;
						}
					}
				}
			}
			dropDownMenu.ShowAsContext ();

		}

		public void chooseTargetB(DSIfStatement statement) {

			GenericMenu dropDownMenu = new GenericMenu ();
			for (int i = 0; i < ds.datas.Count; i++) {
				for (int j = 0; j < ds.datas [i].fields.Count; j++) {
					if (ds.datas [i].fields [j].type == statement.targetType) {
						string itemName = ds.datas [i].name + "/" + ds.datas [i].fields [j].name;
						switch (statement.targetType) {
						case DSDataType.Int:
							DSInt intItem = (DSInt)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								statement.intTargetB = intItem;
								statement.targetBString = itemName;
							});
							break;
						case DSDataType.Float:
							DSFloat floatItem = (DSFloat)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								statement.floatTargetB = floatItem;
								statement.targetBString = itemName;
							});
							break;
						case DSDataType.Bool:
							DSBool boolItem = (DSBool)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								statement.boolTargetB = boolItem;
								statement.targetBString = itemName;
							});
							break;
						case DSDataType.String:
							DSString stringItem = (DSString)ds.datas [i].fields [j];
							dropDownMenu.AddItem (new GUIContent (itemName), false, () => {
								statement.stringTargetB = stringItem;
								statement.targetBString = itemName;
							});
							break;
						}
					}
				}
			}
			dropDownMenu.ShowAsContext ();

		}

		public bool compare(DSCompareType type, int valueA, int valueB) {

			switch (type) {
			case DSCompareType.EqualTo:
				return valueA == valueB;
			case DSCompareType.NotEqualTo:
				return valueA != valueB;
			case DSCompareType.GreaterThan:
				return valueA > valueB;
			case DSCompareType.GreaterThanOrEqualTo:
				return valueA >= valueB;
			case DSCompareType.LessThan:
				return valueA < valueB;
			case DSCompareType.LessThanOrEqualTo:
				return valueA <= valueB;
			}
			return false;

		}

		public bool compare(DSCompareType type, float valueA, float valueB) {

			switch (type) {
			case DSCompareType.EqualTo:
				return valueA == valueB;
			case DSCompareType.NotEqualTo:
				return valueA != valueB;
			case DSCompareType.GreaterThan:
				return valueA > valueB;
			case DSCompareType.GreaterThanOrEqualTo:
				return valueA >= valueB;
			case DSCompareType.LessThan:
				return valueA < valueB;
			case DSCompareType.LessThanOrEqualTo:
				return valueA <= valueB;
			}
			return false;

		}

		public bool compare(DSCompareType type, bool valueA, bool valueB) {

			switch (type) {
			case DSCompareType.EqualTo:
				return valueA == valueB;
			case DSCompareType.NotEqualTo:
				return valueA != valueB;
			}
			return false;

		}

		public bool compare(DSCompareType type, string valueA, string valueB) {

			switch (type) {
			case DSCompareType.EqualTo:
				return valueA.Equals(valueB);
			case DSCompareType.NotEqualTo:
				return !valueA.Equals(valueB);
			}
			return false;

		}

		public override void execute () {

			if (ifStatements.Count > 0) {
				for (int i = 0; i < ifStatements.Count; i++) {
					bool ifResult = false;
					switch (ifStatements [i].targetType) {
					case DSDataType.Int:
						ifResult = compare (
							ifStatements [i].type,
							ifStatements [i].intTargetA.value,
							ifStatements [i].compareValue ? ifStatements [i].intInput : ifStatements [i].intTargetB.value
						);
						break;
					case DSDataType.Float:
						ifResult = compare (
							ifStatements [i].type,
							ifStatements [i].floatTargetA.value,
							ifStatements [i].compareValue ? ifStatements [i].floatInput : ifStatements [i].floatTargetB.value
						);
						break;
					case DSDataType.Bool:
						ifResult = compare (
							ifStatements [i].type,
							ifStatements [i].boolTargetA.value,
							ifStatements [i].compareValue ? ifStatements [i].boolInput : ifStatements [i].boolTargetB.value
						);
						break;
					case DSDataType.String:
						ifResult = compare (
							ifStatements [i].type,
							ifStatements [i].stringTargetA.value,
							ifStatements [i].compareValue ? ifStatements [i].stringInput : ifStatements [i].stringTargetB.value
						);
						break;
					}
					if (i == 0) {
						result = ifResult;
					} else {
						result = DSLogicGate.GetOutput (logicOperators [i - 1], result, ifResult);
					}
				}
			} else {
				result = true;
			}

		}

		public override string save () {

			StringBuilder saveString = new StringBuilder ();
			saveString.Append (base.save ());
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_START);
			for (int i = 0; i < ifStatements.Count; i++) {
				if (i > 0) {
					saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
				}
				saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_START);
				saveString.Append (ifStatements[i].type);
				saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
				saveString.Append (ifStatements[i].targetType);
				saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
				saveString.Append (ifStatements[i].compareValue);
				saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
				saveString.Append (ifStatements [i].targetAString);
				saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
				if (ifStatements [i].compareValue) {
					switch (ifStatements [i].targetType) {
					case DSDataType.Int:
						saveString.Append (ifStatements [i].intInput);
						break;
					case DSDataType.Float:
						saveString.Append (ifStatements [i].floatInput);
						break;
					case DSDataType.Bool:
						saveString.Append (ifStatements [i].boolInput);
						break;
					case DSDataType.String:
						saveString.Append (ifStatements [i].stringInput);
						break;
					}
				} else {
					saveString.Append (ifStatements [i].targetBString);
				}
				saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_END);
			}
			saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_END);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_START);
			for (int i = 0; i < logicOperators.Count; i++) {
				if (i > 0) {
					saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
				}
				saveString.Append (logicOperators[i]);
			}
			saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_END);
			return saveString.ToString ();

		}

	}

}