using UnityEngine;
using System.Collections;
using ParticlePlayground;
using DG.Tweening;

public class Desk : MBehavior {

	[SerializeField] GameObject[] personList;
	[SerializeField] MinMax personPerDesk;
	[SerializeField] PlaygroundParticlesC particleC;
	[SerializeField] ParticleSystem particleSystem;
	[SerializeField] Gradient particleColor;
	[SerializeField] Gradient[] colorGradientList;
	[SerializeField] AudioSource interactSource;
	[SerializeField] Transform deskCenter;
	[SerializeField] MeshRenderer topUmbrella;
	[SerializeField] Gradient topUmbrellaColor;
	[SerializeField] Transform deskTrans;
	[SerializeField] AudioSource shakingSound;
	CapsuleCollider m_collider;

	public enum Type
	{
		Normal,
		NoParticle,
		Uninteractable,
		Shakable,
	}
	[SerializeField] Type type = Type.Normal;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (m_collider == null) {
			m_collider = GetComponent<CapsuleCollider> ();
		}
		if (m_collider != null) {
			m_collider.isTrigger = true;
		}
	}
	protected override void MStart ()
	{
		base.MStart ();
		transform.Rotate( new Vector3( 0 , Random.Range( 0 , 360f) , 0 ));
		InitPerson ();
		InitParticle ();
		InitUmbrella ();
	}

	void InitUmbrella()
	{
		topUmbrella.material = new Material (Shader.Find ("AlphaSelfIllum_NoFog"));
		topUmbrella.material.SetColor ("_Color" ,topUmbrellaColor.Evaluate (Random.Range (0, 1f)));

	}

	void InitPerson()
	{
		for (int i = 0; i < personList.Length; ++i)
			personList [i].SetActive (false);
		
		int personNum = (int)personPerDesk.RandomBetween;
		float angle = 360f / personNum;
		for( int i = 0 ; i < personNum ; ++i )
		{
			GameObject tem = personList[Random.Range(0,personList.Length)];
//			Debug.Log ("Set " + tem.name + " to true ");
			Quaternion rotation = Quaternion.Euler(new Vector3 (0, i * angle + Random.Range (-angle / 4f, angle / 4f), 0));
			tem.transform.localRotation = rotation;
			tem.SetActive (true);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.D) && Input.GetKey (KeyCode.LeftControl)) {
			InitPerson ();
		}
	}

	void InitParticle()
	{
		if (particleC != null) {
			particleC.lifetimeColor = colorGradientList[Random.Range(0,colorGradientList.Length)];
		}
		if (particleSystem != null) {
			particleSystem.startColor = particleColor.Evaluate (Random.Range (0, 1f));
		}
	}

	public void Interact ()
	{
		if (type != Type.Uninteractable) {
			if ( type != Type.NoParticle) {
				if (particleC != null) {
					particleC.emissionRate = 1f;
					DOTween.To (() => particleC.emissionRate, (x) => particleC.emissionRate = x, 0, 1f);
				}
				if (particleSystem != null) {
					particleSystem.Emit (250);
				}
				if (interactSource != null) {
					interactSource.Play ();
				}
			}

			if (type == Type.Shakable) {
				if ( deskTrans != null )
					deskTrans.DOShakeRotation (Random.Range(0.8f,1.5f), 12f,2);
				if (shakingSound != null) {
					shakingSound.pitch = Random.Range (0.7f, 1.5f);
					shakingSound.Play ();
				}
				foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>()) {
					if (trans.name.StartsWith ("bottle")) {
						Rigidbody rigid = trans.GetComponent<Rigidbody> ();
						if (rigid != null) {
							Vector3 randVel = Random.rotation.eulerAngles;
							randVel = randVel.normalized;
							randVel.y = -randVel.y;
							rigid.velocity = randVel * 1.5f;
						}
					}
				}
			}

			foreach (ColorfulStreetSitPerson person in gameObject.GetComponentsInChildren<ColorfulStreetSitPerson>()) {
				person.ReactToInteract ();
			}
		}
	}
	protected override void MOnTriggerEnter (Collider col)
	{
		base.MOnTriggerEnter (col);
		if (col.tag == "Player" ) {
			Interact ();
		}
	}
}
