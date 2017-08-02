using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardShaderUtils {

	/// <summary>
	/// Rendering mode of the Standard shader, including: Opaque, Cutout, Fade and Transparent
	/// </summary>
	public enum RenderingMode {
		Opaque,
		Cutout,
		Fade,
		Transparent
	}

	/// <summary>
	/// Changes the render mode of the Standard Shader.
	/// Noted that "[material].shader = Shader.Find ("Standard");" should be called before calling this method.
	/// </summary>
	public static void ChangeRenderMode(Material standardShaderMaterial, RenderingMode renderingMode) {

		switch (renderingMode) {
		case RenderingMode.Opaque:
			if (!Mathf.Approximately (standardShaderMaterial.GetFloat ("_Mode"), 0)) {
				standardShaderMaterial.SetFloat ("_Mode", 0);
				standardShaderMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				standardShaderMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				standardShaderMaterial.SetInt ("_ZWrite", 1);
				standardShaderMaterial.DisableKeyword ("_ALPHATEST_ON");
				standardShaderMaterial.DisableKeyword ("_ALPHABLEND_ON");
				standardShaderMaterial.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
				standardShaderMaterial.renderQueue = -1;
			}
			break;
		case RenderingMode.Cutout:
			if (!Mathf.Approximately (standardShaderMaterial.GetFloat ("_Mode"), 1)) {
				standardShaderMaterial.SetFloat ("_Mode", 1);
				standardShaderMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				standardShaderMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				standardShaderMaterial.SetInt ("_ZWrite", 1);
				standardShaderMaterial.EnableKeyword ("_ALPHATEST_ON");
				standardShaderMaterial.DisableKeyword ("_ALPHABLEND_ON");
				standardShaderMaterial.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
				standardShaderMaterial.renderQueue = 2450;
			}
			break;
		case RenderingMode.Fade:
			if (!Mathf.Approximately (standardShaderMaterial.GetFloat ("_Mode"), 2)) {
				standardShaderMaterial.SetFloat ("_Mode", 2);
				standardShaderMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				standardShaderMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				standardShaderMaterial.SetInt ("_ZWrite", 0);
				standardShaderMaterial.DisableKeyword ("_ALPHATEST_ON");
				standardShaderMaterial.EnableKeyword ("_ALPHABLEND_ON");
				standardShaderMaterial.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
				standardShaderMaterial.renderQueue = 3000;
			}
			break;
		case RenderingMode.Transparent:
			if (!Mathf.Approximately (standardShaderMaterial.GetFloat ("_Mode"), 3)) {
				standardShaderMaterial.SetFloat ("_Mode", 3);
				standardShaderMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				standardShaderMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				standardShaderMaterial.SetInt ("_ZWrite", 1);	// prevent from disappear on screen
				standardShaderMaterial.DisableKeyword ("_ALPHATEST_ON");
				standardShaderMaterial.DisableKeyword ("_ALPHABLEND_ON");
				standardShaderMaterial.EnableKeyword ("_ALPHAPREMULTIPLY_ON");
				standardShaderMaterial.renderQueue = 3000;
			}
			break;
		}

	}

}
