using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityTools.Attribute {

	/// <summary>
	/// Show If Equal Attribute.
	/// Support limited types only. (You can check it from ShowIfEqualAttribute.PropertyType)
	/// Reference from:
	/// http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
	/// </summary>
	public class ShowIfEqualAttribute : PropertyAttribute {

		public enum PropertyType {
			Bool,
			Int,
			Float,
			String,
			Long,
			Double,
			EnumIndex
		}

		/// <summary>
		/// The name of the referenced variable.
		/// </summary>
		public string variableName;
		/// <summary>
		/// The type of the referenced variable.
		/// </summary>
		public PropertyType type;

		// The expected value of the referenced variable in order to show the variable with this attribute.
		protected bool boolValue;
		protected int intValue;
		protected float floatValue;
		protected string stringValue;
		protected long longValue;
		protected double doubleValue;
		protected int enumValue;

		public ShowIfEqualAttribute(string _name, bool _value) {

			type = PropertyType.Bool;
			variableName = _name;
			boolValue = _value;

		}

		public ShowIfEqualAttribute(string _name, int _value, bool isEnum = false) {

			type = isEnum ? PropertyType.EnumIndex : PropertyType.Int;
			variableName = _name;
			if (isEnum) {
				enumValue = _value;
			} else {
				intValue = _value;
			}

		}

		public ShowIfEqualAttribute(string _name, float _value) {

			type = PropertyType.Float;
			variableName = _name;
			floatValue = _value;

		}

		public ShowIfEqualAttribute(string _name, string _value) {

			type = PropertyType.String;
			variableName = _name;
			stringValue = _value;

		}

		public ShowIfEqualAttribute(string _name, long _value) {

			type = PropertyType.Long;
			variableName = _name;
			longValue = _value;

		}

		public ShowIfEqualAttribute(string _name, double _value) {

			type = PropertyType.Double;
			variableName = _name;
			doubleValue = _value;

		}

		/// <summary>
		/// Check whether the variable should be hide or show.
		/// </summary>
		public bool checkShowProperty(SerializedObject obj) {

			try {
				switch (type) {
				case PropertyType.Bool:
					return (obj.FindProperty (variableName).boolValue == boolValue);
				case PropertyType.Int:
					return (obj.FindProperty (variableName).intValue == intValue);
				case PropertyType.Float:
					return (obj.FindProperty (variableName).floatValue == floatValue);
				case PropertyType.String:
					return (obj.FindProperty (variableName).stringValue.Equals (stringValue));
				case PropertyType.Long:
					return (obj.FindProperty (variableName).longValue == longValue);
				case PropertyType.Double:
					return (obj.FindProperty (variableName).doubleValue == doubleValue);
				case PropertyType.EnumIndex:
					return (obj.FindProperty (variableName).enumValueIndex == enumValue);
				default:
					return true;
				}
			}
			catch(Exception ex) {
				Debug.Log ("[ShowIfEqual] Exception Found: " + ex.Message);
				return true;
			}

		}

	}

}