using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlow : MonoBehaviour {

    public Transform redObj, blueObj, greenObj, yellowObj, brownObj;//gems prefabs references

    List<GemController> fullList = new List<GemController>();//List of ingame gems

    public delegate void gemClickedFunctionPointer(GameObject gem);
    public gemClickedFunctionPointer gemClick;//delegate behaviour for gemclicks

    GameVariables gameVars;

    void Awake()
    {
        gameVars = gameObject.GetComponent<GameVariables>();
        gemClick = CreateNewGem;
    }
	// Use this for initialization
	void Start () {


        for (int rowNum = -3; rowNum < 6; rowNum++)            //Init with random gems, different from the gem on the left(count-1) and the one below(count-9)
        {
            for (int colNum = -4; colNum < 5; colNum++)
            {
                int whichGem;
                bool okToAdd = true;
                do
                {
                    okToAdd = true;
                    whichGem = Random.Range(1, 6);

                    if (fullList.Count > 0)
                    {
                        if (fullList[fullList.Count - 1].gemType == GameVariables.GetTypeFromInt(whichGem)
                                                                        || GameVariables.GetTypeFromInt(whichGem) == GameVariables.GemType.undefined)
                        {
                            okToAdd = false;                            
                        }
                    }                        
                    

                    if (fullList.Count > 8)
                    {
                        if (fullList[fullList.Count - 9].gemType == GameVariables.GetTypeFromInt(whichGem)
                                                                    || GameVariables.GetTypeFromInt(whichGem) == GameVariables.GemType.undefined)
                        {
                            okToAdd = false;                            
                        }
                    }

                } while (okToAdd == false);
                
                CreateCubeAt(colNum, rowNum,GameVariables.GetTypeFromInt(whichGem));
            }
        }
        

	}
	
    void CreateNewGem(GameObject oldGem)
    {
        int whichGem = Random.Range(1, 6);
        CreateCubeAt(oldGem.transform.position.x, gameVars.YSpawnPosition, GameVariables.GetTypeFromInt(whichGem));
        Destroy(oldGem);
    }

    void CreateCubeAt(float col, float row, GameVariables.GemType whichGem)
    {
        Transform temp;
        switch (whichGem)
        {
            case GameVariables.GemType.red:
                temp = (Transform) Instantiate(redObj, new Vector3(col, row, 0), redObj.rotation);
                break;
            case GameVariables.GemType.blue:
                temp = (Transform)Instantiate(blueObj, new Vector3(col, row, 0), blueObj.rotation);
                break;
            case GameVariables.GemType.green:
                temp = (Transform)Instantiate(greenObj, new Vector3(col, row, 0), greenObj.rotation);
                break;
            case GameVariables.GemType.yellow:
                temp = (Transform)Instantiate(yellowObj, new Vector3(col, row, 0), yellowObj.rotation);
                break;
            case GameVariables.GemType.brown:
                temp = (Transform)Instantiate(brownObj, new Vector3(col, row, 0), brownObj.rotation);
                break;
            default:
                temp = (Transform)Instantiate(brownObj, new Vector3(col, row, 0), brownObj.rotation);
                break;
        }

        fullList.Add(temp.gameObject.GetComponent<GemController>());        
        
    }

    

	// Update is called once per frame
	void Update () {
	
	}
}
