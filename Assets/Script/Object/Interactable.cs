using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Interactable : MBehavior {

	[SerializeField] protected float interactiveRange = 2f;
	[SerializeField] protected MWord interactTips;
//	bool m_inInteractiveRange = false;
	public bool IsInInteractiveRange{ get { return (transform.position - MainCharacter.Instance.transform.position).magnitude < interactiveRange; } }

	public virtual bool IsInteractable()
	{
		return IsInInteractiveRange;
	}

	public virtual Vector3 GetInteractCenter()
	{
		return transform.position;
	}

	public virtual string GetInteractTips()
	{
		return interactTips.word;
	}

	public virtual void Interact()
	{
		
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
	}

	protected override void MAwake ()
	{
		base.MAwake ();
	}






//	 void OnTriggerEnter( Collider col )
//	{
//		if (col.gameObject.tag == "Player") {
//			OnPlayerEnter ();
//		}
//	}
//
//	 void OnTriggerExit( Collider col )
//	{
//		if (col.gameObject.tag == "Player") {
//			OnPlayerExit ();
//		}
//	}
//
//	virtual protected void OnPlayerEnter()
//	{
//		m_inInteractiveRange = true;
//	}
//
//	virtual protected void OnPlayerExit()
//	{
//		m_inInteractiveRange = false;
//	}

	public virtual void OnFocus()
	{
	}

	public virtual void OnOutOfFocus()
	{
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (transform.position, interactiveRange);
	}
}
