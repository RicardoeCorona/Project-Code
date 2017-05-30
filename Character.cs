using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour {

	protected int counter;

	[SerializeField]
	protected float movementSpeed;

	protected bool facingRight;

	protected bool attack;

	public bool TakingDamage{ get; set;}

	public abstract bool IsDead{ get;}

	[SerializeField]
	protected Stat healthStat;

	[SerializeField]
	private EdgeCollider2D swordCollider;

	[SerializeField]
	private List<string> damageSources;

	public Animator MyAnimator{ get; private set; }

	public EdgeCollider2D SwordCollider{
		get{
			return swordCollider;
		}
	}

	// Use this for initialization
	public virtual void Start () {
		facingRight = true;
		MyAnimator = GetComponent<Animator>();
		healthStat.Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public abstract IEnumerator TakeDamage();

	public abstract void Death();

	public virtual void ChangeDirection(){
		facingRight = !facingRight;
		transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y*1, transform.localScale.z*1);
	}
		
	public void MeleeAttack(){
		SwordCollider.enabled = true;
	}

	public virtual void OnTriggerEnter2D(Collider2D other){
		if(damageSources.Contains(other.tag)){
			StartCoroutine (TakeDamage());
		}
	}
}
