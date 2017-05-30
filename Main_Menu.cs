using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Menu : MonoBehaviour {
	public string startLevel;

	public void NewGame(){
		Application.LoadLevel (startLevel);
	}

	public void QuitGame(){
		Application.Quit();
	}
}
