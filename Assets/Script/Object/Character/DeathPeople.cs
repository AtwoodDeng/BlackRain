using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DeathPeople : TalkableCharacter {

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
	[SerializeField] bool StartTakePhotoOnAwake;
	[SerializeField] Transform focusTransform;
	[SerializeField] Gradient umbrellaColor;
	[SerializeField] MeshRenderer umbrellaUp;

	private Vector3 destination;
	private Vector3 focusPosition{
		get { return focusTransform.position; }
	}
	private float takePhotoRange;

	AudioSource flashLightAudioSource;
	[SerializeField] Transform umbrella;

//	private NavMeshAgent m_agent;
//	public NavMeshAgent Agent{ get { 
//			if (m_agent == null)
//				m_agent = GetComponent<NavMeshAgent> ();
//			return m_agent; } }
	
	// Use this for initialization
	protected override void MAwake ()
	{
		base.MAwake ();
		if (takePhotoSetting.flashLightSound != null) {
			flashLightAudioSource = gameObject.AddComponent<AudioSource> ();
			flashLightAudioSource.playOnAwake = false;
			flashLightAudioSource.Stop ();
			flashLightAudioSource.volume = 1f;
			flashLightAudioSource.loop = false;
			flashLightAudioSource.spatialBlend = 1f;
			flashLightAudioSource.clip = takePhotoSetting.flashLightSound;
			flashLightAudioSource.maxDistance = 35f;
			flashLightAudioSource.minDistance = 0.1f;
		}

		if (umbrellaUp != null ) {
			Color color = umbrellaColor.Evaluate (Random.Range (0, 1f));
			color.a = 0.55f;
			umbrellaUp.material = new Material (Shader.Find ("AlphaSelfIllum_NoFog"));
			umbrellaUp.material.SetColor ("_Color", color);
		}

		if (umbrella != null) {
			umbrella.transform.localPosition += new Vector3 (Random.Range (-0.3f, 0.3f), Random.Range (-2f, 0), Random.Range (-0.3f, 0.3f));
			umbrella.transform.Rotate( new Vector3( Random.Range( -7f , 7f ) , Random.Range( -7f , 7f ) , Random.Range( -7f , 7f )));
		}

	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		if (StartTakePhotoOnAwake) {
			
			StartTakingPhoto ();
		}
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();

		StopAllCoroutines ();
		isTakingPhoto = false;
	}

	public void Init( Vector3 _destination, Transform _bodyPosition , float _TakePhotoRange )
	{
		destination = _destination;
//		if ( Agent != null &&  Agent.enabled )
//			Agent.destination = destination;

		focusTransform = _bodyPosition;

		takePhotoRange = _TakePhotoRange;
	}
	
	// Update is called once per frame
	void Update () {
		if ( (transform.position - focusPosition).magnitude < takePhotoRange ) {
			StartTakingPhoto ();
		}
	}

	bool isTakingPhoto = false;
	void StartTakingPhoto()
	{
		if (!isTakingPhoto) {
//			if ( Agent != null )
//				Agent.enabled = false;

				Vector3 lookAt = focusPosition;
				lookAt.y = transform.position.y;
				transform.LookAt (lookAt);

			takePhotoSetting.cellPhone.DOLocalMove (takePhotoSetting.cellphoneEndPosition, takePhotoSetting.takeOutPhoneTime).SetEase (Ease.InOutCirc).OnComplete (AfterTakeOutCellPhone);
			takePhotoSetting.cellPhone.DOLocalRotate (takePhotoSetting.cellphoneEndRotation, takePhotoSetting.takeOutPhoneTime).SetEase (Ease.InOutCirc);
			isTakingPhoto = true;
		}
	}

	void AfterTakeOutCellPhone()
	{
		if ( gameObject.activeInHierarchy )
			StartCoroutine (TakePhoto ());
	}

	IEnumerator TakePhoto()
	{
		
		float timer = Random.Range (takePhotoSetting.takePhotoInterval / 3f, takePhotoSetting.takePhotoInterval);
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
