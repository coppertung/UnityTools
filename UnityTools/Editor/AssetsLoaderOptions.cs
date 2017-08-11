using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetsLoaderOptions : MonoBehaviour {

	public const string loadFromAssetFolderOptions = "UnityTools/Assets Loader/Load From Asset Folder";

	[MenuItem(loadFromAssetFolderOptions)]
	public static void toggleLoadFromAssetFolderOptions() {

		AssetsLoader.loadFromAssetFolder = !AssetsLoader.loadFromAssetFolder;

	}

	[MenuItem(loadFromAssetFolderOptions, true)]
	public static bool toggleLoadFromAssetFolderValidate() {

		Menu.SetChecked (loadFromAssetFolderOptions, AssetsLoader.loadFromAssetFolder);
		return true;

	}

}
