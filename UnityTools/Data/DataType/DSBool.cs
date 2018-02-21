using System.Collections;
using System.Collections.Generic;
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
		#endregion

	}

}
