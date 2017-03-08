using UnityEngine;
using System.Collections;
using DG.Tweening;

//[ExecuteInEditMode]
public class ASelfRotate : MBehavior {

	[SerializeField] Vector3 rotateDirection;
	[Tooltip("The cycle Time to rotate 360 degree")]
	[SerializeField] float rotateCycleTime = 1f;
	[SerializeField] bool playOnEditor = true;
	[SerializeField] LogicManager.GameState startState = LogicManager.GameState.Enter;
	[SerializeField] LogicManager.GameState endState =  LogicManager.GameState.End;
	[SerializeField] bool ifStartRotateAnimation = false;
	[SerializeField] float StartRotateDuration = 5f;
	float rotateSpeed;

	protected override void MStart ()
	{
		base.MStart ();
		rotateSpeed = 360f / rotateCycleTime;

		if (ifStartRotateAnimation) {
			LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {
				if ( toState == startState )
				{
					DOTween.To(()=>rotateSpeed , (x)=>rotateSpeed = x , 0 , StartRotateDuration ).From();
				}
			});
		}
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		if (LogicManager.Instance.State >= startState && LogicManager.Instance.State < endState) {
			if (!(Application.isEditor && !playOnEditor)) {
				Vector3 rotateAngle = rotateDirection.normalized * ( rotateSpeed );
				transform.Rotate (rotateAngle * Time.deltaTime);
			}
		}
	}

}
