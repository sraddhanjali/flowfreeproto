﻿using System; // string
using System.IO; // file
using UnityEngine; // vector3
using System.Collections;
using System.Collections.Generic; // list, dictionary

class PatternGrid{
	public Dictionary<int, int> pattern = new Dictionary<int, int>(); // map of combined grid indices -> android grid indices
	public Dictionary<int, int> patternRev = new Dictionary<int, int>(); // map of android grid indices -> combined grid indices
	//Helper h = new Helper();
	List<int> currentSelPattern = new List<int>();

	public PatternGrid(){
		CreateNumCubeMap ();
	}

	/* first static grid */
	public Dictionary<int, List<List<int>>> firstGrid = new Dictionary<int, List<List<int>>>() {
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

	/* second grid */
	public void CreateNumCubeMap(){ // mapping of second grid in the scheme of 9X3 grid
		
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

	List<int> GetFirstGrid(int start){ // get first 3X3 grid
		int startCubeNumber = patternRev [start];
		List<List<int>> firstGridOptions = firstGrid [startCubeNumber];
		int indexFirstGrid = UnityEngine.Random.Range (0, 3);
		List<int> grid = new List<int> ();
		grid.AddRange(firstGridOptions [indexFirstGrid]);
		return grid;
	}

	List<string> ReadPatternFileIntoList(){ // read the pattern file and add each pattern strings into a list
		List<string> patternStringLists = new List<string>();
		TextAsset wordFile = Resources.Load("easy") as TextAsset; 
		if (wordFile){
			string line;
			StringReader textStream = new StringReader(wordFile.text);
			while((line = textStream.ReadLine()) != null){
				patternStringLists.Add(line);
			}
			textStream.Close();
		}
		return patternStringLists;
	}

	string GetRandomPatternLine(List<string> patternList){ // get random line from the pattern string list
		return patternList[UnityEngine.Random.Range(0, patternList.Count)];
	}

	List<int> ChangePatternStringToList(string selectedPattern){ // turns numeric string to list of ints
		List<int> selectedPatternList = new List<int>();
		for (int i = 0; i < selectedPattern.Length; i++) {
			int s = (int)(selectedPattern[i] - '0');
			selectedPatternList.Add(pattern[s]);
		}
		return selectedPatternList;
	}

	List<int> CombineGrids(List<int> grid, List<int> selectedPatternList){ // combination of first and second grids
		grid.AddRange (selectedPatternList);
		selectedPatternList = new List<int> ();
		selectedPatternList = grid;
		return selectedPatternList;
	}

	List<GameObject> GetPatternGameobjects(List<int> selectedPatternList){ // get gameobject for selected pattern
		List<GameObject> patternCube = new List<GameObject>();
		int cubeNumber;
		GameObject g;
		for (int j = 0; j < selectedPatternList.Count; j++) {
			cubeNumber = selectedPatternList [j];
			g = GameObject.Find (String.Format ("{0}", cubeNumber));
			patternCube.Add(g);
		}
		return patternCube;
	}

	void SetCurrentSelPattern(List<int> curr){
		currentSelPattern = curr;
	}

	public List<int> GetCurrentSelPattern(){
		return currentSelPattern;
	}

	public List<GameObject> GetPatterns(){
		
		/* get second grid */
		List<string> patternStringLists = ReadPatternFileIntoList ();
		string selectedPattern = GetRandomPatternLine (patternStringLists);
		List<int> selectedPatternList = ChangePatternStringToList(selectedPattern);

		/* get first grid */
		List<int> grid = GetFirstGrid (selectedPatternList[0]);

		/* combine grids */
		List<int> combinedGrid = CombineGrids (grid, selectedPatternList);

		/* set total grid to current selected pattern */
		SetCurrentSelPattern (combinedGrid);

		/* get list of pattern gameobjects */
		return GetPatternGameobjects (combinedGrid);
	}

	public List<List<int>> SamplePatterns(){
		/*
		List<List<int>> combinedGridList1 = new List<List<int>> () {
			new List<int>() {4, 8, 6, 7, 10, 13, 14, 11},
			new List<int>() {4, 8, 6, 7, 11, 13, 14, 12},
			new List<int>() {4, 8, 6, 7, 14, 15, 17, 18},
			new List<int>() {4, 8, 6, 7, 11, 13, 17, 15, 14},
			new List<int>() {4, 8, 6, 7, 10, 14, 15, 11, 13},
			new List<int>() {4, 8, 6, 7, 10, 14, 12, 17, 13},
			new List<int>() {4, 8, 6, 7, 10, 15, 18, 14, 12},
			new List<int>() {4, 8, 6, 7, 10, 13, 18, 17, 12},
			new List<int>() {4, 3, 5, 8, 11, 14, 10, 14, 18},
			new List<int>() {4, 3, 5, 8, 13, 14, 12, 11, 14, 17},
			new List<int>() {1, 2, 5, 8, 9, 10, 17, 14, 11, 12},
			new List<int>() {5, 6, 8, 7, 10, 11, 14, 16, 17}
		};
		*/

		List<List<int>> combinedGridList2 = new List<List<int>> () {
			new List<int>() {4, 8, 6, 7, 10, 13, 14, 11}, // label a
			new List<int>() {2, 5, 8, 12, 14, 16, 17, 18}, // b
			new List<int>() {4, 3, 5, 8, 10, 11, 14, 17, 18}, // c
			new List<int>() {4, 3, 5, 8, 11, 13, 14, 12}, // d
			new List<int>() {5, 6, 8, 9, 14, 15, 17, 18}, // e
		};
		return combinedGridList2;
	}

	public List<GameObject> GetSimplePatterns(){
		
		List<List<int>> combinedGridList = SamplePatterns ();
		/* get random pattern from list of patterns
		List<int> combinedGrid = combinedGridList[UnityEngine.Random.Range(0, combinedGridList.Count)];
		*/

		List<int> combinedGrid = combinedGridList [Main.patternIndex];
		/* set total grid to current selected pattern */
		SetCurrentSelPattern (combinedGrid);

		/* get list of pattern gameobjects */
		return GetPatternGameobjects (combinedGrid);
	}

	public List<GameObject> GetZPatterns(){
		List<int> zPatternList = new List<int>() {2, 5, 8, 10, 11, 12, 14, 16, 17, 18};
		SetCurrentSelPattern (zPatternList);
		return GetPatternGameobjects(zPatternList);
	}

	public void InitialCubesColor(){
		GameObject[] objects = GameObject.FindGameObjectsWithTag("Cube");
		for (int i = 0; i < objects.Length; i++)
		{
			GameObject b = objects[i];
			b.layer = 8;
			b.GetComponent<Renderer>().material.color = Color.white;
		}
	}
}