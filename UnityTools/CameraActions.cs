using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools {

	#region Interfaces
	/// <summary>
	/// Interface of the camera action.
	/// </summary>
	public interface ICameraAction {

		void execute (Camera cam);

	}
	#endregion

	#region Camera_Action_Classes
	/// <summary>
	/// Camera fix aspect ratio action.
	/// Set up the target aspect ratio width and height, then call the execute function.
	/// </summary>
	[System.Serializable]
	public class CameraFixAspectRatio : ICameraAction {

		#region Fields_And_Properties
		/// <summary>
		/// Take action or not?
		/// </summary>
		public bool takeAction;
		/// <summary>
		/// The width of the target aspect ratio.
		/// </summary>
		public int targetAspectRatioWidth;
		/// <summary>
		/// The height of the target aspect ratio.
		/// </summary>
		public int targetAspectRatioHeight;
		#endregion

		#region Functions
		/// <summary>
		/// Function to apply the fix aspect ratio action on the specified camera.
		/// </summary>
		public void execute(Camera cam) {

			if (takeAction) {
				float targetAspectRatio = (float)targetAspectRatioWidth / targetAspectRatioHeight;
				float windowAspect = (float)Screen.width / Screen.height;
				float scaleHeight = windowAspect / targetAspectRatio;

				if (scaleHeight < 1f) {
					Rect rect = cam.rect;
					rect.width = 1f;
					rect.height = scaleHeight;
					rect.x = 0;
					rect.y = (1f - scaleHeight) / 2f;
					cam.rect = rect;
				} else {
					float scaleWidth = 1f / scaleHeight;
					Rect rect = cam.rect;
					rect.width = scaleWidth;
					rect.height = 1f;
					rect.x = (1f - scaleWidth) / 2f;
					rect.y = 0;
					cam.rect = rect;
				}
			}

		}
		#endregion

	}

	/// <summary>
	/// Camera zoom action.
	/// Update the FOV (Field Of View) of the camera, then call the exeute function.
	/// More details about the FOV can be found in:
	/// https://en.wikipedia.org/wiki/Field_of_view
	/// </summary>
	[System.Serializable]
	public class CameraZoom : ICameraAction, IUpdateable {

		#region Fields_And_Properties
		/// <summary>
		/// Is the action be actived?
		/// </summary>
		public bool isActive;
		/// <summary>
		/// The smooth time of the zooming action.
		/// Default is 0.1 second.
		/// </summary>
		public float smoothTime = 0.1f;
		/// <summary>
		/// Maximum field of view of the camera.
		/// </summary>
		[Range(1f, 360f)]
		public float maxFieldOfView;
		/// <summary>
		/// Minimum field of view of the camera.
		/// </summary>
		[Range(1f, 360f)]
		public float minFieldOfView;
		/// <summary>
		/// Current field of view of the camera.
		/// </summary>
		[HideInInspector]
		public float currentFieldOfView {
			get {
				return currentFieldOfView;
			}
			set {
				currentFieldOfView = Mathf.Clamp (value, minFieldOfView, maxFieldOfView);
			}
		}
		private Camera targetCam = null;
		#endregion

		#region Functions
		/// <summary>
		/// Update the field of view of the camera.
		/// Require call execute in order to apply the changes.
		/// </summary>
		public void updateFOV(float fov) {

			currentFieldOfView = fov;

		}

		/// <summary>
		/// Zoom in.
		/// Require call execute in order to apply the changes.
		/// </summary>
		public void zoomIn(float fov) {

			currentFieldOfView -= fov;

		}

		/// <summary>
		/// Zoom out.
		/// Require call execute in order to apply the changes.
		/// </summary>
		public void zoomOut(float fov) {

			currentFieldOfView += fov;

		}

		/// <summary>
		/// Function to apply zooming action on the specified camera.
		/// </summary>
		public void execute(Camera cam) {

			if (isActive) {
				if (currentFieldOfView == 0) {
					currentFieldOfView = cam.fieldOfView;
				}
				targetCam = cam;
				UpdateManager.RegisterUpdate (this);
			}

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
			if (targetCam != null) {
				targetCam.fieldOfView = Mathf.Lerp (targetCam.fieldOfView, currentFieldOfView, Time.deltaTime * smoothTime);
				if (Mathf.Approximately (targetCam.fieldOfView, currentFieldOfView)) {
					targetCam = null;
					UpdateManager.UnregisterUpdate (this);
				}
			}
		}
		#endregion

	}
	#endregion

}
