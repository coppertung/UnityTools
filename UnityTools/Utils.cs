﻿using System;
using System.Security.Cryptography;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace UnityTools {

    public class Utils {

		#if UNITY_EDITOR
		/// <summary>
		/// Gets the build platform.
		/// </summary>
		public static string buildPlatform {
			get {
				switch (EditorUserBuildSettings.activeBuildTarget) {
				case BuildTarget.Android:
					return "Android";
				case BuildTarget.iOS:
					return "iOS";
				case BuildTarget.StandaloneOSXIntel:
				case BuildTarget.StandaloneOSXIntel64:
				case BuildTarget.StandaloneOSXUniversal:
					return "OSX";
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return "Windows";
				case BuildTarget.WebGL:
					return "WebGL";
				default:
					return null;
				}
			}
		}
		#endif
        
        /// <summary>
        /// The default start date time of the unix time stamp.
        /// </summary>
        public static DateTime UnixStartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

		///	<summary>
		///	Generated an integer number randomly in range [0, max).
		///	</summary>
		public static int Random(int max, bool positiveOnly = true) {

			byte[] randomBytes = new byte[sizeof(int)];
			RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
			rngCrypto.GetBytes(randomBytes);
			int rngNum = BitConverter.ToInt32(randomBytes, 0);	// Generate Random Number
			rngNum = rngNum % max;
			if(positiveOnly) {
				rngNum = Math.Abs(rngNum);
			}
			return rngNum;

		}

		///	<summary>
		///	Generated a float number randomly in range [0, max).
		///	</summary>
		public static float Random(float max, bool positiveOnly = true) {
	
			byte[] randomBytes = new byte[sizeof(float)];
			RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
			rngCrypto.GetBytes(randomBytes);
			float rngNum = BitConverter.ToSingle (randomBytes, 0);	// Generate Random Number
			rngNum = rngNum % max;
			if(positiveOnly) {
				rngNum = Math.Abs(rngNum);
			}
			return rngNum;

		}

		/// <summary>
		/// Gets the screen scale from the canvas scaler of the parent of the input gameobject (assumed that it is a canvas).
		/// </summary>
		public static Vector2 GetScreenScaleFromCanvasScaler(GameObject uiComponent) {

			CanvasScaler canvasScaler = uiComponent.GetComponentInParent<CanvasScaler> ();
			if (canvasScaler) {
				return new Vector2 (canvasScaler.referenceResolution.x / Screen.width, canvasScaler.referenceResolution.y / Screen.height);
			} else {
				return Vector2.one;
			}

		}

        /// <summary>
        /// Convert unix time stamp to DateTime object.
        /// </summary>
        public static DateTime UnixTimestampToDateTime(double unixTime) {

            long unixTimestampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(UnixStartDateTime.Ticks + unixTimestampInTicks, System.DateTimeKind.Utc);

        }

        /// <summary>
        /// Convert DateTime object to unix time stamp.
        /// Noted that you can use DateTimeOffset structure instead of calling this function in .NET Framework 4.6 or above.
        /// More details can be found in https://msdn.microsoft.com/library/system.datetimeoffset.aspx
        /// </summary>
        public static double DateTimeToUnixTimestamp(DateTime dateTime) {

            long unixTimestampInTicks = (dateTime.ToUniversalTime() - UnixStartDateTime).Ticks;
            return (double)unixTimestampInTicks / TimeSpan.TicksPerSecond;

        }

	}

}