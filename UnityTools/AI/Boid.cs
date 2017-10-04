using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AI {

	public abstract class Boid : MonoBehaviour, IFixedUpdateable {

		// The update call will be called in prior if the priority is larger.
		public int priority {
			get;
			set;
		}
		public float neighbourRadius;
		public float moveToNeighbourGroupChance = 50;
		public float getOutFromOriginalGroupChance = 5;
		public FlockController flockController;

		void OnEnable() {

			UpdateManager.RegisterFixedUpdate (this);
			flockController.register (this);

		}

		void OnDisable() {

			UpdateManager.UnregisterFixedUpdate (this);
			flockController.unregister (this);

		}

		protected virtual void alignment() {
		}

		protected virtual void cohesion() {
		}

		protected virtual void seperation() {
		}

		public virtual void fixedUpdateEvent() {
			// Used to replace the FixedUpdate().
			// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
		}

	}

}