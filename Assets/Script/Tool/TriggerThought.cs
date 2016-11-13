using UnityEngine;
using System.Collections;

public class TriggerThought : MBehavior {

//	[SerializeField] Thought thought;
	[SerializeField] ThoughtScritableObject thoughtList;
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
			if (thoughtList != null && thoughtList.mainThought != null) {
				ThoughtManager.Instance.SendThought (thoughtList.Thought);
			}
			if ( isOnce )
				isSended = true;
		}
	}
}
