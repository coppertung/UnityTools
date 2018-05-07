#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Attribute {

	/// <summary>
	/// Show If Equal Attribute Property Drawer.
	/// Reference from:
	/// http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
	/// </summary>
	[CustomPropertyDrawer(typeof(ShowIfEqualAttribute))]
	public class ShowIfEqualDrawer : PropertyDrawer {

		private ShowIfEqualAttribute _showIfEqualAttribute;

		public ShowIfEqualAttribute showIfEqualAttribute {
			get {
				if (_showIfEqualAttribute == null) {
					_showIfEqualAttribute = (ShowIfEqualAttribute)attribute;
				}
				return _showIfEqualAttribute;
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			
			bool enabled = showIfEqualAttribute.checkShowProperty (property.serializedObject);

			if (enabled) {
				return EditorGUI.GetPropertyHeight (property, label, true);
			} else {
				return -EditorGUIUtility.standardVerticalSpacing;
			}

		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			
			bool enabled = showIfEqualAttribute.checkShowProperty (property.serializedObject);

			bool wasEnabled = GUI.enabled;
			GUI.enabled = true;
			if (enabled) {
				EditorGUI.PropertyField (position, property, label, true);
			}
			GUI.enabled = wasEnabled;

		}

	}

}
#endif