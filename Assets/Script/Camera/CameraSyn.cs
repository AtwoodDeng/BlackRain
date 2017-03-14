using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSyn : MBehavior {
	[SerializeField] Camera targetCamera;
	[SerializeField] public Camera thisCamera;
	[SerializeField] bool synFieldOfView = false;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (thisCamera == null)
			thisCamera = GetComponent<Camera> ();
		if (targetCamera == null)
			targetCamera = Camera.main;
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		if (thisCamera != null && targetCamera != null) {
			if (synFieldOfView && thisCamera.fieldOfView != targetCamera.fieldOfView) {
				thisCamera.fieldOfView = targetCamera.fieldOfView;
			}
				
		}
	}
}
