using System.IO;
using UnityEditor;

namespace UnityTools.Assets {

    public class AssetBundleBuilder {

		public static string AssetBundleFolder = "AssetBundles/";

		public static void Build() {

			string outputPath = AssetBundleFolder + Utils.buildPlatform + "/";
			if (!Directory.Exists (outputPath))
				Directory.CreateDirectory (outputPath);
			BuildPipeline.BuildAssetBundles (outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

		}

	}

}