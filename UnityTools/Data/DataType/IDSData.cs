using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Data.DataType {

	public enum DSDataType {
		Bool = 0, Int = 1, Float = 2, String = 3
	}

	public interface IDSData {

		DSDataType type {
			get;
			set;
		}
		string name {
			get;
			set;
		}

		string save ();
		void load(string saveString);

	}

	public interface IDSNumericData : IDSData {
		
		bool isRandom {
			get;
			set;
		}

		void genRandomValue ();

	}

}