using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools {

	#region Interfaces
	/// <summary>
	/// Interface of the camera screen effect.
	/// </summary>
	public interface ICameraPostRenderEffect {

		string ShaderName {
			get;
		}

		void render (RenderTexture src, RenderTexture dest);

	}
	#endregion

	#region Camera_Screen_Effect_Classes
	/// <summary>
	/// Greyscale effect.
	/// Set up the intensity, then activate it.
	/// Reference:
	/// https://www.alanzucconi.com/2015/07/08/screen-shaders-and-postprocessing-effects-in-unity3d/
	/// </summary>
	[System.Serializable]
	public class CameraGreyScale : ICameraPostRenderEffect {

		#region Fields_And_Properties
		public string ShaderName {
			get {
				return "UnityTools/Camera/GreyScale";
			}
		}
		/// <summary>
		/// Is the effect be activated?
		/// </summary>
		public bool isActive;
		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[Range(0f, 1f)]
		public float intensity;
		protected Material screenEffectMaterial;
		#endregion

		#region Functions
		/// <summary>
		/// Activate or Inactivate the screen effect.
		/// </summary>
		public void setActive(bool _active) {

			isActive = _active;

		}

		/// <summary>
		/// Set the intensity of the screen effect.
		/// </summary>
		public void setIntensity(float _intensity) {

			intensity = Mathf.Clamp (_intensity, 0f, 1f);

		}

		/// <summary>
		/// Function to render specific screen effect.
		/// Should be called by CameraAdjustment only.
		/// </summary>
		public void render (RenderTexture src, RenderTexture dest) {
			
			if (isActive) {
				if (screenEffectMaterial == null) {
					screenEffectMaterial = new Material (Shader.Find (ShaderName));
				} else if (!screenEffectMaterial.shader.name.Equals (ShaderName)) {
					screenEffectMaterial.shader = Shader.Find (ShaderName);
				}
				screenEffectMaterial.SetFloat ("_Strength", intensity);
				Graphics.Blit (src, dest, screenEffectMaterial);
			}

		}
		#endregion
	}

	/// <summary>
	/// Inverse color effect.
	/// Set up the intensity, then activate it.
	/// </summary>
	[System.Serializable]
	public class CameraInverseColor : ICameraPostRenderEffect {

		#region Fields_And_Properties
		public string ShaderName {
			get {
				return "UnityTools/Camera/Inverse Color";
			}
		}
		/// <summary>
		/// Is the effect be activated?
		/// </summary>
		public bool isActive;
		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[Range(0f, 1f)]
		public float intensity;
		protected Material screenEffectMaterial;
		#endregion

		#region Functions
		/// <summary>
		/// Activate or Inactivate the screen effect.
		/// </summary>
		public void setActive(bool _active) {

			isActive = _active;

		}

		/// <summary>
		/// Set the intensity of the screen effect.
		/// </summary>
		public void setIntensity(float _intensity) {

			intensity = Mathf.Clamp (_intensity, 0f, 1f);

		}

		/// <summary>
		/// Function to render specific screen effect.
		/// Should be called by CameraAdjustment only.
		/// </summary>
		public void render (RenderTexture src, RenderTexture dest) {

			if (isActive) {
				if (screenEffectMaterial == null) {
					screenEffectMaterial = new Material (Shader.Find (ShaderName));
				} else if (!screenEffectMaterial.shader.name.Equals (ShaderName)) {
					screenEffectMaterial.shader = Shader.Find (ShaderName);
				}
				screenEffectMaterial.SetFloat ("_Strength", intensity);
				Graphics.Blit (src, dest, screenEffectMaterial);
			}

		}
		#endregion
	}
	#endregion

}