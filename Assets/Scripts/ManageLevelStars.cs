using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageLevelStars : MonoBehaviour {

    public List<GameObject> myStars;
    public Material blank;
    public Material filled;
    public int myRank = 0;

    LevelBlocksController lvlBlocks;

    void Start()
    {
        lvlBlocks = GameObject.FindObjectOfType<LevelBlocksController>();
        StartCoroutine("WaitForFileLoad");
        lvlBlocks.updateUIStars += SetFilledStars;
    }

    IEnumerator WaitForFileLoad()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(0.2f);
        SetFilledStars();
    }

    public void SetFilledStars()
    {
        int starsToFill = lvlBlocks.levelStars[myRank];
        //Debug.Log("Setting filled starts : " + starsToFill);
        if (starsToFill <= myStars.Count)
        {
            for (int i = 0; i < starsToFill; i++)
            {
				myStars[i].GetComponent<AnimationScript>().SetFilled();
            }
        }
        else
        {
            Debug.Log("More Stars to fill than count");
        }
    }
}
