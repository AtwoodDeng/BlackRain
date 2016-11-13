using UnityEngine;
using System.Collections;

public class TrafficControl : MObject {

	[SerializeField] GameObject[] RedLight;
	[SerializeField] GameObject[] GreenLight;

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (LogicEvents.TrafficRedLight, OnTrafficRed);
		M_Event.RegisterEvent (LogicEvents.TrafficGreenLight, OnTrafficGreen);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (LogicEvents.TrafficRedLight, OnTrafficRed);
		M_Event.UnregisterEvent (LogicEvents.TrafficGreenLight, OnTrafficGreen);
	}

	void OnTrafficRed(LogicArg MsgArg)
	{
		Debug.Log ("On Traffic Red ");
		foreach (GameObject obj in RedLight) {
			obj.SetActive (true);
		}
		foreach (GameObject obj in GreenLight) {
			obj.SetActive (false);
		}
	}

	void OnTrafficGreen(LogicArg arg)
	{

		Debug.Log ("On Traffic Green ");
		foreach (GameObject obj in RedLight) {
			obj.SetActive (false);
		}
		foreach (GameObject obj in GreenLight) {
			obj.SetActive (true);
		}
	}
}
