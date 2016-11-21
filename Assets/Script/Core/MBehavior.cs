using UnityEngine;
using System.Collections;

public class MBehavior : MonoBehaviour {

	void Awake()
	{
		MAwake ();
	}

	void Start()
	{
		MStart ();
	}

	void Update()
	{
		MUpdate ();
	}

	void OnEnable()
	{
		MOnEnable ();
	}

	void OnDisable()
	{
		MOnDisable ();
	}

	void FixedUpdate()
	{
		MFixedUpdate ();
	}

	void OnCollisionEnter( Collision col )
	{
		MOnCollisionEnter (  col );
	}
	void OnTriggerEnter(Collider col )
	{
		MOnTriggerEnter (col);
	}

	void OnTriggerExit(Collider col )
	{
		MOnTriggerExit (col);
	}

	virtual protected void MAwake() {

	}

	// Use this for initialization
	virtual protected void MStart () {
		
	}
	
	// Update is called once per frame
	virtual protected void MUpdate () {
	
	}

	virtual protected void MOnEnable() {
	}

	virtual protected void MOnDisable() {
	}

	virtual protected void MFixedUpdate() {
	}

	virtual protected void MOnCollisionEnter( Collision col )
	{
		
	}
	virtual protected void MOnTriggerEnter( Collider col )
	{
	}

	virtual protected void MOnTriggerExit( Collider col )
	{
	}
}
