#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Assets {

	public class LocalAssetsLoader : UnityEngine.AsyncOperation {

		#region Fields_And_Properties
		public UnityEngine.Object asset;

		public new bool isDone {
			get {
				return asset != null;
			}
		}
		public new float progress {
			get {
				return asset == null ? 0 : 1;
			}
		}
		#endregion

		#region Functions
		public LocalAssetsLoader(string assetPath, System.Type type) {

			asset =  AssetDatabase.LoadAssetAtPath (assetPath, type);

		}

		public T GetAsset<T>() where T : UnityEngine.Object {

			return asset as T;

		}
		#endregion

	}

}
#endif