using UnityEngine;
using System.Collections;

public class TriggerSensor : MBehavior {
	[SerializeField] LogicEvents enterEvent;
	[SerializeField] bool isOnce = true;

	bool isSended = false;
	void OnTriggerEnter( Collider col )
	{
		if (col.tag == "Player" && !isSended ) {
			M_Event.FireLogicEvent (enterEvent, new LogicArg (this));
			if ( isOnce )
				isSended = true;
		}
	}
}
