using System.Collections.Generic;

namespace UnityTools {

    public class DataSynchronizer<T> {

		#region Delegate_Functions_Declaration
		public delegate bool SynchronizationMethod();
		#endregion

		#region Fields_And_Properties
		/// <summary>
		/// A Buffer used to store the expected respond of the requests.
		/// </summary>
		public List<T> requestBuffer {
			get;
			private set;
		}
		/// <summary>
		/// A Buffer used to store the actual responds.
		/// </summary>
		public List<T> respondBuffer {
			get;
			private set;
		}	
		/// <summary>
		/// Count for errors.
		/// </summary>
		public int errorCount {
			get;
			private set;
		}

		/// <summary>
		/// Return true if there is no error and return false if there is any.
		/// Call doSynchronization.Invoke() to do synchronization.
		/// </summary>
		public SynchronizationMethod doSynchronization {
			get;
			private set;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the synchronizer.
		/// </summary>
		public DataSynchronizer() {

			requestBuffer = new List<T> ();
			respondBuffer = new List<T> ();

		}
		#endregion

		#region Functions
		/// <summary>
		/// Add the sychronization function to the delegated function doSynchronization (Read-only).
		/// </summary>
		public void addSyncMethod(SynchronizationMethod method) {

			doSynchronization += method;

		}

		/// <summary>
		/// Add the sychronization function to the delegated function doSynchronization (Read-only).
		/// </summary>
		public void removeSyncMethod(SynchronizationMethod method) {

			doSynchronization += method;

		}

		/// <summary>
		/// Add a data into request buffer.
		/// </summary>
		public void addRequest(T data) {

			requestBuffer.Add (data);

		}

		/// <summary>
		/// Add a data into respond buffer.
		/// </summary>
		public void addRespond(T data) {

			respondBuffer.Add (data);

		}

		/// <summary>
		/// Remove a data from request buffer.
		/// </summary>
		public void removeRequest(T data) {
	
			requestBuffer.Remove (data);
	
		}

		/// <summary>
		/// Remove a data into respond buffer.
		/// </summary>
		public void removeRespond(T data) {
	
			respondBuffer.Remove (data);

		}

		/// <summary>
		/// Clear the data buffers (both request and respond).
		/// </summary>
		public void clearBuffers() {

			requestBuffer.Clear ();
			respondBuffer.Clear ();

		}

		/// <summary>
		/// Clear the request buffer.
		/// </summary>
		public void clearRequest() {

			requestBuffer.Clear ();

		}

		/// <summary>
		/// Clear the respond buffer.
		/// </summary>
		public void clearRespond() {

			respondBuffer.Clear ();

		}

		/// <summary>
		/// Add 1 to the error count.
		/// This mean an error occurred.
		/// </summary>
		public void addErrorCount() {

			errorCount += 1;

		}

		/// <summary>
		/// Resets the error count.
		/// </summary>
		public void resetErrorCount() {

			errorCount = 0;

		}
		#endregion

	}

}