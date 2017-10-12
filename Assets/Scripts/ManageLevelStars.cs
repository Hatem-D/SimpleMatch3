using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageLevelStars : MonoBehaviour {

    public List<GameObject> myStars;
    public Material blank;
    public Material filled;
    public int myRank = 0;

    LevelBlocksController lvlBlocks;

    void Awake()
    {
        lvlBlocks = GameObject.FindObjectOfType<LevelBlocksController>();
        //StartCoroutine("WaitForFileLoad");
        lvlBlocks.updateUIStars += SetFilledStars;
    }

    IEnumerator WaitForFileLoad()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(0.2f);
        SetFilledStars();
    }

    public void SetFilledStars()//And Blanks
    {
        int starsToFill = lvlBlocks.levelStars[myRank + lvlBlocks.rankGroupOffset];
        //Debug.Log(gameObject.name + " : " + "Stars : " + starsToFill + " Rank : " + myRank + " Offset : " + lvlBlocks.rankGroupOffset);

        if (starsToFill <= myStars.Count)
        {
            for (int i = 0; i < starsToFill; i++)
            {
				myStars[i].GetComponent<AnimationScript>().SetFilled();
                Debug.Log("Filling");
            }
            for (int i = starsToFill; i < myStars.Count; i++)
            {
                myStars[i].GetComponent<AnimationScript>().SetBlank();
            }
        }
        else
        {
            Debug.Log("More Stars to fill than count");
        }
    }
}
