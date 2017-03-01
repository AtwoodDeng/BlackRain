using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MBehavior {

	[SerializeField] AudioSource m_source;
	[SerializeField] AudioClip[] clips;
	[SerializeField] MinMax interval;
	[SerializeField] bool isPlayOnAwake;
	[SerializeField] MinMax pitch;
	[SerializeField] MinMax volume;

	protected override void MAwake ()
	{
		base.MAwake ();
		if ( m_source == null )
			m_source = GetComponent<AudioSource> ();
		if (isPlayOnAwake) {
			Play ();
		}
	}

	public void Play()
	{
		StartCoroutine (PlayCor ());
	}

	IEnumerator PlayCor()
	{
		while (true) {
			if (m_source != null) {
				m_source.clip = clips [Random.Range (0, clips.Length)];
				m_source.volume = volume.RandomBetween;
				m_source.pitch = pitch.RandomBetween;
				m_source.Play ();
				m_source.loop = false;
//				Debug.Log ("PLay sound " + m_source.clip.length + " " + interval.RandomBetween);
			}

			yield return new WaitForSeconds (m_source.clip.length + interval.RandomBetween);
		}
	}
}
