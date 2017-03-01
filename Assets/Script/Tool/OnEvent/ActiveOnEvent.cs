using UnityEngine;
using System.Collections;

public class ActiveOnEvent : OnEventResponsor {
	[SerializeField] GameObject target;
	[SerializeField] bool setActiveTo;
	[SerializeField] float delay;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
		StartCoroutine (SetActive (delay));
	}

	IEnumerator SetActive(float delay)
	{
		yield return new WaitForSeconds (delay);
		target.SetActive (setActiveTo);

	}

}
