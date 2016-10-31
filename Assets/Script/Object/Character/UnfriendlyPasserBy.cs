using UnityEngine;
using System.Collections;

public class UnfriendlyPasserBy : NormalPasserBy {

	public override void OnFocus ()
	{
		base.OnFocus ();

		DisplaySubDialog ();

		Kick ();
	}

	void Kick()
	{
		MainCharacter.Instance.Kick (MainCharacter.Instance.transform.position - transform.position);
	}
}
