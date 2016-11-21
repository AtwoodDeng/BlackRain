using UnityEngine;
using System.Collections;

public class NormalPasserbyAnimationState : MAnimationState {

	Character character;
	[SerializeField] string EndAnimationInfo;

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter (animator, stateInfo, layerIndex);
		character = animator.transform.parent.GetComponent<Character> ();
	}

	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateExit (animator, stateInfo, layerIndex);
		character.OnAnimationEnd (EndAnimationInfo);
	}
}
