using UnityEngine;
using System.Collections;

public class Character : Interactable {

	protected CapsuleCollider m_collider;

	protected override void MAwake ()
	{
		base.MAwake ();
		m_collider = GetComponent<CapsuleCollider> ();
		m_collider.isTrigger = true;
	}

}
