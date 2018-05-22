﻿using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour {
	
	public Shader shader1;
	public static Sprite[] sprites;
	public AudioClip moveSound;
	public static int repetition = 20;
	public int totalRepetition = 0;
	public static int level = 0;
	public static int patternIndex = 0;
	public bool moveFlag = false;

	GridDecorate gd = new GridDecorate();
	GameLogic gl = new GameLogic();
	GUIStyle guiStyle = new GUIStyle();
	GUIStyle boxStyle = new GUIStyle();
	GUIStyle boxStyle1 = new GUIStyle();
	List<Board> boardList = new List<Board>();
	Loader loader = new Loader();
	
	public static string touchDataPath;
	public static string wrongDataPath;
	public static int playerPoints = 0;
	public static string statusText;
	public static bool right = false;
	private List<string> labels = new List<string>();
	
	protected void OnGUI(){
		guiStyle.fontSize = 50;
		guiStyle.normal.textColor = Color.black;
		boxStyle.fontSize = 70;
		boxStyle.normal.textColor = Color.green;
		boxStyle1.fontSize = 70;
		boxStyle1.normal.textColor = Color.red;
		GUILayout.Label ("\n level: " + level + "          points:" + playerPoints, guiStyle);
		if (right) {
			GUI.Box(new Rect(250, 400, 500, 100), statusText, boxStyle);	
		}
		else if(!right) {
			GUI.Box(new Rect(250, 400, 500, 100), statusText, boxStyle1);
		}
		else {
			GUI.Box(new Rect(250, 400, 500, 100), statusText, boxStyle1);
		}
	}

	void LoadSprites(){
		sprites = Resources.LoadAll<Sprite>("sprites/Scavengers_SpriteSheet");
		shader1 = Shader.Find ("Outlined/Silhouetted Diffuse");
	}

	void SaveFile(){
		string filePath = Application.persistentDataPath;
		string f1 =  string.Format(@"RIGHT{0}.csv", Guid.NewGuid());
		string f2 =  string.Format(@"WRONG{0}.csv", Guid.NewGuid());
		touchDataPath = filePath + "/" + f1;
		wrongDataPath = filePath + "/" + f2;
		File.Create(touchDataPath);
		File.Create(wrongDataPath);
	}

	void Awake(){
		LoadSprites();
		SaveFile();
	}

	void SetTotalRepetition(){
		totalRepetition = (repetition * labels.Count)/2; // Labels are 14*2 but occur in pair so divide by 2 
	}

	void GameOver() {
		level = 0;
		ListenerStop();
		SceneManager.LoadScene("Menu");
	}

	void ListenerStop() {
		EventManager.StopListening("success", NextBoard);
		EventManager.StopListening("fail", ReloadLevel);
		EventManager.StopListening("gameover", GameOver);
		EventManager.StopListening("matches", Vibration);
	}
	
	Board GetBoard(){
		return boardList[patternIndex];
	}

	void Vibration() {
		Handheld.Vibrate();
	}

	void PlayWrongMoveSound() {
		AudioSource audio = GetComponent<AudioSource>();
		audio.pitch = 0.95f;
		audio.clip = moveSound;
		audio.Play();
	}

	void SetMoveFlag() {
		moveFlag = true;
	}

	void UnsetMoveFlag() {
		moveFlag = false;
	}
	
	void ListenersInit() {
		EventManager.StartListening("success", NextBoard);
		EventManager.StartListening("fail", ReloadLevel);
		EventManager.StartListening("gameover", GameOver);
		EventManager.StartListening("matches", Vibration);
	}
	
	void Start(){
		boardList = loader.ReadFileTest();
		labels = loader.GetLabels();
		SetTotalRepetition ();
		ListenersInit();
		InitBoard();
	}
	
	void ClearBoard() {
		Debug.Log("clearing " + level.ToString() + "in main.cs");
		gd.Clear();
		GetBoard().ClearVariableState();
	}
	
	void SetBoardLevel(){
		if (level % repetition == 0 && level != 0) {
			patternIndex += 1;
		}
	}

	void InitBoard(){
		SetBoardLevel ();
		GetBoard().LoadLinkedList();
		SphereController.instance.SetBoard(GetBoard());
	}

	void NextBoard() {
		statusText = "Correct Pattern!";
		right = true;
		playerPoints += 100;
		ClearBoard ();
		level += 1;
		InitBoard();
	}

	void ReloadLevel() {
		PlayWrongMoveSound();
		statusText = "Wrong Pattern!";
		right = false;
		ClearBoard();
		GetBoard().LoadLinkedList();
		SphereController.instance.SetBoard(GetBoard());
	}

	void Update(){
		if (level < totalRepetition) {
			gl.TouchLogic (GetBoard());	
		}
		else {
			Debug.Log("gameover triggered in Main");
			EventManager.TriggerEvent("gameover");
		}
	}
}