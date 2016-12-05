using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class BGMController : MBehavior {

	[SerializeField] AudioClip switchBGM;
	[SerializeField] bool isExitSwitchToDefault = true;

	protected override void MAwake ()
	{
		base.MAwake ();
		GetComponent<Collider> ().isTrigger = true;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "MainCamera") {
			LogicArg arg = new LogicArg (this);
			arg.AddMessage (M_Event.EVENT_SWITCH_BGM_CLIP, switchBGM);
			M_Event.FireLogicEvent (LogicEvents.SwitchBGM, arg);
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "MainCamera" && isExitSwitchToDefault ) {
			M_Event.FireLogicEvent (LogicEvents.SwitchDefaultBGM, new LogicArg (this));
		}
	}
}
