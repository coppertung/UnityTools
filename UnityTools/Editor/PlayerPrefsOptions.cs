using UnityEditor;
using UnityEngine;

namespace UnityTools {

    public class PlayerPrefsOptions : MonoBehaviour {

		public const string clearOptions = "UnityTools/PlayerPrefs/Clear All";

		[MenuItem(clearOptions)]
		public static void clearPlayerPrefs() {

			PlayerPrefs.DeleteAll ();

		}

	}

}