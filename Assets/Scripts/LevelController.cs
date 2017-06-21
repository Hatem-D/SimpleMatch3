using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LevelController : MonoBehaviour {

    List<GemController> gemList;
    int gemListLastCount;
	List<GameObject> levelUIStars;//UI Stars container
    List<GameObject> levelUIMoves;//UI Moves container

    public delegate void gemClickedFunctionPointer(GemController gem);
    public gemClickedFunctionPointer gemClick;//delegate behaviour for gemclicks
    public delegate void gemDiedFunctionPointer(GameObject gem);
    public gemDiedFunctionPointer gemDeath;//delegate behaviour for dying gems

    delegate void UpdateFunctionPtr();
    UpdateFunctionPtr doUpdateStuff;

    
    public GameObject levelTextContainer;
    TextMesh levelText;
    public GameObject goalsTextContainer;
    TextMesh goalsText;
	public GameObject starsUIPrefab;
    public GameObject movesTextContainer;
    TextMesh movesText;
	public GameObject movesUIPrefab;

    public LevelBlocksController myBlock;
    [HideInInspector]
    public int myRank;

    public int levelPar;
    public int startingStars = 3;
    [HideInInspector]
    public int objectiveStars = 0;//player's previous best stars score will be used to send a save command at gameover
    public int movesAboveParPerStar = 1;

    int moves;
    int lastCombo = 0;
    int bestCombo = 0;
    GameObject starsHolder;
    Vector3 starsFinalPosition;

    public int Moves
    {
        get
        {
            return moves;
        }

        set
        {
            moves = value;
            UpdateStars();
            UpdateMovesText();
            
        }
    }

    int stars;
    public int Stars
    {
        get
        {
            return stars;
        }

        set
        {
            if (value < 0) stars = 0;
            else stars = value;
            UpdategoalsText();
        }
    }

    

    void Awake()
    {
        gemList = new List<GemController>();
        
    }

    void Start()
    {
        gemClick = WaitClick;
        gemDeath = GemDied;
        doUpdateStuff += WaitForStart;
                
        levelText = levelTextContainer.GetComponent<TextMesh>();
        goalsText = goalsTextContainer.GetComponent<TextMesh>();
        movesText = movesTextContainer.GetComponent<TextMesh>();
        InitStars();
        InitMoves();
        Stars = startingStars;
        Moves = 0;
        UpdateLevelText("");
        
        StartCoroutine("CheckHighestCube");
    }

	void InitStars(){
		if (starsUIPrefab != null) {

            

            starsHolder = new GameObject("StarsHolder");
            starsHolder.transform.SetParent(gameObject.transform);
            starsHolder.transform.position = goalsText.transform.position;

			levelUIStars = new List<GameObject> ();
			for (int i = 0; i < startingStars; i++) {
                levelUIStars.Add(GameObject.Instantiate(starsUIPrefab));
                levelUIStars[i].transform.SetParent(gameObject.transform);
                levelUIStars[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                levelUIStars[i].transform.position = new Vector3(goalsText.transform.position.x, 
                    goalsText.transform.position.y - levelUIStars[i].GetComponent<MeshRenderer>().bounds.size.y, 
                    goalsText.transform.position.z);

                levelUIStars[i].transform.position = new Vector3 (levelUIStars[i].transform.position.x + (i * levelUIStars[i].GetComponent<MeshRenderer>().bounds.size.x), 
                levelUIStars[i].transform.position.y, levelUIStars[i].transform.position.z);

                levelUIStars[i].transform.SetParent(starsHolder.transform);
            }

            starsFinalPosition = new Vector3(
                levelTextContainer.transform.position.x - levelUIStars[0].GetComponent<MeshRenderer>().bounds.size.x - 1.4f,
                levelTextContainer.transform.position.y - levelUIStars[0].GetComponent<MeshRenderer>().bounds.size.y,
                levelTextContainer.transform.position.z);

            Debug.Log(starsFinalPosition);
        }
	}

    void CenterStars()
    {
        Vector3 targetScale = starsHolder.transform.localScale * 1.3f;
        starsHolder.transform.position = Vector3.Lerp(
            starsHolder.transform.position,
            starsFinalPosition, 
            Time.deltaTime);
        starsHolder.transform.localScale = Vector3.Lerp(
            starsHolder.transform.localScale,
            targetScale,
            Time.deltaTime);

        if (Vector3.Distance(starsHolder.transform.position, starsFinalPosition) < 0.1f)
        {
            doUpdateStuff -= CenterStars;
        }
    }

    void InitMoves()
    {
        if (movesUIPrefab != null)
        {
            levelUIMoves = new List<GameObject>();
            for (int i = 0; i < levelPar; i++)
            {
                levelUIMoves.Add(GameObject.Instantiate(movesUIPrefab));
                levelUIMoves[i].transform.SetParent(gameObject.transform);
                levelUIMoves[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                levelUIMoves[i].transform.position = movesText.transform.position;
                levelUIMoves[i].transform.position = new Vector3(levelUIMoves[i].transform.position.x + (i * levelUIMoves[i].GetComponent<MeshRenderer>().bounds.size.x),
                levelUIMoves[i].transform.position.y, levelUIMoves[i].transform.position.z);
            }
        }
    }

    void Update()
    {
        if (doUpdateStuff != null)
        {
            doUpdateStuff();
        }
    }

    public void WaitForStart()
    {
        if (gemList.Count > 1)
        {
            doUpdateStuff += MonitorLevelsVelocity;
            doUpdateStuff -= WaitForStart;
        }
    }

    public void GemClicked(GemController oldGem) {
        //Debug.Log("Clicked : " + oldGem.name);

        oldGem.raycasted = true;//mark as part of the gems to destroy
        gemClick = WaitClick;//send clicks to waiting function while monitoring velocity        
        lastCombo = 1;
        oldGem.GetMeAndMySisters();//destroy gems of the same color        
        doUpdateStuff += MonitorLevelsVelocity;        
        Moves++;
    }

    public void AddGemToLevelList(GemController gem)
    {
        gemList.Add(gem);
        gemListLastCount = gemList.Count;
        
        //Debug.Log(gemList[0].transform.position.y + " " + gemList[gemList.Count - 1].transform.position.y);
    }

    IEnumerator CheckHighestCube()
    {
        if (gemListLastCount > gemList.Count)
        {
            gemList.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
            gemListLastCount = gemList.Count;
        }
        yield return null;
    }

    public void WaitClick(GemController gem) { Debug.Log("Waitclick"); }

    public void GemDied(GameObject gem)
    {
        if (lastCombo > 0) { lastCombo++; }
        
        gemList.Remove(gem.GetComponent<GemController>());        
        gem.GetComponent<GemController>().Destroy();
    }


    void MonitorLevelsVelocity()
    //Monitor gems velocity - allow clicking again when landed 
    //Designed For the levels game 
    {
        
        if (gemList.Count > 0)
        {
            if (gemList[gemList.Count - 1].gameObject.GetComponent<Rigidbody>().velocity.y >= 0.0f)
            {
                gemClick = GemClicked;
                doUpdateStuff -= MonitorLevelsVelocity;
                ResetLastCombo();
            }
        }
        else
        {
            UpdateLevelText("\r\n Level cleared ");
            
            if (Stars > objectiveStars)
            {
                myBlock.LevelUpdateStars(myRank, Stars);
            }
            doUpdateStuff = null;
            doUpdateStuff += CenterStars;
            ResetLastCombo();
        }
    }

    void UpdateLevelText(string txt)
    {
        levelText.text = txt;
    }

    void UpdateMovesText()
    {
		if (movesText != null) {
            movesText.text = "\r\nMoves : " + Moves;
            if (bestCombo > 0) movesText.text += "\r\nBest Combo : " + bestCombo;
		}
    }

    void UpdateStars()
    {
        if (Moves > levelPar)
        {
            int starsToSub = (Moves - levelPar);
            if (movesAboveParPerStar == 1 ) Stars = Stars - 1;
            else if (movesAboveParPerStar > 1)
            {
                starsToSub = starsToSub / movesAboveParPerStar;
                Stars = Stars - starsToSub;
            }
        }
    }

    void UpdategoalsText()
    {
		if (goalsText != null) {
            goalsText.text = "Goal : " + levelPar + " moves";
		}
		if (levelUIStars.Count > 0) {
			for (int i = 0; i < levelUIStars.Count; i++)
            {
                if (i < Stars) {
                    levelUIStars[i].GetComponent<AnimationScript>().SetFilled();
                }else { levelUIStars[i].GetComponent<AnimationScript>().SetBlank(); }

            }
		}
    }

    void ResetLastCombo()
    {
        if (lastCombo > bestCombo) bestCombo = lastCombo;
        lastCombo = 0;
        UpdateMovesText();
    }
    


}
