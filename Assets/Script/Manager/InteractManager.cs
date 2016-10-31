using UnityEngine;
using System.Collections;

public class InteractManager : MBehavior{

	static private InteractManager m_Instance;
	static public InteractManager Instance{
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<InteractManager> ();
			return m_Instance;
		}
	}

	[SerializeField] float interactRange = 5f;
	[SerializeField] LayerMask interactiveMask = -1;

	Interactable s_Interactable;
	Interactable tem_Interactable
	{
		get{ return s_Interactable; }
		set{
			if (s_Interactable != value) {
				if (s_Interactable != null)
					s_Interactable.OnOutOfFocus ();
				s_Interactable = value;
				if (s_Interactable != null)
					s_Interactable.OnFocus ();
			}
		}
	}
	public Interactable TempInteractable
	{
		get { return tem_Interactable; }
	}

	/// <summary>
	/// return the focuse point in screen position
	/// </summary>
	/// <returns>The point.</returns>
	static public Vector2 FocusPoint
	{
		get{
			if ( Application.platform == RuntimePlatform.WindowsPlayer || 
				Application.platform == RuntimePlatform.OSXPlayer || 
				Application.platform == RuntimePlatform.LinuxPlayer )
				return Input.mousePosition;
			return Input.mousePosition;
		}
	}

	protected override void MAwake ()
	{
		base.MAwake ();

	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.Interact] += OnInteract;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.Interact] -= OnInteract;
	}

	void OnInteract(LogicArg arg )
	{
		if (TempInteractable != null)
			TempInteractable.Interact ();
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateTemInteractable ();
	}

	void UpdateTemInteractable()
	{
		RaycastHit[] hits;
//		Ray mainCharacterRay = new Ray (MainCharacter.Instance.transform.position, MainCharacter.Instance.transform.forward);
//		Ray mainCharacterRay = new Ray( Camera.main.transform.position , Camera.main.transform.forward);
		Ray mainCharacterRay = Camera.main.ScreenPointToRay(FocusPoint);
		hits = Physics.RaycastAll (mainCharacterRay, interactRange , interactiveMask.value);

		Interactable target = null;
		foreach( RaycastHit hit in hits )
		{
			target = hit.collider.GetComponent<Interactable> ();
			if (target != null) {
				if (target.IsInteractable() )
					break;
				else
					target = null;
			}
		}

		tem_Interactable = target;
	}

}
