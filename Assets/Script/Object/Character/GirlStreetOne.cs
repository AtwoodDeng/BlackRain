using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlStreetOne : Interactable {
	[SerializeField] FilmController meetFilm;
	[SerializeField] float followDistance = 1.5f;
	[SerializeField] float moveIntense = 1f;
	[SerializeField] Transform head;

	public enum State
	{
		None,
		Stand,
		See,
		Follow,

	}
	AStateMachine<State,LogicEvents> m_stateMachine;

	protected override void MAwake ()
	{
		base.MAwake ();
		InitStateMachine ();
	}

	void InitStateMachine()
	{

		m_stateMachine = new AStateMachine<State, LogicEvents> (State.None);

		m_stateMachine.BlindStateChangeEvent (LogicEvents.SeeGrilInStreetOne, State.See, State.Follow); 
		m_stateMachine.State = State.Stand;
	}

	Vector3 velocity;

	protected override void MUpdate ()
	{
		base.MUpdate ();
		m_stateMachine.Update ();

		if (m_stateMachine.State == State.Follow) {
			Vector3 target = MainCharacter.Instance.transform.position;
			Vector3 toward = target - transform.position;
			if (toward.magnitude > followDistance) {
				velocity += moveIntense * Time.deltaTime * toward.normalized;
			} else {
				velocity *= 0.6f;
			}
			transform.position += velocity;
		}
	}

	public override void Interact ()
	{
		m_stateMachine.State = State.See;
		meetFilm.Work ();

	}
	void OnEnable()
	{
		M_Event.RegisterAll (OnEvent);
	}

	void OnDisable()
	{
		M_Event.RegisterAll (OnEvent);
	}

	void OnEvent(LogicArg arg)
	{
		m_stateMachine.OnEvent (arg.type);
	}

	public void OnSelectedGizmosDraw()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (transform.position, followDistance);
	}

	public override Vector3 GetInteractCenter ()
	{
		return head.position;
	}
}
