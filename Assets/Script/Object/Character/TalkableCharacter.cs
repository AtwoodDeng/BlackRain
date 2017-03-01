using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TalkableCharacter : Character {

	[SerializeField] protected NarrativePlotScriptableObject mainPlot;
	[SerializeField] protected NarrativePlotScriptableObject[] subPlots;

	[SerializeField] Transform head;

	[SerializeField] bool OnlySubPlots;
	[HideInInspector] public bool IsMainEnded = false;
	[HideInInspector] public bool IsTalking = false;
	float becomeTalkableDelay = 1.5f;
	[HideInInspector] public bool isMainTalking = false;
	[SerializeField] LogicEvents MainTalkEndEvent;
	[SerializeField] Transform talkCamera;
	[SerializeField] float talkCameraChangeDuration = 1f;
	[SerializeField] FilmController filmController;
	[SerializeField] bool filmOnce;
	bool isFilmPlayed = false;
	public bool IsFlimPlayable{
		get {
			return (!filmOnce) || (!isFilmPlayed);
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
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.EndDisplayDialog] -= OnEndDisplayDialog;
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

	virtual protected void OnEndDisplayDialog( LogicArg arg )
	{
		TalkableCharacter character = (TalkableCharacter)arg.GetMessage (M_Event.EVENT_END_DISPLAY_SENDER);
		if (character == this) {
			if (isMainTalking) {

				if (MainTalkEndEvent != LogicEvents.None) {
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

			if (filmController != null) {
				filmController.Work ();
				isFilmPlayed = true;
			}else if (OnlySubPlots || IsMainEnded ) {
				DisplaySubDialog ();
			} else {
				DisplayDialog (mainPlot);
				isMainTalking = true;
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

	public override Vector3 GetInteractCenter ()
	{
		if (head != null) {
//			Debug.Log ("Head " + head.transform.position + "offset " + InteractionPointOffset);
			return head.transform.position + InteractionPointOffset;
		}

		return base.GetInteractCenter ();
	}

	public override bool IsInteractable ()
	{
		return base.IsInteractable () && !IsTalking && ( mainPlot != null || subPlots.Length > 0 || (filmController != null && IsFlimPlayable) );
	}

}
