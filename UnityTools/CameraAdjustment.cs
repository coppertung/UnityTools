using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools {

	/// <summary>
	/// This is uesd for attaching into the Camera Component in order to do some adjustment or action on it.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class CameraAdjustment : MonoBehaviour {

		/// <summary>
		/// Camera fix aspect ratio action. Will be checked and executed at start.
		/// </summary>
		public CameraFixAspectRatio fixAspectRatio;
		/// <summary>
		/// The camera.
		/// </summary>
		private Camera cam;

		void Awake() {

			cam = GetComponent<Camera> ();

			if (fixAspectRatio.takeAction) {
				fixAspectRatio.execute (cam);
			}

		}

		/// <summary>
		/// Invokes the action after t seconds.
		/// </summary>
		public IEnumerator invokeAction(ICameraAction action, float t = 0) {

			if (t > 0) {
				yield return new WaitForSeconds (t);
			}
			action.execute (cam);
			yield return null;

		}

	}

}