﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuButtons: MonoBehaviour {
	static public int speed;
	
	void Awake() {
		//Debug.Log("menu is awake");
	}

	public void PlayGame () {	
		SceneManager.LoadScene("GameScene");
	}
	
	public void QuitGame () {
		Application.Quit ();
	}
	
	void Update() {
		//Debug.Log("update");
	}

	public void GetInput(string userInput) {
		speed = Int32.Parse(userInput);
		Debug.Log("You entered " + userInput);
	}
	
}
