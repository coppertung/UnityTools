using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.AI {

	public class FlockController : MonoBehaviour, IUpdateable {

		// The update call will be called in prior if the priority is larger.
		public int priority {
			get;
			set;
		}
		[SerializeField]
		private List<Boid> _boidList;
		private Dictionary<int, int> _boidGroup;

		/// <summary>
		/// List of boids that is being controlled by the controller.
		/// </summary>
		public List<Boid> boidList {
			get {
				return _boidList;
			}
		}
		/// <summary>
		/// Dictionary to record the group that the boids belong to.
		/// </summary>
		public Dictionary<int, int> boidGroup {
			get {
				return _boidGroup;
			}
		}

		/// <summary>
		/// Update group rate, which means the group of the boids will be updated per how much frames.
		/// Default is 10.
		/// </summary>
		public int updateGroupRate = 10;

		/// <summary>
		/// Register the specified boid.
		/// </summary>
		public void register(Boid boid) {

			if (_boidList == null) {
				_boidList = new List<Boid> ();
			}
			_boidList.Add (boid);
			addToDict (boid);

		}

		/// <summary>
		/// Unregister the specified boid.
		/// </summary>
		public void unregister(Boid boid) {

			_boidList.Remove (boid);
			_boidGroup.Remove (boid.GetInstanceID ());

		}

		/// <summary>
		/// Get the neighbour boids of the specified boid.
		/// </summary>
		public List<Boid> foundNeighbours(Boid boid) {

			List<Boid> neighbours = new List<Boid> ();
			for (int i = 0; i < _boidList.Count; i++) {
				if (_boidList [i].groupID == boid.groupID && _boidList [i] != boid && Vector3.Distance (_boidList [i].transform.position, boid.transform.position) < boid.neighbourRadius) {
					neighbours.Add (boidList [i]);
				}
			}
			return neighbours;

		}

		/// <summary>
		/// Set direction position to all boids, this will initialize the velocity vector of the boids.
		/// </summary>
		public void setDirection(Vector3 position) {

			for (int i = 0; i < _boidList.Count; i++) {
				_boidList [i].velocity = (position - _boidList [i].transform.position).normalized * _boidList [i].speed;
			}

		}

		/// <summary>
		/// Set direction to all boids of the specified group, this will initialize the velocity vector of the boids.
		/// </summary>
		public void setDirection(Vector3 position, int groupID) {

			for (int i = 0; i < _boidList.Count; i++) {
				if (_boidList [i].groupID == groupID) {
					_boidList [i].velocity = (position - _boidList [i].transform.position).normalized * _boidList [i].speed;
				}
			}

		}

		public void updateEvent() {
			// Used to replace the Update().
			// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
			if (Time.frameCount % updateGroupRate == 0) {
				updateBoidGroups ();
			}

		}

		private void addToDict(Boid boid) {

			if (_boidGroup == null) {
				_boidGroup = new Dictionary<int, int> ();
			}
			int groupID = -1;
			int maxGroupID = -1;
			for (int i = 0; i < _boidList.Count; i++) {
				if (_boidList [i] != boid) {
					int targetGroupID;
					_boidGroup.TryGetValue (_boidList [i].GetInstanceID (), out targetGroupID);
					if (Vector3.Distance (boid.transform.position, _boidList [i].transform.position) < boid.neighbourRadius) {
						if (groupID == -1 || Utils.Random (100) < _boidList [i].moveToNeighbourGroupChance) {
							groupID = targetGroupID;
						}
					} else {
						if (targetGroupID > maxGroupID) {
							maxGroupID = targetGroupID;
						}
					}
				}
			}
			boid.groupID = (groupID == -1 ? (maxGroupID + 1) : groupID);
			boid.neighbours = foundNeighbours (boid);
			_boidGroup.Add (boid.GetInstanceID (), boid.groupID);

		}

		// **REQUIRE ENHANCEMENT**
		private void updateBoidGroups() {

			for (int i = 0; i < _boidList.Count; i++) {
				bool moveToNeighbour = false;
				int selfID = _boidList [i].GetInstanceID ();
				int selfGroupID;
				int maxGroupID = -1;
				for (int j = 0; j < _boidList.Count; j++) {
					if (i != j) {
						int neighbourID = _boidList [j].GetInstanceID ();
						int neighbourGroupID;
						_boidGroup.TryGetValue (selfID, out selfGroupID);
						_boidGroup.TryGetValue (neighbourID, out neighbourGroupID);
						if (neighbourGroupID > maxGroupID) {
							maxGroupID = neighbourGroupID;
						}
						if (selfGroupID != neighbourGroupID && Vector3.Distance (_boidList [i].transform.position, _boidList [j].transform.position) < _boidList [i].neighbourRadius
							&& Utils.Random (100) < _boidList [i].moveToNeighbourGroupChance) {
							_boidGroup [selfID] = neighbourGroupID;
							moveToNeighbour = true;
							break;
						}
					}
				}
				if (!moveToNeighbour && Utils.Random (100) < _boidList [i].getOutFromOriginalGroupChance) {
					_boidGroup [selfID] = maxGroupID + 1;
				}
				_boidGroup.TryGetValue (selfID, out _boidList [i].groupID);
				_boidList [i].neighbours = foundNeighbours (_boidList [i]);
			}

		}
			
	}

}