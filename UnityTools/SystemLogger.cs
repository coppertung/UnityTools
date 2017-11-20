using System;
using System.IO;
using UnityEngine;

namespace UnityTools {

    public class SystemLogger {

		#region Fields_And_Properties
        /// <summary>
        /// The directory of the log file.
        /// By default, it will be "persistent data path + /Log/".
        /// </summary>
		public static string logDirectory = Application.persistentDataPath + "/Log/";
        /// <summary>
        /// The name of the log file.
        /// By defualt, it will be "(product name)-systemLog-(year)-(month)-(day).log".
        /// </summary>
		public static string logFile = Application.productName + "-systemLog-" + DateTime.Now.Year.ToString () + "-" + DateTime.Now.Month.ToString () + "-" + DateTime.Now.Day.ToString () + ".log";
		#endregion

		#region Functions
		/// <summary>
		/// Log the specified message to a log file, which will be stored in persistent data path.
		/// </summary>
		public static void Log(string msg) {

			StreamWriter sw;
			if (!Directory.Exists (logDirectory))
				Directory.CreateDirectory (logDirectory);
			if (File.Exists (logDirectory + logFile)) {
				sw = File.AppendText (logDirectory + logFile);
			} else {
				sw = File.CreateText (logDirectory + logFile);
			}
			sw.WriteLine (DateTime.Now.ToString () + "\t" + msg);
			sw.Close ();

		}

		/// <summary>
		/// Log the specified message to a custom log file, which will be stored in persistent data path.
		/// </summary>
		public static void Log(string msg, string fileName) {

			StreamWriter sw;
			if (!Directory.Exists (logDirectory))
				Directory.CreateDirectory (logDirectory);
			if (File.Exists (logDirectory + fileName)) {
				sw = File.AppendText (logDirectory + fileName);
			} else {
				sw = File.CreateText (logDirectory + fileName);
			}
			sw.WriteLine (DateTime.Now.ToString () + "\t" + msg);
			sw.Close ();

		}

		/// <summary>
		/// Log the specified message to a custom log file, which will be stored in custom data path.
		/// </summary>
		public static void Log(string msg, string fileName, string fileDirectory) {

			StreamWriter sw;
			if (!Directory.Exists (fileDirectory))
				Directory.CreateDirectory (fileDirectory);
			if (File.Exists (fileDirectory + fileName)) {
				sw = File.AppendText (fileDirectory + fileName);
			} else {
				sw = File.CreateText (fileDirectory + fileName);
			}
			sw.WriteLine (DateTime.Now.ToString () + "\t" + msg);
			sw.Close ();

		}
		#endregion

	}

}
