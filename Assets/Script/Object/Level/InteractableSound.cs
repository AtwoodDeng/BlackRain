using UnityEngine;
using System.Collections;

public class InteractableSound : Interactable {

	[SerializeField] AudioSource sound;
	[SerializeField] ThoughtScritableObject thought;
	[SerializeField] bool randomPitch;
	[SerializeField] float interactCenterHeight = 1f;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (sound != null && randomPitch)
			sound.pitch = Random.Range (0.7f, 1.5f);
	}

	public override void Interact ()
	{
		base.Interact ();
		if (sound != null)
			sound.Play ();

		if (thought != null)
			ThoughtManager.Instance.SendThought (thought.Thought);
	}

	public override Vector3 GetInteractCenter ()
	{
		return transform.position + Vector3.up * interactCenterHeight;
	}
}
