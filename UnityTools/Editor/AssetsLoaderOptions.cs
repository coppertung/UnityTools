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

		[MenuItem(loadFromAssetFolderOptions, true, 1)]
		public static bool toggleLoadFromAssetFolderValidate() {

			Menu.SetChecked (loadFromAssetFolderOptions, AssetsLoader.loadFromAssetFolder);
			return true;

		}

		[MenuItem(buildAssetBundleOptions, false, 2)]
		public static void buildAssetBundle() {

			AssetBundleBuilder.Build ();

		}

	}

}