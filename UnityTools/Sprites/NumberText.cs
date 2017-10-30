using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTools.Sprites {

    public class NumberText {

		#region Fields_And_Properties
		private static Dictionary<string, Sprite> _numTextDict = new Dictionary<string, Sprite> ();

		/// <summary>
		/// Dictionary which related to the sprites of numbers.
		/// Noted that positive/negative sign and dollar sign can also be included.
		/// </summary>
		public static Dictionary<string, Sprite> numTextDict {
			get {
				return _numTextDict;
			}
		}
		#endregion

		#region Functions
		/// <summary>
		/// Add the sprite to the dictionary.
		/// Noted that it only supports for the number, dot, dollar sign, positive and negative sign.
		/// </summary>
		public static void addToDict(Sprite image) {
				
			if (image.name.Contains ("dot") || image.name.Contains (".")) {
				_numTextDict.Add (".", image);
			} else if (image.name.Contains ("positive") || image.name.Contains ("+")) {
				_numTextDict.Add ("+", image);
			} else if (image.name.Contains ("negative") || image.name.Contains ("-")) {
				_numTextDict.Add ("-", image);
			} else if (image.name.Contains ("dollar") || image.name.Contains ("$")) {
				_numTextDict.Add ("$", image);
			} else {
				int num = int.Parse (Regex.Replace (image.name, "[^0-9]", ""));
				if (num < 10) {
					_numTextDict.Add (num.ToString (), image);
				}
			}

		}

		/// <summary>
		/// Clear the dictionary.
		/// </summary>
		public static void clearDict() {

			_numTextDict.Clear ();

		}

		/// <summary>
		/// Show the sprites of the requested value in horizontal way started from the reference point.
		/// Reminded that a reference object with image (not raw image) component is necessary to provide in order to instantiate it on screen.
		/// </summary>
		public static void showNumberInHorizontal(GameObject referenceObject, Transform referencePoint, float value, int toDecimalPoint, bool isMoney, float scale, float spacing = 1f, bool showPositiveSign = true, bool positive = true) {

			Vector3 refPos = referencePoint.position;
			Vector2 scaling = Utils.GetScreenScaleFromCanvasScaler (referencePoint.gameObject);
			Vector2 refSize = referenceObject.GetComponent<Image> ().rectTransform.sizeDelta;
			refSize = new Vector2 (refSize.x / scaling.x, refSize.y / scaling.y) * spacing;

			string valueInString = value.ToString ("F" + toDecimalPoint.ToString ());
			char[] valueInCharArr = valueInString.ToCharArray ();

			// positive/negative sign
			if (showPositiveSign) {
				if (positive) {
					createSpriteObject ("+", refPos, scale, referenceObject, referencePoint);
				} else {
					createSpriteObject ("-", refPos, scale, referenceObject, referencePoint);
				}
			}
			// dollar sign
			if (isMoney) {
				if (showPositiveSign) {
					refPos += new Vector3 (refSize.x * 0.8f * scale, 0, 0);
				}
				createSpriteObject ("$", refPos, scale, referenceObject, referencePoint);
			}
			if (showPositiveSign || isMoney) {
				refPos += new Vector3 (refSize.x * scale, 0, 0);
			}
			for (int i = 0; i < valueInString.Length; i++) {
				string numChar = valueInCharArr [i].ToString ();
				if (numChar.Equals (".")) {
					refPos += new Vector3 (-refSize.x * 0.4f * scale, 0, 0);
					createSpriteObject (numChar, refPos, scale, referenceObject, referencePoint);
					refPos += new Vector3 (refSize.x * 0.6f * scale, 0, 0);
				} else {
					createSpriteObject (numChar, refPos, scale, referenceObject, referencePoint);
					refPos += new Vector3 (refSize.x * scale, 0, 0);
				}
			}

		}

		private static void createSpriteObject(string keyword, Vector3 position, float scale, GameObject model, Transform parent) {

			Sprite spriteObject;
			_numTextDict.TryGetValue (keyword, out spriteObject);
			GameObject newNumber = GameObject.Instantiate (model);
			newNumber.GetComponent<Image> ().sprite = spriteObject;
			newNumber.transform.SetParent (parent);
			newNumber.transform.position = position;
			newNumber.transform.localScale = new Vector3 (scale, scale, scale);

		}
		#endregion

	}

}