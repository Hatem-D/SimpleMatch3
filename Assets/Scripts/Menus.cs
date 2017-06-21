using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;



public class Menus : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("FacebookGameScene");
    }
    public void LoadOptions()
    {
        SceneManager.LoadScene("FacebookOptions");
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("FacebookMenu");
    }
    public void LoadHiscores()
    {
        SceneManager.LoadScene("FacebookHiScores");
    }
    public void LoadHelp()
    {
        SceneManager.LoadScene("FacebookHelp");
    }
    public void LoadAbout()
    {
        SceneManager.LoadScene("FacebookAbout");
    }
    public void LoadLevels()
    {
        SceneManager.LoadScene("LevelsScene");
    }
    public void LoadFreeMode()
    {
        SceneManager.LoadScene("FreeplayGameScene");
    }
    public void LoadLevelHelp()
    {
        SceneManager.LoadScene("LevelHelpScene");
    }
}
