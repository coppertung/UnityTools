using System;
using System.IO;
using UnityEngine;

namespace UnityTools {

    public class SystemLogger {

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

	}

}
