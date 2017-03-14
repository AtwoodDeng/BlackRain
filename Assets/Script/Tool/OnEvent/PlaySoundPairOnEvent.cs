using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundPairOnEvent : OnEventResponsor {
	[SerializeField] AudioManager.LogicClipPair pair;

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);
		AudioManager.Instance.StartPlayPair (pair);
	}
}
