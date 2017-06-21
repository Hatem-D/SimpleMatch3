using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Facebook.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FBScript : MonoBehaviour {

    public GameObject DialogLoggedIn;
    public GameObject DialogLoggedOut;
    public Image DialogFBUserPicture;
    public Text DialogFBUserName;

    //public Text hiScoresFBNamesText;
    //public Text hiScoresFBScoresText;    
	public GameObject FBErrorLog;
	public Text FBErrorLogText;
    public GameObject noPublishActionsPrompt;
    public GameObject ScoreEntryPanel;
    public GameObject ScrollScoresList;

	public Text playerScoreText;

	delegate void WaitFunctionPointer();


	IEnumerator coroutine;

    // Use this for initialization
    void Awake () {
		FacebookManager.Instance.InitFB ();
		//FacebookManager.Instance.FBErrorEvent = ShowFBError;

		if (noPublishActionsPrompt != null)
			noPublishActionsPrompt.SetActive (false);
	}

	void Start(){
        DealWithFBMenus(FB.IsLoggedIn);
        if (!FB.IsLoggedIn && SceneManager.GetActiveScene().name == "FacebookMenu") StartCoroutine("WaitForLogin");
    }


	public void FBLogin() {
		FacebookManager.Instance.InitFB ();
        if (FB.IsLoggedIn) {
            DealWithFBMenus(FB.IsLoggedIn);
        }
        else { 
            List<string> permissions = new List<string>();
            permissions.Add ("public_profile");
            permissions.Add ("user_friends");
            FB.LogInWithReadPermissions(permissions, AuthCallBack);
        }
    }

	public void FBLogout(){
		if (FB.IsLoggedIn)
		{                                                                                  
			FB.LogOut (); 
			StartCoroutine ("CheckForSuccessfullLogout");
		} 
	}
    
	IEnumerator	 CheckForSuccessfullLogout(){
		while (FB.IsLoggedIn){			
			yield return null;
		}
		DealWithFBMenus(FB.IsLoggedIn);
        FacebookManager.Instance.FBLogout();
	}

    void AuthCallBack(IResult result) {
        if (result.Error != null)
        {
            //Debug.Log(result.Error);//400 = user token prblm
        }else
        {
            if (FB.IsLoggedIn)
            {
				FacebookManager.Instance.IsLoggedIn = true;
				FacebookManager.Instance.GetProfile ();
				//Debug.Log("FB is logged in "+ result.RawResult);
            }
            //else { Debug.Log("FB is not logged in"); }
            DealWithFBMenus(FB.IsLoggedIn);
            
        }
    }

    void DealWithFBMenus(bool isLoggedIn)
    {
       if (isLoggedIn) {
            DialogLoggedIn.SetActive(true);
            DialogLoggedOut.SetActive(false); 

			if (FacebookManager.Instance.ProfileName != null) {
				DialogFBUserName.text = FacebookManager.Instance.ProfileName;
			} else {
				StartCoroutine ("WaitForProfileName");
			}
			if (FacebookManager.Instance.ProfilePic != null) {
				DialogFBUserPicture.sprite = FacebookManager.Instance.ProfilePic;
			} else {
				StartCoroutine ("WaitForProfilePic");
			}           
        }
        else {
            DialogLoggedIn.SetActive(false);
            DialogLoggedOut.SetActive(true);
        }
        //ShowFBError("Instance logged in : " + FacebookManager.Instance.IsLoggedIn + "\r\n FB Logged in : " + FB.IsLoggedIn + "\r\n Scene " + SceneManager.GetActiveScene().name);
    }

    
	IEnumerator WaitForProfileName(){
		while (FacebookManager.Instance.ProfileName == null){			
			yield return null;
		}
		DealWithFBMenus(FacebookManager.Instance.IsLoggedIn);
	}

	IEnumerator WaitForProfilePic(){
		while (FacebookManager.Instance.ProfilePic == null){
			yield return null;
		}
		DealWithFBMenus(FacebookManager.Instance.IsLoggedIn);
	}

	IEnumerator WaitForLogin(){
		while (FacebookManager.Instance.IsLoggedIn == false) {
			//Debug.Log ("Waiting for login");
			yield return null;
		}
        DealWithFBMenus(FB.IsLoggedIn);
	}

	IEnumerator WaitForScore(WaitFunctionPointer action){
		while (!FacebookManager.Instance.canSaveScore) {
			Debug.Log ("Waiting for score");
			yield return null;
		}
		if (action != null) action ();
	}

	public void Share()
	{
		FacebookManager.Instance.Share ();
	}

	public void Invite()
	{
		FacebookManager.Instance.Invite ();
	}

	public void ShareWithUsers(){
		FacebookManager.Instance.ShareWithUsers ();
	}


    public void QueryFBScores()
    {
        if (FacebookManager.Instance != null)
        {
            if (FacebookManager.Instance.IsLoggedIn)
                FacebookManager.Instance.GetScores(ShowFBScore);                        
        }        
    }

    public void ShowFBScore(IResult result)
    {
        IDictionary<string, object> data = result.ResultDictionary;
        List<object> scoreList = (List<object>)data["data"];        

        foreach (Transform child in ScrollScoresList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (object obj in scoreList)
        {
            var entry = (Dictionary<string, object>)obj;
            var user = (Dictionary<string, object>)entry["user"];

            //Debug.Log(user["name"].ToString() + " , " + entry["score"].ToString());

            GameObject scorePanel;
            scorePanel = Instantiate(ScoreEntryPanel) as GameObject;
            scorePanel.transform.SetParent(ScrollScoresList.transform, false);

            Transform FName = scorePanel.transform.Find("FriendName");
            Transform FScore = scorePanel.transform.Find("FriendScore");
            Transform FAvatar = scorePanel.transform.Find("FriendAvatar");

            Text FNameText = FName.GetComponent<Text>();
            Text FScoreText = FScore.GetComponent<Text>();
            Image FUserAvatar = FAvatar.GetComponent<Image>();

            FNameText.text = user["name"].ToString();
            FScoreText.text = entry["score"].ToString();

            

            FB.API(user["id"].ToString() + "/picture?width=128&height=128", HttpMethod.GET,
                delegate (IGraphResult picResult)
            {
                if (picResult.Error != null)
                {
                    //Debug.Log(picResult.ToString());
                }
                else
                {
					FUserAvatar.sprite = Sprite.Create(picResult.Texture, new Rect(0, 0, picResult.Texture.width, picResult.Texture.height), new Vector2(0, 0));
                }
            });

        }
    }

	public void QueryMyFBScore()
	{
		FacebookManager.Instance.AskMyFBScore ();
		coroutine = WaitForScore (ShowMyFBScore);
		StartCoroutine (coroutine);
	}

	void ShowMyFBScore(){
		if (playerScoreText != null) {
			playerScoreText.text = FacebookManager.Instance.profileScore.ToString();
		} //else Debug.Log ("no player score text set");
        Debug.Log("show my fb score called");
    }

    public bool GameOver(int newScore)
	{
		scoreToSend = newScore;
		if (FacebookManager.Instance != null) {
			if (FacebookManager.Instance.IsLoggedIn) {
                if (FacebookManager.Instance.canSaveScore)
                {
                    if (newScore > FacebookManager.Instance.profileScore)
                    {
                        if (AccessToken.CurrentAccessToken.Permissions.Contains("publish_actions"))
                        {
                            //Debug.Log("have publish actions");
                            SetFBScore();
                        }
                        else
                        {
                            noPublishActionsPrompt.SetActive(true);
                            Debug.Log("no publish actions");
                        }

                        return true;
                    }
                    else
                    {
                        //Debug.Log ("score lower");
                        return false;
                    }
                }else {                   
                    return false;//Cannot save score
                }
			} else {
				//Debug.Log ("Not logged in");
				return false;
			}
		} else {
			//Debug.Log ("No Facebook Manager set");
			return false;
		}
	}


    public void ResetScore(){
		scoreToSend = 0;
        if (FacebookManager.Instance != null)
        {
            if (FacebookManager.Instance.IsLoggedIn)
            {
                if (AccessToken.CurrentAccessToken.Permissions.Contains("publish_actions"))
                {
                    //ShowFBError("have publish actions");
                    SetFBScore();
                }
                else
                {
                    //ShowFBError("no publish actions");
                    noPublishActionsPrompt.SetActive(true);
                    
                }
            }            
        }        
	}

	int scoreToSend = 0;

	public void AskForPublishActions(){
		FacebookManager.Instance.AskForPublishActions (GameOverPublishPermissionsCallback);
		noPublishActionsPrompt.SetActive (false);
	}

	public void GameOverPublishPermissionsCallback(ILoginResult result){
		//Debug.Log ("Publish actions result : " + result.RawResult);
		//ShowFBError ("Publish actions result : " + result.RawResult);
		if (AccessToken.CurrentAccessToken.Permissions.Contains ("publish_actions")) {
			SetFBScore ();
		} /*else {
			//Debug.Log ("Still no publish actions : " + result.RawResult);
			ShowFBError ("Still no publish actions : \r\n" + result.RawResult);
		}*/
	}

	void SetFBScore()
    {
        Dictionary<string, string> scoreData = new Dictionary<string, string>();
        scoreData["score"] = scoreToSend.ToString();
        
        FacebookManager.Instance.SendScore(scoreData, ScoreSubmitCallback);
       
    }

    public void ScoreSubmitCallback(IGraphResult result)
    {
		Debug.Log("Score Sub result " + result.RawResult);
		//ShowFBError (result.RawResult);
		QueryMyFBScore ();
    }

	public void ShowFBError(string errorText)
	{
		if (FBErrorLog != null){
			FBErrorLog.gameObject.SetActive (true);
			FBErrorLogText.text	= errorText;
		}
	}

	public void HideFBErrorLog(){
		if (FBErrorLog != null) {
			FBErrorLog.gameObject.SetActive (false);
		}
	}
}
