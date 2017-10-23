using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AI.FSM {

	/// <summary>
	/// Interface to define the states used in FSM.
	/// </summary>
	public interface IFSMState {

		/// <summary>
		/// Transition list of the state.
		/// </summary>
		List<StateTransition> transitions {
			get;
			set;
		}

		/// <summary>
		/// Called when the FSM enters to this state.
		/// </summary>
		void enterState();

		/// <summary>
		/// Called when the FSM is in this state (update event).
		/// </summary>
		void doAction();

		/// <summary>
		/// Called when the FSM exits from this state.
		/// </summary>
		void exitState();

	}

	/// <summary>
	/// This is the controller of the Finite State Machine, which use IFSMState as its state.
	/// </summary>
	public class FSMController : IUpdateable, IDisposable {

		// The update call will be called in prior if the priority is larger.
		public int priority {
			get;
			set;
		}

		private Dictionary<string, IFSMState> states = null;

		/// <summary>
		/// Integer variables.
		/// </summary>
		public Dictionary<string, int> intVariables = null;
		/// <summary>
		/// Float variables.
		/// </summary>
		public Dictionary<string, float> floatVariables = null;
		/// <summary>
		/// Boolean variables.
		/// </summary>
		public Dictionary<string, bool> boolVariables = null;

		/// <summary>
		/// The default state.
		/// </summary>
		public readonly string defaultState;
		/// <summary>
		/// The current state.
		/// </summary>
		public IFSMState currentState = null;

		/// <summary>
		/// Constructor.
		/// Name of start state or default state must be inputed here.
		/// </summary>
		public FSMController(string startStateName) {

			UpdateManager.RegisterUpdate (this);
			states = new Dictionary<string, IFSMState> ();
			defaultState = startStateName;

		}

		/// <summary>
		/// Releases all resources.
		/// Must be called before the object is deleted or set to null.
		/// </summary>
		public void Dispose() {

			if (UpdateManager.IsRegistered (this)) {
				UpdateManager.UnregisterUpdate (this);
			}

		}

		/// <summary>
		/// Add an integer variable.
		/// </summary>
		public void addVariable(string variableName, int variableValue) {

			if (intVariables == null) {
				intVariables = new Dictionary<string, int> ();
			}
			intVariables.Add (variableName, variableValue);

		}

		/// <summary>
		/// Add a float variable.
		/// </summary>
		public void addVariable(string variableName, float variableValue) {

			if (floatVariables == null) {
				floatVariables = new Dictionary<string, float> ();
			}
			floatVariables.Add (variableName, variableValue);

		}

		/// <summary>
		/// Add a boolean variable.
		/// </summary>
		public void addVariable(string variableName, bool variableValue) {

			if (boolVariables == null) {
				boolVariables = new Dictionary<string, bool> ();
			}
			boolVariables.Add (variableName, variableValue);

		}

		/// <summary>
		/// Set an integer variable.
		/// </summary>
		public void setVariable(string variableName, int variableValue) {

			if (intVariables == null || !intVariables.ContainsKey (variableName)) {
				throw new KeyNotFoundException (variableName + " not found!");
			}
			intVariables [variableName] = variableValue;

		}

		/// <summary>
		/// Set a float variable.
		/// </summary>
		public void setVariable(string variableName, float variableValue) {

			if (floatVariables == null || !floatVariables.ContainsKey (variableName)) {
				throw new KeyNotFoundException (variableName + " not found!");
			}
			floatVariables [variableName] = variableValue;

		}

		/// <summary>
		/// Set a boolean variable.
		/// </summary>
		public void setVariable(string variableName, bool variableValue) {

			if (boolVariables == null || !boolVariables.ContainsKey (variableName)) {
				throw new KeyNotFoundException (variableName + " not found!");
			}
			boolVariables [variableName] = variableValue;

		}

		/// <summary>
		/// Remove variable.
		/// </summary>
		public void removeVariable(string variableName, bool isInt, bool isFloat, bool isBool) {

			if (isInt) {
				if (floatVariables == null || !boolVariables.ContainsKey (variableName)) {
					throw new KeyNotFoundException (variableName + " not found!");
				}
				intVariables.Remove (variableName);
			}
			if (isFloat) {
				if (floatVariables == null || !boolVariables.ContainsKey (variableName)) {
					throw new KeyNotFoundException (variableName + " not found!");
				}
				floatVariables.Remove (variableName);
			}
			if (isBool) {
				if (boolVariables == null || !boolVariables.ContainsKey (variableName)) {
					throw new KeyNotFoundException (variableName + " not found!");
				}
				boolVariables.Remove (variableName);
			}

		}

		/// <summary>
		/// Clear one or more specific type of variable(s).
		/// </summary>
		public void clearVariables(bool clearInt, bool clearFloat, bool clearBool) {

			if (clearInt && intVariables != null) {
				intVariables.Clear ();
			}
			if (clearFloat && floatVariables != null) {
				floatVariables.Clear ();
			}
			if (clearBool && boolVariables != null) {
				boolVariables.Clear ();
			}

		}

		/// <summary>
		/// Clear all variable(s).
		/// </summary>
		public void clearAllVariables() {

			intVariables.Clear ();
			floatVariables.Clear ();
			boolVariables.Clear ();

		}

		/// <summary>
		/// Add a new state.
		/// </summary>
		public void addState(string stateName, IFSMState state) {

			if (states == null) {
				states = new Dictionary<string, IFSMState> ();
			}
			states.Add (stateName, state);

		}

		/// <summary>
		/// Remove a state.
		/// </summary>
		public void removeState(string stateName) {

			if (states.ContainsKey (stateName)) {
				states.Remove (stateName);
			}

		}

		/// <summary>
		/// Clear all state.
		/// </summary>
		public void clearStates() {

			states.Clear ();

		}

		/// <summary>
		/// Switch to the specified state.
		/// </summary>
		public void switchState(string state) {

			if (currentState != null) {
				currentState.exitState ();
			}
			if (states.ContainsKey (state)) {
				currentState = states [state];
				currentState.enterState ();
			} else {
				throw new KeyNotFoundException (state + " not found!");
			}

		}

		public void updateEvent() {
			// Used to replace the FixedUpdate().
			// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
			if (currentState == null) {
				switchState (defaultState);
			}
			currentState.doAction ();
			for (int i = 0; i < currentState.transitions.Count; i++) {
				if (currentState.transitions [i].checkCondition (this)) {
					switchState (currentState.transitions [i].nextState);
					break;
				}
			}

		}

	}

}