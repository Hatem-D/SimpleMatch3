using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HiScoresTable {

	string tableName;
	ushort maxMoves;
	int rows;
	ushort gemTypes;

	public string TableName{ get{ return tableName; } }
	public ushort MaxMoves{get{ return maxMoves;}}


	List<ScoreEntry> scoresTable = new List<ScoreEntry>();
    
	static string[] defaultHardNames = new string[10] {"Farhat H","Tahar S","Ali BH" ,"Mahmoud EM","Abdelaziz T", "Ahmed D", "Farhat H","Tahar H", "Habib B", "Salah BY" };
	//static string[] defaultHardNames = new string[10] {"ph1","ph1","ph1","ph1","ph1", "Roland", "ph1","ph1", "ph1", "ph1" };
	static uint[] defaultHardScores = new uint[10]{463, 454, 416, 402, 392, 389, 388, 383, 379, 352};
	static ushort[] defaultHardBestCombos = new ushort[10]{15, 13, 15, 13, 16, 8, 16, 14, 16, 14};

	static string[] defaultNormalNames = new string[10] {"Bechir S","Tahar S","Ali BH" ,"Mahmoud EM","Abdelaziz T", "Ahmed D", "Farhat H","Tahar H", "Habib B", "Salah BY" };
	//static string[] defaultNormalNames = new string[10] {"ph1","ph1","ph1" ,"ph1","ph1", "ph1", "ph1","ph1", "ph1", "ph1" };
	static uint[] defaultNormalScores = new uint[10]{1482, 1469, 1413, 1365, 1201, 1155, 1121, 1115, 1106, 1084};
	static ushort[] defaultNormalBestCombos = new ushort[10]{32, 30, 29, 26, 29, 27, 23, 22, 27, 26};

	static string[] defaultEasyNames = new string[10] {"Bechir S","Tahar S","Ali BH" ,"Mahmoud EM","Abdelaziz T", "Ahmed D", "Farhat H","Tahar H", "Habib B", "Salah BY" };
	//static string[] defaultEasyNames = new string[10] {"ph1","ph1","ph1" ,"Caro","ph1", "ph1", "ph1","ph1", "ph1", "ph1" };
	static uint[] defaultEasyScores = new uint[10]{2803, 2677, 2650, 2638, 2504, 2465, 2419, 2402, 2362, 2350};
	static ushort[] defaultEasyBestCombos = new ushort[10]{43, 41, 39, 39, 37, 37, 37, 37, 35, 37};

    public struct ScoreEntry {
        public string name;
        public uint score;
        public ushort bestCombo;
        
        public ScoreEntry(string nName, uint nScore, ushort nBestCombo)
        {
            name = nName;
            score = nScore;
            bestCombo = nBestCombo;
        }

        bool Equals(ScoreEntry other)
        {
            if (this.name != other.name) return false;
            if (this.score != other.score) return false;
            if (this.bestCombo != other.bestCombo) return false;
            return true;
        }

        public override string ToString() 
        {
            return ("name : " + name + " score " + score + " bestCombo " + bestCombo);
        }

		public string ToFileString()
		{
			return (name+";"+score+";"+bestCombo+";");
		}
    }
  
    public HiScoresTable(string name, ushort nMaxMoves, int nRows, ushort nGemTypes)
    {
		tableName = name.Trim();
        maxMoves = nMaxMoves;
        rows = nRows;
        gemTypes = nGemTypes;        
        InitDefaultScores();
    }

	public HiScoresTable(string fileString)
	{
		string[] file = fileString.Split(';');
		tableName = file [0].Trim();
		if (!ushort.TryParse (file [1], out maxMoves)) {
			Debug.Log ("tried to parse " + file [1] + " in maxMoves");
		}
		if (!int.TryParse(file [2], out rows)) {
			Debug.Log ("tried to parse " + file [2] + " in rows");
		}
		if (!ushort.TryParse (file [3], out gemTypes)) {
			Debug.Log ("tried to parse " + file [2] + " in gemTypes");
		}


		for (int i = 4; i < file.Length - 1; i = i+3) {
			bool entryOk = true;

			uint newScore;
			if (!(entryOk = uint.TryParse (file [i + 1], out newScore))) {
				Debug.Log ("tried to parse " + file [i + 1] + " in newScore");
			}

			ushort newBestCombo;
			if (!(entryOk = ushort.TryParse (file [i + 2], out newBestCombo))) {
				Debug.Log ("tried to parse " + file [i + 2] + " in newBestCombo");
			}

			if (entryOk) {
				ScoreEntry entry = new ScoreEntry (file [i].Trim(), newScore, newBestCombo);
				scoresTable.Add (entry);
			}
		}

		scoresTable.Sort((a, b) => a.score.CompareTo(b.score));
		scoresTable.Reverse ();
	}

	public void AddEntryToTable(string newName, uint newScore, ushort newBestCombo){
		AddEntryToTable (new ScoreEntry (newName, newScore, newBestCombo));
	}

	void AddEntryToTable(ScoreEntry entry)
    {
		scoresTable.Add (entry);
		scoresTable.Sort ((a, b) => a.score.CompareTo(b.score));
		scoresTable.Reverse ();
		scoresTable.RemoveAt (scoresTable.Count - 1);
    }

    void InitDefaultScores()
    {
		string[] defaultNames;
		uint[] defaultScores;
		ushort[] defaultBestCombos;

		defaultNames = defaultHardNames;
		defaultScores = defaultHardScores;
		defaultBestCombos = defaultHardBestCombos;
		
		if (tableName == "normal") {
			defaultNames = defaultNormalNames;
			defaultScores = defaultNormalScores;
			defaultBestCombos = defaultNormalBestCombos;
		}
		else if (tableName == "easy") {
			defaultNames = defaultEasyNames;
			defaultScores = defaultEasyScores;
			defaultBestCombos = defaultEasyBestCombos;
		}

        for (int i = 0; i < 10; i++)
        {
			ScoreEntry entry = new ScoreEntry(defaultNames[i], defaultScores[i], defaultBestCombos[i]);
            scoresTable.Add(entry);
        }
        scoresTable.Sort((a, b) => a.score.CompareTo(b.score));
		scoresTable.Reverse ();
    }


    public string ToFileString()
    {
		string result=tableName+";"+maxMoves+";"+rows+";"+gemTypes+";";
        foreach (ScoreEntry se in scoresTable)
        {
            result += se.ToFileString();
        }        
        return result;
    }

	public bool IsInTable(uint newScore)
	{
		if (newScore > scoresTable [scoresTable.Count-1].score)
			return true;
		else
			return false;
	}

	public string GetNamesString(){
		string result = "";
		foreach (ScoreEntry score in scoresTable) {
			result += score.name + "\r\n\r\n";
		}
		return result;
	}

	public string GetScoresString (){
		string result = "";
		foreach (ScoreEntry score in scoresTable) {
			result += score.score + "\r\n\r\n";
		}
		return result;
	}

	public string GetBestCombosString (){
		string result = "";
		foreach (ScoreEntry score in scoresTable) {
			result += score.bestCombo + "\r\n\r\n";
		}
		return result;
	}
}

