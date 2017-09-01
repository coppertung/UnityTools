using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityTools.Languages {

	/// <summary>
	/// This class is used to store the dictionary of the words in xml form in order to achieve multi-languages.
	/// Text files, web files or text assets are supported.
	/// Here is an example for you to store the words in xml file:
	/// <?xml version="1.0" encoding="utf-8"?>
	/// <languages>
	/// 	<English>
	/// 		<string name="test">testing</string>
	/// 	</English>
	/// </languages>
	/// </summary>
	public class XmlWordDict {

		private Dictionary<string, string> wordsDict;
		private XmlDocument xmlDoc;
		private string language;

		/// <summary>
		/// Is the dictionary completed loading?
		/// </summary>
		public bool isLoaded {
			get;
			private set;
		}

		/// <summary>
		/// Import the dictionary using a xml file with a provided file path.
		/// The file path should include the file name and extension also, e.g. Application.dataPath + "/test.xml".
		/// Noted that the parameter language should reference with your xml file and should not contains any space.
		/// </summary>
		public XmlWordDict(string filePath, string language, MonoBehaviour obj) {

			this.isLoaded = false;
			language.Replace (" ", "_");
			this.language = language;
			this.xmlDoc = new XmlDocument ();
			loadFromFile (filePath, obj);

		}

		/// <summary>
		/// Import the dictionary using a xml file with a provided text asset.
		/// Noted that the parameter language should reference with your xml file and should not contains any space.
		/// </summary>
		public XmlWordDict(TextAsset asset, string language, MonoBehaviour obj) {

			this.isLoaded = false;
			language.Replace (" ", "_");
			this.language = language;
			this.xmlDoc = new XmlDocument ();
			loadFromAsset (asset, obj);

		}

		/// <summary>
		/// Import the dictionary using a xml file with a provided url.
		/// As the file will be downloaded directly from url, the url must be a xml file, e.g. http://www.test.com/test.xml.
		/// The downloaded file will NOT be stored in the local directory.
		/// Noted that the parameter language should reference with your xml file and should not contains any space.
		/// **REQUIRE TESTING**
		/// </summary>
		public XmlWordDict(string url, string language, Action<Exception> errorHandler, MonoBehaviour obj) {

			this.isLoaded = false;
			language.Replace (" ", "_");
			this.language = language;
			this.xmlDoc = new XmlDocument ();
			obj.StartCoroutine (loadFromWeb (url, errorHandler));

		}

		private IEnumerator init() {

			if (wordsDict == null)
				wordsDict = new Dictionary<string, string> ();
			else
				wordsDict.Clear ();
			XmlElement element = xmlDoc.DocumentElement [language];
			if (element != null) {
				IEnumerator field = element.GetEnumerator ();
				while (field.MoveNext ()) {
					XmlElement item = (XmlElement)field.Current;
					wordsDict.Add (item.GetAttribute ("name"), item.InnerText);
					yield return null;
				}
			} else {
				throw new NullReferenceException ("There is no element found!");
			}
			isLoaded = true;

		}

		private void loadFromFile(string filePath, MonoBehaviour obj) {

			xmlDoc.Load (filePath);
			obj.StartCoroutine (init ());

		}

		private void loadFromAsset(TextAsset asset, MonoBehaviour obj) {

			xmlDoc.LoadXml (asset.text);
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
				xmlDoc.LoadXml (download.downloadHandler.text);
				yield return init ();
			}

			// clear the cache to release memory
			Caching.ClearCache ();

		}

		/// <summary>
		/// Switch the language of the dictionary.
		/// </summary>
		public void setLanguage(string language, MonoBehaviour obj) {

			this.language = language;
			obj.StartCoroutine (init ());

		}

		/// <summary>
		/// Get the word from the dictionary with specific name.
		/// </summary>
		public string getWord(string name) {

			if (!wordsDict.ContainsKey (name)) {
				throw new KeyNotFoundException ();
			} else {
				string value = null;
				wordsDict.TryGetValue (name, out value);
				return value;
			}

		}

	}

}