using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityTools.Data.DataType {

	public class DSFloat : IDSNumericData {

		public float value;
		public float minValue;
		public float maxValue;

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
			saveString.Append (isRandom);
			saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
			if (isRandom) {
				saveString.Append (minValue);
				saveString.Append (DataSimulator.DS_SAVELOAD_SEPERATOR);
				saveString.Append (maxValue);
			} else {
				saveString.Append (value);
			}
			return saveString.ToString ();

		}

		public void load(string saveString) {
		}
		#endregion

		#region IDSNumericData
		public bool isRandom {
			get;
			set;
		}

		public void genRandomValue () {

			value = Random.Range (minValue - 1, maxValue) + 1;

		}
		#endregion

	}

}