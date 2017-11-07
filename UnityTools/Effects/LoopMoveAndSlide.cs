using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Effects {

	public class LoopMoveAndSlide : MonoBehaviour, IUpdateable {

		#region Fields_And_Properties
		/// <summary>
		/// Loop objects.
		/// </summary>
		public GameObject[] loopObjects;
		/// <summary>
		/// Automatically looping movement action.
		/// </summary>
		public LoopMove loopMoveAction;
		/// <summary>
		/// Manually sliding movement action.
		/// </summary>
		public SlideMove slideMoveAction;
		#endregion

		#region MonoBehaviour
		void Awake() {

			// initialization
			loopMoveAction.init (this);

		}

		void OnEnable() {

			UpdateManager.RegisterUpdate (this);

		}

		void OnDisable() {

			UpdateManager.UnregisterUpdate (this);

		}
		#endregion

		#region IUpdateable
		// The update call will be called in prior if the priority is larger.
		public int priority {
			get;
			set;
		}

		public void updateEvent() {
			// Used to replace the Update().
			// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
			loopMoveAction.updatePosition();

		}
		#endregion

	}

}