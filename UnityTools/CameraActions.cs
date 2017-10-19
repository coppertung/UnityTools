using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools {

	/// <summary>
	/// Interface of the camera action.
	/// </summary>
	public interface ICameraAction {

		void execute (Camera cam);

	}

	/// <summary>
	/// Camera fix aspect ratio action.
	/// Set up the target aspect ratio width and height, then call the execute function.
	/// </summary>
	[System.Serializable]
	public class CameraFixAspectRatio : ICameraAction {

		public bool takeAction;
		public int targetAspectRatioWidth;
		public int targetAspectRatioHeight;

		/// <summary>
		/// Function to apply the fix aspect ratio action on the specified camera.
		/// </summary>
		public void execute(Camera cam) {

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

}
