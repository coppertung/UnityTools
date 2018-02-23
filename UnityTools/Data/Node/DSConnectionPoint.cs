using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Data.Node {

	public enum DSConnectionPointType {
		In = 0, Out = 1, TrueOut = 2, FalseOut = 3
	}

	public class DSConnectionPoint {

		public Rect rect;
		public string letter;
		public DSConnectionPointType type;
		public int nodeID;

		private DataSimulator ds;
		private DSNode node;

		public DSConnectionPoint (int nodeID, DSConnectionPointType type, DataSimulator ds) {

			this.ds = ds;
			this.nodeID = nodeID;
			this.type = type;
			rect = new Rect (0, 0, 15, 20);
			switch (type) {
			case DSConnectionPointType.In:
				letter = "<";
				break;
			case DSConnectionPointType.Out:
				letter = ">";
				break;
			case DSConnectionPointType.TrueOut:
				letter = "T";
				break;
			case DSConnectionPointType.FalseOut:
				letter = "F";
				break;
			}

		}

		public void draw() {

			if (node != null) {
				switch (type) {
				case DSConnectionPointType.In:
					rect.x = node.rect.x - rect.width + 1f;
					rect.y = node.rect.y + node.rect.height / 2 - rect.height / 2;
					break;
				case DSConnectionPointType.Out:
					rect.x = node.rect.x + node.rect.width - 1f;
					rect.y = node.rect.y + node.rect.height / 2 - rect.height / 2;
					break;
				case DSConnectionPointType.TrueOut:
					rect.x = node.rect.x + node.rect.width - 1f;
					rect.y = node.rect.y + node.rect.height / 4 - rect.height / 2;
					break;
				case DSConnectionPointType.FalseOut:
					rect.x = node.rect.x + node.rect.width - 1f;
					rect.y = node.rect.y + 3 * node.rect.height / 4 - rect.height / 2;
					break;
				}
				if (GUI.Button (rect, letter, GUI.skin.box)) {
					switch (type) {
					case DSConnectionPointType.In:
						ds.OnClickInPoint (this);
						break;
					case DSConnectionPointType.Out:
					case DSConnectionPointType.TrueOut:
					case DSConnectionPointType.FalseOut:
						ds.OnClickOutPoint (this);
						break;
					}
				}
			} else {
				node = ds.nodes.Find (x => x.id == nodeID);
			}

		}

	}

}