using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools {

	public class DataSynchronizer<T> {

		private List<T> _requestBuffer;
		private List<T> _respondBuffer;
		private int _errorCount;

		/// <summary>
		/// A Buffer used to store the expected respond of the requests.
		/// </summary>
		public List<T> requestBuffer {
			get {
				return _requestBuffer;
			}
		}
		/// <summary>
		/// A Buffer used to store the actual responds.
		/// </summary>
		public List<T> respondBuffer {
			get {
				return _respondBuffer;
			}
		}	
		/// <summary>
		/// Count for errors.
		/// </summary>
		public int errorCount {
			get {
				return _errorCount;
			}
		}
		/// <summary>
		/// Implemented by the users.
		/// Usually used to check for invalid data.
		/// </summary>
		public readonly Func<T> doSynchronization;

		/// <summary>
		/// Initializes a new instance of the synchronizer.
		/// Noted that self-defined synchronize method should be included.
		/// </summary>
		public DataSynchronizer(Func<T> synchronizeMethod) {

			_requestBuffer = new List<T> ();
			_respondBuffer = new List<T> ();
			doSynchronization = synchronizeMethod;

		}

		/// <summary>
		/// Add a data into request buffer.
		/// </summary>
		public void addRequest(T data) {

			_requestBuffer.Add (data);

		}

		/// <summary>
		/// Add a data into respond buffer.
		/// </summary>
		public void addRespond(T data) {

			_respondBuffer.Add (data);

		}

		/// <summary>
		/// Remove a data from request buffer.
		/// </summary>
		public void removeRequest(T data) {
	
			_requestBuffer.Remove (data);
	
		}

		/// <summary>
		/// Remove a data into respond buffer.
		/// </summary>
		public void removeRespond(T data) {
	
			_respondBuffer.Remove (data);

		}

		/// <summary>
		/// Clear the data buffer.
		/// </summary>
		public void clearBuffers() {

			_requestBuffer.Clear ();
			_respondBuffer.Clear ();

		}

		/// <summary>
		/// Clear the request buffer.
		/// </summary>
		public void clearRequest() {

			_requestBuffer.Clear ();

		}

		/// <summary>
		/// Clear the respond buffer.
		/// </summary>
		public void clearRespond() {

			_respondBuffer.Clear ();

		}

		/// <summary>
		/// Add 1 to the error count.
		/// This mean an error occurred.
		/// </summary>
		public void addErrorCount() {

			_errorCount += 1;

		}

		/// <summary>
		/// Resets the error count.
		/// </summary>
		public void resetErrorCount() {

			_errorCount = 0;

		}

	}

}