using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerPrefsOptions : MonoBehaviour {

	public const string clearOptions = "UnityTools/PlayerPrefs/Clear All";

	[MenuItem(clearOptions)]
	public static void clearPlayerPrefs() {

		PlayerPrefs.DeleteAll ();

	}

}
