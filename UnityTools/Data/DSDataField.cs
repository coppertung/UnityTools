#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityTools.Data.DataType;

namespace UnityTools.Data {

	public class DSDataField {

		private List<IDSData> _fields;

		public string name;
		public Vector2 scrollView;	// for gui
		public List<IDSData> fields {
			get {
				if (_fields == null) {
					_fields = new List<IDSData> ();
				}
				return _fields;
			}
		}

		public void addField() {

			if (_fields == null) {
				_fields = new List<IDSData> ();
			}
			DSInt newField = new DSInt ();
			newField.name = "Variable" + _fields.Count;
			newField.type = DSDataType.Int;
			_fields.Add (newField);

		}

		public void removeField(int dataIndex) {

			_fields.RemoveAt (dataIndex);

		}

		public void updateFieldType(int dataIndex, DSDataType type) {

			if (_fields [dataIndex].type != type) {
				IDSData toReplace = _fields [dataIndex];
				_fields.RemoveAt (dataIndex);
				switch (type) {
				case DSDataType.Bool:
					DSBool newBool = new DSBool ();
					newBool.name = toReplace.name;
					newBool.type = DSDataType.Bool;
					_fields.Insert (dataIndex, newBool);
					break;
				case DSDataType.Float:
					DSFloat newFloat = new DSFloat ();
					newFloat.name = toReplace.name;
					newFloat.type = DSDataType.Float;
					_fields.Insert (dataIndex, newFloat);
					break;
				case DSDataType.Int:
					DSInt newInt = new DSInt ();
					newInt.name = toReplace.name;
					newInt.type = DSDataType.Int;
					_fields.Insert (dataIndex, newInt);
					break;
				case DSDataType.String:
					DSString newString = new DSString ();
					newString.name = toReplace.name;
					newString.type = DSDataType.String;
					_fields.Insert (dataIndex, newString);
					break;
				}
			}

		}

		public string save() {

			StringBuilder saveString = new StringBuilder ();
			saveString.Append (name);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_START);
			for (int i = 0; i < _fields.Count; i++) {
				if (i > 0) {
					saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
				}
				saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_START);
				saveString.Append (_fields [i].save ());
				saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_END);
			}
			saveString.Append (DataSimulator.DS_SAVELOAD_CHILD_END);
			return saveString.ToString ();

		}

		public void load(string save) {

			List<string> dataStrings = new List<string> ();
			StringBuilder buffer = new StringBuilder ();
			int level = 0;
			for (int i = 0; i < save.Length; i++) {
				if (level > 0) {
					buffer.Append (save [i]);
				} else {
					if (save [i] == DataSimulator.DS_SAVELOAD_SEPERATOR && buffer.Length > 0) {
						dataStrings.Add (buffer.ToString ());
						buffer.Length = 0;
						buffer.Capacity = 0;
					} else {
						buffer.Append (save [i]);
					}
				}
				if (save [i] == DataSimulator.DS_SAVELOAD_CHILD_END) {
					level -= 1;
					if (level == 0) {
						dataStrings.Add (buffer.ToString ());
						buffer.Length = 0;
						buffer.Capacity = 0;
					}
				}
				if (save [i] == DataSimulator.DS_SAVELOAD_CHILD_START) {
					level += 1;
				} 
			}

			name = dataStrings [0];
			parseDataString (dataStrings [1]);

		}

		public void parseDataString(string save) {

			if (_fields == null) {
				_fields = new List<IDSData> ();
			}
			_fields.Clear ();

			StringBuilder buffer = new StringBuilder ();
			int level = 0;
			for (int i = 0; i < save.Length; i++) {
				if (save [i] == DataSimulator.DS_SAVELOAD_CHILD_END) {
					level -= 1;
					if (level == 1) {
						string bufferString = buffer.ToString ();
						if (bufferString.Contains (DSDataType.Int.ToString())) {
							_fields.Add (new DSInt ());
						}
						else if (bufferString.Contains (DSDataType.Float.ToString())) {
							_fields.Add (new DSFloat ());
						}
						else if (bufferString.Contains (DSDataType.String.ToString())) {
							_fields.Add (new DSString ());
						}
						else if (bufferString.Contains (DSDataType.Bool.ToString())) {
							_fields.Add (new DSBool ());
						}
						_fields [_fields.Count - 1].load (bufferString);
						buffer.Length = 0;
						buffer.Capacity = 0;
					}
				}
				if (level > 1) {
					buffer.Append (save [i]);
				}
				if (save [i] == DataSimulator.DS_SAVELOAD_CHILD_START) {
					level += 1;
				} 
			}

		}

	}

}
#endif