using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilmControlOnEvent : OnEventResponsor {

	[SerializeField] FilmController controller;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (controller == null) {
			controller = GetComponent<FilmController> ();
		}
	}

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
		if ( controller != null )
			controller.Work ();
	}
}
