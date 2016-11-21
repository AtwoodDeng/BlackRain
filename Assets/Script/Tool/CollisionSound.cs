using UnityEngine;
using System.Collections;

public class CollisionSound : MBehavior {

	[SerializeField] AudioSource source;
	[SerializeField] bool onlyPlayer;
	[SerializeField] MinMax randomPitch;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (source == null)
			source = GetComponent<AudioSource> ();
		if (source != null) {
			if (randomPitch.RandomBetween != 0)
				source.pitch = randomPitch.RandomBetween;
		}
	}

	protected override void MOnCollisionEnter (Collision col)
	{
		base.MOnCollisionEnter (col);

		if (source != null) {
			if ( !onlyPlayer || col.collider.tag == "Player" )
				source.Play ();
		}
	}
}
