using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityTools.Languages {

    /// <summary>
    /// The outer part of the dictionary, which holds elements.
    /// </summary>
	[System.Serializable]
	public class JsonWordDictDocument {

		public JsonWordDictElement[] elements;

	}

    /// <summary>
    /// The element of the dictionary, which holds language and entities.
    /// </summary>
	[System.Serializable]
	public class JsonWordDictElement {

		public string language;
		public JsonWordDictEntity[] entities;

	}

    /// <summary>
    /// The entity of the dictionary, which holds id (key) and value.
    /// </summary>
	[System.Serializable]
	public class JsonWordDictEntity {

		public string id;
		public string value;

	}

	/// <summary>
	/// This class is used to store the dictionary of the words in json form in order to achieve multi-languages.
	/// Text files, web files or text assets are supported.
	/// Here is an example for you to store the words in json file:
	/// {
	/// 	"elements" : 
	/// 	[
	/// 	{
	/// 		"language" : "English",
	/// 		"entities" :
	/// 		[
	/// 		{
	/// 			"id" : "test",
	/// 			"value" : "testing"
	/// 		}
	/// 		]
	/// 	}
	/// 	]
	/// }
	/// </summary>
	public class JsonWordDict : WordDict {

		private JsonWordDictDocument jsonDoc;

		/// <summary>
		/// Import the dictionary using a json file with a provided file path.
		/// The file path should include the file name and extension also, e.g. Application.dataPath + "/test.json".
		/// Noted that the parameter language should reference with your json file and should not contains any space.
		/// </summary>
		public JsonWordDict(string filePath, string language, MonoBehaviour obj) {

			this.isLoaded = false;
			language.Replace (" ", "_");
			this.language = language;
			loadFromFile (filePath, obj);

		}

		/// <summary>
		/// Import the dictionary using a json file with a provided text asset.
		/// Noted that the parameter language should reference with your json file and should not contains any space.
		/// **REQUIRE TESTING**
		/// </summary>
		public JsonWordDict(TextAsset asset, string language, MonoBehaviour obj) {

			this.isLoaded = false;
			language.Replace (" ", "_");
			this.language = language;
			loadFromAsset (asset, obj);

		}

		/// <summary>
		/// Import the dictionary using a json file with a provided url.
		/// As the file will be downloaded directly from url, the url must be a json file, e.g. http://www.test.com/test.json.
		/// The downloaded file will NOT be stored in the local directory.
		/// Noted that the parameter language should reference with your json file and should not contains any space.
		/// **REQUIRE TESTING**
		/// </summary>
		public JsonWordDict(string url, string language, Action<Exception> errorHandler, MonoBehaviour obj) {

            isLoaded = false;
			language.Replace (" ", "_");
			this.language = language;
			obj.StartCoroutine (loadFromWeb (url, errorHandler));

		}

		protected override IEnumerator init() {
			
			if (wordsDict == null)
				wordsDict = new Dictionary<string, string> ();
			else
				wordsDict.Clear ();
			JsonWordDictElement jsonElement = new JsonWordDictElement ();
			int i = 0;
			for (i = 0; i < jsonDoc.elements.Length; i++) {
				if (jsonDoc.elements [i].language.Equals (language)) {
					jsonElement = jsonDoc.elements [i];
                    break;
				}
			}
			yield return null;
			if (jsonElement == null || jsonElement.entities.Length == 0) {
				throw new  KeyNotFoundException ();
			} else {
				for (i = 0; i < jsonElement.entities.Length; i++) {
					wordsDict.Add (jsonElement.entities [i].id, jsonElement.entities [i].value);
				}
			}
			isLoaded = true;

		}

		private void loadFromFile(string filePath, MonoBehaviour obj) {

			StringBuilder strBuilder = new StringBuilder ();
			try {
				StreamReader reader = new StreamReader (filePath);

				string line;
				while ((line = reader.ReadLine ()) != null) {
					strBuilder.Append (line);
				}
			}
			catch(Exception ex) {
				throw ex;
			}
			jsonDoc = JsonUtility.FromJson<JsonWordDictDocument> (strBuilder.ToString ());
			obj.StartCoroutine (init ());

		}

		private void loadFromAsset(TextAsset asset, MonoBehaviour obj) {

			jsonDoc = JsonUtility.FromJson<JsonWordDictDocument> (asset.text);
			obj.StartCoroutine (init ());

		}

		private IEnumerator loadFromWeb(string url, Action<Exception> errorHandler) {

			UnityWebRequest download = UnityWebRequest.Get (url);
			download.Send ();

			while (!download.isDone) {
				// wait for the download complete
				yield return null;
			}

			if (download.isNetworkError) {
				errorHandler (new Exception ("Network error"));
			} else if (download.isHttpError) {
				errorHandler (new Exception ("HTTP Error"));
			} else {
				jsonDoc = JsonUtility.FromJson<JsonWordDictDocument> (download.downloadHandler.text);
				yield return init ();
			}

			// clear the cache to release memory
			Caching.ClearCache ();

		}

	}

}