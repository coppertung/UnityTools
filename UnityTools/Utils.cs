using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Utils {

	///	<summary>
	///	Generated number randomly in range [0, max).
	///	</summary>
	public static int Random(int max, bool positiveOnly = true) {

		byte[] randomBytes = new byte[10];
		RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
		rngCrypto.GetBytes(randomBytes);
		int rngNum = BitConverter.ToInt32(randomBytes, 0);	// Generate Random Number
		rngNum = rngNum % max;
		if(positiveOnly) {
			rngNum = Math.Abs(rngNum);
		}
		return rngNum;

	}

}
