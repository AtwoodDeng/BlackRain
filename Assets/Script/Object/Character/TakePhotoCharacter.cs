using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class TakePhotoCharacter : TalkableCharacter {


	[SerializeField] Gradient umbrellaColor;
	[SerializeField] MeshRenderer umbrellaUp;
	[SerializeField] MeshRenderer umbrellaDown;
	[SerializeField] Transform umbrella;
	[SerializeField] Animator m_animator;
	[SerializeField] Transform toward;

	[System.Serializable]
	public struct TakePhotoSetting
	{
		public Transform cellPhone;
		public Light flashLight;
		public ParticleSystem flashLightPar;
		public AudioSource flashLightSouce;
		public AudioClip flashLightSound;
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
	[System.Serializable]
	public struct WalkBetweenSetting
	{
		public bool IfWork;
		public Transform[] WalkList;
		[ReadOnlyAttribute] public int index;
		public NavMeshAgent agent;
		public float speed;
	}
	[SerializeField] WalkBetweenSetting walkBetweenSetting;
	AudioSource flashLightAudioSource;


	protected override void MAwake ()
	{
		base.MAwake ();

		if (takePhotoSetting.flashLightSouce != null) {
			flashLightAudioSource = takePhotoSetting.flashLightSouce;
		} else if (takePhotoSetting.flashLightSound != null) {
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
			umbrellaUp.material = new Material (umbrellaUp.material.shader);
			umbrellaUp.material.SetColor ("_Color", color);
			if (umbrellaDown != null)
				umbrellaDown.material = umbrellaUp.material;
		}

		if (umbrella != null) {
			umbrella.transform.localPosition += new Vector3 (Random.Range (-0.3f, 0.3f), Random.Range (-2f, 0), Random.Range (-0.3f, 0.3f));
			umbrella.transform.Rotate( new Vector3( Random.Range( -7f , 7f ) , Random.Range( -7f , 7f ) , Random.Range( -7f , 7f )));
		}

		if (m_animator == null) {
			m_animator = GetComponentInChildren<Animator> ();
		}

		if (toward != null) {
			Vector3 towardPos = toward.position;
			towardPos.y = 0;
			transform.LookAt (towardPos );
		}

		if (walkBetweenSetting.IfWork) {
			walkBetweenSetting.agent = gameObject.AddComponent<NavMeshAgent> ();
			walkBetweenSetting.agent.stoppingDistance = 0.1f;
			walkBetweenSetting.agent.speed = walkBetweenSetting.speed;
			walkBetweenSetting.agent.angularSpeed = 60f;
		}
			
	}

	protected override void MStart ()
	{
		base.MStart ();

		m_animator.speed = Random.Range (0.8f, 1.2f);

		timer = Random.Range (takePhotoSetting.takePhotoInterval / 3f, takePhotoSetting.takePhotoInterval);
	}

	protected override void MOnTriggerEnter (Collider col)
	{
		base.MOnTriggerEnter (col);

		if (col.tag == "Player") {
			Vector3 playerPos = col.gameObject.transform.position;
			Vector3 toPlayer = (playerPos - transform.position).normalized;
			if ( Vector3.Dot( toPlayer , - transform.right ) > 0.3f )
				m_animator.SetTrigger ("MoveLeft");
			if ( Vector3.Dot( toPlayer , transform.right ) > 0.3f )
				m_animator.SetTrigger ("MoveRight");
			if ( Vector3.Dot( toPlayer , - transform.forward ) > 0.3f )
				m_animator.SetTrigger ("MoveBack");
			if ( Vector3.Dot( toPlayer , transform.forward ) > 0.3f )
				m_animator.SetTrigger ("MoveFront");
		}
	}

	protected override void MOnTriggerExit (Collider col)
	{
		base.MOnTriggerExit (col);
	}

	public override Vector3 GetInteractCenter ()
	{
		return base.GetInteractCenter () + Vector3.up * 0.5f ;
	}

	float timer;
	float lastTime = 0;

	protected override void MUpdate ()
	{
		base.MUpdate ();

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
		UpdateAI ();
	}

	public void UpdateAI()
	{
		if (walkBetweenSetting.IfWork) {
			if (walkBetweenSetting.agent.remainingDistance <= walkBetweenSetting.agent.stoppingDistance) {
				if (!walkBetweenSetting.agent.hasPath || walkBetweenSetting.agent.velocity.magnitude == 0) {
					walkBetweenSetting.agent.destination = walkBetweenSetting.WalkList [walkBetweenSetting.index % walkBetweenSetting.WalkList.Length].position;
					walkBetweenSetting.index++;
				}
			}
		}
	}

//	IEnumerator TakePhoto()
//	{
//
//		float timer = Random.Range (takePhotoSetting.takePhotoInterval / 3f, takePhotoSetting.takePhotoInterval);
//		float lastTime = 0;
//
//		while (true) {
//			lastTime = timer;
//			timer -= Time.deltaTime;
//			if (timer < 0 && lastTime >= 0 ) {
//				if (takePhotoSetting.flashLight != null)
//					takePhotoSetting.flashLight.enabled = true;
//				if (takePhotoSetting.flashLightPar != null) {
//					takePhotoSetting.flashLightPar.startLifetime = takePhotoSetting.flashLightTime;
//					takePhotoSetting.flashLightPar.Emit (1);
//				}
//				if (flashLightAudioSource != null)
//					flashLightAudioSource.Play ();
//			}
//
//			if (timer < -takePhotoSetting.flashLightTime) {
//				if (takePhotoSetting.flashLight != null)
//					takePhotoSetting.flashLight.enabled = false;
//				timer = Random.Range (takePhotoSetting.takePhotoInterval / 3f, takePhotoSetting.takePhotoInterval);
//			}
//
//
//
//			yield return new WaitForEndOfFrame ();
//		}
//		yield return null;
//	}

}
