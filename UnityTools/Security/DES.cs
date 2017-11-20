using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UnityTools.Security {

	public class DES {

		#region Fields_And_Properties
		private string _key;
		private string _iv;

		/// <summary>
		/// Key of DES encryption and decryption.
		/// </summary>
		public string key {
			get {
				return _key;
			}
		}
		/// <summary>
		/// Initialization Vector of DES encryption and decryption.
		/// If it is null, key will be return as default.
		/// </summary>
		public string iv {
			get {
				if (string.IsNullOrEmpty (_iv)) {
					return _key;
				}
				return _iv;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Basic Constructor.
		/// </summary>
		public DES() { }

		/// <summary>
		/// Intialize key and iv with key value.
		/// </summary>
		public DES(string key) {

			_key = key;
			_iv = key;

		}

		/// <summary>
		/// Intialize key and iv with specific value.
		/// </summary>
		public DES(string key, string iv) {

			_key = key;
			_iv = iv;

		}
		#endregion

		#region Functions
		/// <summary>
		/// Set the key.
		/// </summary>
		public void setKey(string key) {

			_key = key;

		}

		/// <summary>
		/// Set the Initialization Vector.
		/// </summary>
		public void setIV(string iv) {

			_iv = iv;

		}

		/// <summary>
		/// Check if the key is null or not.
		/// If the key is null, NullReferenceException will be thrown out.
		/// </summary>
		public void checkKey() {

			if (string.IsNullOrEmpty (key))
				throw new NullReferenceException ("Key is empty!");

		}

		private byte[] encrypt(byte[] toEncrypt, Encoding keyEncode) {

			checkKey ();
			byte[] result = null;
			if (toEncrypt.Length > 0) {
				try {
					DESCryptoServiceProvider des = new DESCryptoServiceProvider();
					des.Key = keyEncode.GetBytes(key);
					des.IV = keyEncode.GetBytes(iv);
					MemoryStream mStream = new MemoryStream();
					CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(), CryptoStreamMode.Write);
					cStream.Write(toEncrypt, 0, toEncrypt.Length);
					cStream.FlushFinalBlock();
					cStream.Close();
					result = mStream.ToArray();
				}
				catch(Exception ex) {
					throw ex;
				}
			}
			return result;

		}

		/// <summary>
		/// DES Encryption output in base 64 string.
		/// </summary>
		public string encrypt(string toEncrypt, Encoding keyEncode, Encoding textEncode) {

			string result = null;
			if (!string.IsNullOrEmpty (toEncrypt)) {
				byte[] inputByteArray = textEncode.GetBytes(toEncrypt);
				result = Convert.ToBase64String (encrypt (inputByteArray, keyEncode));
			}
			return result;

		}

		/// <summary>
		/// DES Encryption output in Hexadecimal format string.
		/// </summary>
		public string encryptToHex(string toEncrypt, Encoding keyEncode, Encoding textEncode) {

			string result = null;
			if (!string.IsNullOrEmpty (toEncrypt)) {
				byte[] inputByteArray = textEncode.GetBytes(toEncrypt);
				byte[] output = encrypt (inputByteArray, keyEncode);
				StringBuilder str = new StringBuilder ();
				foreach(byte b in output) {
					str.AppendFormat ("{0:X2}", b);
				}
				result = str.ToString();
			}
			return result;

		}

		private byte[] decrypt(byte[] toDecrypt, Encoding keyEncode) {

			checkKey ();
			byte[] result = null;
			if (toDecrypt.Length > 0) {
				try {
					DESCryptoServiceProvider des = new DESCryptoServiceProvider ();
					des.Key = keyEncode.GetBytes (key);
					des.IV = keyEncode.GetBytes (iv);
					MemoryStream mStream = new MemoryStream ();
					CryptoStream cStream = new CryptoStream (mStream, des.CreateDecryptor (), CryptoStreamMode.Write);
					cStream.Write (toDecrypt, 0, toDecrypt.Length);
					cStream.FlushFinalBlock ();
					cStream.Close ();
					result = mStream.ToArray();
				} catch (Exception ex) {
					throw ex;
				}
			}
			return result;

		}

		/// <summary>
		/// DES Decryption output in specific encoding format from basic 64 string.
		/// </summary>
		public string decrypt(string toDecrypt, Encoding byteEncode, Encoding keyEncode, Encoding textEncode) {

			string result = null;
			if (!string.IsNullOrEmpty (toDecrypt)) {
				byte[] inputByteArray = byteEncode.GetBytes(toDecrypt);
				result = textEncode.GetString (decrypt (inputByteArray, keyEncode));
			}
			return result;

		}

		/// <summary>
		/// DES Decryption output in specific encoding format from Hexadecimal string.
		/// </summary>
		public string decryptFromHex(string toDecrypt, Encoding keyEncode, Encoding textEncode) {
			
			string result = null;
			if (!string.IsNullOrEmpty (toDecrypt)) {
				byte[] inputByteArray = new byte[toDecrypt.Length / 2];
				for (int i = 0; i < toDecrypt.Length / 2; i++) {
					int num = Convert.ToInt32 (toDecrypt.Substring (i * 2, 2), 16);
					inputByteArray [i] = (byte)num;
				}
				result = textEncode.GetString (decrypt (inputByteArray, keyEncode));
			}
			return result;

		}
		#endregion

	}

}