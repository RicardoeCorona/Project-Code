using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Over : MonoBehaviour {
	
	public string retryLevel;

	public void Restart(){
		Application.LoadLevel(retryLevel);
	}

	public void quit(){
		Application.Quit ();
	}
}
