using UnityEngine;
using System.Collections;

public class TalkableCharacter : Character {

	[SerializeField] NarrativePlotScriptableObject mainPlot;
	[SerializeField] NarrativePlotScriptableObject[] subPlots;

	[SerializeField] Transform head;

	[SerializeField] bool OnlySubPlots;
	public bool IsMainTalked = false;
	[HideInInspector] public bool IsTalking = false;

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

	void OnEndDisplayDialog( LogicArg arg )
	{
		IsTalking = false;
	}

	public override void Interact ()
	{
		if (!IsTalking) {
			base.Interact ();

			if (OnlySubPlots || IsMainTalked ) {
				DisplayDialog (subPlots [Random.Range (0, subPlots.Length)]);
			} else {
				DisplayDialog (mainPlot);
				IsMainTalked = true;
			}
		}
	}

	void DisplayDialog( NarrativePlotScriptableObject plot )
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
			return head.transform.position + Vector3.down * 0.5f ;

		return base.GetInteractCenter ();
	}

	public override bool IsInteractable ()
	{
		return base.IsInteractable () && !IsTalking;
	}
}
