using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class DanceCharacter : MBehavior {

	public enum State
	{
		None,
		Normal,
		GotoDance,
		Dance,
		GotoDisappear,
		Disappear,
		StandRight,
		BackToNormal,
	}

//	public enum NormalState
//	{
//		None,
//		RandomMove,
//		Stand,
//	}

	public enum DanceAnimation
	{
		None = 0,
		Normal = 1,
		HoldUp = 2,
		Spin = 3 ,
		Front = 4,
		SpinFront = 5,
		Run = 6,
		FrontHold = 7,
		HoldDown = 8,
		SpinSide = 9,
	}

	[ReadOnlyAttribute] public Collider m_collider;
	[ReadOnlyAttribute] public Animator m_animator;
	[ReadOnlyAttribute] public NavMeshAgent m_agent;
	[SerializeField] public MinMax moveSpeed;
	AStateMachine<State,LogicEvents> m_stateMachine = new AStateMachine<State,LogicEvents>(State.None);
	[ReadOnlyAttribute] public State m_state;
	[SerializeField] State initState;
//	[SerializeField] NormalState normalType;
	[SerializeField] RangeTarget rangeTarget;
	[SerializeField] BasicCharacter dancerCharacter;
	[SerializeField] BasicCharacter normalCharacter;
	[ReadOnlyAttribute] public DanceStrategy m_strategy;
	[ReadOnlyAttribute] public NormalStrategy m_normalStrategy;
	[ReadOnlyAttribute] public Vector3 OriginalPosition;
	[ReadOnlyAttribute] public Quaternion OriginalRotation;
	[SerializeField] DanceCharacter nextCharacter;
//	[SerializeField] LogicEvents startEvent;
	[SerializeField] bool isShowUpOnStart = true;
	[SerializeField] bool isAliveOnEnd = false;
	[SerializeField] LogicEvents endEvent;
	public float runSpeedUp = 2.25f;
	[HideInInspector] public int countRecord;

	[System.Serializable]
	public class CharacterColor
	{
		public Gradient DancerUmbrellaColor;
		public Gradient DancerBodyColor;
		public Gradient NormalUmbrellaColor;
		public Gradient NormalBodyColor;
		public bool UseFakeShadow;
	}
	[SerializeField] CharacterColor characterColor;
	 
	protected override void MAwake ()
	{
		base.MAwake ();
		if ( m_collider == null )
			m_collider = GetComponent<Collider> ();
		m_collider.isTrigger = true;
		if (m_animator == null)
			m_animator = GetComponentInChildren<Animator> ();
		if (m_agent == null) {
			m_agent = gameObject.AddComponent<NavMeshAgent> ();
			m_agent.speed = moveSpeed.RandomBetween;
			m_agent.angularSpeed = 120f;
		}
		if (m_strategy == null) {
			m_strategy = gameObject.GetComponent<DanceStrategy> ();
			if (m_strategy == null)
				m_strategy = gameObject.AddComponent<DanceStrategy> ();
			m_strategy.Init (this);
		}
		if (m_normalStrategy == null) {
			m_normalStrategy = gameObject.GetComponent<NormalStrategy> ();
			if (m_normalStrategy != null)
				m_normalStrategy.Init (this);
		}
		OriginalPosition = transform.position;
		OriginalRotation = transform.rotation;
	
		dancerCharacter.isShowShadow = characterColor.UseFakeShadow;
		dancerCharacter.UpdateColor (characterColor.DancerUmbrellaColor, characterColor.DancerBodyColor );
		normalCharacter.UpdateColor (characterColor.NormalUmbrellaColor, characterColor.NormalBodyColor );
	}

	protected override void MStart ()
	{
		base.MStart ();
		InitStateMachine ();

		if ( isShowUpOnStart )
			transform.position = GetInitPosition ();
	}


	void InitStateMachine()
	{
		m_stateMachine.BlindStateChangeEvent (LogicEvents.ToDark, State.Normal, State.GotoDance);
//		m_stateMachine.BlindStateChangeEvent (LogicEvents.ToModern, State.Dance, State.Normal);
//		if ( startEvent != null )
		//			m_stateMachine.BlindFromEveryState (startEvent, initState);
		if ( endEvent != null )
			m_stateMachine.BlindFromEveryState (endEvent, State.GotoDisappear);
		if (isAliveOnEnd) {
			m_stateMachine.BlindStateChangeEvent (LogicEvents.ToModern, State.GotoDance, State.BackToNormal);
			m_stateMachine.BlindStateChangeEvent (LogicEvents.ToModern, State.Dance, State.BackToNormal);
		} else {
			m_stateMachine.BlindStateChangeEvent (LogicEvents.ToModern, State.GotoDance, State.GotoDisappear);
			m_stateMachine.BlindStateChangeEvent (LogicEvents.ToModern, State.Dance, State.GotoDisappear);
		}


		m_stateMachine.AddUpdate (State.Normal, delegate() {
			if ( m_normalStrategy == null )
			{
				if ( IsGetDestination()) {
					m_agent.SetDestination( GetNormalDestination() );
				}
			}
			else{
				m_normalStrategy.OnNormalUpdate();
			}
//			switch(normalType )
//			{
//				case NormalState.RandomMove:
//					break;
//				case NormalState.Stand:
//					// just stand
//					break;
//			}
		});


		m_stateMachine.AddEnter (State.StandRight, delegate() {
			transform.position = OriginalPosition;
			transform.rotation = OriginalRotation;
			m_stateMachine.State = State.Dance;	
		});

		m_stateMachine.AddEnter (State.GotoDance, delegate() {
			m_agent.speed *= runSpeedUp;
			SetTrigger(DanceAnimation.Run);
			m_strategy.OnGotoDanceEnter();
			dancerCharacter.m_bodyCollider.enabled = false;
			normalCharacter.m_bodyCollider.enabled = false;
		});

		m_stateMachine.AddUpdate (State.GotoDance, delegate() {
			m_strategy.OnGotoDanceUpdate();
		});

		m_stateMachine.AddExit (State.GotoDance, delegate() {
			m_agent.speed /= runSpeedUp;
			SetTrigger(DanceAnimation.Normal);
			dancerCharacter.m_bodyCollider.enabled = true;
			normalCharacter.m_bodyCollider.enabled = true;
		});

		m_stateMachine.AddEnter (State.Dance, delegate() {
			m_strategy.OnDanceEnter();	
		});

		m_stateMachine.AddUpdate (State.Dance, delegate() {
			m_strategy.OnDanceUpdate();
		});

		m_stateMachine.AddEnter (State.GotoDisappear, delegate() {
			m_agent.speed *= runSpeedUp;
			SetTrigger(DanceAnimation.Run);

			if ( nextCharacter != null )
				m_agent.SetDestination( nextCharacter.transform.position );
			else 
				m_agent.SetDestination( OriginalPosition );

			dancerCharacter.m_bodyCollider.enabled = false;
			normalCharacter.m_bodyCollider.enabled = false;
			
		});

		m_stateMachine.AddUpdate (State.GotoDisappear, delegate() {
			if ( IsGetDestination() )
			{
//				m_stateMachine.State = State.Disappear;
				if ( nextCharacter != null )
				{
					transform.forward = Vector3.Lerp( transform.forward , nextCharacter.transform.forward , 0.2f );
					if ( Vector3.Angle( transform.forward , nextCharacter.transform.forward ) < 5f )
						m_stateMachine.State = State.Disappear;
				}else {
					m_stateMachine.State = State.Disappear;
				}
			}
		});

		m_stateMachine.AddExit (State.GotoDisappear, delegate() {
			m_agent.speed /= runSpeedUp;
			SetTrigger(DanceAnimation.Normal);
			if ( nextCharacter != null )
				nextCharacter.Active();	

			dancerCharacter.m_bodyCollider.enabled = true;
			normalCharacter.m_bodyCollider.enabled = true;
		});


		m_stateMachine.AddEnter (State.Disappear, delegate {
			dancerCharacter.gameObject.SetActive( false );
			normalCharacter.gameObject.SetActive( false );
			m_agent.enabled = false;
			m_collider.enabled = false;
		});

		m_stateMachine.AddExit (State.Disappear, delegate() {
			dancerCharacter.gameObject.SetActive( true );
			normalCharacter.gameObject.SetActive( true );
			m_agent.enabled = true;
			m_collider.enabled = true;
				
		});

		m_stateMachine.AddEnter (State.BackToNormal, delegate() {

			dancerCharacter.m_bodyCollider.enabled = false;
			normalCharacter.m_bodyCollider.enabled = false;
			m_agent.speed *= runSpeedUp;
			SetTrigger(DanceAnimation.Normal);
			m_agent.SetDestination( GetNormalDestination() );
		});

		m_stateMachine.AddUpdate (State.BackToNormal, delegate() {

			if ( IsGetDestination())
				m_stateMachine.State = State.Normal;
		});

		m_stateMachine.AddEnter (State.BackToNormal, delegate() {

			m_agent.speed /= runSpeedUp;
			dancerCharacter.m_bodyCollider.enabled = true;
			normalCharacter.m_bodyCollider.enabled = true;
		});

		if ( isShowUpOnStart )
			m_stateMachine.State = initState;
		else
			m_stateMachine.State = State.Disappear;
	}

	public void Active()
	{
		m_stateMachine.State = initState;
	}

	public void SetStateFromTo( State fromState , State toState ){
		if (m_stateMachine.State == fromState)
			m_stateMachine.State = toState;
	}


	public void SetTrigger( DanceAnimation animation )
	{
		if (animation != DanceAnimation.None) {
			normalCharacter.SetTrigger (animation.ToString ());
			dancerCharacter.SetTrigger (animation.ToString ());
		}
	}

	public Vector3 GetInitPosition()
	{
		return GetNormalDestination ();
	}

	public Vector3 GetNormalDestination()
	{
		Vector3 destination = transform.position;
		if ( rangeTarget  != null )
			destination = rangeTarget.GetRangeTarget();
		else 
		{
			destination = Random.insideUnitSphere * 3f;
			destination.y = 0 ;
			destination += transform.position;
		}
		return destination;
		
	}

	public bool IsGetDestination()
	{
		return (m_agent.remainingDistance <= m_agent.stoppingDistance ) &&
			( !m_agent.hasPath || m_agent.velocity.magnitude == 0);
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		m_stateMachine.Update ();
		m_state = m_stateMachine.State;
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterAll (OnEvent);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnRegisterAll (OnEvent);
	}

	void OnEvent( LogicArg arg )
	{
		m_stateMachine.OnEvent (arg.type);
		if (arg.type == LogicEvents.MusicBeat) {
			int count = (int)arg.GetMessage (M_Event.EVENT_BEAT_COUNT);
			countRecord = count;
			if ( m_stateMachine.State == State.Dance) {
				OnBeat (count);
			}
		}
	}

	void OnBeat( int count ){
		m_strategy.OnBeat ( count );
	}

	void OnDrawGizmos()
	{
		if (nextCharacter != null) {
			Gizmos.color = Color.blue;
			Gizmos.DrawLine (transform.position, nextCharacter.transform.position);
		}
	}

	void OnDrawGizmosSelected()
	{
		if (nextCharacter != null) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine (transform.position, nextCharacter.transform.position);
		}
	}

}
