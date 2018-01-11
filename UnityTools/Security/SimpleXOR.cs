using System.Text;

namespace UnityTools.Security {

	public class SimpleXOR {

		/// <summary>
		/// Using Simple XOR Encryption to encrypt or decrypt the specified text with provided key.
		/// </summary>
		public static byte[] EncryptDecrypt(byte[] text, byte[] key) {

			byte[] result = new byte[text.Length];
			for (int i = 0; i < text.Length; i++) {
				result[i] = (byte)(text [i] ^ key [i % key.Length]);
			}
			return result;

		}

	}

}