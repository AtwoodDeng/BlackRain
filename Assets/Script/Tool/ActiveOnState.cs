using UnityEngine;
using System.Collections;

public class ActiveOnState : MBehavior {

	[SerializeField] LogicManager.GameState startState;
	[SerializeField] LogicManager.GameState endState;
	[SerializeField] bool setActiveTo = false;
	[SerializeField] GameObject[] targetGameObjects;

	protected override void MStart ()
	{
		base.MStart ();
		SetActive (LogicManager.Instance.State);
		LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {
//			Debug.Log("Active On State" + toState );
			SetActive( toState );	
		});
	}

	void SetActive( LogicManager.GameState state )
	{
		bool to = (state >= startState && state < endState) ? setActiveTo : !setActiveTo;

		foreach (GameObject obj in targetGameObjects) {
			if ( obj != null )
				obj.SetActive (to);
		}
	}


}
