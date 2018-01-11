using System.Text;

namespace UnityTools.Security {

	/// <summary>
	/// Caesar cipher.
	/// Reference from:
	/// https://en.wikipedia.org/wiki/Caesar_cipher
	/// </summary>
	public class CaesarCipher {

		/// <summary>
		/// Using Caesar Cipher to encrypt the specified string with a number.
		/// </summary>
		public static string Encrypt(string toEncrypt, int shiftNum) {

			StringBuilder builder = new StringBuilder ();
			for (int i = 0; i < toEncrypt.Length; i++) {
				char temp = toEncrypt [i];
				if (temp >= 'a' && temp <= 'z') {
					temp = (char)((temp - 'a' + shiftNum) % 26 + 'a');
				}
				else if (temp >= 'A' && temp <= 'Z') {
					temp = (char)((temp - 'A' + shiftNum) % 26 + 'A');
				}
				builder.Append (temp);
			}
			return builder.ToString ();

		}

		/// <summary>
		/// Using Caesar Cipher to decrypt the specified string with a number (must be same as the one used in encryption).
		/// </summary>
		public static string Decrypt(string toDecrypt, int shiftNum) {

			StringBuilder builder = new StringBuilder ();
			for (int i = 0; i < toDecrypt.Length; i++) {
				char temp = toDecrypt [i];
				if (temp >= 'a' && temp <= 'z') {
					temp = (char)((temp - 'a' - shiftNum + 26) % 26 + 'a');
				}
				else if (temp >= 'A' && temp <= 'Z') {
					temp = (char)((temp - 'A' - shiftNum + 26) % 26 + 'A');
				}
				builder.Append (temp);
			}
			return builder.ToString ();

		}

	}

}