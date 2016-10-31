using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DeathPeople : MPeople {

	[System.Serializable]
	public struct TakePhotoSetting
	{
		public Transform cellPhone;
		public Light flashLight;
		public ParticleSystem flashLightPar;
		public AudioClip flashLightSound;
		public Vector3 cellphoneEndPosition;
		public Vector3 cellphoneEndRotation;
		/// <summary>
		/// Time to take out the cell phone
		/// </summary>
		public float takeOutPhoneTime;
		/// <summary>
		/// The interval between two flash light
		/// </summary>
		public float takePhotoInterval;
		/// <summary>
		/// How long the flash light will last
		/// </summary>
		public float flashLightTime;
	}
	[SerializeField] TakePhotoSetting takePhotoSetting;

	private Vector3 destination;
	private Vector3 bodyPosition;
	private float takePhotoRange;

	AudioSource flashLightAudioSource;
	// Use this for initialization
	void Awake () {
		if (takePhotoSetting.flashLightSound != null) {
			flashLightAudioSource = gameObject.AddComponent<AudioSource> ();
			flashLightAudioSource.playOnAwake = false;
			flashLightAudioSource.Stop ();
			flashLightAudioSource.volume = 0.5f;
			flashLightAudioSource.loop = false;
			flashLightAudioSource.spatialBlend = 1f;
			flashLightAudioSource.clip = takePhotoSetting.flashLightSound;
		}

	}

	public void Init( Vector3 _destination, Vector3 _bodyPosition , float _TakePhotoRange )
	{
		destination = _destination;
		Agent.destination = destination;

		bodyPosition = _bodyPosition;

		takePhotoRange = _TakePhotoRange;
	}
	
	// Update is called once per frame
	void Update () {
		if ( (transform.position - bodyPosition).magnitude < takePhotoRange ) {
			StartTakingPhoto ();
		}
	}

	bool isTakingPhoto = false;
	void StartTakingPhoto()
	{
		if (!isTakingPhoto) {
			Agent.enabled = false;

			Vector3 lookAt = bodyPosition;
			lookAt.y = transform.position.y;
			transform.LookAt (lookAt);

			takePhotoSetting.cellPhone.DOLocalMove (takePhotoSetting.cellphoneEndPosition, takePhotoSetting.takeOutPhoneTime).SetEase (Ease.InOutCirc).OnComplete (AfterTakeOutCellPhone);
			takePhotoSetting.cellPhone.DOLocalRotate (takePhotoSetting.cellphoneEndRotation, takePhotoSetting.takeOutPhoneTime).SetEase (Ease.InOutCirc);
			isTakingPhoto = true;
		}
	}

	void AfterTakeOutCellPhone()
	{
		StartCoroutine (TakePhoto ());
	}

	IEnumerator TakePhoto()
	{
		
		float timer = 0.1f;
		float lastTime = 0;
		while (true) {
			lastTime = timer;
			timer -= Time.deltaTime;
			if (timer < 0 && lastTime >= 0 ) {
				if (takePhotoSetting.flashLight != null)
					takePhotoSetting.flashLight.enabled = true;
				if (takePhotoSetting.flashLightPar != null) {
					takePhotoSetting.flashLightPar.startLifetime = takePhotoSetting.flashLightTime;
					takePhotoSetting.flashLightPar.Emit (1);
				}
				if (flashLightAudioSource != null)
					flashLightAudioSource.Play ();
			}

			if (timer < -takePhotoSetting.flashLightTime) {
				if (takePhotoSetting.flashLight != null)
					takePhotoSetting.flashLight.enabled = false;
				timer = Random.Range (takePhotoSetting.takePhotoInterval / 3f, takePhotoSetting.takePhotoInterval);
			}



			yield return new WaitForEndOfFrame ();
		}
		yield return null;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color (1f, 0.7f, 0);
		Gizmos.DrawWireCube (takePhotoSetting.cellphoneEndPosition, new Vector3( 0.2f , 0.4f , 0.1f ) );
	}
}
