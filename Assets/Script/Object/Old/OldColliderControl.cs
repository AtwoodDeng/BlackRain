﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldColliderControl : MBehavior {
	[ReadOnlyAttribute] public bool isActiveOnAwake;
	[ReadOnlyAttribute] public bool isOld;
	[ReadOnlyAttribute] public bool isMorden;
	[ReadOnlyAttribute] public bool isDark;
	[SerializeField] bool resetCollider = true;
	[ReadOnlyAttribute] public Collider m_collider;
	[SerializeField] bool resetColliderInChildren = true;
	[ReadOnlyAttribute] public Collider[] collidersInChildren;

	protected override void MAwake ()
	{
		base.MAwake ();
		if ( isActiveOnAwake )
			gameObject.SetActive (true);


		isOld = gameObject.layer == LayerMask.NameToLayer ("Old");
		isMorden = gameObject.layer == LayerMask.NameToLayer ("Morden");
		isDark = gameObject.layer == LayerMask.NameToLayer ("Dark");

		m_collider = GetComponent<Collider> ();
		collidersInChildren = GetComponentsInChildren<Collider> ();
	}

	protected override void MStart ()
	{
		base.MStart ();


		OnToModern (new LogicArg(this));
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (LogicEvents.ToOld, OnToOld);
		M_Event.RegisterEvent (LogicEvents.ToModern, OnToModern);
		M_Event.RegisterEvent (LogicEvents.ToDark, OnToDark);

	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (LogicEvents.ToOld, OnToOld);
		M_Event.UnregisterEvent (LogicEvents.ToModern, OnToModern);
		M_Event.UnregisterEvent (LogicEvents.ToDark, OnToDark);

	}

	void OnToDark( LogicArg arg )
	{
		float delay = 0;
		float duration = 0;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);

		if (isDark) {
			StartCoroutine (SetToDelay (true, delay));
		}

		if (isMorden || isOld) {
			StartCoroutine (SetToDelay (false, delay));
		}
	}

	void OnToOld( LogicArg arg )
	{
		float delay = 0;
		float duration = 0;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);

		if (isOld) {
			StartCoroutine (SetToDelay (true, delay));
		}

		if (isMorden || isDark) {
			StartCoroutine (SetToDelay (false, delay));
		}
	}


	void OnToModern( LogicArg arg )
	{
		float delay = 0;
		float duration = 0;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);
		

		if (isMorden) {
			StartCoroutine (SetToDelay (true, delay));
		}

		if (isOld || isDark) {
			StartCoroutine (SetToDelay (false, delay));
		}
	}

	IEnumerator SetToDelay( bool active , float delay )
	{
		yield return new WaitForSeconds (delay);

		if (resetCollider && m_collider != null )
			m_collider.enabled = active;
		if (resetColliderInChildren && collidersInChildren != null)
			foreach (Collider c in collidersInChildren)
				c.enabled = active;
	}

}
