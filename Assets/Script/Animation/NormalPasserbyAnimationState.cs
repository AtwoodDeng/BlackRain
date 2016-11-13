using UnityEngine;
using System.Collections;

public class NormalPasserbyAnimationState : MAnimationState {

	NormalPasserBy normalPasserBy;
	[SerializeField] string EndAnimationInfo;

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter (animator, stateInfo, layerIndex);
		normalPasserBy = animator.transform.parent.GetComponent<NormalPasserBy> ();
	}

	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateExit (animator, stateInfo, layerIndex);
		normalPasserBy.OnAnimationEnd (EndAnimationInfo);
	}
}
