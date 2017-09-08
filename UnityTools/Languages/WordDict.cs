using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Languages {

	/// <summary>
	/// This abstract class is used to store the dictionary of the words in order to achieve multi-languages.
	/// Inherit this abstract class to implement it.
	/// </summary>
	public abstract class WordDict {

		protected Dictionary<string, string> wordsDict;
		protected string language;

		/// <summary>
		/// Is the dictionary completed loading?
		/// </summary>
		public bool isLoaded {
			get;
			protected set;
		}

		protected abstract IEnumerator init();

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

        /// <summary>
        /// Clear the dictionary.
        /// </summary>
        public void clear() {

            wordsDict.Clear();

        }

	}

}