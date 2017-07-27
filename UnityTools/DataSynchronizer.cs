using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSynchronizer<T> {

	private List<T> _dataBuffer;
	private float _lastInput;

	public List<T> dataBuffer {
		get {
			return _dataBuffer;
		}
	}
	public float lastInput {
		get {
			return _lastInput;
		}
	}

	/// <summary>
	/// Initializes a new instance of the synchronizer
	/// </summary>
	public DataSynchronizer() {

		_dataBuffer = new List<T> ();

	}

	/// <summary>
	/// Add a data into data buffer
	/// </summary>
	public void addToBuffer(T data) {

		_dataBuffer.Add (data);
		_lastInput = Time.time;

	}

	/// <summary>
	/// Clear the data buffer
	/// </summary>
	public void clearBuffer() {

		_dataBuffer.Clear ();
		_lastInput = 0;

	}

	/// <summary>
	/// Do the synchronization and clear the data buffer
	/// Will return the most suitable data through sorting (Using IComparable to define the comparsion method)
	/// </summary>
	public T doSynchronization () {

		// get the most suitable data by sorting them
		_dataBuffer.Sort ();
		// get the first one
		T result = _dataBuffer [0];
		clearBuffer ();
		return result;

	}

}
