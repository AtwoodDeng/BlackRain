using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GirlStreetOne : TalkableCharacter {
//	[SerializeField] FilmController meetFilm;
	[SerializeField] float followDistance = 1.5f;
	[SerializeField] float moveAcc = 0.1f;
	[SerializeField] float maxAccTime = 0.5f;
	[SerializeField] LayerMask checkUpMask;
	[SerializeField] float sneezeInterval;
	[SerializeField] AudioClip sneezeSound;
	[SerializeField] Animator m_Animator;
	[SerializeField] Transform WatchCrowStay;
	[SerializeField] Transform Crow;
	[SerializeField] FilmController crowController;
	[SerializeField] NarrativePlotScriptableObject crowPlot;
	//	[SerializeField] Transform head;

	float sneezeDuration = 0;

	public enum State
	{
		None,
		Stand,
		See,
		Follow,
		ToWatchCrow,
		StayWatchCrow,
		FollowEnd,
		End,
	}
	AStateMachine<State,LogicEvents> m_stateMachine;

	protected override void MAwake ()
	{
		base.MAwake ();
		InitStateMachine ();
		if (m_Animator == null)
			m_Animator = GetComponentInChildren<Animator> ();
	}

	void InitStateMachine()
	{
		m_stateMachine = new AStateMachine<State, LogicEvents> (State.None);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.SeeOldGrilStreetTwo, State.See, State.Follow); 
		m_stateMachine.BlindStateChangeEvent (LogicEvents.StreetTwoWatchCrow, State.Follow, State.ToWatchCrow);
		m_stateMachine.BlindStateChangeEvent (LogicEvents.StreetTwoWatchCrowEnd, State.StayWatchCrow, State.FollowEnd);
		m_stateMachine.BlindTimeStateChange (State.FollowEnd, State.End, 5f);

		m_stateMachine.AddUpdate (State.Follow, delegate {

			Vector3 target = MainCharacter.Instance.GetShareUmbrellaCenter();
			Vector3 toward = target - transform.position;
			toward.y = 0;
			if (toward.magnitude > followDistance) {
				velocity += moveAcc * Time.deltaTime * toward.normalized;
				velocity.y = 0;
				velocity = Vector3.ClampMagnitude (velocity, moveAcc * maxAccTime);
				transform.position += velocity;
				transform.forward = velocity.normalized;
			} else {
				velocity *= 0.6f;
				transform.position += velocity;
			}

			if ( !CheckUnderObject() )
			{
				sneezeDuration -= Time.deltaTime;
				if ( sneezeDuration < 0 ) {
					sneezeDuration = sneezeInterval;
					OnSneeze();
				}
			}
		});

		m_stateMachine.AddUpdate (State.ToWatchCrow, delegate {
			Vector3 target = WatchCrowStay.position;
			Vector3 toward = target - transform.position;
			toward.y = 0;
			if (toward.magnitude > 0.1f ) {
				velocity += moveAcc * Time.deltaTime * toward.normalized;
				velocity.y = 0;
				velocity = Vector3.ClampMagnitude (velocity, moveAcc * maxAccTime);
				transform.position += velocity;
				transform.forward = velocity.normalized;
			} else {
				m_stateMachine.State = State.StayWatchCrow;
			}
		});

		m_stateMachine.AddEnter (State.StayWatchCrow, delegate {
			Vector3 toward = Crow.position - transform.position;
			toward.y = 0;
			transform.forward = toward;
			m_Animator.SetTrigger("HeadUp");
			if ( filmController != null )
			{
				isFilmPlayed = false;
				filmController = crowController;
			}

			if ( mainPlot != null )
			{
				IsMainEnded = false;
				mainPlot = crowPlot;
			}

		});

		m_stateMachine.AddUpdate (State.FollowEnd, delegate {

			Vector3 target = MainCharacter.Instance.GetShareUmbrellaCenter();
			Vector3 toward = target - transform.position;
			toward.y = 0;
			if (toward.magnitude > followDistance) {
				velocity += moveAcc * Time.deltaTime * toward.normalized;
				velocity.y = 0;
				velocity = Vector3.ClampMagnitude (velocity, moveAcc * maxAccTime);
				transform.position += velocity;
				transform.forward = velocity.normalized;
			} else {
				velocity *= 0.6f;
				transform.position += velocity;
			}

			if ( !CheckUnderObject() )
			{
				sneezeDuration -= Time.deltaTime;
				if ( sneezeDuration < 0 ) {
					sneezeDuration = sneezeInterval;
					OnSneeze();
				}
			}
		});

		m_stateMachine.AddEnter (State.End, delegate {
		
			gameObject.SetActive( false );
		});


		m_stateMachine.State = State.Stand;

	}

	Vector3 velocity;


	protected override void MUpdate ()
	{
		base.MUpdate ();
		m_stateMachine.Update ();
	}

	bool CheckUnderObject()
	{ 
		RaycastHit hitInfo;
		if (Physics.Raycast (transform.position, Vector3.up, out hitInfo, 100f, checkUpMask.value )) {
			return true;
		}
		return false;
	}

	public void OnSneeze()
	{
		AudioManager.PlaySoundOn (sneezeSound, gameObject, 0, 0.8f);
		if (m_Animator != null)
			m_Animator.SetTrigger ("Sneeze");
	}

	public override void Interact ()
	{
		base.Interact ();
		m_stateMachine.State = State.See;
	}


	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterAll (OnEvent);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
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

	void OnGUI()
	{
		GUILayout.Label ("Girl's State " + m_stateMachine.State);
	}
}
