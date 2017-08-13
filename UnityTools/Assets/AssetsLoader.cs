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

namespace UnityTools.Assets {

	public class AssetsLoader {

		public static AssetBundle assetBundle;
		private static bool loadingAssetBundle = false;

		#if UNITY_EDITOR
		/// <summary>
		/// Are the assets loaded from the folder?
		/// Default is true.
		/// Can be set in editor.
		/// BUG: ALWAYS being reset in editor play mode
		/// **NOT WORKING UNTIL UNITY CURRENT VERSION**
		/// </summary>
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
		public static LocalAssetsLoader LoadAssetsInEditor(string assetBundleName, string assetName, System.Type type, Action<Exception> errorHandler) {

			LocalAssetsLoader loader;
			string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, assetName);
			if (assetPaths.Length == 0) {
				errorHandler (new Exception ("Asset Not Found!"));
			}
			// get the first asset we found
			loader = new LocalAssetsLoader (assetPaths[0], type);
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
		public static IEnumerator LoadAssetsFromFolderAsync(string file, Action<float> progressFunction, Action<Exception> errorHandler) {

			if (assetBundle != null) {
				ClearAssetBundle (true);
			}
			loadingAssetBundle = true;
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync (file);
			if (!File.Exists (file)) {
				errorHandler (new Exception ("File Not Found"));
			} else {
				while (!request.isDone) {
					progressFunction (request.progress);
					yield return null;
				}
				if (request.isDone) {
					assetBundle = request.assetBundle;
				}
			}
			loadingAssetBundle = false;

		}

		/// <summary>
		/// Loads the assets from specific path or assets folder.
		/// Noted that assets bundle should be downloaded and be stored before calling this function.
		/// </summary>
		public static IEnumerator LoadAssets<T>(string assetBundleName, string assetName, string filePath, System.Type type, Action<float> progressFunction, Action<T> completeFunction, Action<Exception> errorHandler) where T : UnityEngine.Object {

			#if UNITY_EDITOR
			if (loadFromAssetFolder) {
				Debug.Log("Start loading from the asset folder...");
				LocalAssetsLoader loader = LoadAssetsInEditor(assetBundleName, assetName, type, errorHandler);
				while(!loader.isDone) {
					yield return null;
				}
				if (loader.isDone) {
					completeFunction(loader.GetAsset<T>());
				}
			}
			else {
			#endif
				// ensure no assetbundle is being loaded at the moment
				while (loadingAssetBundle) {
					// wait until asset bundle is being loaded
					yield return new WaitForSecondsRealtime (0.1f);
				}
				if (assetBundle == null || !assetBundle.name.Equals(assetBundleName)) {
					yield return LoadAssetsFromFolderAsync (filePath + assetBundleName, progressFunction, errorHandler);
					while (loadingAssetBundle) {
						// wait until asset bundle is being loaded
						yield return new WaitForSecondsRealtime (0.1f);
					}
				}
				Debug.Log ("Start loading from the assetbundle...");
				AssetBundleRequest request;
				request = assetBundle.LoadAssetAsync (assetName, type);
				while (!request.isDone) {
					yield return null;
				}
				if (request.isDone) {
					Debug.Log (request.asset);
					completeFunction ((T)request.asset);
				}
			#if UNITY_EDITOR
			}
			#endif
		}

		public static void ClearAssetBundle(bool unloadAllLoadedObjects) {

			assetBundle.Unload (unloadAllLoadedObjects);

		}

	}

}