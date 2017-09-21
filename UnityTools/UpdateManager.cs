using System;
using System.Collections.Generic;
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
    /// This is the interface to use for register the Update Manager.
    /// FixedUpdate calls must implemented in fixedUpdateEvent().
    /// </summary>
    public interface IFixedUpdateable
    {

        /// <summary>
        /// The update call will be called in prior if the priority is larger.
        /// </summary>
        int priority
        {
            get;
            set;
        }

        /// <summary>
        /// Used to replace the FixedUpdate().
        /// Noted that it will be automatically called by the Update Manager once it is registered.
        /// </summary>
        void fixedUpdateEvent();

    }

    /// <summary>
    /// This is the interface to use for register the Update Manager.
    /// LateUpdate calls must implemented in lateUpdateEvent().
    /// </summary>
    public interface ILateUpdateable
    {

        /// <summary>
        /// The update call will be called in prior if the priority is larger.
        /// </summary>
        int priority
        {
            get;
            set;
        }

        /// <summary>
        /// Used to replace the LateUpdate().
        /// Noted that it will be automatically called by the Update Manager once it is registered.
        /// </summary>
        void lateUpdateEvent();

    }

    /// <summary>
    /// Update Manager is used to manage the update calls.
    /// Must be attached to gameobject that won't be destroy.
    /// However, it is not necessary to use the Update Manager if there is a small amount of gameobjects and update calls only.
    /// Recommended to use when there is over **1k (need to test)** update calls in each frame.
    /// </summary>
    public class UpdateManager : Singleton<UpdateManager> {

		private static List<IUpdateable> _updateablesList = new List<IUpdateable>();
        private static List<IFixedUpdateable> _fixedUpdateablesList = new List<IFixedUpdateable>();
        private static List<ILateUpdateable> _lateUpdateablesList = new List<ILateUpdateable>();
        /// <summary>
        /// List which stored all the registered updateables.
        /// </summary>
        public static List<IUpdateable> updateablesList {
			get {
				return _updateablesList;
			}
        }
        /// <summary>
        /// List which stored all the registered fixedUpdateables.
        /// </summary>
        public static List<IFixedUpdateable> fixedUpdateablesList
        {
            get
            {
                return _fixedUpdateablesList;
            }
        }
        /// <summary>
        /// List which stored all the registered lateUpdateables.
        /// </summary>
        public static List<ILateUpdateable> lateUpdateablesList
        {
            get
            {
                return _lateUpdateablesList;
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

        void FixedUpdate()
        {

            if (_fixedUpdateablesList.Count > 0)
            {
                for (int i = 0; i < _fixedUpdateablesList.Count; i++)
                {
                    if (_fixedUpdateablesList[i] != null)
                    {
                        _fixedUpdateablesList[i].fixedUpdateEvent();
                    }
                }
            }

        }

        void LateUpdate()
        {

            if (_lateUpdateablesList.Count > 0)
            {
                for (int i = 0; i < _lateUpdateablesList.Count; i++)
                {
                    if (_lateUpdateablesList[i] != null)
                    {
                        _lateUpdateablesList[i].lateUpdateEvent();
                    }
                }
            }

        }

        /// <summary>
        /// Register the specified Update Call.
        /// Can invoke the sort by setting the autoSort as true (default is false).
        /// </summary>
        public static void RegisterUpdate(IUpdateable updateable, bool autoSort = false) {

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
		public static void UnregisterUpdate(IUpdateable updateable) {

			if (updateable == null) {
				throw new ArgumentNullException ();
			} else {
				_updateablesList.Remove (updateable);
			}

		}

        /// <summary>
        /// Register the specified FixedUpdate Call.
        /// Can invoke the sort by setting the autoSort as true (default is false).
        /// </summary>
        public static void RegisterFixedUpdate(IFixedUpdateable fixedUpdateable, bool autoSort = false)
        {

            if (fixedUpdateable == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                _fixedUpdateablesList.Add(fixedUpdateable);
            }
            if (autoSort)
                Sort();

        }

        /// <summary>
        /// Unregister the specified FixedUpdate Call.
        /// </summary>
        public static void UnregisterFixedUpdate(IFixedUpdateable fixedUpdateable)
        {

            if (fixedUpdateable == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                _fixedUpdateablesList.Remove(fixedUpdateable);
            }

        }

        /// <summary>
        /// Register the specified LateUpdate Call.
        /// Can invoke the sort by setting the autoSort as true (default is false).
        /// </summary>
        public static void RegisterLateUpdate(ILateUpdateable lateUpdateable, bool autoSort = false)
        {

            if (lateUpdateable == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                _lateUpdateablesList.Add(lateUpdateable);
            }
            if (autoSort)
                Sort();

        }

        /// <summary>
        /// Unregister the specified LateUpdate Call.
        /// </summary>
        public static void UnregisterLateUpdate(ILateUpdateable lateUpdateable)
        {

            if (lateUpdateable == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                _lateUpdateablesList.Remove(lateUpdateable);
            }

        }

		/// <summary>
		/// Determine if the specified updateable is being registered.
		/// </summary>
		public static bool IsRegistered(IUpdateable updateable) {

			for (int i = 0; i < _updateablesList.Count; i++) {
				if (updateable == _updateablesList [i]) {
					return true;
				}
			}
			return false;

		}

		/// <summary>
		/// Determine if the specified fixedUpdateable is being registered.
		/// </summary>
		public static bool IsRegistered(IFixedUpdateable fixedUpdateable) {

			for (int i = 0; i < _fixedUpdateablesList.Count; i++) {
				if (fixedUpdateable == _fixedUpdateablesList [i]) {
					return true;
				}
			}
			return false;

		}

		/// <summary>
		/// Determine if the specified lateUpdateable is being registered.
		/// </summary>
		public static bool IsRegistered(ILateUpdateable lateUpdateable) {

			for (int i = 0; i < _lateUpdateablesList.Count; i++) {
				if (lateUpdateable == _lateUpdateablesList [i]) {
					return true;
				}
			}
			return false;

		}

        /// <summary>
        /// Sort the lists, where depends on the priority property that the update call will be called in prior if it got a larger priority.
        /// </summary>
        public static void Sort() {

            if (_updateablesList.Count > 0) {
                _updateablesList.Sort(delegate (IUpdateable x, IUpdateable y)
                {
                    if (x.priority > y.priority)
                        return 1;   // move forwards
                    else if (x.priority == y.priority)
                        return 0;   // stay unchange
                    else
                        return -1;  // move backwards
                });
            }
            if (_fixedUpdateablesList.Count > 0) {
                _fixedUpdateablesList.Sort(delegate (IFixedUpdateable x, IFixedUpdateable y)
                {
                    if (x.priority > y.priority)
                        return 1;   // move forwards
                    else if (x.priority == y.priority)
                        return 0;   // stay unchange
                    else
                        return -1;  // move backwards
                });
            }
            if (_lateUpdateablesList.Count > 0)
            {
                _lateUpdateablesList.Sort(delegate (ILateUpdateable x, ILateUpdateable y)
                {
                    if (x.priority > y.priority)
                        return 1;   // move forwards
                    else if (x.priority == y.priority)
                        return 0;   // stay unchange
                    else
                        return -1;  // move backwards
                });
            }

        }

	}

}