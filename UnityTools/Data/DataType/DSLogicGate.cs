using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Data.DataType {

	public enum DSLogicGateType {
		AND = 0, OR = 1
	}

	public class DSLogicGate {

		public static bool GetOutput(DSLogicGateType logicOperator, bool input1, bool input2) {

			switch (logicOperator) {
			case DSLogicGateType.AND:
				return input1 && input2;
			case DSLogicGateType.OR:
				return input1 || input2;
			default:
				return true;
			}

		}

	}

}