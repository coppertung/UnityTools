using System;
using System.Security.Cryptography;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace UnityTools {

    public class Utils {

		#region Platform_dependency
		#if UNITY_EDITOR
		/// <summary>
		/// Get the build platform.
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
		#endregion

		#region System_language
		/// <summary>
		/// Get the system language in string, more detail can be found at:
		/// https://docs.unity3d.com/ScriptReference/SystemLanguage.html
		/// Noted that space will be shown in "_" instead, for instance, "Traditional_Chinese".
		/// </summary>
		public static string systemLanguage {
			get {
				switch (Application.systemLanguage) {
				case SystemLanguage.Afrikaans:
					return "Afrikaans";
				case SystemLanguage.Arabic:
					return "Arabic";
				case SystemLanguage.Basque:
					return "Basque";
				case SystemLanguage.Belarusian:
					return "Belarusian";
				case SystemLanguage.Bulgarian:
					return "Bulgarian";
				case SystemLanguage.Catalan:
					return "Catalan";
				case SystemLanguage.Chinese:
					return "Chinese";
				case SystemLanguage.ChineseSimplified:
					return "Simplified_Chinese";
				case SystemLanguage.ChineseTraditional:
					return "Traditional_Chinese";
				case SystemLanguage.Czech:
					return "Czech";
				case SystemLanguage.Danish:
					return "Danish";
				case SystemLanguage.Dutch:
					return "Dutch";
				case SystemLanguage.English:
					return "English";
				case SystemLanguage.Estonian:
					return "Estonian";
				case SystemLanguage.Faroese:
					return "Faroese";
				case SystemLanguage.Finnish:
					return "Finnish";
				case SystemLanguage.French:
					return "French";
				case SystemLanguage.German:
					return "German";
				case SystemLanguage.Greek:
					return "Greek";
				case SystemLanguage.Hebrew:
					return "Hebrew";
				case SystemLanguage.Hungarian:
					return "Hungarian";
				case SystemLanguage.Icelandic:
					return "Icelandic";
				case SystemLanguage.Indonesian:
					return "Indonesian";
				case SystemLanguage.Italian:
					return "Italian";
				case SystemLanguage.Japanese:
					return "Japanese";
				case SystemLanguage.Korean:
					return "Korean";
				case SystemLanguage.Latvian:
					return "Latvian";
				case SystemLanguage.Lithuanian:
					return "Lithuanian";
				case SystemLanguage.Norwegian:
					return "Norwegian";
				case SystemLanguage.Polish:
					return "Polish";
				case SystemLanguage.Portuguese:
					return "Portuguese";
				case SystemLanguage.Romanian:
					return "Romanian";
				case SystemLanguage.Russian:
					return "Russian";
				case SystemLanguage.SerboCroatian:
					return "SerboCroatian";
				case SystemLanguage.Slovak:
					return "Slovak";
				case SystemLanguage.Slovenian:
					return "Slovenian";
				case SystemLanguage.Spanish:
					return "Spanish";
				case SystemLanguage.Swedish:
					return "Swedish";
				case SystemLanguage.Thai:
					return "Thai";
				case SystemLanguage.Turkish:
					return "Turkish";
				case SystemLanguage.Ukrainian:
					return "Ukrainian";
				case SystemLanguage.Unknown:
					return "Unknown";
				case SystemLanguage.Vietnamese:
					return "Vietnamese";
				default:
					return null;
				}
			}
		}
		#endregion

		#region Secure_Random_Number_Generator
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
		#endregion

		#region Screen_Scale_Issue
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
		#endregion

		#region Unix_Time
		/// <summary>
		/// The default start date time of the unix time stamp.
		/// </summary>
		public static DateTime UnixStartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

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
		#endregion

		#region Animator
		/// <summary>
		/// The current animation of specified animator is ended or not?
		/// </summary>
		public static bool AnimatorCurrentAnimationIsEnded(Animator anim, int layerIndex = 0) {

			AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo (layerIndex);
			return state.normalizedTime > state.length / 2;

		}
		#endregion

	}

}