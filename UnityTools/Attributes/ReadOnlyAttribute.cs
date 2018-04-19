using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityTools.Attribute {

	/// <summary>
	/// Read only Attribute.
	/// Reference from:
	/// https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
	/// </summary>
	public class ReadOnlyAttribute : PropertyAttribute {

	}

}