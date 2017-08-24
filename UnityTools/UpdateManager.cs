﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools {

	/// <summary>
	/// This is the interface to use for register the Update Manager.
	/// Update calls must implemented in updateEvent().
	/// </summary>
	public interface IUpdateable {

		/// <summary>
		/// The update call will be called in prior if the priority is larger.
		/// </summary>
		int priority {
			get;
			set;
		}

		/// <summary>
		/// Used to replace the Update().
		/// Noted that it will be automatically called by the Update Manager once it registered.
		/// </summary>
		void updateEvent();

	}

	/// <summary>
	/// Update Manager is used to manage the update calls.
	/// However, it is not necessary to use the Update Manager if there is a small amount of gameobjects and update calls only.
	/// Recommended to use when there is over **1k (need to test)** update calls in each frame.
	/// **REQUIRE TESTING LATER**
	/// </summary>
	public class UpdateManager : MonoBehaviour {

		private static UpdateManager _instance;
		public static UpdateManager Instance {
			get {
				return _instance;
			}
		}

		private static List<IUpdateable> updateCallsList = new List<IUpdateable>();


		void Awake() {

			if (_instance == null) {
				_instance = this;
			} else {
				throw new Exception ("There should not be more than one Update Manager!");
			}

		}

		void Update () {

			if (updateCallsList.Count > 0) {
				for (int i = 0; i < updateCallsList.Count; i++) {
					updateCallsList [i].updateEvent ();
				}
			}

		}

		/// <summary>
		/// Register the specified Update Call.
		/// Can invoke the sort by setting the autoSort as true.
		/// </summary>
		public static void Register(IUpdateable updateCall, bool autoSort = false) {

			if (updateCall == null) {
				throw new ArgumentNullException ();
			} else {
				updateCallsList.Add (updateCall);
			}
			if (autoSort)
				Sort ();

		}

		/// <summary>
		/// Unregister the specified Update Call.
		/// </summary>
		public static void Unregister(IUpdateable updateCall) {

			if (updateCall == null) {
				throw new ArgumentNullException ();
			} else {
				updateCallsList.Remove (updateCall);
			}

		}

		/// <summary>
		/// Sort the Update Call list, where depends on the priority property that the update call will be called in prior if it got a larger priority.
		/// </summary>
		public static void Sort() {

			updateCallsList.Sort (delegate(IUpdateable x, IUpdateable y) {
				if (x.priority > y.priority)
					return 1;	// move forwards
				else if (x.priority == y.priority)
					return 0;	// stay unchange
				else
					return -1;	// move backwards
			});

		}

	}

}