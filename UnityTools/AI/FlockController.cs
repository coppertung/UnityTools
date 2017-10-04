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
		private List<Boid> boidList;
		private Dictionary<int, int> boidGroup;

		public int updateGroupRate = 1;

		public void register(Boid boid) {

			if (boidList == null) {
				boidList = new List<Boid> ();
			}
			boidList.Add (boid);
			addToDict (boid);

		}

		public void unregister(Boid boid) {

			boidList.Remove (boid);
			boidGroup.Remove (boid.GetInstanceID ());

		}

		public void updateEvent() {
			// Used to replace the Update().
			// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
			if (Time.frameCount % updateGroupRate == 0) {
				updateBoidGroups ();
			}

		}

		private void addToDict(Boid boid) {

			if (boidGroup == null) {
				boidGroup = new Dictionary<int, int> ();
			}
			int groupID = -1;
			int maxGroupID = -1;
			for (int i = 0; i < boidList.Count; i++) {
				if (boidList [i] != boid) {
					int targetGroupID;
					boidGroup.TryGetValue (boidList [i].GetInstanceID (), out targetGroupID);
					if (Vector3.Distance (boid.transform.position, boidList [i].transform.position) < boid.neighbourRadius) {
						if (groupID == -1 || Utils.Random (100) < boidList [i].moveToNeighbourGroupChance) {
							groupID = targetGroupID;
						}
					} else {
						if (targetGroupID > maxGroupID) {
							maxGroupID = targetGroupID;
						}
					}
				}
			}
			boidGroup.Add (boid.GetInstanceID (), (groupID == -1 ? (maxGroupID + 1) : groupID));

		}

		private void updateBoidGroups() {

			for (int i = 0; i < boidList.Count; i++) {
				bool moveToNeighbour = false;
				int selfID = boidList [i].GetInstanceID ();
				int selfGroupID;
				int maxGroupID = -1;
				for (int j = 0; j < boidList.Count; j++) {
					if (i != j) {
						int neighbourID = boidList [j].GetInstanceID ();
						int neighbourGroupID;
						boidGroup.TryGetValue (selfID, out selfGroupID);
						boidGroup.TryGetValue (neighbourID, out neighbourGroupID);
						if (neighbourGroupID > maxGroupID) {
							maxGroupID = neighbourGroupID;
						}
						if (selfGroupID != neighbourGroupID && Vector3.Distance (boidList [i].transform.position, boidList [j].transform.position) < boidList [i].neighbourRadius
						    && Utils.Random (100) < boidList [i].moveToNeighbourGroupChance) {
							boidGroup [selfID] = neighbourGroupID;
							moveToNeighbour = true;
							break;
						}
					}
				}
				if (!moveToNeighbour && Utils.Random (100) < boidList [i].getOutFromOriginalGroupChance) {
					boidGroup [selfID] = maxGroupID + 1;
				}
			}

		}
			
	}

}