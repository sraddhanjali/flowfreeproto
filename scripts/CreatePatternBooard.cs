﻿using System;
using UnityEngine;
using System.Collections;
using System.IO;                    // For parsing text file, StringReader
using System.Collections.Generic;


public class CreatePatternBooard : MonoBehaviour {

	Dictionary<int, List<List<int>>> firstGrid = new Dictionary<int, List<List<int>>>() {
		{1, new List<List<int>>{ new List<int>{1, 2, 3, 5, 7, 8, 9}, new List<int>{3, 5, 6, 8}, new List<int>{7, 4, 1, 2, 3, 5, 9}, new List<int>{1, 4, 7, 5, 3, 6, 9}, new List<int>{3, 2, 1, 4, 7, 8, 9}, new List<int>{7, 4, 1, 2, 3, 6, 9} } }, 
		{2, new List<List<int>>{ new List<int>{3, 2, 1, 4, 5, 6, 9, 8, 7}, new List<int>{1, 2, 3, 5, 7}, new List<int>{1, 2, 3, 6, 9, 8, 7}, new List<int>{3, 2, 1, 5, 9, 8, 7}, new List<int>{1, 2, 3, 5, 7, 8, 9}, new List<int>{3, 5, 6, 8}, new List<int>{7, 4, 1, 2, 3, 5, 9}, new List<int>{1, 4, 7, 5, 3, 6, 9}, new List<int>{3, 2, 1, 4, 7, 8, 9}, new List<int>{7, 4, 1, 2, 3, 6, 9} } },
		{3, new List<List<int>>{ new List<int>{1, 2, 3, 5, 7, 8, 9}, new List<int>{3, 5, 6, 8}, new List<int>{7, 4, 1, 2, 3, 5, 9}, new List<int>{1, 4, 7, 5, 3, 6, 9}, new List<int>{3, 2, 1, 4, 7, 8, 9}, new List<int>{7, 4, 1, 2, 3, 6, 9} } },
		{4, new List<List<int>>{ new List<int>{3, 2, 1, 4, 5, 6, 9, 8, 7}, new List<int>{1, 2, 3, 5, 7}, new List<int>{1, 2, 3, 6, 9, 8, 7}, new List<int>{3, 2, 1, 5, 9, 8, 7}, new List<int>{1, 2, 3, 5, 7, 8, 9}, new List<int>{3, 5, 6, 8}, new List<int>{7, 4, 1, 2, 3, 5, 9}, new List<int>{1, 4, 7, 5, 3, 6, 9}, new List<int>{3, 2, 1, 4, 7, 8, 9}, new List<int>{7, 4, 1, 2, 3, 6, 9} }},
		{5, new List<List<int>>{ new List<int>{1, 8, 13}, new List<int>{3, 8, 9, 11}, new List<int>{1, 5, 4, 11} } },
		{6, new List<List<int>>{ new List<int>{4, 11, 14}, new List<int>{2, 4, 11, 12}, new List<int>{1, 8, 10, 13, 14} } },
		{7, new List<List<int>>{ new List<int>{1, 2, 3, 5, 9, 11}, new List<int>{4, 8, 9, 12, 15}, new List<int>{7, 8, 4} } },
		{8, new List<List<int>>{ new List<int>{4, 11, 14}, new List<int>{9, 6, 5, 12, 14, 17}, new List<int>{1, 8, 10, 13} } },
		{9, new List<List<int>>{ new List<int>{2, 9, 11}, new List<int>{6, 8, 15}, new List<int>{4, 7, 10, 17} } }
	}; 

	public Sprite[] sprites;

	[SerializeField]
	public float LINEWIDTH = 0.5f;
	public AudioSource chop;

	public Shader shader1;

	public int SWIPETHRESHOLD = 2;

	List<GameObject> go = new List<GameObject>();

	public TextAsset wordFile;
	private List<string> lineList = new List<string>(); 

	Vector3 startPos;	
	Collider2D[] cubeColliders;

	GUIStyle guiStyle = new GUIStyle();

	private static bool increaseLevel = false;
	private static bool reloadLevel = false;

	private static int level = 1;

	string currentPattern;

	List<int> currentPatternList = new List<int> ();

	List<int> currentPaths = new List<int>();

	Dictionary<int, int> pattern = new Dictionary<int, int>();
	Dictionary<int, int> patternRev = new Dictionary<int, int>();

	bool settingGame = false;

	protected void OnGUI(){

		guiStyle.fontSize = 40; 
		GUILayout.Label ("\n Level: " + level, guiStyle);
	}
		
	void CreateNumCubeMap(){

		pattern.Add (1, 10);
		pattern.Add (2, 11);
		pattern.Add (3, 12);
		pattern.Add (4, 13);
		pattern.Add (5, 14);
		pattern.Add (6, 15);
		pattern.Add (7, 16);
		pattern.Add (8, 17);
		pattern.Add (9, 18);

		patternRev.Add (10, 1);
		patternRev.Add (11, 2);
		patternRev.Add (12, 3);
		patternRev.Add (13, 4);
		patternRev.Add (14, 5);
		patternRev.Add (15, 6);
		patternRev.Add (16, 7);
		patternRev.Add (17, 8);
		patternRev.Add (18, 9);
	}

	void PrintList(List<int> anyList){
		Debug.Log ("-------------printing current pattern--------------");
		for (int i = 0; i < anyList.Count; i++) {
			Debug.Log(anyList [i]);
		}
		Debug.Log ("-------------------------------------------------");

	}

	void ColorCubePath(List<GameObject> patternCube){
		Color c = Color.grey;
		GameObject b;
		for (int i = 0; i < patternCube.Count; i++) {
			b = patternCube [i];
			b.layer = 8;
		}
	}

	void DrawLines(List<GameObject> patternCube){
		LineRenderer ln;
		for (int i = 0; i < patternCube.Count; i++) {
			if (i != patternCube.Count - 1) {
				GameObject g1 = patternCube [i];
				GameObject g2 = patternCube [i + 1];
				if (g1.GetComponent<LineRenderer> ()) {
					ln = g1.GetComponent<LineRenderer> ();
				} else {
					ln = g1.AddComponent<LineRenderer> ();
				}
				//g1.GetComponent<Renderer> ().material.shader = shader1;
				//g2.GetComponent<Renderer> ().material.shader = shader1;
				ln.SetPosition (0, g1.transform.position);
				ln.SetPosition (1, g2.transform.position);
				ln.material.color = Color.white;
				ln.startWidth = LINEWIDTH;
				ln.endWidth = LINEWIDTH;
			} 
		}
	}

	IEnumerator RemoveLines(List<int> p){
		yield return new WaitForSeconds(3f);
			
		LineRenderer ln;
		for (int i = 0; i < p.Count; i++) {
			if (i != p.Count - 1) {
				int p1 = p [i];
				int p2 = p [i + 1];
				GameObject g1 = GameObject.Find (String.Format ("{0}", p1));
				ln = g1.GetComponent<LineRenderer> ();
				ln.SetPosition (0, Vector3.zero);
				ln.SetPosition (1, Vector3.zero);
			}
		}
	}

	void ClearVariables(){
		RemoveLines (currentPatternList);
		currentPaths.Clear ();
		currentPatternList.Clear ();
		lineList.Clear ();
		go.Clear ();
	}

	public string GetRandomLine()
	{
		return lineList[UnityEngine.Random.Range(0, lineList.Count)];
	}

	bool CheckEqual(List<int> List1, List<int> List2){
		int list1C = List1.Count;
		int list2C = List2.Count;
		if (list1C == list2C){
			for(int i = 0; i < list1C; i++){
				if(List1[i] != List2[i]){
					return false;
				}
			}
			return true;
		}
		return false;
	}

	void ChangePathsColors(List<int> paths){
		for (int m = 0; m < paths.Count; m++) {
			go[paths[m]].gameObject.GetComponent<Renderer>().material.color = Color.white;
		}
	}

	public void GetPatterns()
	{
		if (wordFile){
			string line;
			StringReader textStream = new StringReader(wordFile.text);
			while((line = textStream.ReadLine()) != null){
				lineList.Add(line);
			}
			textStream.Close();
		}

		currentPattern = GetRandomLine ();
		GetPatternList();
		AddPatternsBothGrids ();
	}

	void GetPatternList(){
		currentPatternList.Clear ();
		for (int i = 0; i < currentPattern.Length; i++) {
			currentPatternList.Add(pattern[(int)(currentPattern[i]-'0')]);
		}
	}

	List<int> GetFirstGrid(){
		int startCubeNumber = patternRev [currentPatternList [0]];
		List<List<int>> firstGridOptions = firstGrid [startCubeNumber];
		int indexFirstGrid = UnityEngine.Random.Range (0, 3);
		List<int> grid = new List<int> ();
		grid.AddRange(firstGridOptions [indexFirstGrid]);
		return grid;
	}

	void AddPatternsBothGrids(){
		List<int> grid = GetFirstGrid ();
		grid.AddRange (currentPatternList);
		currentPatternList = grid;
	}

	void InitialCubesColor(){
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Cube");
		for (int i = 0; i < objects.Length; i++) {
			go.Add (objects [i]);
			GameObject b = go [i];
			b.layer = 8;
			b.GetComponent<Renderer>().material.color = Color.white;
		}
	}

	void GetCubePatterns(){
		List<GameObject> patternCube = new List<GameObject>();
		GameObject g;
		for (int j = 0; j < currentPatternList.Count; j++) {
			int cubeNumber = currentPatternList [j];
			g = GameObject.Find (String.Format ("{0}", cubeNumber));
			patternCube.Add(g);
		}
		ColorCubePath (patternCube);
		DrawLines (patternCube);
		patternCube.Clear ();
		StartCoroutine (RemoveLines(currentPatternList));	
	}

	void InitSetup(){
		settingGame = true;
		GetPatterns();
		InitialCubesColor ();
		GetCubePatterns ();
		settingGame = false;
	}

	void SwipeCube(int currentCube){
		int currentPathSize = currentPaths.Count;
		Debug.Log (currentCube);
		GameObject a = GameObject.Find (currentCube.ToString ());
		if (currentPaths.Contains (currentCube)) {
			Debug.Log ("already exists");
		} else {
			if (currentCube == currentPatternList [currentPathSize]) {
				currentPaths.Add (currentCube);
				a.GetComponent<Renderer> ().material.color = Color.red;	
				chop.Play ();
				Debug.Log ("Current cube added:" + currentCube);
				if (CheckEqual (currentPatternList, currentPaths)) {
					ChangePathsColors (currentPatternList);
					increaseLevel = true;
					ClearVariables ();
					Debug.Log ("DONEEEEE!!!");
				} else {
					if (currentPaths.Contains (currentCube)) {
						Debug.Log ("already swiped");
					} else {
						currentPaths.Clear ();
					}
				}
			} 
		}
	}

	void TouchLogic(){
		int currentCube;

		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch (0);
			Vector3 pos = Camera.main.ScreenToWorldPoint (touch.position);
			pos.z = -1;
			Collider2D[] currentFrame = Physics2D.OverlapPointAll (new Vector2 (pos.x, pos.y), LayerMask.GetMask ("Cube"));
			foreach (Collider2D c2 in currentFrame) {
				Debug.Log (c2.name);
				currentCube = int.Parse (c2.name);
				SwipeCube (currentCube);
			}
		}

		/*
		foreach (Touch touch in Input.touches) {
			switch (touch.phase) {

			case TouchPhase.Moved:
				Debug.Log ("moved");
				Vector3 pos = Camera.main.ScreenToWorldPoint (touch.position);
				pos.z = -1;
				Collider2D[] currentFrame = Physics2D.OverlapPointAll (new Vector2 (pos.x, pos.y), LayerMask.GetMask ("Cube"));
				Vector3 offset = pos - startPos;
				if (offset.sqrMagnitude > SWIPETHRESHOLD) {
					Debug.Log (offset.ToString ());
					foreach (Collider2D c2 in currentFrame) {
						if (cubeColliders.Length != 0) {
							for (int i = 0; i < cubeColliders.Length; i++) {
								if (c2 == cubeColliders [i]) {
									currentCube = int.Parse (c2.name);
									SwipeCube (currentCube);
								}
							}
						}
					}
				}
				cubeColliders = currentFrame; 
				startPos = touch.position;
				break;
			
			case TouchPhase.Ended:
				if (CheckEqual (currentPatternList, currentPaths)) {
					continue;
				} else {
					reloadLevel = true;
				}
				break;
			}
		}*/
	}

	void Awake(){
		LoadSprites ();
		CreateNumCubeMap ();
		shader1 = Shader.Find ("Outlined/Silhouetted Diffuse");
	}

	void LoadSprites(){
		sprites = Resources.LoadAll<Sprite>("sprites/Scavengers_SpriteSheet");
	}

	void Start () {
		InitSetup ();
	}

	IEnumerator NewLevelWork(){
		level += 1;
		currentPaths.Clear ();
		currentPatternList.Clear ();
		lineList.Clear ();
		go.Clear ();
		increaseLevel = false;
		yield return new WaitForSeconds (1f);
		InitSetup ();
	}


	void Update () {
		if (settingGame) {
			return;
		} else if (increaseLevel) {
			StartCoroutine (NewLevelWork());
		}
		TouchLogic ();
	}
}