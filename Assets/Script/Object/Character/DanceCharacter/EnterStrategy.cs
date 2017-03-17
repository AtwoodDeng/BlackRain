using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnterStrategy : DanceStrategy {
	[SerializeField] DanceCharacter.DanceAnimation beatUpAnimation = DanceCharacter.DanceAnimation.None;
	[SerializeField] DanceCharacter.DanceAnimation beatDownAnimation = DanceCharacter.DanceAnimation.None;
	[SerializeField] DanceCharacter.DanceAnimation enterAnimation = DanceCharacter.DanceAnimation.None;
	[SerializeField] LogicEvents enterEvent = LogicEvents.None;
	[SerializeField] DanceCharacter.DanceAnimation exitAnimation = DanceCharacter.DanceAnimation.None;
	[SerializeField] LogicEvents exitEvent = LogicEvents.None;

	bool isPlayerIn = false;

	protected override void MOnTriggerEnter (Collider col)
	{
		base.MOnTriggerEnter (col);
		if (col.tag == "Player" && parent.m_state == DanceCharacter.State.Dance ) {
			isPlayerIn = true;
			if (enterAnimation != DanceCharacter.DanceAnimation.None)
				parent.SetTrigger (enterAnimation);
			if (enterEvent != LogicEvents.None)
				M_Event.FireLogicEvent (enterEvent, new LogicArg (this));
		}
	}

	protected override void MOnTriggerExit (Collider col)
	{
		base.MOnTriggerExit (col);
		if (col.tag == "Player" && parent.m_state == DanceCharacter.State.Dance ) {
			isPlayerIn = false;
			if (exitAnimation != DanceCharacter.DanceAnimation.None)
				parent.SetTrigger (exitAnimation);
			if (exitEvent != LogicEvents.None)
				M_Event.FireLogicEvent (exitEvent, new LogicArg (this));
		}
	}

	public override void OnDanceEnter ()
	{
		base.OnDanceEnter ();
		transform.DOMove (parent.OriginalPosition, 1f).SetEase(Ease.InOutCirc);
		transform.DORotate (parent.OriginalRotation.eulerAngles, 1f);
	}

	public override void OnBeatRhythm (int index)
	{
		if ( !isPlayerIn) {
			if (index == 0)
				parent.SetTrigger (beatUpAnimation);
			else
				parent.SetTrigger (beatDownAnimation);
		}
	}

//	public override void OnBeat (int count)
//	{
//		base.OnBeat (count);
//		if (!isPlayerIn && (GetBeatIndex(count) == 0)) {
//			int beatIndex = ((count - beatOffset) / beatFliter) % 2;
////			Debug.Log ("Enter Strategy On Beat " + count + " " + beatIndex);
//			if (beatIndex == 0 )
//				parent.SetTrigger (beatUpAnimation);
//			else
//				parent.SetTrigger (beatDownAnimation);
//		}
//	}
}
