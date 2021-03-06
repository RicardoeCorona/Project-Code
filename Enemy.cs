﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character{

	private IEnemyState currentState;

	public GameObject Target { get; set; }

	[SerializeField]
	private float meleeRange;

	public bool InMeleeRange{
		get{
			if(Target != null){
				return Vector2.Distance (transform.position, Target.transform.position) <= meleeRange;
			}
			return false;
		}
	}

	private Vector3 startPos;

	[SerializeField]
	private Transform leftEdge;

	[SerializeField]
	private Transform rightEdge;


	// Use this for initialization
	public override void Start () {
		base.Start ();
		this.startPos = transform.position;
		ChangeState(new IdleState());
	}

	// Update is called once per frame
	void Update () {
		if(!IsDead){
			if(!TakingDamage){
				currentState.Execute();
			}
			LookAtTarget ();
		}
		else{
			if(healthStat.CurrentVal <= 0){
				counter += 1;
			}
		}
	}

	public void RemoveTarget(){
		Target = null;
		ChangeState (new PatrolState());
	}

	private void LookAtTarget(){
		if (Target != null) {
			float xDir = Target.transform.position.x - transform.position.x;
			if(xDir < 0 && facingRight || xDir > 0 && !facingRight){
				ChangeDirection ();
			}
		}
	}

	public void ChangeState(IEnemyState newState){
		if (currentState != null) {
			currentState.Exit ();
		}
		currentState = newState;
		currentState.Enter (this);
	}

	public void Move(){
		if((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) || (GetDirection().x < 0 && transform.position.x > leftEdge.position.x)){
			MyAnimator.SetFloat ("speed", 1);
			transform.Translate (GetDirection()*(movementSpeed * Time.deltaTime));
		}
		else if(currentState is PatrolState){
			ChangeDirection();
		}
	}

	public Vector2 GetDirection(){
		return facingRight ? Vector2.right : Vector2.left;		
	}

	public override void OnTriggerEnter2D(Collider2D other){
		base.OnTriggerEnter2D (other);
		currentState.OnTriggerEnter (other);
	}
		

	#region implemented abstract members of Character

	public override void Death ()
	{
		Destroy(gameObject);
	}

	#endregion

	#region implemented abstract members of Character

	public override bool IsDead {
		get {
			return healthStat.CurrentVal <= 0;
		}
	}

	#endregion

	#region implemented abstract members of Character
	public override IEnumerator TakeDamage ()
	{ 
		healthStat.CurrentVal -= 10;
		if (!IsDead) {
			MyAnimator.SetTrigger ("damage");
		} 
		else {
			MyAnimator.SetTrigger ("die");
			yield return null;
		}
	}
	#endregion
}
