using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Data.DataType {

	public class DSString : IDSData {

		public string value;

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