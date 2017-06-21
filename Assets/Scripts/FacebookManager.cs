using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour {

	private static FacebookManager _instance;

	public delegate void FBErrorsEventsPointer(string errorResult);
	public FBErrorsEventsPointer FBErrorEvent; 

	public static FacebookManager Instance
	{
		get{ 
			if (_instance == null) {
				GameObject fbm = new GameObject ("FBManager");
				fbm.AddComponent<FacebookManager> ();
				//Debug.Log ("creating manager");
			}
			return _instance;
		}
	}

	public bool IsLoggedIn { get; set;}
    public bool canSaveScore = true;

	public string ProfileName { get; set;}
	public Sprite ProfilePic { get; set;}
	public int profileScore;

    public string AppLinkUrl { get; set;}
    
   	void Awake()
	{
		DontDestroyOnLoad (this.gameObject);
		_instance = this;	
		
		profileScore = 0;
	}

	public void InitFB()
	{
		if (!FB.IsInitialized) {
			//Debug.Log ("tried to init fb");
			FB.Init (SetInit, OnHideUnity);
		} else {
			IsLoggedIn = FB.IsLoggedIn;
		}
	}
	void SetInit() {
		if (FB.IsLoggedIn)
		{
			//Debug.Log("FB is logged in");
			GetProfile ();
		}//else { Debug.Log("FB is not logged in"); }
		IsLoggedIn = FB.IsLoggedIn;
	}

	void OnHideUnity(bool isGameShown) {
		if (!isGameShown)
		{
			Time.timeScale = 0.0f;
		}
		else {
			Time.timeScale = 1.0f;
		}
	}

	public void GetProfile()
	{
		FB.API("/me?fields=first_name", HttpMethod.GET, GetUserName);
		FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, GetProfilePicture);
		//FB.GetAppLink (DealWithAppLink); only works if app is approved and has a dedicated page on fb
		AppLinkUrl = "http://www.canardpc.com";
	}

	void GetUserName(IResult result)
	{
		if (result.Error == null) {
			ProfileName = (string) result.ResultDictionary["first_name"];
		}
		else { 
			//Debug.Log(result.Error); 
			if (FBErrorEvent != null) {
				FBErrorEvent (result.Error);
			}
		}        
	}

	void GetProfilePicture(IGraphResult result)
	{
		if (result.Texture != null)
		{
			ProfilePic = Sprite.Create(result.Texture, new Rect(0 ,0,result.Texture.width,result.Texture.height), new Vector2());
		}else {
			//Debug.Log("result texture null");
			if (result.Error != null) {
				//Debug.Log (result.Error);
				if (FBErrorEvent != null) {
					FBErrorEvent (result.Error);
				}
			}
		}
	}

	void DealWithAppLink(IAppLinkResult result){
		if (!String.IsNullOrEmpty (result.Url)) {
			AppLinkUrl = result.Url;
		}
	}


	public void Share (){
		FB.FeedShare (
			string.Empty,
			new Uri(AppLinkUrl),
			"Title",
			"Caption",
			"Check this out",
			new Uri("http://cuteoverload.files.wordpress.com/2016/01/a-very-friendly-quokka-imgur.jpg"),
			string.Empty,
			ShareCallback
		);
	}

	void ShareCallback(IResult result){
		if (result.Cancelled) {
			//Debug.Log ("Share cancelled");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			//Debug.Log ("Error on Share : " + result.Error);
			if (FBErrorEvent != null) {
				FBErrorEvent (result.Error);
			}
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			//Debug.Log ("Share succeeded");
		}
	}

	public void Invite()
	{
		FB.Mobile.AppInvite (
			new Uri (AppLinkUrl),
			new Uri ("http://cuteoverload.files.wordpress.com/2016/01/a-very-friendly-quokka-imgur.jpg"),
			InviteCallback
		);	
	}

	void InviteCallback(IResult result){
		if (result.Cancelled) {
			//Debug.Log ("Invite cancelled");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			//Debug.Log ("Error on Invite : " + result.Error);
			if (FBErrorEvent != null) {
				FBErrorEvent (result.Error);
			}
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("Invite succeeded");
		}
	}

	public void ShareWithUsers(){
		FB.AppRequest (
			"Check out my score",
			null,
			new List<object>(){"app_users"},
			null,
			null,
			null,
			"Send a challenge",
			ShareWithUsersCallback
		);
	}

	void ShareWithUsersCallback(IAppRequestResult result)
	{
		Debug.Log (result.RawResult);
		if (FBErrorEvent != null) {
			FBErrorEvent (result.RawResult);
		}
		if (result.Cancelled) {
			Debug.Log ("Brag cancelled");

		} else if (!string.IsNullOrEmpty (result.Error)) {
			Debug.Log ("Error on Brag : " + result.Error);
			if (FBErrorEvent != null) {
				FBErrorEvent (result.Error);
			}
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("Brag succeeded");
		}
	}
	///Scores Code
	/// 

	public void AskMyFBScore ()
	{
        canSaveScore = false;
		FB.API ("/me/scores", HttpMethod.GET, GetMyScore);
	}

	public void GetScores(FacebookDelegate<IGraphResult> callbackDelegate)
	{
		FB.API ("/app/scores?fields=score,user.limit(10)", HttpMethod.GET, callbackDelegate);
	}

	public void GetMyScore (IGraphResult result)
	{
		//Debug.Log ("Get My Score : " + result.RawResult);
		if (FBErrorEvent != null) {
			FBErrorEvent (result.RawResult);
		}
		if (result.Error == null) {
            canSaveScore = true;
            IDictionary<string, object> data = result.ResultDictionary;
			List<object> scorelist = (List<object>)data ["data"];
			foreach (object obj in scorelist) {
				var entry = (Dictionary<string, object>)obj;
				//var user = (Dictionary<string, object>)entry ["user"];
				//Debug.Log (user ["name"].ToString () + " , " + entry ["score"]);
				if (!Int32.TryParse (entry ["score"].ToString (), out profileScore)) {
					Debug.Log ("try parse on score entry failed");
				}
				//Debug.Log ("profile score = " + profileScore);
			}
		}else {
            canSaveScore = false;
			if (FBErrorEvent != null) {
				FBErrorEvent ("Error triggered on my score : " + result.Error);
			}
		}
	}

	public void SendScore(Dictionary<string, string> scoreData, FacebookDelegate<IGraphResult> resultDelegate){
		FB.API ("/me/scores", HttpMethod.POST, resultDelegate, scoreData);
	}

	public void AskForPublishActions(FacebookDelegate<ILoginResult> callbackDelegate)
	{
		FB.LogInWithPublishPermissions (new List<string>(){"publish_actions"},callbackDelegate);
	}

    public void FBLogout()
    {
        IsLoggedIn = FB.IsLoggedIn;
        canSaveScore = false;
        ProfilePic = null;
        ProfileName = null;
    }
}
