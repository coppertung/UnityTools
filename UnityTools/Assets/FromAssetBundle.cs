using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityTools.Attribute;

namespace UnityTools.Assets {
	
	public class FromAssetBundle : MonoBehaviour {

		#region Enumeration
		public enum AssetType {
			None = 0,
			Image = 1,
			Sprite = 2,
			Prefab = 3
		}
		#endregion

		#region Fields_And_Properties
		/// <summary>
		/// The name of the asset bundle.
		/// </summary>
		public string assetBundleName;
		/// <summary>
		/// The name of the asset.
		/// </summary>
		public string assetName;
		/// <summary>
		/// The file path of the asset bundle.
		/// </summary>
		[ReadOnly]
		public string filepath;
		/// <summary>
		/// The type of the asset.
		/// </summary>
		public AssetType type;
		/// <summary>
		/// [Prefab type only]
		/// This stored the prefab asset.
		/// </summary>
		public GameObject prefab {
			get;
			protected set;
		}
		/// <summary>
		/// [Prefab type only]
		/// Instantiate the prefab object after the asset is loaded?
		/// </summary>
		#if UNITY_EDITOR
		[ShowIfEqual("type", (int)AssetType.Prefab, true)]
		#endif
		public bool instantiatePrefabAfterLoaded;
		#endregion

		#region MonoBehaviour
		void Awake() {
			
			filepath = Application.persistentDataPath;
			loadAsset ();

		}
		#endregion

		#region Functions
		/// <summary>
		/// Load the asset.
		/// </summary>
		public void loadAsset() {

			switch (type) {
			case AssetType.Image:
				StartCoroutine (AssetsLoader.LoadAssets<Sprite> (assetBundleName, assetName, filepath, inProgress, loadImage, exceptionHandler));
				break;
			case AssetType.Sprite:
				StartCoroutine (AssetsLoader.LoadAssets<Sprite> (assetBundleName, assetName, filepath, inProgress, loadSprite, exceptionHandler));
				break;
			case AssetType.Prefab:
				StartCoroutine (AssetsLoader.LoadAssets<GameObject> (assetBundleName, assetName, filepath, inProgress, loadPrefab, exceptionHandler));
				break;
			case AssetType.None:
			default:
				break;
			}

		}

		/// <summary>
		/// Instantiates the prefab.
		/// It will be instantiate with the position and rotation of this gameobject by default.
		/// </summary>
		public GameObject instantiatePrefab() {

			return instantiatePrefab (transform.position, transform.rotation);

		}

		/// <summary>
		/// Instantiates the prefab with specified position and rotation.
		/// </summary>
		public GameObject instantiatePrefab(Vector3 position, Quaternion rotation) {

			GameObject instantiatedObject = null;
			if (type == AssetType.Prefab && prefab != null) {
				instantiatedObject = Instantiate (prefab, position, rotation);
			}
			return instantiatedObject;

		}

		private void loadImage(Sprite _sprite) {

			try {
				GetComponent<Image> ().sprite = _sprite;
			}
			catch (Exception ex) {
				exceptionHandler (ex);
			}

		}

		private void loadSprite(Sprite _sprite) {

			try {
				GetComponent<SpriteRenderer> ().sprite = _sprite;
			}
			catch (Exception ex) {
				exceptionHandler (ex);
			}

		}

		private void loadPrefab(GameObject _gameObject) {

			prefab = _gameObject;
			if (instantiatePrefabAfterLoaded) {
				Instantiate (prefab, transform.position, transform.rotation);
			}

		}

		/// <summary>
		/// Function that is called while the loading asset function is in progress.
		/// Do nothing by default.
		/// Override it in order to do extension.
		/// </summary>
		protected virtual void inProgress(float progress) {
		}

		/// <summary>
		/// Function that is called when exception occured.
		/// Throw exception only by default.
		/// Override it in order to do extension.
		/// </summary>
		protected virtual void exceptionHandler(Exception ex) {
			throw ex;
		}
		#endregion

	}

}