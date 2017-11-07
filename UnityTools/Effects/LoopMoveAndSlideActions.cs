using UnityEngine;

namespace UnityTools.Effects {

	/// <summary>
	/// Loop move action.
	/// **REQUIRE TESTING**
	/// </summary>
	[System.Serializable]
	public class LoopMove {

		#region Enumerations
		public enum Direction {
			top, bottom, left, right,
			topLeft, topRight,
			bottomLeft, bottomRight
		}
		#endregion

		#region Fields_And_Properties
		/// <summary>
		/// Is this action activated?
		/// </summary>
		public bool isActive = false;

		[SerializeField]
		private float _speed;
		[SerializeField]
		private float _objectDistance;
		private LoopMoveAndSlide parent = null;
		private Vector3 velocity;

		[SerializeField]
		/// <summary>
		/// Direction of objects movement.
		/// </summary>
		public Direction direction;
		/// <summary>
		/// Maximum displayed objects number on screen.
		/// </summary>
		public int maxDisplay;
		/// <summary>
		/// Distance between two objects.
		/// </summary>
		public float objectDistance {
			get {
				return _objectDistance;
			}
		}
		/// <summary>
		/// Speed of objects movement.
		/// </summary>
		public float speed {
			get {
				return _speed;
			}
		}
		#endregion

		#region Functions
		/// <summary>
		/// Initialization.
		/// </summary>
		public void init(LoopMoveAndSlide parent) {

			this.parent = parent;
			switch (direction) {
			case Direction.top:
				velocity = new Vector3 (0, speed);
				break;
			case Direction.topLeft:
				velocity = new Vector3 (-speed, speed);
				break;
			case Direction.topRight:
				velocity = new Vector3 (speed, speed);
				break;
			case Direction.bottom:
				velocity = new Vector3 (0, -speed);
				break;
			case Direction.bottomLeft:
				velocity = new Vector3 (-speed, -speed);
				break;
			case Direction.bottomRight:
				velocity = new Vector3 (speed, -speed);
				break;
			case Direction.left:
				velocity = new Vector3 (-speed, 0);
				break;
			case Direction.right:
				velocity = new Vector3 (speed, 0);
				break;
			}

		}

		/// <summary>
		/// Update the position of the loop objects.
		/// **REQUIRE TESTING FOR THE POSITION**
		/// </summary>
		public void updatePosition() {
			
			if (isActive && parent != null) {
				for (int i = 0; i < parent.loopObjects.Length; i++) {
					parent.loopObjects [i].transform.Translate (velocity * Time.deltaTime);
					if (direction == Direction.left || direction == Direction.topLeft || direction == Direction.bottomLeft) {
						if (parent.loopObjects [i].transform.localPosition.x < parent.transform.localPosition.x - objectDistance * (float)maxDisplay) {
							parent.loopObjects [i].transform.localPosition = new Vector3 (
								parent.loopObjects [(i > 0 ? i : parent.loopObjects.Length) - 1].transform.localPosition.x + objectDistance,
								parent.loopObjects [i].transform.localPosition.y,
								0
							);
						}
					} else if (direction == Direction.right || direction == Direction.topRight || direction == Direction.bottomRight) {
						if (parent.loopObjects [i].transform.localPosition.x < parent.transform.localPosition.x + objectDistance * (float)maxDisplay) {
							parent.loopObjects [i].transform.localPosition = new Vector3 (
								parent.loopObjects [i < parent.loopObjects.Length - 1 ? i + 1 : 0].transform.localPosition.x - objectDistance,
								parent.loopObjects [i].transform.localPosition.y,
								0
							);
						}
					}
					if (direction == Direction.top || direction == Direction.topLeft || direction == Direction.topRight) {
						if (parent.loopObjects [i].transform.localPosition.y > parent.transform.localPosition.y + objectDistance * (float)maxDisplay) {
							parent.loopObjects [i].transform.localPosition = new Vector3 (
								parent.loopObjects [i].transform.localPosition.x,
								parent.loopObjects [(i > 0 ? i : parent.loopObjects.Length) - 1].transform.localPosition.y - objectDistance,
								0
							);
						}
					} else if (direction == Direction.bottom || direction == Direction.bottomLeft || direction == Direction.bottomRight) {
						if (parent.loopObjects [i].transform.localPosition.y < parent.transform.localPosition.y - objectDistance * (float)maxDisplay) {
							parent.loopObjects [i].transform.localPosition = new Vector3 (
								parent.loopObjects [i].transform.localPosition.x,
								parent.loopObjects [i < parent.loopObjects.Length - 1 ? i + 1 : 0].transform.localPosition.y + objectDistance,
								0
							);
						}
					}
				}
			}

		}
		#endregion

	}

	[System.Serializable]
	public class SlideMove {
	}

}
