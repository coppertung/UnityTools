using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace UnityTools.Assets {

	public class AssetsLoader {

		#region Fields_And_Properties
		private static List<AssetBundle> _assetBundles;
		private static bool _loadingAssetBundle = false;

		/// <summary>
		/// The assetbundle(s) that are currently loaded.
		/// </summary>
		public static List<AssetBundle> assetBundles {
			get {
				return _assetBundles;
			}
		}
		/// <summary>
		/// Whether the assetbundle is being loading or not.
		/// </summary>
		public static bool loadingAssetBundle {
			get {
				return _loadingAssetBundle;
			}
		}
		#endregion

		#region Editor_Only
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
		/// Load the assets directly from the asset folder, noted that it can operate in editor only.
		/// **SHOULD BE ASYNCHRONOUS FUNCTION THEORETICALLY BUT NOT SUPPORTED IN PERSONAL VERSION**
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
		#endregion

		#region Functions
		/// <summary>
		/// Check whether the asset exists.
		/// </summary>
		public static bool AssetFound(string assetBundleName, string assetName) {

			#if UNITY_EDITOR
			if (loadFromAssetFolder)
				return (AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, assetName).Length > 0);
			else {
			#endif
				AssetBundle assetBundle = assetBundles.Find (x => x.name.Equals (assetBundleName));
				return (assetBundle != null && assetBundle.Contains (assetName));
			#if UNITY_EDITOR
			}
			#endif

		}

		///	<summary>
		///	Download and store the asset bundle in the local drive in asyncchronous way.
		///	</summary>
		public static IEnumerator DownloadAssetsAsync(string url, string filepath, string filename, Action<float> progressFunction, Action completeFunction, Action<Exception> errorHandler) {
				
			UnityWebRequest download = UnityWebRequest.Get (url);
			#if UNITY_2017_3_OR_NEWER
			download.SendWebRequest ();
			#else
			download.Send ();
			#endif
			while (!download.isDone) {
				if (progressFunction != null) {
					progressFunction (download.downloadProgress);
				}
				yield return null;
			}
			#if UNITY_2017_1_OR_NEWER
			if (download.isNetworkError) {
			errorHandler (new Exception ("Network Error"));
			} else if (download.isHttpError) {
			errorHandler (new Exception ("HTTP Error"));
			#else
			if (download.isError) {
				errorHandler (new Exception (download.error));
			#endif
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
				#if UNITY_2017_1_OR_NEWER
				Caching.ClearCache ();
				#else
				Caching.CleanCache();
				#endif
				completeFunction ();
			}

		}

		///	<summary>
		///	Load the asset bundle from the local drive in asyncchronous way.
		///	</summary>
		public static IEnumerator LoadAssetsFromFolderAsync(string file, Action<float> progressFunction, Action<Exception> errorHandler) {

			_loadingAssetBundle = true;
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync (file);
			if (!File.Exists (file)) {
				errorHandler (new Exception ("File Not Found"));
			} else {
				while (!request.isDone) {
					if (progressFunction != null) {
						progressFunction (request.progress);
					}
					yield return null;
				}
				if (request.isDone) {
					// remove older bundle if repeated loaded
					if (_assetBundles.Contains (request.assetBundle)) {
						_assetBundles.Remove (request.assetBundle);
					}
					_assetBundles.Add (request.assetBundle);
				}
			}
			_loadingAssetBundle = false;

		}

		/// <summary>
		/// Loads the assets from specific path or assets folder.
		/// Noted that assets bundle should be downloaded and be stored before calling this function.
		/// </summary>
		public static IEnumerator LoadAssets<T>(string assetBundleName, string assetName, string filePath, Action<float> progressFunction, Action<T> completeFunction, Action<Exception> errorHandler) where T : UnityEngine.Object {

			#if UNITY_EDITOR
			if (loadFromAssetFolder) {
				LocalAssetsLoader loader = LoadAssetsInEditor(assetBundleName, assetName, typeof(T), errorHandler);
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
				AssetBundle assetBundle = assetBundles.Find (x => x.name.Equals (assetBundleName));
				if (assetBundle == null) {
					yield return LoadAssetsFromFolderAsync (filePath + assetBundleName, progressFunction, errorHandler);
					while (loadingAssetBundle) {
						// wait until asset bundle is being loaded
						yield return new WaitForSecondsRealtime (0.1f);
					}
				}
				AssetBundleRequest request;
				request = assetBundle.LoadAssetAsync (assetName, typeof(T));
				while (!request.isDone) {
					yield return null;
				}
				if (request.isDone) {
					completeFunction ((T)request.asset);
				}
			#if UNITY_EDITOR
			}
			#endif
		}

        /// <summary>
        /// Clear the loaded asset bundle.
        /// Noted that if unloadAllLoadedObjects is true, then all loaded assets of this asset bundle will be destroy also.
        /// In the other hands, if unloadAllLoadedObjects is false, then all loaded assets of this asset bundle will become instances after the asset bundle being destroyed.
        /// </summary>
		public static void ClearAssetBundle(bool unloadAllLoadedObjects) {

			for (int i = assetBundles.Count - 1; i >= 0; i++) {
				_assetBundles [i].Unload (unloadAllLoadedObjects);
				_assetBundles.RemoveAt (i);
			}

		}
		#endregion

	}

}