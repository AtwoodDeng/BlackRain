using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Audio;

/// <summary>
/// manage the sound effect
/// play the sound effect when recieve an event 
/// 
/// </summary>
public class AudioManager : MBehavior {
	public enum SnapshotType{
		Old = 1,
		Modern = 2,
	}

	public AudioManager() { s_Instance = this; }
	public static AudioManager Instance { get { return s_Instance; } }
	private static AudioManager s_Instance;

	/// <summary>
	/// input pair for recording the input sound effect
	/// </summary>
	//	[System.Serializable]
	//	public struct InputClipPair
	//	{
	//		public MInputType input;
	//		public AudioClip clip;
	//	};
	//	[SerializeField] InputClipPair[] InputClipPairs;

	/// <summary>
	/// for pairing the logic event and the sound effect
	/// </summary>
	[System.Serializable]
	public struct LogicClipPair
	{
		public LogicEvents type;
		public AudioClip clip;
		public bool switchBGM;
	};
	[SerializeField] LogicClipPair[] LogicClipPairs;

	[SerializeField] float bgmFadeTime = 1f;
	[SerializeField] AudioClip defaultBGM;
	[SerializeField] AudioClip illusionBGM;
	[SerializeField] AudioClip endingBGM;
	[SerializeField] List<AudioClip> playableMusicList;
	[SerializeField] AudioMixerSnapshot old;
	[SerializeField] AudioMixerSnapshot modern;
	private AudioSource bgmSource;
	private AudioSource bgmSwitchableSource;
	public string switchBGMName{
		get {
			string name = "";
			if (bgmSwitchableSource != null && bgmSwitchableSource.clip != null)
				name = bgmSwitchableSource.clip.name;
			
				return name;
		}
	}

	protected override void MAwake ()
	{
		base.MAwake ();
		SwitchBGM (defaultBGM , false);
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		for (int i = 0; i < M_Event.logicEvents.Length; ++i) {
			M_Event.logicEvents [i] += OnLogicEvent;
		}
		M_Event.logicEvents [(int)LogicEvents.SwitchBGM] += OnSwitchBGM;
		M_Event.logicEvents [(int)LogicEvents.PlayMusic] += OnPlayMusic;
		M_Event.logicEvents [(int)LogicEvents.SwitchDefaultBGM] += OnDefaultBGM;
		M_Event.logicEvents [(int)LogicEvents.PlayEndBGM] += OnPlayEndBGM;
		M_Event.logicEvents [(int)LogicEvents.BeginDamage] += OnBeginDamage;
		M_Event.logicEvents [(int)LogicEvents.EndDamage] += OnEndDamge;
		M_Event.logicEvents [(int)LogicEvents.DeathEnd] += OnDeathEnd;
		M_Event.logicEvents [(int)LogicEvents.ToOld] += OnToOld;
		M_Event.logicEvents [(int)LogicEvents.ToModern] += OnToModern;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		for (int i = 0; i < M_Event.logicEvents.Length ; ++i) {
			M_Event.logicEvents [i] -= OnLogicEvent;
		}
		M_Event.logicEvents [(int)LogicEvents.SwitchBGM] -= OnSwitchBGM;
		M_Event.logicEvents [(int)LogicEvents.PlayMusic] -= OnPlayMusic;
		M_Event.logicEvents [(int)LogicEvents.SwitchDefaultBGM] -= OnDefaultBGM;
		M_Event.logicEvents [(int)LogicEvents.PlayEndBGM] -= OnPlayEndBGM;
		M_Event.logicEvents [(int)LogicEvents.BeginDamage] -= OnBeginDamage;
		M_Event.logicEvents [(int)LogicEvents.EndDamage] -= OnEndDamge;
		M_Event.logicEvents [(int)LogicEvents.DeathEnd] -= OnDeathEnd;
		M_Event.logicEvents [(int)LogicEvents.ToOld] -= OnToOld;
		M_Event.logicEvents [(int)LogicEvents.ToModern] -= OnToModern;

	}

	public void OnToOld( LogicArg arg )
	{

		float delay = 0;
		float duration = 0;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);

		StartCoroutine( ChangeSnapshotTo( delay , duration , SnapshotType.Old ));
	}

	public void OnToModern( LogicArg arg )
	{

		float delay = 0;
		float duration = 0;
		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DELAY ) )
			delay = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DELAY);

		if ( arg.ContainMessage(M_Event.EVENT_OMSWITCH_DURATION ) )
			duration = (float)arg.GetMessage(M_Event.EVENT_OMSWITCH_DURATION);
		
		StartCoroutine( ChangeSnapshotTo( delay , duration , SnapshotType.Modern ));
				
	}

	IEnumerator ChangeSnapshotTo( float delay , float duration, SnapshotType type )
	{
		yield return new WaitForSeconds (delay);
		if ( type == SnapshotType.Old )
			old.TransitionTo (duration);
		if ( type == SnapshotType.Modern )
			modern.TransitionTo (duration);
	}

	public void OnPlayEndBGM( LogicArg arg )
	{
		SwitchBGM (endingBGM, false);
	}

	public string GetMusicName()
	{
		if (bgmSwitchableSource != null)
			return bgmSwitchableSource.clip.name;
		return "";
	}

	void OnPlayMusic( LogicArg arg )
	{
		
		string name = (string)arg.GetMessage (M_Event.EVENT_PLAY_MUSIC_NAME);
		foreach (AudioClip music in playableMusicList) {
			if (music.name == name) {
				SwitchBGM (music , false);

			}
		}
	}

	void OnBeginDamage(LogicArg arg)
	{
//		SwitchBGM (illusionBGM);
	}

	void OnEndDamge(LogicArg arg)
	{
//		SwitchBGM (defaultBGM);
	}

	void OnDeathEnd(LogicArg arg )
	{
		SwitchBGM (defaultBGM , false);	
	}

	void OnLogicEvent( LogicArg logicEvent )
	{
		foreach (LogicClipPair pair in LogicClipPairs) {
			if (pair.type == logicEvent.type) {
//				StartCoroutine(PlayerClip(pair.clip));
				if (pair.switchBGM)
					SwitchBGM (pair.clip, false);
				else
					StartCoroutine (PlayerClip (pair.clip));
			}
		}
	}

	IEnumerator PlayerClip( AudioClip clip )
	{
		if (clip == null)
			yield break;
		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.clip = clip;
		source.playOnAwake = source.loop = false;

		source.Play ();
		while (source.isPlaying) {
			yield return null;
		}

		Destroy (source);

	}

	void OnSwitchBGM( LogicArg arg )
	{
		AudioClip clip = (AudioClip)arg.GetMessage (M_Event.EVENT_SWITCH_BGM_CLIP);

		SwitchBGM (clip , true);
	}

	void OnDefaultBGM( LogicArg arg )
	{
		float fadeTime = -1f;
		if (arg.ContainMessage (M_Event.EVENT_BGM_FADE_TIME)) {
			fadeTime = (float)arg.GetMessage (M_Event.EVENT_BGM_FADE_TIME);
		}
		SwitchBGM (defaultBGM , false , fadeTime);
	}

	void SwitchBGM( AudioClip to , bool randomPlay , float duration = -1f )
	{
		if (duration < 0)
			duration = bgmFadeTime;
		
		if (bgmSource == null) {
			bgmSource = gameObject.AddComponent<AudioSource> ();
			bgmSource.loop = true;
			bgmSource.volume = 0.5f;
			bgmSource.spatialBlend = 0f;
			bgmSource.clip = defaultBGM;
			bgmSource.Play ();
		}

		if (bgmSwitchableSource == null) {
			bgmSwitchableSource = gameObject.AddComponent<AudioSource> ();
			bgmSwitchableSource.loop = true;
			bgmSwitchableSource.volume = 0.5f;
			bgmSwitchableSource.spatialBlend = 0f;
		}

		if (bgmSource != null) {
			if (to != defaultBGM) {
				bgmSource.DOFade (0.2f, duration);
			} else {
				bgmSource.DOFade (0.5f, duration);
			}
		}
		if (bgmSwitchableSource != null) {
			if (to != defaultBGM) {
				bgmSwitchableSource.DOFade (0, duration).OnComplete (delegate {
					bgmSwitchableSource.clip = to;
					if (randomPlay)
						bgmSwitchableSource.time = Random.Range (0, bgmSwitchableSource.clip.length);
					bgmSwitchableSource.Play ();
					bgmSwitchableSource.DOFade (0.5f, duration);
				});
			} else {

				bgmSwitchableSource.DOFade (0, duration).OnComplete (delegate {
					bgmSwitchableSource.clip = null;
				});
			}
		}

	}

	static public AudioSource PlaySoundOn( AudioClip clip , GameObject target , float delay = 0 , float volume = 0.65f)
	{
		AudioSource source = target.AddComponent<AudioSource> ();
		source.clip = clip;
		source.spatialBlend = 1f;
		source.volume = volume;

		source.PlayDelayed (delay);
		source.DOFade (0, 0.5f).SetDelay (clip.length + delay).OnComplete (delegate {
			DestroyObject(source);	
		});
		return source;
	}

}
