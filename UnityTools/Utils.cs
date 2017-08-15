using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

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
				case BuildTarget.WebGL:
					return "WebGL";
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return "Windows";
				default:
					return null;
				}
			}
		}
		#endif

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

	}

}