using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityTools.Network {

	public class HTTPConnection {

		/// <summary>
		/// Sending a GET request to the server.
		/// T stands for response object.
		/// </summary>
		public static IEnumerator GET<T>(string url, Action<T> responseCallback, Action<Exception> errorHandler) {

			UnityWebRequest request = UnityWebRequest.Get (url);
			#if UNITY_2017_3_OR_NEWER
			yield return request.SendWebRequest ();
			#else
			yield return request.Send ();
			#endif

			try {
				#if UNITY_2017_1_OR_NEWER
				if (request.isNetworkError) {
				errorHandler (new Exception ("Network Error"));
				} else if (request.isHttpError) {
				errorHandler (new Exception ("HTTP Error"));
				#else
				if (request.isError) {
					errorHandler (new Exception ("Network Error: " + request.error));
				#endif
				} else {
					responseCallback (JsonUtility.FromJson<T> (request.downloadHandler.text));
				}
			}
			catch(Exception ex) {
				Debug.Log(ex.Message);
				Debug.Log(request.downloadHandler.text);
			}

		}

		/// <summary>
		/// Sending a GET request to the server with header in order to pass cookies or tokens, noted that the keys of header are case-sensitive.
		/// T stands for response object.
		/// **REQUIRE TESTING LATER**
		/// </summary>
		public static IEnumerator GET<T>(string url, Hashtable header, Action<T> responseCallback, Action<Exception> errorHandler) {

			UnityWebRequest request = UnityWebRequest.Get (url);
			foreach (DictionaryEntry headerValue in header) {
				request.SetRequestHeader ((string)headerValue.Key, (string)headerValue.Value);
			}
			#if UNITY_2017_3_OR_NEWER
			yield return request.SendWebRequest ();
			#else
			yield return request.Send ();
			#endif

			try {
				#if UNITY_2017_1_OR_NEWER
				if (request.isNetworkError) {
				errorHandler (new Exception ("Network Error"));
				} else if (request.isHttpError) {
				errorHandler (new Exception ("HTTP Error"));
				#else
				if (request.isError) {
					errorHandler (new Exception ("Network Error: " + request.error));
				#endif
				} else {
					responseCallback (JsonUtility.FromJson<T> (request.downloadHandler.text));
				}
			}
			catch(Exception ex) {
				Debug.Log(ex.Message);
				Debug.Log(request.downloadHandler.text);
			}

		}

		/// <summary>
		/// Sending a POST request to the server, noted that data sent must be a json string.
		/// T stands for response object.
		/// </summary>
		public static IEnumerator POST<T>(string url, string jsonData, Action<T> responseCallback, Action<Exception> errorHandler) {
				
			byte[] postData = Encoding.UTF8.GetBytes (jsonData);
			UnityWebRequest request = new UnityWebRequest (url, "POST");
			request.uploadHandler = new UploadHandlerRaw(postData);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader ("Content-Type", "application/json");
			#if UNITY_2017_3_OR_NEWER
			yield return request.SendWebRequest ();
			#else
			yield return request.Send ();
			#endif

			try {
				#if UNITY_2017_1_OR_NEWER
				if (request.isNetworkError) {
				errorHandler (new Exception ("Network Error"));
				} else if (request.isHttpError) {
				errorHandler (new Exception ("HTTP Error"));
				#else
				if (request.isError) {
					errorHandler (new Exception ("Network Error: " + request.error));
				#endif
				} else {
					responseCallback (JsonUtility.FromJson<T> (request.downloadHandler.text));
				}
			}
			catch(Exception ex) {
				Debug.Log(ex.Message);
				Debug.Log(request.downloadHandler.text);
			}

		}
	
		/// <summary>
		/// Sending a POST request to the server and get a cookie string, noted that data sent must be a json string.
		/// T stands for response object.
		/// </summary>
		public static IEnumerator POST<T>(string url, string jsonData, string cookieName, Action<string> cookieHandler, Action<T> responseCallback, Action<Exception> errorHandler) {
	
			byte[] postData = Encoding.UTF8.GetBytes (jsonData);
			UnityWebRequest request = new UnityWebRequest (url, "POST");
			request.uploadHandler = new UploadHandlerRaw(postData);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader ("Content-Type", "application/json");
			#if UNITY_2017_3_OR_NEWER
			yield return request.SendWebRequest ();
			#else
			yield return request.Send ();
			#endif

			try {
				#if UNITY_2017_1_OR_NEWER
				if (request.isNetworkError) {
				errorHandler (new Exception ("Network Error"));
				} else if (request.isHttpError) {
				errorHandler (new Exception ("HTTP Error"));
				#else
				if (request.isError) {
					errorHandler (new Exception ("Network Error: " + request.error));
				#endif
				} else {
					Dictionary<String, String> headers = request.GetResponseHeaders ();
					string cookies = null;
					headers.TryGetValue ("SET-COOKIE", out cookies);
					string[] frags = cookies.Split (';');
					for (int i = 0; i < frags.Length; i++) {
						string[] fragDetails = frags [i].Split (',');
						for (int j = 0; j < fragDetails.Length; j++) {
							string[] values = fragDetails [j].Split ('=');
							if (values [0].Trim ().Equals (cookieName)) {
								cookieHandler (values [1].Trim ());
							}
						}
					}
					responseCallback (JsonUtility.FromJson<T> (request.downloadHandler.text));
				}
			}
			catch(Exception ex) {
				Debug.Log(ex.Message);
				Debug.Log(request.downloadHandler.text);
			}
	
		}

	}

}