#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LocalAssetsLoader : IEnumerator {

	public UnityEngine.Object obj;

	public LocalAssetsLoader(UnityEngine.Object obj) {
		this.obj = obj;
	}

	public bool isDone {
		get {
			return obj != null;
		}
	}

	public object Current
	{
		get
		{
			return null;
		}
	}

	public bool MoveNext()
	{
		return !isDone;
	}

	public void Reset()
	{
	}

}
#endif