using UnityEngine;
using System.Collections;

public class TriggerDialog : TalkableCharacter {

	//	[SerializeField] Thought thought;
	[SerializeField] bool isOnce = true;

	bool isSended = false;

	protected override void MAwake ()
	{
		base.MAwake ();
		gameObject.layer = LayerMask.NameToLayer ("PlayerTrigger");
	}

	void OnTriggerEnter( Collider col )
	{
		if (col.tag == "Player" && !isSended) {
			Interact ();
		}
	}

	public override bool IsInteractable ()
	{
		return false;
	}
}
