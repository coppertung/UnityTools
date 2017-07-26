using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerPrefsOptions : MonoBehaviour {

	public const string clearOptions = "UnityTools/PlayerPrefs/Clear All";

	[MenuItem(clearOptions)]
	public static void clearPlayerPrefs() {

		PlayerPrefs.DeleteAll ();

	}

}
