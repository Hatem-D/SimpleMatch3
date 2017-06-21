using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour {

    public List<Transform> gemsPrefabs;//gems prefabs references
	List<Color> gemsColors = new List<Color>();

    List<GemController> fullList = new List<GemController>();//List of ingame gems

    public delegate void gemClickedFunctionPointer(GemController gem);
    public gemClickedFunctionPointer gemClick;//delegate behaviour for gemclicks
    public delegate void gemDiedFunctionPointer(GameObject gem);
    public gemDiedFunctionPointer gemDeath;//delegate behaviour for dying gems
    private delegate void updateStuffFunction();
    private updateStuffFunction doUpdateStuff;

    public bool gameOver = false;
    GameVariables gameVars; //global game variables handler

    public GameObject confirmExitHolder;

    void Awake()
    {
        if (gemsPrefabs == null || gemsPrefabs.Count < 5) { Debug.Log("Warning not enough prefabs set"); }
        

        if (SceneManager.GetActiveScene().name == "FacebookGameScene" || SceneManager.GetActiveScene().name == "FreeplayGameScene")
        {
            gemClick = GemClicked;
            gemDeath = CreateNewGem;
            gameVars = gameObject.GetComponent<GameVariables>();
        }        
        
        doUpdateStuff = null;
    }

	// Use this for initialization
	void Start () {
		InitColorsTable ();
        if (gameVars != null) InitTable();
        //gameVars.TurnNameInputField (false);
        if (confirmExitHolder != null)
        {
            HideConfirmExit();
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (doUpdateStuff != null)
        {
            doUpdateStuff();
        }
		if (Input.GetKeyDown (KeyCode.Escape)) {
			LoadMenu ();
		}
        
    }

    

    int GetRandomGemType()
    {
        int ret;
        do { ret = Random.Range(0, gemsPrefabs.Count); } while (ret > gemsPrefabs.Count - 1);
        return ret;
    }

    void GemClicked(GemController oldGem)
    {
        //Debug.Log("GemClick");
        oldGem.raycasted = true;//mark as part of the gems to destroy
        gemClick = WaitClick;//send clicks to waitin function while monitoring velocity
		gameVars.GemClicked(oldGem.dynamicGemType, gemsColors[oldGem.dynamicGemType]);//send gem type to score data
        gameVars.SetComboScore();//start comboingScore
        oldGem.GetMeAndMySisters();//destroy gems of the same color        
        gameVars.Moves++;//add a move to the move counter

    }

    public void CreateNewGem(GameObject oldGem)
    {
        int whichGem = GetRandomGemType();        
        CreateCubeAt(oldGem.transform.position.x, gameVars.getYSpawnPosition(oldGem.transform.position.x), whichGem);

        gameVars.AddScore();
		if (gameOver)
			gameVars.UpdateGameOverText ();
        //Destroy(oldGem);        
		oldGem.GetComponent<GemController>().Destroy();
        doUpdateStuff = MonitorGameVelocity;

    }

    void CreateCubeAt(float col, float row, int dynamicGemType)
    {
        Transform temp;
        temp = (Transform)Instantiate(gemsPrefabs[dynamicGemType], new Vector3(col, row, 0), gemsPrefabs[dynamicGemType].rotation);

        temp.gameObject.GetComponent<GemController>().dynamicGemType = dynamicGemType;        
        fullList.Add(temp.gameObject.GetComponent<GemController>());
        
    }

    void MonitorGameVelocity()
    //Monitor last created gem's velocity - allow clicking again when landed, reset spawn array (upwards from board), go back to no bonus scoring
    //Designed For the scoring game 
    {
        if (fullList[fullList.Count-1].gameObject.GetComponent<Rigidbody>().velocity.y > 0.0f)
        {
            gameVars.ResetSpawnArray();
            gameVars.SetRegularScore();
            if (!gameOver) { 
                gemClick = GemClicked;
                doUpdateStuff = null;			
            }
        }        
    }

  

    public void LoadMenu()
    {
        if (!gameOver)
        {
            gemClick = WaitClick;
            confirmExitHolder.SetActive(true);
        }else
        {
            LoadMenuConfirmed();
        }
    }

    public void HideConfirmExit()
    {
        gemClick = GemClicked;
        confirmExitHolder.SetActive(false);
    }

    public void LoadMenuConfirmed()
    {
        SceneManager.LoadScene("FacebookMenu");
    }

	public void LoadHiScores()
	{
		SceneManager.LoadScene ("FacebookHiScores");
	}
	public void RestartGame(){
		SceneManager.LoadScene ("FacebookGameScene");
	}

    void WaitClick(GemController gem) { 
        //Debug.Log("WaitClick"); 
    }

	void InitColorsTable ()//init a table of colors, gets each color from gemPrefabs material
	{
		foreach (Transform gemPrefab in gemsPrefabs) {
			gemsColors.Add (gemPrefab.gameObject.GetComponent<MeshRenderer> ().sharedMaterial.color);
		}	
	}

    void InitTable()//Init with random gems, different from the gem on the left(count-1) and the one below(count-9)
    {
        for (int rowNum = gameVars.BottomArrayLimit; rowNum < gameVars.TopArrayLimit; rowNum++)
        {
            for (int colNum = gameVars.LeftArrayLimit; colNum < gameVars.RightArrayLimit; colNum++)
            {
                int whichGem;
                bool okToAdd;
                do
                {
                    okToAdd = true;
                    whichGem = GetRandomGemType();

                    if (fullList.Count > 0)
                    {
                        if (fullList[fullList.Count - 1].dynamicGemType == whichGem)
                        {
                            okToAdd = false;
                        }
                    }


                    if (fullList.Count > 8)
                    {
                        if (fullList[fullList.Count - 9].dynamicGemType == whichGem)
                        {
                            okToAdd = false;
                        }
                    }

                } while (okToAdd == false);

                CreateCubeAt(colNum, rowNum, whichGem);
            }
        }
    }
    

    public void GameOver()
    {
        gameOver = true;
    }
}
