using System;
using System.Reflection;
using UnityEngine;

namespace UnityTools.Network {

	/// <summary>
	/// Json parser, which used to do parsing between objects and json strings.
	/// Support the OtherNameInJson attribute, which allows users to rename the variable other than following the json strings only
	/// in order to provide more flexibility to the users.
	/// However, if different classes with same variable name exists in one json string will have wrong parsed result.
	/// </summary>
	public class JsonParser : MonoBehaviour {

		/// <summary>
		/// Parse the object to json string.
		/// </summary>
		public static string ToJson(System.Object obj) {

			string jsonString = JsonUtility.ToJson (obj);
			jsonString = replaceVariableName (obj.GetType (), jsonString);
			Debug.Log ("To Json: " + jsonString);
			return jsonString;

		}

		/// <summary>
		/// Parse the json string to object.
		/// </summary>
		public static T FromJson<T>(string jsonString) {

			string newString = replaceVariableName<T> (jsonString);
			T obj = JsonUtility.FromJson<T> (newString);
			Debug.Log ("From Json: " + newString);
			return obj;

		}

		private static string replaceVariableName<T>(string jsonString) {

			string newString = jsonString;
			FieldInfo[] objFields = typeof(T).GetFields (BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < objFields.Length; i++) {
				if (!(objFields [i].FieldType.IsPrimitive || objFields [i].FieldType == typeof(String))) {
					Debug.Log ("Object Found " + objFields [i].Name);
					newString = replaceVariableName (objFields [i].FieldType, newString);
				} else {
					OtherNameInJson attribute = Attribute.GetCustomAttribute (objFields [i], typeof(OtherNameInJson)) as OtherNameInJson;
					Debug.Log ("Attribute Try To Found From " + objFields [i].Name);
					if (attribute != null) {
						Debug.Log ("Attribute Found From " + objFields [i].Name);
						newString = newString.Replace (attribute.originalName, objFields [i].Name);
					}
				}
			}

			return newString;

		}

		private static string replaceVariableName(System.Type type, string jsonString) {

			string newString = jsonString;
			FieldInfo[] objFields = type.GetFields (BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < objFields.Length; i++) {
				if (!(objFields [i].FieldType.IsPrimitive || objFields [i].FieldType == typeof(String))) {
					Debug.Log ("Object Found " + objFields [i].Name);
					newString = replaceVariableName (objFields [i].FieldType, newString);
				} else {
					OtherNameInJson attribute = Attribute.GetCustomAttribute (objFields [i], typeof(OtherNameInJson)) as OtherNameInJson;
					Debug.Log ("Attribute Try To Found From " + objFields [i].Name);
					if (attribute != null) {
						Debug.Log ("Attribute Found From " + objFields [i].Name);
						newString = newString.Replace (attribute.originalName, objFields [i].Name);
					}
				}
			}

			return newString;

		}

	}

}