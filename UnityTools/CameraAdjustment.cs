using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools {

	[RequireComponent(typeof(Camera))]
	public class CameraAdjustment : MonoBehaviour {

		public int targetAspectRatioWidth;
		public int targetAspectRatioHeight;

		private float targetAspectRatio;
		private float windowAspect;
		private float scaleHeight;

		Camera cam;

		void Awake() {

			targetAspectRatio = (float)targetAspectRatioWidth / targetAspectRatioHeight;
			windowAspect = (float)Screen.width / Screen.height;
			scaleHeight = windowAspect / targetAspectRatio;
			cam = GetComponent<Camera> ();

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

}