using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(CapsuleCollider))]
public class TalkableCharacter : Character {

	[SerializeField] Transform head;
	[SerializeField] protected NarrativePlotScriptableObject mainPlot;
	[SerializeField] protected NarrativePlotScriptableObject[] subPlots;
	[SerializeField] bool OnlySubPlots;
	[ReadOnlyAttribute] public bool IsMainEnded = false;
	[ReadOnlyAttribute] public bool IsTalking = false;
	public bool IsTalkable{
		get {
			if (mainPlot != null && !IsMainEnded)
				return true;
			if (subPlots.Length > 0)
				return true;
			return false;
		}
	}
	float becomeTalkableDelay = 1.5f;
	[ReadOnlyAttribute] public bool isMainTalking = false;
	[SerializeField] LogicEvents MainTalkEndEvent;
	[SerializeField] Transform talkCamera;
	[SerializeField] float talkCameraChangeDuration = 1f;
	[SerializeField] protected FilmController filmController;
	[SerializeField] protected bool filmOnce;
	protected bool isFilmPlayed = false;
	public bool IsFlimPlayable{
		get {
			
			return ( filmController != null ) && ((!filmOnce) || (!isFilmPlayed));
		}
	}
	[SerializeField] IconNarrativeDialog[] iconNarrativeList;
	[SerializeField] bool isLockIconNarrative;
	[ReadOnlyAttribute] int iconNarrativeIndex = -1;
	public bool IsIconNarrativePlayable{
		get {
			return iconNarrativeList != null && iconNarrativeList.Length > 0;
		}
	}

	static public  Vector3 InteractionPointOffset
	{
		get { return Vector3.down * 0.5f; }
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.EndDisplayDialog] += OnEndDisplayDialog;
		M_Event.logicEvents [(int)LogicEvents.EndFilmControl] += OnEndFilmControl;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.EndDisplayDialog] -= OnEndDisplayDialog;
		M_Event.logicEvents [(int)LogicEvents.EndFilmControl] -= OnEndFilmControl;
	}

	protected override void MAwake ()
	{
		base.MAwake ();
		if (interactTips.wordEng == "")
			interactTips.wordEng = "Talk";
		if (interactTips.wordChinese == "")
			interactTips.wordChinese = "交谈";
		if (mainPlot != null)
			mainPlot.important = true;

		if (head == null) {
			foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>()) {
				if (trans.name.Contains ("Head"))
					head = trans;
//			if (trans.name == "body") {
//				if (trans.GetComponent<Collider> () == null) {
//					trans.gameObject.AddComponent<CapsuleCollider> ();
//					trans.gameObject.layer = LayerMask.NameToLayer ("PasserByBody");
//				}
//			}
			}
		}
	}

	virtual protected void OnEndFilmControl( LogicArg arg )
	{
		
	}

	virtual protected void OnEndDisplayDialog( LogicArg arg )
	{
		TalkableCharacter character = (TalkableCharacter)arg.GetMessage (M_Event.EVENT_END_DISPLAY_SENDER);
//		Debug.Log ("On End Display Dialog");
		if (character == this) {
			if (isMainTalking) {

				if (MainTalkEndEvent != LogicEvents.None) {
//					Debug.Log ("Fire Main talk End" + MainTalkEndEvent.ToString ());
					M_Event.FireLogicEvent (MainTalkEndEvent, new LogicArg (this));
				}
				isMainTalking = false;
				IsMainEnded = true;
			}

			if (IsTalking && gameObject.activeSelf) {
				StartCoroutine (DelayBecomeTalkable (becomeTalkableDelay));
			}
		}
	}

	IEnumerator DelayBecomeTalkable( float delay )
	{
		yield return new WaitForSeconds (delay);

		IsTalking = false;
	}

	public override void Interact ()
	{
		if (!IsTalking) {
			base.Interact ();

			if (NarrativeManager.Instance.narrativeType == NarrativeManager.NarrativeType.Dialog) {
				if (OnlySubPlots || IsMainEnded ) {
					DisplaySubDialog ();
				} else {
					DisplayDialog (mainPlot);
					isMainTalking = true;
				}
			}
			if (NarrativeManager.Instance.narrativeType == NarrativeManager.NarrativeType.Icon) {
				if (IsFlimPlayable) {
					if (filmController.character == null)
						filmController.character = this;
					filmController.Work ();
					isFilmPlayed = true;
				} else if (IsIconNarrativePlayable) {
					LogicArg arg = new LogicArg (this);
					if (iconNarrativeIndex == -1 || !isLockIconNarrative)
						iconNarrativeIndex = Random.Range (0, iconNarrativeList.Length);
					iconNarrativeList [iconNarrativeIndex].thisCharacter = this;
					arg.AddMessage(M_Event.EVENT_ICON_NARRATIV_DIALOG , iconNarrativeList[ iconNarrativeIndex ] );
					M_Event.FireLogicEvent (LogicEvents.DisplayIconDialog, arg);
				}
			}

		}
	}
	protected void DisplaySubDialog()
	{
		if ( subPlots.Length > 0 )
			DisplayDialog (subPlots [Random.Range (0, subPlots.Length)]);
	}

	virtual protected void DisplayDialog( NarrativePlotScriptableObject plot )
	{
		if (plot !=null && plot.dialogs != null && plot.dialogs.Count > 0) {
			if (plot.dialogs.Count > 1)
				plot.important = true;

			// display dialog
			LogicArg arg = new LogicArg (this);
			arg.AddMessage (M_Event.EVENT_DISPLAY_DIALOG_PLOT, plot);
			M_Event.FireLogicEvent (LogicEvents.DisplayDialog, arg);

			// switch to talking if the plot is important
			if ( plot.important )
				IsTalking = true;
		}
	}

	public virtual bool NeedMoveCamera()
	{
		return talkCamera != null && !IsMainEnded;
	}

	public virtual void MoveCamera( Camera cam )
	{
		cam.transform.DOMove (talkCamera.position, talkCameraChangeDuration);
		cam.transform.DORotate (talkCamera.eulerAngles, talkCameraChangeDuration);
	}

	public Vector3 GetViewCenter()
	{
		if (head != null) {
			return head.transform.position;
		}
		return transform.position;
	}

	public override Vector3 GetInteractCenter ()
	{
		if (head != null) {
			return head.transform.position + InteractionPointOffset;
		}

		return base.GetInteractCenter ();
	}

	public override bool IsInteractable ()
	{
		if (NarrativeManager.Instance.narrativeType == NarrativeManager.NarrativeType.Dialog)
			return base.IsInteractable () && !IsTalking && IsTalkable;
		
		return base.IsInteractable () && ( IsFlimPlayable || IsIconNarrativePlayable );
	}

}
