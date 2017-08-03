using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemLogger {

	public static string logDirectory = Application.persistentDataPath + "/Log/";
	public static string logFile = "systemLog-" + DateTime.Now.Year.ToString () + "-" + DateTime.Now.Month.ToString () + "-" + DateTime.Now.Day.ToString () + ".log";

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
