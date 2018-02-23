using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityTools.Data.DataType;

namespace UnityTools.Data.Node {

	public class DSSelectionNode : DSNode {

		public DSConnectionPoint trueOutPoint;
		public DSConnectionPoint falseOutPoint;

		public bool result;

		public DSSelectionNode(int id, Vector2 position, DataSimulator ds) {

			this.id = id;
			this.ds = ds;
			isSelectionNode = true;
			inPoint = new DSConnectionPoint (id, DSConnectionPointType.In, ds);
			trueOutPoint = new DSConnectionPoint (id, DSConnectionPointType.TrueOut, ds);
			falseOutPoint = new DSConnectionPoint (id, DSConnectionPointType.FalseOut, ds);

		}

		public override void drawInOutPoint () {

			if (inPoint != null) {
				inPoint.draw ();
			}
			if (trueOutPoint != null) {
				trueOutPoint.draw ();
			}
			if (falseOutPoint != null) {
				falseOutPoint.draw ();
			}

		}

	}

}