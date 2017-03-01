using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWithFlim : Interactable {

	[SerializeField] FilmController filmController;

	public override void Interact ()
	{
		if (filmController != null)
			filmController.Work ();
	}

	public override bool IsInteractable ()
	{
		return true;
	}
}
