﻿using System.Collections;
using System.Collections.Generic;
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

	}

}