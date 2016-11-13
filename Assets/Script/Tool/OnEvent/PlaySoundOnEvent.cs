using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlaySoundOnEvent : OnEventResponsor {

	[SerializeField] AudioSource target;
	[SerializeField] float fadeto = 1f;
	[SerializeField] float delay = 0f;
	[SerializeField] float duration = 1f;
	[SerializeField] bool isRelative = false;
	[SerializeField] bool isToPlay = true;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);


		if (target != null) {
			if ( isToPlay )
				target.Play ();
			target.DOFade (fadeto, duration).SetRelative (isRelative).SetDelay(delay);
		}
	}
}
