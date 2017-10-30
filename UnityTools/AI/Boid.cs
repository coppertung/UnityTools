using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AI {

	/// <summary>
	/// **REQUIRE TEST**
	/// Abstract class used in FlockController.
	/// Used to control the basic movement of the single boid.
	/// By Craig Reynold's definition, Boid is a term that refers to a bird-like object, and used to describe each individual object in
	/// flock algorithm.
	/// Reference from:
	/// http://www.red3d.com/cwr/boids/
	/// https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444.
	/// </summary>
	public abstract class Boid : MonoBehaviour, IFixedUpdateable {

		#region Fields_And_Properties
		/// <summary>
		/// Group ID of the boid.
		/// </summary>
		public int groupID;
		/// <summary>
		/// List of neighbour boids that is in the same group of this boid.
		/// </summary>
		public List<Boid> neighbours;
		/// <summary>
		/// Distance that can be detected as the neighbour of the boid.
		/// </summary>
		public float neighbourRadius;
		/// <summary>
		/// Chance of the boid move from current group to a neighbour group.
		/// Default is 50%.
		/// </summary>
		public float moveToNeighbourGroupChance = 50;
		/// <summary>
		/// Chance of the boid get out from the current group.
		/// Default is 5%.
		/// </summary>
		public float getOutFromOriginalGroupChance = 5;
		/// <summary>
		/// FlockController that is used to control this boid.
		/// </summary>
		public FlockController flockController {
			get {
				return _flockController;
			}
			set {
				if (_flockController != null) {
					_flockController.unregister (this);
				}
				_flockController = value;
				_flockController.register (this);
			}
		}
		[SerializeField]
		private FlockController _flockController;

		/// <summary>
		/// Weight of alignment variable [0,1].
		/// Default is 1.
		/// </summary>
		[Range(0, 1)]
		public float alignmentWeight = 1;
		/// <summary>
		/// Weight of cohesion variable [0,1].
		/// Default is 1.
		/// </summary>
		[Range(0, 1)]
		public float cohesionWeight = 1;
		/// <summary>
		/// Weight of seperation variable [0,1].
		/// Default is 1.
		/// </summary>
		[Range(0, 1)]
		public float seperationWeight = 1;
		/// <summary>
		/// Speed factor of the boid.
		/// </summary>
		public float speed;
		/// <summary>
		/// Velocity of the boid.
		/// </summary>
		public Vector3 velocity;
		#endregion

		#region MonoBehaviour
		protected virtual void OnEnable() {
			
			UpdateManager.RegisterFixedUpdate (this);
			if (_flockController != null) {
				_flockController.register (this);
			}

		}

		protected virtual void OnDisable() {

			UpdateManager.UnregisterFixedUpdate (this);
			if (_flockController != null) {
				_flockController.unregister (this);
			}

		}
		#endregion

		#region Functions
		/// <summary>
		/// This function is used to find out the alignment vector.
		/// By definition, alignment is the behaviour that causes a particular agent to line up with agents close by.
		/// </summary>
		protected virtual Vector3 alignment() {

			Vector3 direction = new Vector3 ();
			int count = 0;
			for (int i = 0; i < neighbours.Count; i++) {
				if (neighbours [i].groupID == groupID) {
					direction += neighbours [i].velocity;
					count++;
				}
			}
			direction /= count;
			return direction.normalized;

		}

		/// <summary>
		/// This function is used to find out the cohesion vector.
		/// By definition, cohesion is the behaviour that causes agents to steer towards the "center of mass" - that is, the average position
		/// of the agents within a certain radius.
		/// </summary>
		protected virtual Vector3 cohesion() {

			Vector3 direction = new Vector3 ();
			float distance = 0;
			for (int i = 0; i < neighbours.Count; i++) {
				direction += neighbours [i].transform.position;
				distance += Vector3.Distance (neighbours [i].transform.position, transform.position);
			}
			direction /= neighbours.Count;
			distance /= neighbours.Count;
			return (direction - transform.position).normalized;

		}

		/// <summary>
		/// This function is used to find out the seperation vector.
		/// By definition, seperation is the behaviour that causes an agent to steer away from all of it neighbours.
		/// </summary>
		protected virtual Vector3 seperation() {

			Vector3 direction = new Vector3 ();
			for (int i = 0; i < neighbours.Count; i++) {
				direction += neighbours[i].transform.position - transform.position;
			}
			direction /= neighbours.Count;
			direction *= -1;
			return direction.normalized;

		}
		#endregion

		#region IFixedUpdateable
		// The update call will be called in prior if the priority is larger.
		public int priority {
			get;
			set;
		}

		public virtual void fixedUpdateEvent() {
			// Used to replace the FixedUpdate().
			// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
			if (neighbours.Count > 0) {
				Vector3 alignmentDirection = alignment () * alignmentWeight;
				Vector3 cohesionDirection = cohesion () * cohesionWeight;
				Vector3 seperationDirection = seperation () * seperationWeight;
				velocity = (alignmentDirection + cohesionDirection + seperationDirection).normalized * speed * Time.deltaTime;
				Debug.Log ("Alignment: " + alignmentDirection + "[" + alignmentWeight + "], Cohesion: " + cohesionDirection + "[" + cohesionWeight + "], Seperation: "
				+ seperationDirection + "[" + seperationWeight + "] | velocity: (" + velocity.x + ", " + velocity.y + ", " + velocity.z + ")");
			}
			if (velocity.magnitude != 0) {
				transform.Translate (velocity);
			}

		}
		#endregion

	}

}