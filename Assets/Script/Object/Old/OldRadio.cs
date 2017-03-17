using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldRadio : MBehavior {
	[ReadOnlyAttribute] public AudioSource m_source;
	bool ifDone = false;

	protected override void MAwake ()
	{
		base.MAwake ();
		m_source = GetComponent<AudioSource> ();
	}


	protected override void MUpdate ()
	{
		base.MUpdate ();

		if (LogicManager.Instance.State == LogicManager.GameState.WalkWithGirlModern && !ifDone) {
			CopyFromBGM ();
			ifDone = true;
		}
	}

	public void CopyFromBGM() {
		if (m_source != null) {
			m_source.clip = AudioManager.Instance.BackgroundMusicSource.clip;
			m_source.time = AudioManager.Instance.BackgroundMusicSource.time;
		}

	}
}
