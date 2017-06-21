using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HiScoresUI : MonoBehaviour {

    void Start()
    {
        if (FacebookManager.Instance != null) {
            if (FacebookManager.Instance.IsLoggedIn)
            {
                GameObject.FindObjectOfType<FBScript>().QueryFBScores();
            }
        }
    }

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			LoadMenu ();
		}
	}

    public void LoadMenu()
    {
        SceneManager.LoadScene("FacebookMenu");
    }


}
