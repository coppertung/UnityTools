using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace UnityTools {

	public class Utils {

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