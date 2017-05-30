using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadEventhandler();

public class Player : Character {



	public event DeadEventhandler Dead;

	private Rigidbody2D myRigidBody;

	[SerializeField]
	private Transform[] groundPoints;

	[SerializeField]
	private float groundRadius;

	[SerializeField]
	private LayerMask whatIsGround;
	private bool isGrounded;
	private bool jump;

	[SerializeField]
	private bool airControl;

	[SerializeField]
	private float jumpForce;

	private bool Immortal = false;

	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float immortalTime;

	private Vector3 startPos;

	// Use this for initialization
	public override void Start () {
		base.Start();
		startPos = transform.position;
		myRigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		startPos = transform.position;
		healthStat.Initialize();
	}

	void Update(){
		if(!TakingDamage && !IsDead){
			if(transform.position.y <= -14f){
				Death();
			}
			HandleInput ();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!TakingDamage && !IsDead){
			float horizontal = Input.GetAxis ("Horizontal");
			isGrounded = IsGrounded ();
			handleMovement (horizontal);
			flip (horizontal);
			HandleAttacks ();
			handleLayers ();
			ResetValues ();
		}
	}

	public void OnDead(){
		if(Dead != null){
			Dead();
		}
	}

	private void handleMovement(float horizontal){

		if(myRigidBody.velocity.y < 0){
			MyAnimator.SetBool ("land", true);
		}

		if (isGrounded && jump) {
			isGrounded = false;
			myRigidBody.AddForce (new Vector2 (0, jumpForce));
			MyAnimator.SetTrigger ("jump");
		}

		if (!this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && (isGrounded || airControl)){
			myRigidBody.velocity = new Vector2 (horizontal*movementSpeed, myRigidBody.velocity.y);
		}
		MyAnimator.SetFloat ("speed", Mathf.Abs(horizontal));
	}

	private void HandleAttacks(){
		if (attack && isGrounded &&!this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) {
			MyAnimator.SetTrigger ("attack");
			myRigidBody.velocity = Vector2.zero;
		}
	}

	private void HandleInput(){
		if (Input.GetKeyDown (KeyCode.K)) {
			attack = true;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			jump = true;
		}
	}

	private void flip(float horizontal){
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			ChangeDirection ();
		}
	}

	private void ResetValues(){
		attack = false;
		jump = false;
	}

	private bool IsGrounded(){
		if(myRigidBody.velocity.y <= 0){
			foreach (Transform point in groundPoints) {
				Collider2D[] colliders = Physics2D.OverlapCircleAll (point.position, groundRadius, whatIsGround);
				for (int i = 0; i < colliders.Length; i++) {
					if (colliders [i].gameObject != gameObject) {
						MyAnimator.ResetTrigger ("jump");
						MyAnimator.SetBool ("land", false);
						return true;
					}
				}
			}
		}
		return false;
	}

	private void handleLayers(){
		if (!isGrounded) {
			MyAnimator.SetLayerWeight (1, 1);
		} 
		else {
			MyAnimator.SetLayerWeight (1, 0);
		}
	}

	private IEnumerator IndicateImmortal(){
		while(Immortal){
			spriteRenderer.enabled = false;
			yield return new WaitForSeconds (.1f);
			spriteRenderer.enabled = true;
			yield return new WaitForSeconds (.1f);
		}
	}
		
	#region implemented abstract members of Character

	public override void Death ()
	{
		myRigidBody.velocity = Vector2.zero;
		MyAnimator.SetTrigger ("Idle");
		healthStat.CurrentVal = healthStat.MaxVal;
		transform.position = startPos;
	}

	#endregion

	#region implemented abstract members of Character
	public override bool IsDead {
		get {
			if(healthStat.CurrentVal <= 0){
				OnDead();
			}
			return healthStat.CurrentVal <= 0;
		}
	}
	#endregion

	#region implemented abstract members of Character
	public override IEnumerator TakeDamage ()
	{
		if(!Immortal){
			healthStat.CurrentVal -= 10;
			if (!IsDead) {
				MyAnimator.SetTrigger ("damage");
				Immortal = true;
				StartCoroutine (IndicateImmortal());
				yield return new WaitForSeconds (immortalTime);
				Immortal = false;
			} 
			else {
				MyAnimator.SetLayerWeight (1, 0);
				MyAnimator.SetTrigger ("die");
				Application.LoadLevel ("Game Over");
			}
		}
	}
	#endregion
}
