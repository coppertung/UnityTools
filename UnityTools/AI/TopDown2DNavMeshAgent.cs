using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools;

public class TopDown2DNavMeshAgent : MonoBehaviour, IUpdateable {

	// The update call will be called in prior if the priority is larger.
	public int priority {
		get;
		set;
	}

	public void updateEvent() {
		// Used to replace the Update().
		// Noted that it will be automatically called by the Update Manager once it registered with UpdateManager.Register.
	}

}
