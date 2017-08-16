using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTools.Sprites {
	
	public class NumberText : MonoBehaviour {

		private static Dictionary<string, Sprite> _numTextDict = new Dictionary<string, Sprite> ();
		public static Dictionary<string, Sprite> numTextDict {
			get {
				return _numTextDict;
			}
		}

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

		public static void showNumberInHorizontal(GameObject referenceObject, Transform referencePoint, float value, int toDecimalPoint, bool positive, bool isMoney, float scale, bool showPositiveSign = true) {
				
			Vector3 refPos = referencePoint.position;
			Vector2 refSize = referenceObject.GetComponent<Image> ().rectTransform.sizeDelta;
			int placeValue = 0;
			for (placeValue = 0; placeValue < int.MaxValue; placeValue++) {
				if ((int)(value / Mathf.Pow (10, placeValue)) > 0 || (value / Mathf.Pow (10, placeValue) == 0 && (int)(value % Mathf.Pow (10, placeValue)) > 0)) {
					continue;
				} else {
					break;
				}
			}
			// positive/negative sign
			if (showPositiveSign) {
				if (positive) {
					Sprite positiveSprite;
					_numTextDict.TryGetValue ("+", out positiveSprite);
					GameObject positiveSign = Instantiate (referenceObject);
					positiveSign.GetComponent<Image> ().sprite = positiveSprite;
					positiveSign.transform.SetParent (referencePoint);
					positiveSign.transform.position = refPos;
					positiveSign.transform.localScale = new Vector3 (scale, scale, scale);
				} else {
					Sprite negativeSprite;
					_numTextDict.TryGetValue ("+", out negativeSprite);
					GameObject negativeSign = Instantiate (referenceObject);
					negativeSign.GetComponent<Image> ().sprite = negativeSprite;
					negativeSign.transform.SetParent (referencePoint);
					negativeSign.transform.position = refPos;
					negativeSign.transform.localScale = new Vector3 (scale, scale, scale);
				}
			}
			// dollar sign
			if (isMoney) {
				if (showPositiveSign) {
					refPos += new Vector3 (refSize.x * 0.8f * scale, 0, 0);
				}
				Sprite dollarSprite;
				_numTextDict.TryGetValue ("$", out dollarSprite);
				GameObject dollarSign = Instantiate (referenceObject);
				dollarSign.GetComponent<Image> ().sprite = dollarSprite;
				dollarSign.transform.SetParent (referencePoint);
				dollarSign.transform.position = refPos;
				dollarSign.transform.localScale = new Vector3 (scale, scale, scale);
			}
			// number till unit
			if (placeValue == 0) {
				Sprite numberSprite;
				_numTextDict.TryGetValue ("0", out numberSprite);
				GameObject newNumber = Instantiate (referenceObject);
				newNumber.GetComponent<Image> ().sprite = numberSprite;
				newNumber.transform.SetParent (referencePoint);
				newNumber.transform.position = refPos;
				newNumber.transform.localScale = new Vector3 (scale, scale, scale);
			} else {
				for (int i = placeValue - 1; i >= 0; i--) {
					if (showPositiveSign || i > 0) {
						refPos += new Vector3 (refSize.x * scale, 0, 0);
					}
					int num = 0;
					if (i == 0) {
						// find the unit number
						num = (int)((value % 100) / 10);
					} else {
						num = (int)((value % Mathf.Pow (10, i + 1)) / Mathf.Pow (10, i));
					}
					Debug.Log (i + ":" + num + "[" + value + "]");
					Sprite numberSprite;
					_numTextDict.TryGetValue (num.ToString (), out numberSprite);
					GameObject newNumber = Instantiate (referenceObject);
					newNumber.GetComponent<Image> ().sprite = numberSprite;
					newNumber.transform.SetParent (referencePoint);
					newNumber.transform.position = refPos;
					newNumber.transform.localScale = new Vector3 (scale, scale, scale);
				}
			}
			// decimal point and decimal numbers part
			if (toDecimalPoint > 0) {
				refPos += new Vector3 (refSize.x * 0.6f * scale, 0, 0);
				Sprite dotSprite;
				_numTextDict.TryGetValue (".", out dotSprite);
				GameObject dotObject = Instantiate (referenceObject);
				dotObject.transform.SetParent (referencePoint);
				dotObject.GetComponent<Image> ().sprite = dotSprite;
				dotObject.transform.position = refPos;
				dotObject.transform.localScale = new Vector3 (scale, scale, scale);
				refPos += new Vector3 (refSize.x * 0.6f * scale, 0, 0);
				for (int i = 0; i < toDecimalPoint; i++) {
					int num = Mathf.RoundToInt ((value * Mathf.Pow (10, i + 1) % 100) / 10);
					Sprite decimalNumberSprite;
					_numTextDict.TryGetValue (num.ToString(), out decimalNumberSprite);
					GameObject newDecimalNumber = Instantiate (referenceObject);
					newDecimalNumber.GetComponent<Image> ().sprite = decimalNumberSprite;
					newDecimalNumber.transform.SetParent (referencePoint);
					newDecimalNumber.transform.position = refPos;
					newDecimalNumber.transform.localScale = new Vector3 (scale, scale, scale);
					refPos += new Vector3 (refSize.x * scale, 0, 0);
				}
			}
		
		}

	}

}