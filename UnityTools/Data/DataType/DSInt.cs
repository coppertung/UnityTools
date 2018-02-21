using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Data.DataType {

	public class DSInt : IDSNumericData {

		public int value;
		public int minValue;
		public int maxValue;

		#region IDSData
		public DSDataType type {
			get;
			set;
		}

		public string name {
			get;
			set;
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