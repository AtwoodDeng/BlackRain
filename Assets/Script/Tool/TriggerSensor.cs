using UnityEngine;
using System.Collections;

public class TriggerSensor : MBehavior {
	[SerializeField] LogicEvents enterEvent;

	void OnTriggerEnter( Collider col )
	{
		if (col.tag == "Player") {
			M_Event.FireLogicEvent (enterEvent, new LogicArg (this));
		}
	}
}
