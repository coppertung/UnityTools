using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools.Data.Node;

namespace UnityTools.Data {

	public class DataSimulator {

		protected List<DSDataField> _datas;
		protected List<DSNode> _nodes;
		protected List<DSConnection> _connections;
		protected int idCount;

		public DSConnectionPoint selectedInPoint;
		public DSConnectionPoint selectedOutPoint;

		public List<DSDataField> datas {
			get {
				if (_datas == null) {
					_datas = new List<DSDataField> ();
				}
				return _datas;
			}
		}
		public List<DSNode> nodes {
			get {
				if (_nodes == null) {
					_nodes = new List<DSNode> ();
				}
				return _nodes;
			}
		}
		public List<DSConnection> connections {
			get {
				if (_connections == null) {
					_connections = new List<DSConnection> ();
				}
				return _connections;
			}
		}

		public DataSimulator() {

			_datas = new List<DSDataField> ();
			_nodes = new List<DSNode> ();
			initNode ();
			_connections = new List<DSConnection> ();
		}

		public void addData() {
			
			DSDataField newData = new DSDataField ();
			newData.name = "DataSet" + _datas.Count;
			_datas.Add (newData);

		}

		public void removeData(int dataIndex) {

			_datas.RemoveAt (dataIndex);

		}

		public void clearData() {
			
			_datas.Clear ();

		}

		public void initNode() {

			idCount = 0;
			addNode (DSNodeType.Start, new Vector2 (25f, 37.5f));

		}

		public void addNode(DSNodeType nodeType, Vector2 position) {

			switch (nodeType) {
			case DSNodeType.Start:
				_nodes.Add (new DSNode (idCount, position, this));
				break;
			case DSNodeType.MathAction:
				_nodes.Add (new DSMathActionNode (idCount, position, this));
				break;
			}
			idCount += 1;

		}

		public void removeNode(DSNode node) {

			List<DSConnection> connectionsToRemove = new List<DSConnection> ();
			for (int i = 0; i < _connections.Count; i++) {
				if (_connections [i].inPoint == node.inPoint || _connections [i].outPoint == node.outPoint) {
					connectionsToRemove.Add (_connections [i]);
				}
			}
			if (connectionsToRemove != null && connectionsToRemove.Count > 0) {
				for (int i = 0; i < connectionsToRemove.Count; i++) {
					removeConnection (connectionsToRemove [i]);
				}
			}
			_nodes.Remove (node);

		}

		public void resetNode() {

			_nodes.Clear ();
			_connections.Clear ();
			initNode ();

		}

		public void clearSelectedPoints() {

			selectedInPoint = null;
			selectedOutPoint = null;

		}

		public void createConnection() {

			DSConnection newConnection = new DSConnection (selectedInPoint, selectedOutPoint, this);
			DSConnection oldConnection = _connections.Find (x => x.outPoint == newConnection.outPoint);
			if (oldConnection != null) {
				_connections.Remove (oldConnection);
			}
			_connections.Add (newConnection);

		}

		public void removeConnection(DSConnection connection) {

			_connections.Remove (connection);

		}

		public void OnClickInPoint(DSConnectionPoint inPoint) {

			selectedInPoint = inPoint;
			if (selectedOutPoint != null) {
				if (selectedInPoint.nodeID != selectedOutPoint.nodeID) {
					createConnection ();
				}
				clearSelectedPoints ();
			}

		}

		public void OnClickOutPoint(DSConnectionPoint outPoint) {
			
			selectedOutPoint = outPoint;
			if (selectedInPoint != null) {
				if (selectedOutPoint.nodeID != selectedInPoint.nodeID) {
					createConnection ();
				}
				clearSelectedPoints ();
			}

		}

		public void Save(string filepath, string filename) {

//			if (!Directory.Exists (filepath)) {
//				Directory.CreateDirectory (filepath);
//			}
//			string fullPath = filepath + "/" + filename;
//			if (File.Exists (fullPath)) {
//				File.Delete (fullPath);
//			}

		}

		public void Load(string filepath) {
		}

	}

}