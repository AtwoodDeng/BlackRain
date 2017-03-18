using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoDoor : MBehavior {
	public enum State
	{
		Close,
		Open
	}

	public enum Tags{
		PasserBy,
		Player,
	}

	[SerializeField] Vector3 MoveTo;
	[SerializeField] GameObject moveTarget;
	[ReadOnlyAttribute]  float moveTime = 1f;
	[SerializeField] Ease easeType = Ease.InOutSine;
	[SerializeField] Collider m_collider;
	[SerializeField] Tags senseTag;
	[ReadOnlyAttribute] public List<GameObject> inObject = new List<GameObject>();
	AStateMachine<State,LogicEvents> m_stateMachine;
	Vector3 oriPosition;
	[SerializeField] LogicManager.GameState startState = LogicManager.GameState.Enter;
	[SerializeField] LogicManager.GameState endState = LogicManager.GameState.End;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (m_collider == null)
			m_collider = GetComponent<Collider> ();
		if (m_collider != null)
			m_collider.isTrigger = true;

		InitStateMachine ();
		oriPosition = moveTarget.transform.position;
	}

	public void InitStateMachine()
	{
		m_stateMachine = new AStateMachine<State, LogicEvents> (State.Close);

		m_stateMachine.AddEnter (State.Close, delegate() {
			if ( moveTarget != null )
			{
				moveTarget.transform.DOKill();
				Debug.Log("Enter Colse");
				moveTarget.transform.DOMove(oriPosition, moveTime).SetEase(easeType);
			}
		});

		m_stateMachine.AddUpdate (State.Close, delegate {
			if ( inObject.Count > 0 )
				m_stateMachine.State = State.Open;
		});

		m_stateMachine.AddEnter (State.Open, delegate() {
			if ( moveTarget != null )
			{
				moveTarget.transform.DOKill();
				Debug.Log("EnterOpen");
				moveTarget.transform.DOMove(oriPosition + MoveTo, moveTime).SetEase(easeType);
			}

		});

		m_stateMachine.AddUpdate (State.Open, delegate {
			if ( inObject.Count <= 0 )
				m_stateMachine.State = State.Close;

		});
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		m_stateMachine.Update ();
	}

	protected override void MOnTriggerEnter (Collider col)
	{
		base.MOnTriggerEnter (col);
	
		if ( LogicManager.Instance.State >= startState && LogicManager.Instance.State < endState )
		if (col.gameObject.tag == senseTag.ToString()) {
			if (!inObject.Contains (col.gameObject))
				inObject.Add (col.gameObject);
			
		}
	}

	protected override void MOnTriggerExit (Collider col)
	{
		base.MOnTriggerExit (col);

		if ( LogicManager.Instance.State >= startState && LogicManager.Instance.State < endState )
		if (inObject.Contains (col.gameObject))
			inObject.Remove (col.gameObject);
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (transform.position, transform.position + MoveTo);
	}
}
