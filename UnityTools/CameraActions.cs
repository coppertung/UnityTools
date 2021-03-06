﻿using System.Collections;
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

		public enum AspectRatioMatch {
			Width, Height, None
		}

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
		/// <summary>
		/// What did the aspect ratio match?
		/// </summary>
		public AspectRatioMatch match = AspectRatioMatch.None;
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
					match = AspectRatioMatch.Width;
				} else {
					float scaleWidth = 1f / scaleHeight;
					Rect rect = cam.rect;
					rect.width = scaleWidth;
					rect.height = 1f;
					rect.x = (1f - scaleWidth) / 2f;
					rect.y = 0;
					cam.rect = rect;
					match = AspectRatioMatch.Height;
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
		/// Is the action be activated?
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
		public float maxFieldOfView = 150;
		/// <summary>
		/// Minimum field of view of the camera.
		/// </summary>
		[Range(1f, 360f)]
		public float minFieldOfView = 20;
		private float _currentFieldOfView;
		/// <summary>
		/// Current field of view of the camera.
		/// </summary>
		public float currentFieldOfView {
			get {
				return _currentFieldOfView;
			}
			set {
				_currentFieldOfView = Mathf.Clamp (value, minFieldOfView, maxFieldOfView);
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
					targetCam.fieldOfView = currentFieldOfView;
					targetCam = null;
					UpdateManager.UnregisterUpdate (this);
				}
			}
		}
		#endregion

	}

	/// <summary>
	/// Camera transition action.
	/// Set up the destination, then call the execute function.
	/// </summary>	
	[System.Serializable]
	public class CameraTransition : ICameraAction, IUpdateable {

		#region Fields_And_Properties
		/// <summary>
		/// Is the action be activated?
		/// </summary>
		public bool isActive;
		/// <summary>
		/// The smooth time of the transition action.
		/// Default is 0.1 second.
		/// </summary>
		public float smoothTime = 0.1f;
		/// <summary>
		/// The error distance of the transition action.
		/// Default is 0.1 unit.
		/// </summary>
		public float errorDistance = 0.1f;
		/// <summary>
		/// The destination of the transition action.
		/// </summary>
		public Vector3 destination {
			get;
			protected set;
		}

		private Vector3 refSpeed = Vector3.one;
		private Camera targetCam = null;
		#endregion

		#region Functions
		/// <summary>
		/// Set the destination of the transition action.
		/// However, execute function must be called in order to apply transition action.
		/// </summary>
		public void setDestination(Vector3 position) {

			destination = position;

		}

		/// <summary>
		/// Function to apply transition action on the specified camera.
		/// </summary>
		public void execute(Camera cam) {

			if (isActive) {
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
				targetCam.transform.position = Vector3.SmoothDamp (targetCam.transform.position, destination, ref refSpeed, smoothTime);
				if (Vector3.Distance(targetCam.transform.position, destination) <= errorDistance) {
					targetCam.transform.position = destination;
					targetCam = null;
					UpdateManager.UnregisterUpdate (this);
				}
			}
		}
		#endregion

	}

	/// <summary>
	/// Camera shaking action.
	/// Set up the intensity and decay, then call the execute function.
	/// Reference from:
	/// http://www.mikedoesweb.com/2012/camera-shake-in-unity/
	/// </summary>	
	[System.Serializable]
	public class CameraShake : ICameraAction, IUpdateable {
		
		#region Fields_And_Properties
		/// <summary>
		/// Is the action be activated?
		/// </summary>
		public bool isActive;
		/// <summary>
		/// Intensity of the shaking force.
		/// Suggested as 0.1.
		/// </summary>
		public float intensity {
			get;
			protected set;
		}
		/// <summary>
		/// Decay of the shaking force.
		/// Suggested as 0.02.
		/// </summary>
		public float decay {
			get;
			protected set;
		}
		/// <summary>
		/// Original position of the target camera.
		/// </summary>
		public Vector3 originalPosition;
		/// <summary>
		/// Original rotation of the target camera.
		/// </summary>
		public Quaternion originalRotation;
		protected bool isShaking;
		protected float currentIntensity;
		protected Camera targetCamera;
		#endregion

		#region Functions
		/// <summary>
		/// Set the intensity of the shaking force.
		/// </summary>
		public void setIntensity(float _intensity) {

			intensity = _intensity;

		}

		/// <summary>
		/// Set the decay of the shaking force.
		/// </summary>
		public void setDecay(float _decay) {

			decay = _decay;

		}

		/// <summary>
		/// Function to apply shaking action on the specified camera.
		/// If this function being call before the end of last shaking action,
		/// it may stop the last shaking action immediately or adding intensity to the last shaking action depends on the target of new shaking action.
		/// </summary>
		public void execute(Camera cam) {

			if (isActive) {
				if (targetCamera != null && targetCamera != cam) {
					targetCamera.transform.localPosition = originalPosition;
					targetCamera.transform.localRotation = originalRotation;
					isShaking = false;
				}
				if (!isShaking) {
					targetCamera = cam;
					originalPosition = cam.transform.localPosition;
					originalRotation = cam.transform.localRotation;
				}
				isShaking = true;
				currentIntensity += intensity;
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
			if (!isShaking) {
				UpdateManager.UnregisterUpdate (this);
			} else {
				if (currentIntensity > 0) {
					targetCamera.transform.localPosition = originalPosition + UnityEngine.Random.insideUnitSphere * currentIntensity;
					targetCamera.transform.localRotation = new Quaternion (
						originalRotation.x + UnityEngine.Random.Range (-currentIntensity, currentIntensity) * 0.1f,
						originalRotation.y + UnityEngine.Random.Range (-currentIntensity, currentIntensity) * 0.1f,
						originalRotation.z + UnityEngine.Random.Range (-currentIntensity, currentIntensity) * 0.1f,
						originalRotation.w + UnityEngine.Random.Range (-currentIntensity, currentIntensity) * 0.1f
					);
					currentIntensity -= decay;
				} else {
					currentIntensity = 0;
					isShaking = false;
					targetCamera.transform.localPosition = originalPosition;
					targetCamera.transform.localRotation = originalRotation;
				}
			}

		}
		#endregion

	}
	#endregion

}
