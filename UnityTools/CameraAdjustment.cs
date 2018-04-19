using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools {

	/// <summary>
	/// This is uesd for attaching into the Camera Component in order to do some adjustment or action on it.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class CameraAdjustment : MonoBehaviour {

		#region Fields_And_Properties
		/// <summary>
		/// Camera fix aspect ratio action. Will be checked and executed at start.
		/// </summary>
		public CameraFixAspectRatio fixAspectRatio;
		/// <summary>
		/// Camera zoom action. Need to update its FOV (Field Of View) before be executed.
		/// </summary>
		public CameraZoom zoom;
		/// <summary>
		/// Camera transition action. Need to assign its destination before be executed.
		/// </summary>
		public CameraTransition transition;
		/// <summary>
		/// Camera shake action. Need to assign its intensity and decay before be executed.
		/// </summary>
		public CameraShake shake;

		/// <summary>
		/// Camera screen effect action. Need to assign its intensity before be activated.
		/// </summary>
		public CameraGreyScale greyScale;
		/// <summary>
		/// Camera screen effect action. Need to assign its intensity before be activated.
		/// </summary>
		public CameraInverseColor inverseColor;

		/// <summary>
		/// The camera.
		/// </summary>
		public Camera cam {
			get;
			protected set;
		}
		#endregion

		#region Unity_Functions
		void Awake() {

			cam = GetComponent<Camera> ();

			// initial the aspect ratio of the game
			fixAspectRatio.execute (cam);
			// initialize zoom action class
			zoom.execute (cam);

		}

		void OnRenderImage(RenderTexture source, RenderTexture destination) {

			greyScale.render (source, destination);
			inverseColor.render (source, destination);

		}
		#endregion

		#region Functions
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
		#endregion

	}

}