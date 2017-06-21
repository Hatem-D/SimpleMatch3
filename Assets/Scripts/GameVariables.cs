 using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameVariables : MonoBehaviour {

    private float[] ySpawnPosition = new float[9];

    public float YSpawnPosition = 9.0f;
    short regularScoreIncreasePerGem = 5;
    short comboScoreIncreasePerGem = 2;
    ushort moves;

    public Text scoreShow;
	public Text movesLeftShow;
    public Text bestComboShow;
	public Text chainShow;
	public Image lastColor;    

	public Text comboShow;
	public float comboFadeDuration = 5.0f;
	public Text gameOverShow;

    public Text localHiScoreText;
    public Text FBProfilePic;

    uint localHiScore = 0;
    ushort localHiScoreBestCombo = 0;
    uint localHiScoreBestChain = 0;

    [HideInInspector]
	public Text maxMovesShow;
	
	public Text movesShow;
	[HideInInspector]
	public InputField inputNewName;

    int bottomArrayLimit = -3;
    int topArrayLimit = 6;
    int leftArrayLimit = -4;
    int rightArrayLimit = 5;

    GameManager gm;
    VariablesStore store;

    bool localSave = true;    

    const uint maxScore = 1000000;

    private uint score = 0;
    private ushort comboCounter = 0;
	private uint chain = 0;
	private uint colorChain = 0;
    private ushort bestCombo = 0;
	private uint bestChain = 0;
    
    /// <summary>
    /// MAXMOOOOOOOOOOVES
    /// </summary>
    private ushort maxMoves = 40;
    public bool freePlay = false;

    int lastGemType = 0;

    public delegate void ScoreAddFunctionPointer();
    public ScoreAddFunctionPointer AddScore;

	private uint Chain
	{
		get { return chain;}
		set {
			chain = value;

			if (bestChain < chain)
				bestChain = chain;

            UpdateGUIChain();
           
            if (gm.gameOver)
				UpdateGameOverText ();
			
			
		}
	}

	private uint ColorChain
	{
		get { return colorChain;}
		set {
			colorChain = value;
		}
	}

	public uint Score
    {
        get
        {
            return score;
        }

        private set
        {
            if (value < maxScore) { 
                score = value;				
            }
        }
    }

    public ushort Moves
    {
        get
        {
            return moves;
        }
        set
        {
            if (!freePlay)
            {
                if (value < maxMoves)
                {
                    moves = value;
                    UpdateGUIElements();
                }
                else
                {
                    moves = maxMoves;
                    GameOver();
                }
            }else {
                moves = value;
                UpdateGUIElements();
                UpdateGUIMoves();
            }
			UpdateChainCounter ();
        }

    }

    #region Accessors
    public int TopArrayLimit { get { return topArrayLimit; } }
    public int LeftArrayLimit { get { return leftArrayLimit; } }
    public int RightArrayLimit { get { return rightArrayLimit; } }
    public int BottomArrayLimit{ get { return bottomArrayLimit; } }
    #endregion

    public void ResetSpawnArray() {
        for (int i = LeftArrayLimit; i < RightArrayLimit; i++)
        {
            if (LeftArrayLimit < 0)
            {
                ySpawnPosition[i - LeftArrayLimit] = YSpawnPosition;
            }
            else {
                ySpawnPosition[i] = YSpawnPosition;
            }
        }        
    }

    public float getYSpawnPosition(float destroyedGemX)
    {
        int column = (int)destroyedGemX;
        if (LeftArrayLimit < 0){
            column  -= LeftArrayLimit;
        }
        ySpawnPosition[column]++;
        return (ySpawnPosition[column] - 1);
    }

    void Start()
    {
		lastGemType = -1;

        UpdateGUIScore();

        SetRegularScore();

        gm = gameObject.GetComponent<GameManager>();

        UpdateGUIMaxMoves();

        gameOverShow.text = "";

        ResetSpawnArray();     

		if (FacebookManager.Instance != null) {
			if (FacebookManager.Instance.IsLoggedIn)
			{
				GameObject.FindObjectOfType<FBScript>().QueryMyFBScore ();
                localSave = false;
			}
		}
        if (localSave && !freePlay) LoadVariables();
    }

    void LoadVariables()
    {
        store = gameObject.GetComponent<VariablesStore>();
        Debug.Log(store.file);
        string[] hiScore = store.file.Split(';');

        if (!uint.TryParse(hiScore[0], out localHiScore)) Debug.Log("could not parse hi score");
        if (!ushort.TryParse(hiScore[1], out localHiScoreBestCombo )) Debug.Log("could not parse best combo");
        if (!uint.TryParse(hiScore[2], out localHiScoreBestChain)) Debug.Log("could not parse best chain");

        if (FBProfilePic != null)
        {
            FBProfilePic.text = "Local Hi Score \r\n" + localHiScore.ToString() + "\r\nBest Combo : " + localHiScoreBestCombo.ToString()
                            + "\r\nBest Chain : " + localHiScoreBestChain.ToString();
        }
        if (localHiScoreText != null)
        {
            localHiScoreText.text = localHiScore.ToString(); ;
        }
    }

    public void SetRegularScore() {
        AddScore = IncreaseScoreRegular;
		comboCounter = 0;        
    }

	public void GemClicked(int gemType, Color gemColor){
		if (lastGemType == -1) {
			lastGemType = gemType;
			lastColor.color = gemColor;
			return;
		}
		if (lastGemType == gemType && Chain > 0) {
			ColorChain++;
		} else {
			lastGemType = gemType;
			lastColor.color = gemColor;
			ColorChain = 0;
		}
	}

	public void UpdateChainCounter()
	{
		if (comboCounter > 1) {
			if (ColorChain > 0)
				Chain += ColorChain + 1;
			else
				Chain++;
		} else {
			Chain = 0;
			ColorChain = 0;
		}


	}

    public void SetComboScore() { AddScore = IncreaseScoreCombo; }

    void IncreaseScoreRegular() { 
		Score += (uint)regularScoreIncreasePerGem;
	}

    void IncreaseScoreCombo() {
		Score += (uint)regularScoreIncreasePerGem + (uint)comboScoreIncreasePerGem * (uint)comboCounter *(1 + (uint)Chain);
        comboCounter++;
        if (bestCombo < comboCounter) bestCombo = comboCounter;

    }

	public void UpdateGUIElements(){
		UpdateGUICombo();
		UpdateGUIChain();
		UpdateGUIScore();
		UpdateGUIBestCombo ();
		UpdateGUIMovesLeft ();
	}

    void UpdateGUIScore()
    {
        if (scoreShow != null)
        {
            scoreShow.text = Score.ToString();
        }
    }

	void UpdateGUIChain() {
        chainShow.text = "Chain\r\n";
		if (chain > 0) chainShow.text += Chain.ToString();
	}
	void UpdateGUIBestCombo(){
		bestComboShow.text = "Best Combo\r\n";
		bestComboShow.text += bestCombo;
	}

	IEnumerator currentCoroutine;

    void UpdateGUICombo() { 
		string comboMessage = "x"+ comboCounter;

		if (comboCounter > 1)
			comboMessage += " Combo !";
		
		if (comboCounter > 1 && comboCounter < 5)
				comboMessage += "\r\nNice !";
		else if (comboCounter > 4 && comboCounter < 7)
			comboMessage += "\r\nGreat !";
		else if (comboCounter > 6 && comboCounter < 11)
			comboMessage += "\r\nFantastic !";
		else if (comboCounter > 10  && comboCounter < 21 )
			comboMessage += "\r\nUnbelievable !";
		else if  (comboCounter > 20  && comboCounter < 30 )
			comboMessage += "\r\nMASTER !";
		else if (comboCounter > 29)
			comboMessage += "\r\nGODLIKE !";

		if (ColorChain > 0 && Chain > 0)
			comboMessage += "\r\n + " + colorChain + " Chain color bonus !";
		
		comboShow.text = comboMessage;

		if (currentCoroutine != null)
			StopCoroutine (currentCoroutine);

		currentCoroutine = FadeTextToZeroAlpha (comboFadeDuration, comboShow);
	
		StartCoroutine(currentCoroutine);


	}


	public IEnumerator FadeTextToFullAlpha(float t, Text i)
	{
		i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
		while (i.color.a < 1.0f)
		{
			i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
			yield return null;
		}
	}

	public IEnumerator FadeTextToZeroAlpha(float t, Text i)
	{
		i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
		while (i.color.a > 0.0f)
		{
			i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
			yield return null;
		}
	}



	void UpdateGUIMoves() { 
		if (movesShow != null) movesShow.text = Moves.ToString(); 
		if (!freePlay) UpdateGUIMovesLeft ();
	}

	ushort GetMaxMoves(){ 
		return maxMoves;
	}

	void UpdateGUIMovesLeft(){ movesLeftShow.text = (GetMaxMoves () - Moves).ToString (); }

    void UpdateGUIMaxMoves() {
        if (maxMovesShow != null) maxMovesShow.text = GetMaxMoves().ToString();
		UpdateGUIMovesLeft ();
    }

	public void UpdateGUIInputFieldText(string inputText){
		
		inputText.Trim ();
		inputText = StringTools.RemoveSpecialCharacters (inputText);

		if (inputText.Length > 11) {							
			inputText = inputText.Remove (11);
		}
		inputNewName.text = inputText;
	}

	public void SaveNewScore(string newName){				
		gm.LoadHiScores ();
	}

    void GameOver()
    {
        gm.GameOver();          
        UpdateGameOverText ();
    }

	public void UpdateGameOverText (){
		
		gameOverShow.text = "Game Over";
		if (FacebookManager.Instance != null) {
			if (FacebookManager.Instance.IsLoggedIn) {
				if (GameObject.FindObjectOfType<FBScript> ().GameOver ((int)Score)) {
					gameOverShow.text += "\r\nWell played you set a new high score !";
				}
			} 
		}
        if (localSave)
        {
            if (Score > localHiScore)
            {
                gameOverShow.text += "\r\nWell played you set a new high score !";
                store.ResetFile();
                store.Save(Score.ToString() + ";" + bestCombo.ToString() + ";" + bestChain.ToString());
                if (FBProfilePic != null)
                {
                    FBProfilePic.text = "Local Hi Score \r\n" + Score.ToString() + "\r\nBest Combo : " + bestCombo.ToString()
                                    + "\r\nBest Chain : " + bestChain.ToString();
                }
                if (localHiScoreText != null)
                {
                    localHiScoreText.text = Score.ToString(); ;
                }
            }
        }
        UpdateGUIElements();
        chainShow.text = "Best Chain\r\n" + bestChain;        
    }


	public void TurnNameInputField(bool state){
		inputNewName.gameObject.SetActive (state);
	}
}

