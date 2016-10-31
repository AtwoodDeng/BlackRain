using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Door : Interactable {

	[SerializeField] Transform hand;
	[SerializeField] AudioSource openSound;
	[SerializeField] Vector3 openMove;
	[SerializeField] float openTime = 0.2f;
	[Range(0,0.1f)]
	[SerializeField] float closeSpeed = 0.1f;
	Rigidbody m_rigidBody;
	[SerializeField] LogicManager.GameState openState;

	bool opended 
	{
		get {
			float angle = transform.localRotation.eulerAngles.y;
			return Mathf.Abs (angle) > 3f;
		}
	}

	public override Vector3 GetInteractCenter ()
	{
		if (hand != null)
			return hand.transform.position;
		return base.GetInteractCenter ();
	}
	public override string GetInteractTips ()
	{
		if ( LogicManager.Language == LogicManager.GameLanguage.English)
		return "To Open";
		if (LogicManager.Language == LogicManager.GameLanguage.Chinese)
			return "打开";
		return "";
	}
		
	public override void Interact ()
	{
		base.Interact ();
		if ( openSound != null )
			openSound.Play ();
		transform.DORotate (openMove, openTime).SetRelative (true);

	}

	public override bool IsInteractable ()
	{
		return base.IsInteractable () && !opended && !m_rigidBody.isKinematic;
	}


	protected override void MAwake ()
	{
		base.MAwake ();
		m_rigidBody = GetComponent<Rigidbody> ();

		if (openState != LogicManager.GameState.None) {
			m_rigidBody.isKinematic = true;

			LogicManager.Instance.RegisterStateChange (delegate(LogicManager.GameState fromState, LogicManager.GameState toState) {
				if (toState == openState) {
					m_rigidBody.isKinematic = false;
				}
			});
		}
	}


	protected override void MUpdate ()
	{
		base.MUpdate ();

		float angle = transform.localRotation.eulerAngles.y;
		if (angle > 180f)
			angle = angle - 360f;
		m_rigidBody.AddTorque (new Vector3 (0, - angle * closeSpeed , 0));
		
	}
}
