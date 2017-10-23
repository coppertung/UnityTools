using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AI.FSM {

	public class StateTransition {

		private Dictionary<string, int> intConditions = null;
		private Dictionary<string, float> floatConditions = null;
		private Dictionary<string, bool> boolConditions = null;

		/// <summary>
		/// The name of next state.
		/// </summary>
		public string nextState = null;

		/// <summary>
		/// Add an integer condition.
		/// </summary>
		public void addCondition(string variableName, int variableValue) {

			if (intConditions == null) {
				intConditions = new Dictionary<string, int> ();
			}
			intConditions.Add (variableName, variableValue);

		}

		/// <summary>
		/// Add a float condition.
		/// </summary>
		public void addCondition(string variableName, float variableValue) {

			if (floatConditions == null) {
				floatConditions = new Dictionary<string, float> ();
			}
			floatConditions.Add (variableName, variableValue);

		}

		/// <summary>
		/// Add a boolean condition.
		/// </summary>
		public void addCondition(string variableName, bool variableValue) {

			if (boolConditions == null) {
				boolConditions = new Dictionary<string, bool> ();
			}
			boolConditions.Add (variableName, variableValue);

		}

		/// <summary>
		/// Set an integer condition.
		/// </summary>
		public void setCondition(string variableName, int variableValue) {

			if (intConditions == null || !intConditions.ContainsKey (variableName)) {
				throw new KeyNotFoundException (variableName + " not found!");
			}
			intConditions [variableName] = variableValue;

		}

		/// <summary>
		/// Set a boolean condition.
		/// </summary>
		public void setCondition(string variableName, float variableValue) {

			if (floatConditions == null || !floatConditions.ContainsKey (variableName)) {
				throw new KeyNotFoundException (variableName + " not found!");
			}
			floatConditions [variableName] = variableValue;

		}

		/// <summary>
		/// Set a boolean condition.
		/// </summary>
		public void setCondition(string variableName, bool variableValue) {

			if (boolConditions == null || !boolConditions.ContainsKey (variableName)) {
				throw new KeyNotFoundException (variableName + " not found!");
			}
			boolConditions [variableName] = variableValue;

		}

		/// <summary>
		/// Remove condition.
		/// </summary>
		public void removeCondition(string variableName, bool isInt, bool isFloat, bool isBool) {

			if (isInt) {
				if (intConditions == null || !intConditions.ContainsKey (variableName)) {
					throw new KeyNotFoundException (variableName + " not found!");
				}
				intConditions.Remove (variableName);
			}
			if (isFloat) {
				if (floatConditions == null || !floatConditions.ContainsKey (variableName)) {
					throw new KeyNotFoundException (variableName + " not found!");
				}
				floatConditions.Remove (variableName);
			}
			if (isBool) {
				if (boolConditions == null || !boolConditions.ContainsKey (variableName)) {
					throw new KeyNotFoundException (variableName + " not found!");
				}
				boolConditions.Remove (variableName);
			}

		}

		/// <summary>
		/// Clear one or more specific type of condition(s).
		/// </summary>
		public void clearConditions(bool clearInt, bool clearFloat, bool clearBool) {

			if (clearInt && intConditions != null) {
				intConditions.Clear ();
			}
			if (clearFloat && floatConditions != null) {
				floatConditions.Clear ();
			}
			if (clearBool && boolConditions != null) {
				boolConditions.Clear ();
			}

		}

		/// <summary>
		/// Clear all condition(s).
		/// </summary>
		public void clearAllConditions() {

			intConditions.Clear ();
			floatConditions.Clear ();
			boolConditions.Clear ();

		}

		/// <summary>
		/// Check whether the condition(s) is/are fulfilled or not.
		/// </summary>
		public bool checkCondition(FSMController controller) {

			if (intConditions != null) {
				foreach (string key in intConditions.Keys) {
					if (controller.intVariables [key] != intConditions [key]) {
						return false;
					}
				}
			}
			if (floatConditions != null) {
				foreach (string key in floatConditions.Keys) {
					if (controller.floatVariables [key] != floatConditions [key]) {
						return false;
					}
				}
			}
			if (boolConditions != null) {
				foreach (string key in boolConditions.Keys) {
					if (controller.boolVariables [key] != boolConditions [key]) {
						return false;
					}
				}
			}
			return true;

		}

	}

}