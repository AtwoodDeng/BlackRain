using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class SavePoint : MBehavior {


	void OnTriggerEnter(Collider col )
	{
		if (col.tag == "Player") {
			LogicArg arg = new LogicArg (this);
			SavePointData data = new SavePointData ();
			data.trans = transform;
			arg.AddMessage (M_Event.EVENT_SAVE_POINT, data);
			M_Event.FireLogicEvent (LogicEvents.EnterSavePoint, arg);
//			gameObject.SetActive (false);
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (transform.position, transform.position + transform.forward * 2f);
	}
}

public class SavePointData
{
	public Transform trans;
}