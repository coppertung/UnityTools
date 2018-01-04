using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Map {

	public class MapEditor : MonoBehaviour, IUpdateable {

		public Camera mainCamera;

		#region MonoBehaviour
		void OnEnable() {

			UpdateManager.RegisterUpdate (this);

		}

		void OnDisable() {

			UpdateManager.UnregisterUpdate (this);

		}
		#endregion

		public void handleInput() {

			Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				MapCell cell = hit.collider.gameObject.GetComponent<MapCell> ();
				if (cell != null) {
					cell.color = MapGenerator.Instance.selectedColor;
					cell.createMesh ();
				} else {
					Debug.Log ("Can't touch any cell!");
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
				handleInput ();
			}

		}
		#endregion

	}

}