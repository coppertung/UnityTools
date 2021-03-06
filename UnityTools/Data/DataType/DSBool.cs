﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityTools.Data.DataType {

	public class DSBool : IDSData {

		public bool value;

		#region IDSData
		public DSDataType type {
			get;
			set;
		}

		public string name {
			get;
			set;
		}

		public string save () {

			StringBuilder saveString = new StringBuilder ();
			saveString.Append (name);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (type);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			saveString.Append (value);
			return saveString.ToString ();

		}

		public void load(string save) {

			string[] saveStrings = save.Split (DataSimulator.DS_SAVELOAD_SEPERATOR);
			name = saveStrings [0];
			type = DSDataType.Bool;
			value = bool.Parse (saveStrings [2]);

		}
		#endregion

		public override string ToString () {

			return value.ToString ();

		}

	}

}
#endif