using UnityEngine;
using System.Collections;

public class TalkableCharacter : Character {

	[SerializeField] NarrativePlotScriptableObject mainPlot;
	[SerializeField] NarrativePlotScriptableObject[] subPlots;

	[SerializeField] Transform head;

	[SerializeField] bool OnlySubPlots;
	[HideInInspector] public bool IsMainEnded = false;
	[HideInInspector] public bool IsTalking = false;
	[HideInInspector] public bool isMainTalking = false;
	[SerializeField] LogicEvents MainTalkEndEvent;

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


	virtual protected void OnEndDisplayDialog( LogicArg arg )
	{
		if (isMainTalking) {

			if (MainTalkEndEvent != LogicEvents.None) {
				M_Event.FireLogicEvent (MainTalkEndEvent, new LogicArg (this));
			}
			isMainTalking = false;
			IsMainEnded = true;
		}
		if (IsTalking) {
			IsTalking = false;
		}
	}

	public override void Interact ()
	{
		if (!IsTalking) {
			base.Interact ();

			if (OnlySubPlots || IsMainEnded ) {
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

	protected void DisplayDialog( NarrativePlotScriptableObject plot )
	{
		
		// display dialog
		LogicArg arg = new LogicArg (this);
		arg.AddMessage (M_Event.EVENT_DISPLAY_DIALOG_PLOT, plot);
		M_Event.FireLogicEvent (LogicEvents.DisplayDialog, arg);

		IsTalking = true;
	}


	public override string GetInteractTips ()
	{
		if (LogicManager.Language == LogicManager.GameLanguage.English)
			return "To Talk";
		if (LogicManager.Language == LogicManager.GameLanguage.Chinese)
			return "交谈";
		return "";
	}

	public override Vector3 GetInteractCenter ()
	{
		if (head != null)
			return head.transform.position + InteractionPointOffset;

		return base.GetInteractCenter ();
	}

	public override bool IsInteractable ()
	{
		return base.IsInteractable () && !IsTalking && ( mainPlot != null || subPlots.Length > 0 );
	}
}
