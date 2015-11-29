﻿using UnityEngine;

public class LevelController : MonoBehaviour {

	public void LoadMenu() {
		Application.LoadLevel("Menu");
	}

	public void LoadGame() {
		Application.LoadLevel("Main");
	}

	public void LoadWW3End() {
		Time.timeScale = 1;
		Application.LoadLevel("WorldWarIII");
	}

	public void LoadCredits() {
		Application.LoadLevel("Credits");
	}
}
