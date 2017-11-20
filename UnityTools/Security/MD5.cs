using System.Security.Cryptography;
using System.Text;

namespace UnityTools.Security {

	public class MD5 {

		/// <summary>
		/// Get the MD5 Hash.
		/// Reference from:
		/// https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
		/// </summary>
		public static string GetMD5Hash(string input, bool toLowerCase) {

			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider ();
			byte[] inputByteArray = Encoding.UTF8.GetBytes (input);
			byte[] md5hash = md5.ComputeHash (inputByteArray);

			StringBuilder str = new StringBuilder ();
			string strFormat = toLowerCase ? "x2" : "X2";
			for (int i = 0; i < md5hash.Length; i++) {
				str.Append (md5hash [i].ToString (strFormat));
			}
			return str.ToString ();

		}

	}

}