using System.Text;

namespace UnityTools.Security {

	public class SimpleXOR {

		/// <summary>
		/// Using Simple XOR Encryption to encrypt or decrypt the specified text with provided key.
		/// </summary>
		public static string EncryptDecrypt(string text, string key) {

			StringBuilder builder = new StringBuilder ();
			for (int i = 0; i < text.Length; i++) {
				char temp = (char)(text [i] ^ key [i % key.Length]);
				builder.Append (temp);
			}
			return builder.ToString ();

		}

	}

}