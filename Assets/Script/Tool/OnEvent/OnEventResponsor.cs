using UnityEngine;
using System.Collections;

public class OnEventResponsor : MBehavior {
	[SerializeField] LogicEvents senseEvent;

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (senseEvent, OnEvent);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (senseEvent, OnEvent);
	}

	virtual public void OnEvent(LogicArg arg)
	{
	}
}
