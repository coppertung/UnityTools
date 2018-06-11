using System;
using System.Collections;
using UnityEngine;

namespace UnityTools.Network {

	/// <summary>
	/// The name of variable is different to that in json string.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
	public class OtherNameInJson : System.Attribute {

		public string originalName;

		public OtherNameInJson(string name) {

			originalName = name;

		}

	}

}