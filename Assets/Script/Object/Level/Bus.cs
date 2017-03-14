using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MBehavior {

	int i = 1 ;
	protected override void MUpdate ()
	{
		base.MUpdate ();
		i = (i == 1) ? -1 : 1;
		transform.position += i * transform.up * 0.01f;
	}
}
