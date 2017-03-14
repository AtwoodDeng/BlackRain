using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestPath : MonoBehaviour {

	public DOTweenPath path;

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.G)) {
			Debug.Log ("Play Path");
			path.DOPlay ();
		}
	}
}
