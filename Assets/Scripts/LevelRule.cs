using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRule : MonoBehaviour {

    LevelController myLevel;
    public enum rules { moves, combo, chain, none}

    public rules selectedRule = rules.none;

    public delegate void updateStarsFunctionPointer();
    public updateStarsFunctionPointer updateStars;

    public delegate string getRuleTextFctPointer();
    public getRuleTextFctPointer getRuleText;

    public delegate string getScoreTextFctPointer();
    public getScoreTextFctPointer getScoreText;

    public delegate string getGoalTextFctPointer();
    public getGoalTextFctPointer getGoalText;

    public int threeStars = 0;
    public int twoStars = 0;
    public int oneStar = 0;

    public rules SelectedRule
    {
        get
        {
            return selectedRule;
        }

        set
        {
            SetRule(value);
            selectedRule = value;
        }
    }
  
    void Awake()
    {
        if (!(myLevel = gameObject.GetComponent<LevelController>()))
        {
            Debug.Log("Warning - no level controller");
        }
        SetRule(selectedRule);
    }

    #region UpdateStarsDelegates

    public void UpdateStarsNone() { }

    public void UpdateStarsMovesRule()
    {
        if (myLevel.Moves <= threeStars) { myLevel.Stars = 3; }
        else {
            if (myLevel.Moves <= twoStars) { myLevel.Stars = 2; }
            else
            {
                if (myLevel.Moves <= oneStar) { myLevel.Stars = 1; }
                else { myLevel.Stars = 0; }
            }
        }            
    }

    public void UpdateStarsComboRule()
    {
        if (threeStars == 0)
        {
            threeStars = myLevel.levelPar;
            twoStars = threeStars - 10;
            oneStar = twoStars - 10;
        }
        
        if (myLevel.bestCombo >= threeStars)
        {
            myLevel.Stars = 3;
        }else if ((myLevel.bestCombo >= twoStars) && (myLevel.Stars < 2))
        {
            myLevel.Stars = 2;
        }
        else if ((myLevel.bestCombo >= oneStar) && (myLevel.Stars < 1))
        {
            myLevel.Stars = 1;
        }
        else if (myLevel.bestCombo < oneStar) myLevel.Stars = 0;
    }

    public void UpdateStarsComboChainRule()
    {
        if (threeStars == 0)
        {
            threeStars = myLevel.levelPar;
            twoStars = threeStars - 2;
            oneStar = twoStars - 2;
        }

        if (myLevel.bestChain >= threeStars)
        {
            myLevel.Stars = 3;
        }
        else if ((myLevel.bestChain >= twoStars) && (myLevel.Stars < 2))
        {
            myLevel.Stars = 2;
        }
        else if ((myLevel.bestChain >= oneStar) && (myLevel.Stars < 1))
        {
            myLevel.Stars = 1;
        }
    }

    #endregion

    #region GetRuleTextDelegates

    string GetRuleTextNone()
    {
        return ("RuleText not set");
    }

    string GetRuleTextMoves()
    {
        return ("Clear the board in\r\n" + threeStars + " moves or less to earn 3 stars !");
    }

    string GetRuleTextCombo()
    {
        return ("Get a combo of " + threeStars + " cubes or more to earn 3 stars !");
    }

    string GetRuleTextComboChain()
    {
        return ("Chain " + threeStars + " combos or more to earn 3 stars !");
    }

    #endregion

    #region GetScoreTextDelegates

    string GetRuleScoreTextNone()
    {
        return ("RuleScore not set");
    }

    string GetRuleScoreTextMoves()
    {        
        return ("\r\nMoves : " + myLevel.Moves);
    }

    string GetRuleScoreTextCombo()
    {
        return ("\r\nBest Combo : " + myLevel.bestCombo);
    }

    string GetRuleScoreTextComboChain()
    {
        return ("Current Chain : "+ myLevel.CurrentComboChain + "\r\nColor Chain : " + myLevel.currentColorChain + "\r\nBest Chain : " + myLevel.bestChain);
    }

    #endregion

    #region GetGoalTextDelegates

    string GetGoalTextNone()
    {
        return ("RuleScore not set");
    }

    string GetGoalTextMoves()
    {
        if (myLevel.Moves <= threeStars)
            return ("Goal : " + threeStars + " moves");
        if (myLevel.Moves <= twoStars)
            return ("Goal : " + twoStars + " moves");
        if (myLevel.Moves <= oneStar)
            return ("Goal : " + oneStar + " moves");
        return ("No stars left !");
    }

    string GetGoalTextCombo()
    {
        if (myLevel.bestCombo < oneStar)
            return ("Goal : " + oneStar + " combo");
        if (myLevel.bestCombo < twoStars)
            return ("Goal : " + twoStars + " combo");
        return ("Goal : " + threeStars + " combo");
    }

    string GetGoalTextComboChain()
    {
        if (myLevel.bestChain < oneStar)
            return ("Next goal : " + oneStar + " combos chained");
        if (myLevel.bestChain < twoStars)
            return ("Next goal : " + twoStars + " combos chained");
        return ("Next goal : " + threeStars + " combos chained");
    }

    #endregion

    #region GetEndGameConditionDelegates

    /*bool GetEndGameConditionNone()
    {
        return false;
    }

    bool GetEndGameConditionMoves()
    {
        
    }

    bool GetEndGameConditionCombo()
    {
        if (myLevel.bestCombo < oneStar)
            return ("Goal : " + oneStar + " combo");
        if (myLevel.bestCombo < twoStars)
            return ("Goal : " + twoStars + " combo");
        return ("Goal : " + threeStars + " combo");
    }

    bool GetEndGameConditionComboChain()
    {
        if (myLevel.bestChain < oneStar)
            return ("Goal : " + oneStar + " combos chained");
        if (myLevel.bestChain < twoStars)
            return ("Goal : " + twoStars + " combos chained");
        return ("Goal : " + threeStars + " combos chained");
    }*/

    #endregion 

    void SetRule(rules newRule)
    {
        //Debug.Log("setting rule : " + newRule);
        switch (newRule){
            case rules.moves:                
                updateStars = UpdateStarsMovesRule;
                getRuleText = GetRuleTextMoves;
                getScoreText = GetRuleScoreTextMoves;
                getGoalText = GetGoalTextMoves;
                break;
            case rules.combo:                
                updateStars = UpdateStarsComboRule;
                getRuleText = GetRuleTextCombo;
                getScoreText = GetRuleScoreTextCombo;
                getGoalText = GetGoalTextCombo;
                break;
            case rules.chain:                
                updateStars = UpdateStarsComboChainRule;
                getRuleText = GetRuleTextComboChain;
                getScoreText = GetRuleScoreTextComboChain;
                getGoalText = GetGoalTextComboChain;                
                break;
            default:                
                updateStars = UpdateStarsNone;
                getRuleText = GetRuleTextNone;
                getScoreText = GetRuleScoreTextNone;
                getGoalText = GetGoalTextNone;
                break;
        }
    }
}
