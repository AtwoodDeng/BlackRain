using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Ship : MBehavior {

	[SerializeField] RangeTarget[] targets;
	[SerializeField] GameObject ship;
	[SerializeField] GameObject flower;
	int index = 0 ;
	Vector3 temTarget
	{
		get {
			if (index < targets.Length) {
				return targets [index].GetRangeTarget ();
			}
			return Vector3.zero;
		}
	}
	[SerializeField] float speed;
	[SerializeField] float rotateSpeed;
	[SerializeField] LogicEvents showUpEvent;
	[SerializeField] LogicEvents flowerEvent;
	[SerializeField] AudioSource horn;

	protected override void MStart ()
	{
		base.MStart ();
		ship.SetActive (false);
//		flower.SetActive (false);
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (showUpEvent, OnShowUp);
		M_Event.RegisterEvent (flowerEvent, OnFlower);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (showUpEvent, OnShowUp);
		M_Event.UnregisterEvent (flowerEvent, OnFlower);
	}

	void OnShowUp( LogicArg arg )
	{
		ship.SetActive (true);

		Sequence seq = DOTween.Sequence ();

		seq.Append (transform.DOMoveY (10f, 1f).SetRelative (true).From ());
		if ( targets != null &&  targets.Length > 0 )
		{
			Vector3 target = targets [0].GetRangeTarget ();
			target.y = transform.position.y;
			seq.Append (transform.DOMove ( target , 25f ));
		}

		if (horn != null)
			horn.Play ();
	}

	void OnFlower( LogicArg arg )
	{
//		flower.SetActive (true);

		Sequence seq = DOTween.Sequence ();

		for (int i = 1; i < targets.Length; ++i) {
			Vector3 target = targets [i].GetRangeTarget ();
			target.y = transform.position.y;
			seq.Append (transform.DOMove ( target , (targets [i].GetRangeTarget () - targets [i - 1].GetRangeTarget ()).magnitude / speed));
			Vector3 toward = (targets [i].GetRangeTarget () - targets [i - 1].GetRangeTarget ()).normalized;
			toward.y = 0;
			float angle = Vector3.Angle (Vector3.right, toward);
			angle = (toward.z > 0) ? angle : -angle;
			seq.Append( transform.DORotate( new Vector3( 0 , angle , 0 ) , 10f ));
		}
	}

}
