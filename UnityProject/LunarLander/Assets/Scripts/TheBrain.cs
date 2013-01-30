using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TheBrain
{
	
	private static TheBrain instance;
	private int saveGame = 0;
	private string currentLevel = null;
	
	//private int currentLevelScore = 0;
	
	private Dictionary<string, int[]> levelScoreMap = new Dictionary<string, int[]> ();
	private static readonly int SCORES_PER_LEVEL = 5;
	private int allTimeScore = 0;
	private int selectedLander;
	
	/*public static TheBrain GetInstance(){
		if(instance == null){
			throw new Exception("You need to load a save game first.");
		}
		return instance;
	}*/
	
	public static TheBrain GetInstance ()
	{
		if (instance == null) {
			instance = new TheBrain ();
		}
		return instance;
	}
	
	private TheBrain ()
	{
		// init stuff
	}
	
	private void InitClearedScoreList ()
	{
		Debug.Log ("Init Cleared List ...");
		foreach (string levelName in this.levelScoreMap.Keys) {
			int[] clearScoreList = new int[SCORES_PER_LEVEL];
			this.levelScoreMap.Add (levelName, clearScoreList);
		}
	}
	
	private bool loadedSaveGame = false;

	public void LoadSaveGame (int saveGame)
	{
		if (!loadedSaveGame && saveGame != this.saveGame) {
			string fileName = Directory.GetCurrentDirectory () + "/saveGames/save" + saveGame + ".csv";
			if (File.Exists (fileName)) {
				Debug.Log ("File " + fileName + " exists. Loading ...");
				FileStream fs = File.Open (fileName, FileMode.Open);
				StreamReader reader = new StreamReader (fs);
			
				// skip first line (csv-Header)
				reader.ReadLine ();
				while (!reader.EndOfStream) {
					string line = reader.ReadLine ();
					int firstSemicolonIndex = line.IndexOf (";");
					if (firstSemicolonIndex > 0) {
						string levelName = line.Substring (0, firstSemicolonIndex);
						string levelScore = line.Substring (firstSemicolonIndex + 1, line.Length - (firstSemicolonIndex + 1));
						string[] scores = levelScore.Split ('_');
						List<int> scoreList = new List<int> ();
						foreach (string score in scores) {
							scoreList.Add (int.Parse (score));
						}
						levelScoreMap.Add (levelName, scoreList.ToArray ());
					}
				}
				Debug.Log ("Finished loading SaveGame " + saveGame + " (" + fileName + ").");
				this.saveGame = saveGame;
				this.loadedSaveGame = true;
				foreach (string levelName in levelScoreMap.Keys) {
					int[] levelScores = levelScoreMap [levelName];
					string levelScoreStr = "";
					foreach (int score in levelScores) {
						levelScoreStr += score.ToString () + ", ";
					}
					Debug.Log (levelName + ": " + levelScoreStr);	
				}
			} else {
				//Debug.LogWarning("SaveGame " + saveGame + " in " + fileName + " does not exist.");
				//Debug.LogWarning(Directory.GetCurrentDirectory());
				InitClearedScoreList ();
				SaveGameState ();
				//LoadSaveGame();
			}
		}
	}
	
	public bool LoadedSaveGame ()
	{
		return this.loadedSaveGame;
	}
	
	private void SaveGameState ()
	{
		string fileName = Directory.GetCurrentDirectory () + "../saveGames/save" + saveGame + ".csv";
		Debug.Log ("Saving Game State " + fileName + " ...");
		FileStream fs = null;
		if (File.Exists (fileName)) {
			Debug.Log ("File " + fileName + " already exists.");
			fs = File.Open (fileName, FileMode.Open);
		} else {
			string dir = Path.GetDirectoryName (fileName);
			if (!Directory.Exists (dir)) {
				Directory.CreateDirectory (dir);
			}
			fs = File.Create (fileName);
		}
		
		StreamWriter writer = new StreamWriter (fs);
		writer.WriteLine ("MapName;MapScores");
		foreach (string levelName in levelScoreMap.Keys) {
			int[] levelScoreList = levelScoreMap [levelName];
			string levelScores = "";
			foreach (int score in levelScoreList) {
				levelScores += score.ToString () + "_";
			}
			if (levelScores.Length > 0) {
				levelScores = levelScores.Substring (0, levelScores.Length - 1);
			}
			writer.WriteLine (levelName + ";" + levelScores);
		}
		writer.Close ();
	}
	
	/*public int GetCurrentLevelScore() {
		return currentLevelScore;
	}*/
	
	public void StartNewLevel (string levelName)
	{
		Debug.Log ("Starting New Level " + levelName);
		foreach(string levelNameInMap in levelScoreMap.Keys){
			if(levelNameInMap == levelName || levelNameInMap.Equals(levelName)) {
				this.currentLevel = levelNameInMap;
				return;
			}
		}
		throw new Exception("The Loaded level is not in the map ... cannot proceed!");
	}
	
	public void ApplyCurrentScore (int currentLevelScore)
	{
		int[] levelScores = this.levelScoreMap [currentLevel];

		//string[] levelScoresSplit = levelScores.Split('_');
		List<int> tempScoreList = new List<int>();
		for (int i=0; i<levelScores.Length; i++) {
			int score = levelScores [i];
			//string score = levelScoresSplit[i];
			if (currentLevelScore > score && currentLevelScore > 0) {
				tempScoreList.Add(currentLevelScore);
				currentLevelScore = 0;
			}
			tempScoreList.Add(score);
		}
		levelScoreMap[currentLevel] = tempScoreList.GetRange(0, 5).ToArray();
		SaveGameState ();
	}
	
	public int[] GetScoreListForLevel (string levelName)
	{
		//Debug.Log("Getting scoreList " + levelName);
		return this.levelScoreMap [levelName];
	}
	
	public string GetCurrentLevelName ()
	{
		return this.currentLevel;
	}
	
	public List<string> GetAvailableLevels ()
	{
		return new List<string> (levelScoreMap.Keys);
	}
	
	/*public string GetNextLevelName() {
		// TODO hier dann evtl. noch eine erweiterte Lösung implementieren
		foreach(string levelName in levelScoreMap.Keys){
			if(levelName.Equals(currentLevel)){
				if(i < levelNames.Length - 1){
					return levelNames[i+1];
				} else {
					return levelNames[0];
				}
			}
		}
		throw new System.Exception("The loaded level "+ currentLevel + " was not specified in LEVEL_NAMES " 
			+ levelNames.ToString());
	}*/
	
}
