using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.iOS;

public class AssetsLoader {

	///	<summary>
	///	Download and store the asset bundle in the local drive in asyncchronous way
	///	</summary>
	public static IEnumerator DownloadAssetsAsync(string url, string filepath, string filename, Action completeFunction, Action<Exception> errorHandler) {

		UnityWebRequest download = UnityWebRequest.Get (url);
		yield return download.Send ();
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
	///	Load the asset bundle from the local drive in asyncchronous way
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
