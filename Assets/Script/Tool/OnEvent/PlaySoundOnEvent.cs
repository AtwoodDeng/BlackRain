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
	[SerializeField] bool loop = false;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);

		Debug.Log ("Play Sound " + target.name);
		if (target != null) {
			target.loop = loop;
			target.DOFade (fadeto, duration).SetRelative (isRelative).SetDelay(delay).OnStart(delegate() {
				if ( isToPlay )
					target.Play ();
			});
		}
	}
}
