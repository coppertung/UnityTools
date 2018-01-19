using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityTools;

namespace UnityTools.Map {

	public class MapEditor : MonoBehaviour, IUpdateable {

		public CameraAdjustment mainCamera;

		[HideInInspector]
		public int colorIndex;
		[HideInInspector]
		public GameObject panelBG;
		[HideInInspector]
		public List<Toggle> colorToggles;
		[HideInInspector]
		public Button btnGenerate;
		[HideInInspector]
		public Button btnClear;

		#region MonoBehaviour
		void Awake() {

			generateEditorUI ();
			colorToggles = new List<Toggle> ();
			btnGenerate.interactable = true;
			btnClear.interactable = false;

		}

		void OnEnable() {

			UpdateManager.RegisterUpdate (this);

		}

		void OnDisable() {

			UpdateManager.UnregisterUpdate (this);

		}
		#endregion

		public void generateEditorUI () {

			// Add Event System
			if (GameObject.Find ("EventSystem") == null) {
				GameObject eventSystem = new GameObject ("EventSystem");
				eventSystem.AddComponent<EventSystem> ();
				eventSystem.AddComponent<StandaloneInputModule> ();
			}

			// Basic Information of the editor panel bg
			panelBG = new GameObject ("Editor Panel");
			Image panelBGImage = panelBG.AddComponent<Image> ();
			Vector2 panelBGSize = new Vector2 (430f, 0f);
			panelBG.transform.SetParent (transform);
			panelBGImage.color = new Color (1f, 1f, 1f, 0.5f);

			DefaultControls.Resources uiResources = new DefaultControls.Resources ();
			Sprite buttonSprite = AssetDatabase.GetBuiltinExtraResource<Sprite> (Utils.defaultSpritePath);
			Sprite backGroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite> (Utils.defaultBackgroundSpritePath);
			Sprite checkMarkSprite = AssetDatabase.GetBuiltinExtraResource<Sprite> (Utils.defaultCheckmarkPath);

			// Color Title
			GameObject colorTitle = DefaultControls.CreateText (uiResources);
			colorTitle.name = "Color Title";
			colorTitle.transform.SetParent (panelBG.transform);
			RectTransform colorTitleRectTransform = colorTitle.GetComponent<RectTransform> ();
			colorTitleRectTransform.sizeDelta = new Vector2 ((panelBGSize.x - 30) / 2, 40);
			Text colorTitleText = colorTitle.GetComponent<Text> ();
			colorTitleText.text = "Color";
			colorTitleText.fontSize = 30;
			colorTitleText.alignment = TextAnchor.MiddleLeft;
			ToggleGroup colorToggleGroup = colorTitle.AddComponent<ToggleGroup> ();
			panelBGSize += new Vector2 (0, colorTitleRectTransform.sizeDelta.y + 10);

			// Color Toggle
			RectTransform[] colorToggleRectTransform = new RectTransform[MapGenerator.Instance.colorList.Length];
			for (int i = 0; i < MapGenerator.Instance.colorList.Length; i++) {
				GameObject colorToggle = DefaultControls.CreateToggle (uiResources);
				colorToggle.name = "Color Toggle " + MapGenerator.Instance.colorList [i].name;
				colorToggle.transform.SetParent (panelBG.transform);
				colorToggleRectTransform [i] = colorToggle.GetComponent<RectTransform> ();
				colorToggleRectTransform [i].sizeDelta = new Vector2 (panelBGSize.x - 20, 40);
				colorToggle.transform.GetChild (0).GetComponent<Image> ().sprite = backGroundSprite;
				colorToggle.transform.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2 (colorToggleRectTransform [i].sizeDelta.y, colorToggleRectTransform [i].sizeDelta.y);
				colorToggle.transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = checkMarkSprite;
				colorToggle.transform.GetChild (0).GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2 (colorToggleRectTransform [i].sizeDelta.y, colorToggleRectTransform [i].sizeDelta.y);
				Text colorToggleText = colorToggle.transform.GetChild (1).GetComponent<Text> ();
				colorToggleText.text = MapGenerator.Instance.colorList [i].name;
				colorToggleText.alignment = TextAnchor.UpperLeft;
				colorToggleText.fontSize = 24;
				Toggle colorToggleComponent = colorToggle.GetComponent<Toggle> ();
				if (i > 0) {
					colorToggleComponent.isOn = false;
				}
				colorToggleComponent.group = colorToggleGroup;
				colorToggleComponent.onValueChanged.AddListener (
					(bool isOn) => {
						if(isOn) {
							string name = colorToggle.name;
							name = name.Replace("Color Toggle ", "");
							for(int n = 0; n < MapGenerator.Instance.colorList.Length; n++) {
								if(MapGenerator.Instance.colorList[n].name.Equals(name)) {
									colorIndex = n;
									break;
								}
							}
						}
					}
				);
				colorToggles.Add (colorToggleComponent);
				panelBGSize += new Vector2 (0, colorToggleRectTransform [i].sizeDelta.y + 10);
			}

			// Basic Information of the generate and clear button
			GameObject generateButton = DefaultControls.CreateButton (uiResources);
			generateButton.name = "Generate Button";
			generateButton.transform.SetParent (panelBG.transform);
			RectTransform generateButtonRectTransform = generateButton.GetComponent<RectTransform> ();
			generateButtonRectTransform.sizeDelta = new Vector2 ((panelBGSize.x - 30) / 2, 50);
			Text generateButtonText = generateButton.transform.GetChild (0).GetComponent<Text> ();
			generateButtonText.text = "Generate";
			generateButtonText.fontSize = 24;
			generateButton.GetComponent<Image> ().sprite = buttonSprite;
			btnGenerate = generateButton.GetComponent<Button> ();
			btnGenerate.onClick.AddListener (
				() => {
					MapGenerator.Instance.generateMap ();
					btnGenerate.interactable = false;
				}
			);
			GameObject clearButton = DefaultControls.CreateButton (uiResources);
			clearButton.name = "Clear Button";
			clearButton.transform.SetParent (panelBG.transform);
			RectTransform clearButtonRectTransform = clearButton.GetComponent<RectTransform> ();
			clearButtonRectTransform.sizeDelta = new Vector2 ((panelBGSize.x - 30) / 2, 50);
			Text clearButtonText = clearButton.transform.GetChild (0).GetComponent<Text> ();
			clearButtonText.text = "Clear";
			clearButtonText.fontSize = 24;
			clearButton.GetComponent<Image> ().sprite = buttonSprite;
			btnClear = clearButton.GetComponent<Button> ();
			btnClear.onClick.AddListener (
				() => {
					MapGenerator.Instance.clearMap ();
					btnGenerate.interactable = true;
					btnClear.interactable = false;
				}
			);
			panelBGSize += new Vector2 (0, generateButtonRectTransform.sizeDelta.y + 20);

			// positioning
			float itemY = panelBGSize.y / 2;
			float itemX = -panelBGSize.x / 2 + 10;
			itemY -= (colorTitleRectTransform.sizeDelta.y + 10);
			colorTitleRectTransform.localPosition = new Vector3 (itemX + colorTitleRectTransform.sizeDelta.x / 2, itemY + colorTitleRectTransform.sizeDelta.y / 2, 0);
			for (int i = 0; i < MapGenerator.Instance.colorList.Length; i++) {
				itemY -= (colorToggleRectTransform [i].sizeDelta.y + 10);
				colorToggleRectTransform [i].localPosition = new Vector3 (itemX + colorToggleRectTransform [i].sizeDelta.x / 2, itemY + colorToggleRectTransform [i].sizeDelta.y / 2, 0);
				colorToggleRectTransform [i].GetChild(1).localPosition = new Vector3 (colorToggleRectTransform [i].sizeDelta.y / 2 + 10, 0, 0);
			}
			itemY -= (generateButtonRectTransform.sizeDelta.y + 10);
			generateButtonRectTransform.localPosition = new Vector3 (itemX + generateButtonRectTransform.sizeDelta.x / 2, itemY + generateButtonRectTransform.sizeDelta.y / 2, 0);
			itemX = panelBGSize.x / 2 - 10;
			clearButtonRectTransform.localPosition = new Vector3 (itemX - clearButtonRectTransform.sizeDelta.x / 2, itemY + clearButtonRectTransform.sizeDelta.y / 2, 0);

			RectTransform panelBGRectTransform = panelBGImage.GetComponent<RectTransform> ();
			panelBGRectTransform.sizeDelta = panelBGSize;
			panelBGRectTransform.localPosition = new Vector3 (-Screen.width / 2 + panelBGSize.x / 2 + 10, Screen.height / 2 - panelBGSize.y / 2 - 10, 0);

		}

		public void handleMouseLeftClick() {

			Vector3 mousePos = Input.mousePosition;
			RectTransform panelBGRectTransform = panelBG.GetComponent<RectTransform> ();
			if ((mousePos.x <= panelBGRectTransform.position.x - panelBGRectTransform.sizeDelta.x || mousePos.x >= panelBGRectTransform.position.x + panelBGRectTransform.sizeDelta.x)
			   || (mousePos.y <= panelBGRectTransform.position.y - panelBGRectTransform.sizeDelta.y || mousePos.y >= panelBGRectTransform.position.y + panelBGRectTransform.sizeDelta.y)) {
				Ray ray = mainCamera.cam.ScreenPointToRay (mousePos);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					MapChunk chunk = hit.collider.gameObject.GetComponent<MapChunk> ();
					if (chunk != null) {
						MapCell cell = chunk.getCellByPosition (hit.point);
						cell.color = MapGenerator.Instance.colorList [colorIndex].color;
						cell.currentChunk.updateMesh ();
						for (int i = 0; i < 8; i++) {
							if (cell.neighbours [i] >= 0 && MapGenerator.Instance.cells [cell.neighbours [i]].currentChunk != cell.currentChunk) {
								MapGenerator.Instance.cells [cell.neighbours [i]].currentChunk.updateMesh ();
							}
						}
					} else {
						Debug.Log ("Can't touch any cell!");
					}
				}
			}

		}

		#region IUpdateable
		// The update call will be called in prior if the priority is larger.
		public int priority {
			get;
			set;
		}

		public void updateEvent() {
			// Used to replace the Update().
			// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
			if (Input.GetMouseButton (0)) {
				handleMouseLeftClick ();
			}
			if (MapGenerator.Instance.generateFinish) {
				btnClear.interactable = true;
			}

		}
		#endregion

	}

}