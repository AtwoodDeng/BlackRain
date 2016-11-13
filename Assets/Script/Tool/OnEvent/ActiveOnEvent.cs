using UnityEngine;
using System.Collections;

public class ActiveOnEvent : OnEventResponsor {
	[SerializeField] GameObject target;
	[SerializeField] bool setActiveTo;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
		target.SetActive (setActiveTo);
	}

}
