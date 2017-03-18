using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWithFlim : Interactable {

	[SerializeField] FilmController filmController;
	[SerializeField] bool onlyWorksWithIcon;

	public override void Interact ()
	{
			if (filmController != null)
				filmController.Work ();
	}

	public override bool IsInteractable ()
	{
		return ((onlyWorksWithIcon && NarrativeManager.Instance.narrativeType == NarrativeManager.NarrativeType.Icon) ||
		!onlyWorksWithIcon);
	}
}
