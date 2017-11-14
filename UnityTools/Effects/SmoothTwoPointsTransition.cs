using UnityEngine;

namespace UnityTools.Effects {

	[System.Serializable]
	public class SmoothTwoPointsTransition {

		/// <summary>
		/// Transition type.
		/// </summary>
		public enum TransitionType {
			AtoB = 0, BtoA = 1
		}

		/// <summary>
		/// The target of the transition.
		/// </summary>
		public GameObject target;
		/// <summary>
		/// Position A.
		/// </summary>
		public Vector3 positionA;
		/// <summary>
		/// Position B.
		/// </summary>
		public Vector3 positionB;
		/// <summary>
		/// Movement time, a.k.a. smooth time, of the transition.
		/// </summary>
		public float moveTime;
		/// <summary>
		/// Acceptable error distance of the transition.
		/// </summary>
		public float errorDistance;

		private Vector3 velocity;

		/// <summary>
		/// Transit the target, must be called in update event.
		/// </summary>
		public void transit(TransitionType type) {

			switch (type) {
			case TransitionType.AtoB:
				if (Vector3.Distance (target.transform.localPosition, positionB) >= errorDistance) {
					target.transform.localPosition = Vector3.SmoothDamp (target.transform.localPosition, positionB, ref velocity, moveTime);
				}
				break;
			case TransitionType.BtoA:
				if (Vector3.Distance (target.transform.localPosition, positionA) >= errorDistance) {
					target.transform.localPosition = Vector3.SmoothDamp (target.transform.localPosition, positionA, ref velocity, moveTime);
				}
				break;
			}

		}

	}

}