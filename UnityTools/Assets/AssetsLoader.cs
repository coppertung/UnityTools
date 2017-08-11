using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.iOS;

public class AssetsLoader {

	#if UNITY_EDITOR
	public static bool loadFromAssetFolder = true;
	/// <summary>
	/// Check whether the asset exists.
	/// </summary>
	public static bool AssetFound(string assetBundleName, string assetName) {

		return (AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, assetName).Length > 0);

	}

	/// <summary>
	/// Load the assets directly from the asset folder, noted that it can operate in editor only.
	/// </summary>
	// TODO: Need to change it into asynchronous function later.
	public static LocalAssetsLoader LoadAssetsInEditor(string assetBundleName, string assetName, System.Type type, Action<Exception> errorHandler) {

		LocalAssetsLoader loader;
		string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, assetName);
		if (assetPaths.Length == 0) {
			errorHandler (new Exception ("Asset Not Found!"));
		}
		// get the first asset we found
		UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath (assetPaths[0], type);		// TODO: change this to an asybchronous function
		loader = new LocalAssetsLoader (obj);
		return loader;

	}
	#endif

	///	<summary>
	///	Download and store the asset bundle in the local drive in asyncchronous way.
	///	</summary>
	public static IEnumerator DownloadAssetsAsync(string url, string filepath, string filename, Action<float> progressFunction, Action completeFunction, Action<Exception> errorHandler) {

		UnityWebRequest download = UnityWebRequest.Get (url);
		download.Send ();
		while (!download.isDone) {
			progressFunction (download.downloadProgress);
			yield return null;
		}
		if (download.isNetworkError) {
			errorHandler (new Exception ("Network Error"));
		} else if (download.isHttpError) {
			errorHandler (new Exception ("HTTP Error"));
		} else {
			// save the bundle
			if (Directory.Exists (filepath)) {
				File.Delete (filepath + filename);
				Directory.Delete (filepath);
			}
			Directory.CreateDirectory (filepath);
			byte[] bytes = download.downloadHandler.data;
			File.WriteAllBytes (filepath + filename, bytes);
			#if UNITY_IOS
			// exclude the asset bundle from iCloud/iTunes backup
			Device.SetNoBackupFlag (filepath + filename);
			#endif
			yield return null;
			// clear the cached file to release the used spaces
			Caching.ClearCache ();
			completeFunction ();
		}

	}

	///	<summary>
	///	Load the asset bundle from the local drive in asyncchronous way.
	///	</summary>
	public static IEnumerator LoadAssetsFromFolderAsync(string file, Action<float> progressFunction, Action<AssetBundle> completeFunction, Action<Exception> errorHandler) {

		AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync (file);
		if (!File.Exists (file)) {
			errorHandler (new Exception ("File Not Found"));
		} else {
			while (!request.isDone) {
				progressFunction (request.progress);
				yield return null;
			}
			if (request.isDone) {
				completeFunction (request.assetBundle);
			}
		}

	}

}
