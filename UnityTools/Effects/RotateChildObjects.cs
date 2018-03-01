using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools;
using UnityTools.Patterns;

namespace UnityTools.Effects {

	public class RotateChildObjects : MonoBehaviour, IUpdateable {

		public enum RotateAxis {

			xAxis = 0, yAxis = 1, zAxis = 2

		}

		public List<GameObject> childObjects;
		public float radius;
		public Vector3 rotation;
		public RotateAxis rotateAxis;
		public bool rotateObjectEular;

		public bool autoRotate;
		public float autoSpeed;

		public float angle;

		public int priority {
			get;
			set;
		}

		public void updateEvent() {

			circulation ();

		}

		void Awake() {

			init ();
			UpdateManager.RegisterUpdate (this);

		}
			
		void OnDestroy() {

			UpdateManager.UnregisterUpdate (this);

		}

		public void init() {

			if (childObjects == null) {
				childObjects = new List<GameObject> ();
			}
			childObjects.Clear ();
			for (int i = 0; i < transform.childCount; i++) {
				childObjects.Add (transform.GetChild (i).gameObject);
			}
			transform.localEulerAngles = rotation;

		}

		public void circulation() {

			if (autoRotate) {
				rotate (autoSpeed * Time.deltaTime);
			}
			for (int i = 0; i < childObjects.Count; i++) {
				float radian = Mathf.Deg2Rad * (angle + i * 360f / childObjects.Count);
				switch(rotateAxis) {
				case RotateAxis.xAxis:
					// not yet test
					childObjects [i].transform.localPosition = new Vector3 (0, radius * Mathf.Sin (radian), radius * Mathf.Cos (radian));
					if (rotateObjectEular) {
						childObjects [i].transform.localEulerAngles = new Vector3 (
							(angle + i * 360f / childObjects.Count) * -1,
							childObjects [i].transform.localEulerAngles.y,
							childObjects [i].transform.localEulerAngles.z
						);
					}
					break;
				case RotateAxis.yAxis:
					childObjects [i].transform.localPosition = new Vector3 (radius * Mathf.Sin (radian), 0, -radius * Mathf.Cos (radian));
					if (rotateObjectEular) {
						childObjects [i].transform.localEulerAngles = new Vector3 (
							childObjects [i].transform.localEulerAngles.x,
							(angle + i * 360f / childObjects.Count) * -1,
							childObjects [i].transform.localEulerAngles.z
						);
					}
					break;
				case RotateAxis.zAxis:
					// not yet test
					childObjects [i].transform.localPosition = new Vector3 (radius * Mathf.Sin (radian), radius * Mathf.Cos (radian), 0);
					if (rotateObjectEular) {
						childObjects [i].transform.localEulerAngles = new Vector3 (
							childObjects [i].transform.localEulerAngles.x,
							childObjects [i].transform.localEulerAngles.y,
							(angle + i * 360f / childObjects.Count) * -1
						);
					}
					break;
				}
			}

		}

		public void rotate(float angle) {

			this.angle += angle;
			if (this.angle > 360f) {
				this.angle -= 360f;
			} else if (this.angle < 0f) {
				this.angle += 360f;
			}

		}

	}

}