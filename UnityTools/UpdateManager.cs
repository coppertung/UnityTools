using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools.Patterns;

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
		/// Noted that it will be automatically called by the Update Manager once it is registered.
		/// </summary>
		void updateEvent();

	}

	/// <summary>
	/// Update Manager is used to manage the update calls.
	/// Must be attached to gameobject that won't be destroy.
	/// However, it is not necessary to use the Update Manager if there is a small amount of gameobjects and update calls only.
	/// Recommended to use when there is over **1k (need to test)** update calls in each frame.
	/// </summary>
	public class UpdateManager : Singleton<UpdateManager> {

		private static List<IUpdateable> _updateablesList = new List<IUpdateable>();
		/// <summary>
		/// List which stored all the registered updateables.
		/// </summary>
		public static List<IUpdateable> updateablesList {
			get {
				return _updateablesList;
			}
		}

		void Update () {

			if (_updateablesList.Count > 0) {
				for (int i = 0; i < _updateablesList.Count; i++) {
					if (_updateablesList [i] != null) {
						_updateablesList [i].updateEvent ();
					}
				}
			}

		}

		/// <summary>
		/// Register the specified Update Call.
		/// Can invoke the sort by setting the autoSort as true (default is false).
		/// </summary>
		public static void Register(IUpdateable updateable, bool autoSort = false) {

			if (updateable == null) {
				throw new ArgumentNullException ();
			} else {
				_updateablesList.Add (updateable);
			}
			if (autoSort)
				Sort ();

		}

		/// <summary>
		/// Unregister the specified Update Call.
		/// </summary>
		public static void Unregister(IUpdateable updateable) {

			if (updateable == null) {
				throw new ArgumentNullException ();
			} else {
				_updateablesList.Remove (updateable);
			}

		}

		/// <summary>
		/// Sort the Update Call list, where depends on the priority property that the update call will be called in prior if it got a larger priority.
		/// </summary>
		public static void Sort() {

			_updateablesList.Sort (delegate(IUpdateable x, IUpdateable y) {
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