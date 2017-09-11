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
			yield return request.Send ();
				
			if (request.isNetworkError) {
				errorHandler (new Exception ("Network error"));
			} else if (request.isHttpError) {
				errorHandler (new Exception ("Http error : " + request.downloadHandler.text));
			} else {
				responseCallback (JsonUtility.FromJson<T> (request.downloadHandler.text));
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
			yield return request.Send ();

			if (request.isNetworkError) {
				errorHandler (new Exception ("Network error"));
			} else if (request.isHttpError) {
				errorHandler (new Exception ("Http error : " + request.downloadHandler.text));
			} else {
				responseCallback (JsonUtility.FromJson<T> (request.downloadHandler.text));
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
			yield return request.Send ();

			if (request.isNetworkError) {
				errorHandler (new Exception ("Network error"));
			} else if (request.isHttpError) {
				errorHandler (new Exception ("Http error : " + request.downloadHandler.text));
			} else {
				responseCallback (JsonUtility.FromJson<T> (request.downloadHandler.text));
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
			yield return request.Send ();
	
			if (request.isNetworkError) {
				errorHandler (new Exception ("Network error"));
			} else if (request.isHttpError) {
				errorHandler (new Exception ("Http error : " + request.downloadHandler.text));
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

	}

}