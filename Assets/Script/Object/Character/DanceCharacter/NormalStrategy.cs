using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStrategy : MBehavior {

	[ReadOnlyAttribute] public DanceCharacter parent;

	virtual public void Init( DanceCharacter _p )
	{
		parent = _p;
	}

	virtual public void OnNormalUpdate()
	{
	}

	virtual public void OnNormalEnter()
	{
	}
}
