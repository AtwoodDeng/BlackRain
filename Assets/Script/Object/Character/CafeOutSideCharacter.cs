using UnityEngine;
using System.Collections;

public class CafeOutSideCharacter : TalkableCharacter {

	[SerializeField] LogicManager.GameState enterState;
	[SerializeField] float ShowUpOffset = 3f ;

	protected override void MStart ()
	{
		base.MStart ();

		LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {
			if ( toState == enterState )
			{
				Vector3 pos = MainCharacter.Instance.transform.position + Vector3.ProjectOnPlane( Camera.main.transform.forward * -1f * ShowUpOffset , Vector3.up );
				transform.position = pos;
			}
		});
	}
}
