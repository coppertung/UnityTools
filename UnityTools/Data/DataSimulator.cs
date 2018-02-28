using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityTools.Data.Node;

namespace UnityTools.Data {

	public class DataSimulator {

		public const char DS_SAVELOAD_PART_SEPERATOR = ';';
		public const char DS_SAVELOAD_SEPERATOR = ',';
		public const char DS_SAVELOAD_CHILD_START = '{';
		public const char DS_SAVELOAD_CHILD_END = '}';

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
			case DSNodeType.IntCal:
				_nodes.Add (new DSIntCalNode (idCount, position, this));
				break;
			case DSNodeType.FloatCal:
				_nodes.Add (new DSFloatCalNode (idCount, position, this));
				break;
			case DSNodeType.FloatToInt:
				_nodes.Add (new DSFloatToIntNode (idCount, position, this));
				break;
			case DSNodeType.IntToFloat:
				_nodes.Add (new DSIntToFloatNode (idCount, position, this));
				break;
			case DSNodeType.SetValue:
				_nodes.Add (new DSSetValueNode (idCount, position, this));
				break;
			case DSNodeType.Output:
				_nodes.Add (new DSOutputNode (idCount, position, this));
				break;
			case DSNodeType.IfStatement:
				_nodes.Add (new DSIfNode (idCount, position, this));
				break;
			}
			idCount += 1;

		}

		public void removeNode(DSNode node) {

			List<DSConnection> connectionsToRemove = new List<DSConnection> ();
			if (node.isSelectionNode) {
				DSSelectionNode selectionNode = (DSSelectionNode)node;
				for (int i = 0; i < _connections.Count; i++) {
					if (_connections [i].inPoint == selectionNode.inPoint || _connections [i].outPoint == selectionNode.trueOutPoint || _connections [i].outPoint == selectionNode.falseOutPoint) {
						connectionsToRemove.Add (_connections [i]);
					}
				}
			} else {
				for (int i = 0; i < _connections.Count; i++) {
					if (_connections [i].inPoint == node.inPoint || _connections [i].outPoint == node.outPoint) {
						connectionsToRemove.Add (_connections [i]);
					}
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

		public void simulate() {

			DSNode currentNode = _nodes.Find (x => x.title.Equals ("Start"));
			while (currentNode != null) {
				currentNode.execute ();
				DSConnection next = null;
				if (currentNode.isSelectionNode) {
					DSSelectionNode currentSelectionNode = (DSSelectionNode)currentNode;
					if (currentSelectionNode.result && currentSelectionNode.trueOutPoint != null) {
						next = _connections.Find (x => x.outPoint == currentSelectionNode.trueOutPoint);
					} else if (!currentSelectionNode.result && currentSelectionNode.falseOutPoint != null) {
						next = _connections.Find (x => x.outPoint == currentSelectionNode.falseOutPoint);
					} else {
						break;
					}
				} else {
					if (currentNode.outPoint == null) {
						break;
					}
					next = _connections.Find (x => x.outPoint == currentNode.outPoint);
				}
				if (next == null) {
					break;
				}
				currentNode = _nodes.Find (x => x.id == next.inPoint.nodeID);
			}

		}

		public void Save(string filepath, string filename) {

			if (!string.IsNullOrEmpty (filename)) {
				string fullPath = filepath + "/" + filename; 
				if (File.Exists (fullPath)) {
					File.Delete (fullPath);
				}
				StreamWriter writer = new StreamWriter (fullPath);
				StringBuilder builder = new StringBuilder ();
				builder.Append (DS_SAVELOAD_CHILD_START);
				for (int i = 0; i < _datas.Count; i++) {
					if (i > 0) {
						builder.Append (DS_SAVELOAD_SEPERATOR);
					}
					builder.Append (DS_SAVELOAD_CHILD_START);
					builder.Append (_datas [i].save ());
					builder.Append (DS_SAVELOAD_CHILD_END);
				}
				builder.Append (DS_SAVELOAD_CHILD_END);
				builder.Append (DS_SAVELOAD_PART_SEPERATOR);
				builder.Append (DS_SAVELOAD_CHILD_START);
				builder.Append (idCount);
				builder.Append (DS_SAVELOAD_SEPERATOR);
				for (int i = 0; i < _nodes.Count; i++) {
					if (i > 0) {
						builder.Append (DS_SAVELOAD_SEPERATOR);
					}
					builder.Append (DS_SAVELOAD_CHILD_START);
					builder.Append (_nodes [i].save ());
					builder.Append (DS_SAVELOAD_CHILD_END);
				}
				builder.Append (DS_SAVELOAD_CHILD_END);
				builder.Append (DS_SAVELOAD_PART_SEPERATOR);
				builder.Append (DS_SAVELOAD_CHILD_START);
				for (int i = 0; i < _connections.Count; i++) {
					if (i > 0) {
						builder.Append (DS_SAVELOAD_SEPERATOR);
					}
					builder.Append (DS_SAVELOAD_CHILD_START);
					builder.Append (_connections [i].save ());
					builder.Append (DS_SAVELOAD_CHILD_END);
				}
				builder.Append (DS_SAVELOAD_CHILD_END);
				writer.WriteLine (builder.ToString ());
				writer.Close ();
			}

		}

		public string Load(string filepath, string fileExtension) {

			string fullPath = EditorUtility.OpenFilePanel ("Open Saved File...", filepath, fileExtension);
			if (!string.IsNullOrEmpty (fullPath)) {
				string fileName = fullPath.Substring (fullPath.LastIndexOf ("/") + 1);
				fileName = fileName.Replace ("." + fileExtension, "");
				StreamReader reader = new StreamReader (fullPath);
				string saveString = reader.ReadLine ();
				reader.Close ();
				string[] partString = saveString.Split (DS_SAVELOAD_PART_SEPERATOR);
				parseDataString (partString [0]);
				parseNodeString (partString [1]);
				parseConnectionString (partString [2]);
				return fileName;
			} else {
				return null;
			}

		}

		public void parseDataString(string save) {

			if (_datas == null) {
				_datas = new List<DSDataField> ();
			}
			_datas.Clear ();

			StringBuilder buffer = new StringBuilder ();
			int level = 0;
			for (int i = 0; i < save.Length; i++) {
				if (save [i] == DS_SAVELOAD_CHILD_END) {
					level -= 1;
					if (level == 1) {
						 _datas.Add (new DSDataField ());
						 _datas [_datas.Count - 1].load (buffer.ToString ());
						buffer.Length = 0;
						buffer.Capacity = 0;
					}
				}
				if (level > 1) {
					buffer.Append (save [i]);
				}
				if (save [i] == DS_SAVELOAD_CHILD_START) {
					level += 1;
				} 
			}

		}

		public void parseNodeString(string save) {

			if (_nodes == null) {
				_nodes = new List<DSNode> ();
			}
			_nodes.Clear ();

			// get problem in this part
			StringBuilder buffer = new StringBuilder ();
			int level = 0;
			for(int i =0 ; i < save.Length; i++) {
				if (level > 1) {
					buffer.Append (save [i]);
				} else if (level == 1) {
					if (save [i] == DataSimulator.DS_SAVELOAD_SEPERATOR && buffer.Length > 0) {
						idCount = int.Parse (buffer.ToString ());
						buffer.Length = 0;
						buffer.Capacity = 0;
					} else if (save [i] != DataSimulator.DS_SAVELOAD_SEPERATOR) {
						buffer.Append (save [i]);
					}
				}
				if (save [i] == DS_SAVELOAD_CHILD_END) {
					level -= 1;
					if (level == 1) {
						string[] temp = buffer.ToString ().Substring (1, buffer.Length - 2).Split (DS_SAVELOAD_SEPERATOR);
						int nodeID = int.Parse (temp [1]);
						Vector2 position = new Vector2 (float.Parse (temp [2]), float.Parse (temp [3]));
						if (temp [0].Equals (DSNodeType.Start.ToString ())) {
							_nodes.Add (new DSNode (nodeID, position, this));
						} else if (temp [0].Equals (DSNodeType.IntCal.ToString ())) {
							_nodes.Add (new DSIntCalNode (nodeID, position, this));
						} else if (temp [0].Equals (DSNodeType.FloatCal.ToString ())) {
							_nodes.Add (new DSFloatCalNode (nodeID, position, this));
						} else if (temp [0].Equals (DSNodeType.FloatToInt.ToString ())) {
							_nodes.Add (new DSFloatToIntNode (nodeID, position, this));
						} else if (temp [0].Equals (DSNodeType.IntToFloat.ToString ())) {
							_nodes.Add (new DSIntToFloatNode (nodeID, position, this));
						} else if (temp [0].Equals (DSNodeType.Output.ToString ())) {
							_nodes.Add (new DSOutputNode (nodeID, position, this));
						} else if (temp [0].Equals (DSNodeType.SetValue.ToString ())) {
							_nodes.Add (new DSSetValueNode (nodeID, position, this));
						} else if (temp [0].Equals (DSNodeType.IfStatement.ToString ())) {
							_nodes.Add (new DSIfNode (nodeID, position, this));
						}
						_nodes [_nodes.Count - 1].load (buffer.ToString ().Substring (1, buffer.Length - 2));
						buffer.Length = 0;
						buffer.Capacity = 0;
					}
				}
				if (save [i] == DS_SAVELOAD_CHILD_START) {
					level += 1;
				} 
			}

		}

		public void parseConnectionString(string save) {

			if (_connections == null) {
				_connections = new List<DSConnection> ();
			}
			_connections.Clear ();

			StringBuilder buffer = new StringBuilder ();
			int level = 0;
			for (int i = 0; i < save.Length; i++) {
				if (save [i] == DS_SAVELOAD_CHILD_END) {
					level -= 1;
					if (level == 1) {
						_connections.Add (new DSConnection (this));
						_connections [_connections.Count - 1].load (buffer.ToString ());
						buffer.Length = 0;
						buffer.Capacity = 0;
					}
				}
				if (level > 1) {
					buffer.Append (save [i]);
				}
				if (save [i] == DS_SAVELOAD_CHILD_START) {
					level += 1;
				} 
			}

		}

	}

}