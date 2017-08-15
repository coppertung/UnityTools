using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Assets {

	public class AssetsLoaderOptions : MonoBehaviour {

		public const string loadFromAssetFolderOptions = "UnityTools/Assets/Load From Asset Folder";
		public const string buildAssetBundleOptions = "UnityTools/Assets/Build AssetBundle";

		[MenuItem(loadFromAssetFolderOptions)]
		public static void toggleLoadFromAssetFolderOptions() {

			AssetsLoader.loadFromAssetFolder = !AssetsLoader.loadFromAssetFolder;

		}

		[MenuItem(loadFromAssetFolderOptions, true)]
		public static bool toggleLoadFromAssetFolderValidate() {

			Menu.SetChecked (loadFromAssetFolderOptions, AssetsLoader.loadFromAssetFolder);
			return true;

		}

		[MenuItem(buildAssetBundleOptions)]
		public static void buildAssetBundle() {

			AssetBundleBuilder.Build ();

		}

	}

}