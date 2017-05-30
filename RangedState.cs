using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedState : MonoBehaviour, IEnemyState {

	private Enemy enemy;
	#region IEnemyState implementation

	public void Execute ()
	{
		if(enemy.InMeleeRange){
			enemy.ChangeState (new MeleeState ());
		}
		else if (enemy.Target != null) {
			enemy.Move ();
		} 
		else {
			enemy.ChangeState (new IdleState ());
		}
	}

	public void Enter (Enemy enemy)
	{
		this.enemy = enemy;
	}

	public void Exit ()
	{
		
	}

	public void OnTriggerEnter (Collider2D other)
	{
		
	}

	#endregion


}
