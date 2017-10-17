using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelBlocksController : MonoBehaviour {

    public List<LevelController> levelGroup1;
    public List<LevelController> levelGroup2;
    public List<LevelController> levelGroup3;

    List<LevelController> levelPrefabs;
    public List<GameObject> levelPages;
    public List<int> levelStars;

    int currentPrefab = 1;
    int currentPage = 0;
    public int rankGroupOffset = 0;

    public GameObject groupSelectUI;
    public GameObject inLevelUI;
    public GameObject levelSelectUI;
    public GameObject resumeGameBtn;
    public GameObject nextLevelBtn;
    public Text levelTextContainer;

    public GameObject sphereFX;
    Material sphereBaseMat;

    LevelController activeLevel;

    string fileName;
    public Text debugLog;
    FileInfo f;

    public delegate void StarsLoadedFuncPtr();
    public StarsLoadedFuncPtr updateUIStars;

    // Use this for initialization
    void Awake () {
        ActivateGroupSelectUI();
        levelPrefabs = levelGroup1;
        if (Application.platform == RuntimePlatform.Android)
        {
            fileName = Application.persistentDataPath;
            fileName += "/levels.txt";
        }
        else
        {
            //Debug.Log("Not Android");
            fileName = Application.persistentDataPath + "\\" + "levels.txt";
        }
        //Debug.Log(fileName);

        InitStars();
        LoadFile();
        InitSphere();
        InitPages();        
    }
	
	// Update is called once per frame
	//void Update () {}

    void InitPages()
    {
        if (levelPages.Count > 0)
        {
            foreach(GameObject page in levelPages)
            {
                page.SetActive(false);
            }
            levelPages[currentPage].SetActive(true);
        }
    }

    public void NextPage()
    {
        if (currentPage + 1 >= levelPages.Count) SetCurrentPage(0);
        else SetCurrentPage(currentPage + 1);        
    }

    public void PreviousPage()
    {        
        if (currentPage - 1 < 0) SetCurrentPage(levelPages.Count - 1);
        else SetCurrentPage(currentPage - 1);        
    }

    void SetCurrentPage(int newPage)
    {
        levelPages[currentPage].SetActive(false);
        currentPage = newPage;
        levelPages[currentPage].SetActive(true);
        if (updateUIStars != null) updateUIStars();
    }
    public void SelectLevel(int level)
    {
        ActivateInLevelUI();
        if (levelPrefabs.Count >= level)
        {
            currentPrefab = level - 1;
            GenerateLevel(currentPrefab);                      
        }else { Debug.Log("Out of bounds"); }        
    }

    public void ResetActiveLevel()
    {
        DestroyActiveLevel();
        SelectLevel(activeLevel.myRank + 1);
    }

    void DestroyActiveLevel()
    {
        if (activeLevel != null)
        {
            GameObject.Destroy(activeLevel.gameObject);
            ResetSphere();
        }
    }

    void InitStars()
    {
        levelStars = new List<int>();

        foreach(LevelController level in levelGroup1)
        {
            levelStars.Add(0);
        }
        foreach (LevelController level in levelGroup2)
        {
            levelStars.Add(0);
        }
        foreach (LevelController level in levelGroup3)
        {
            levelStars.Add(0);
        }
    }

    public void LevelUpdateStars(int levelRank, int stars)
    {
        levelStars[levelRank + rankGroupOffset] = stars;
        ResetFile();
        Save(LevelRankToString());        
    }
    
    public void ShowNextLevelBtn()
    {
        if (activeLevel.myRank + 1 < levelPrefabs.Count) nextLevelBtn.SetActive(true);
    }

    public void HideNextLevelBtn()
    {
        nextLevelBtn.SetActive(false);
    }

    public void LoadNextLevel()
    {
        DestroyActiveLevel();
        SelectLevel(activeLevel.myRank+2);
    }

    string LevelRankToString()
    {
        string result = "";
        foreach (int i in levelStars)
        {
            result += i + ";";
        }

        return result;
    }

    void GenerateLevel(int levelInList)
    {
        GameObject temp = Instantiate(levelPrefabs[levelInList].gameObject);
        activeLevel = temp.GetComponent<LevelController>();
        activeLevel.myRank = levelInList;
        activeLevel.objectiveStars = levelStars[levelInList];
        activeLevel.myBlock = this;
        levelTextContainer.text = temp.GetComponent<LevelRule>().getRuleText();

        resumeGameBtn.SetActive(true);
        HideNextLevelBtn();
    }

    public void BackToLevelSelect()
    {
        DestroyActiveLevel();
        LoadFile();
        ResumeGame();//unpause game
        ActivateLevelSelectUI();
        if (updateUIStars != null) updateUIStars();
    }

    public void ActivateGroupSelectUI()
    {
        levelSelectUI.SetActive(false);

        groupSelectUI.SetActive(true);
    }

    public void ActivateLevelSelectUI()
    {
        inLevelUI.SetActive(false);
        groupSelectUI.SetActive(false);
        
        levelSelectUI.SetActive(true);        
    }

    public void ActivateInLevelUI()
    {
        levelSelectUI.SetActive(false);

        inLevelUI.SetActive(true);
    }

    public void SelectLevelsGroup(int group)
    {
        //Debug.Log("Group : " + group);
        ActivateLevelSelectUI();
        SetCurrentPage(0);
        switch (group)
        {
            case 2:
                levelPrefabs = levelGroup2;
                rankGroupOffset = levelGroup2.Count;
                break;
            case 3:
                levelPrefabs = levelGroup3;
                rankGroupOffset = levelGroup2.Count + levelGroup3.Count;
                break;
            case 1:
            default:
                levelPrefabs = levelGroup1;
                rankGroupOffset = 0;
                break;
        }
        if (updateUIStars != null) updateUIStars();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("FacebookMenu");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        resumeGameBtn.SetActive(false);
    }

   

    

    void LoadFile()
    {
        StreamReader r;
        try
        {
            ShowInDebugLog("loading...platform : " + Application.platform.ToString() + " - datapath : " + Application.persistentDataPath + "/levels.txt");
            r = File.OpenText(Application.persistentDataPath + "/levels.txt");
            if (r != null) ShowInDebugLog("Reader null");
            if (r.ToString().Length > 0) ShowInDebugLog("Reader Empty");
            ShowInDebugLog("loading...platform : " + Application.platform.ToString() + " - datapath : " + Application.persistentDataPath + "/levels.txt" + r.ToString());
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                r.Close();
                r = File.OpenText(fileName);
            }

            string info = r.ReadToEnd();
            ShowInDebugLog(info);
            r.Close();
            LoadPresetsFromFileString(info);
        }
        catch
        {
            ShowInDebugLog("Exception Caught");
            FirstLoad();
        }
    }

    FileInfo GetFileInfo()
    {
        //Debug.Log("Getting file info");
        ShowInDebugLog("Getting file info");
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return (new FileInfo(fileName));
        }
        else
        {
            ShowInDebugLog("Getting file info reckon android platform ");
            return (new FileInfo(Application.persistentDataPath + "/levels.txt"));
        }
    }

    void ShowInDebugLog(string textToShow)
    {
        Text androidDebugLog = GameObject.Find("AndroidDebugLogText").GetComponent<Text>();

        if (androidDebugLog != null)
        {
            //Debug.Log ("android debug log not null !");
            androidDebugLog.text = textToShow;
        } //else Debug.Log ("android debug log null !");
    }

    void Save(string textToSave)
    {
        f = GetFileInfo();
        StreamWriter w;
        if (!f.Exists)
        {
            w = f.CreateText();
            //Debug.Log ("Creating : " + textToSave);
            ShowInDebugLog("Creating : " + textToSave);
        }
        else
        {
            w = f.AppendText();
            //Debug.Log ("Appending : " + textToSave);
            ShowInDebugLog("Appending : " + textToSave);
        }
        ShowInDebugLog(textToSave);
        w.WriteLine(textToSave);
        w.Close();
    }

    void ResetFile()
    {
        f = GetFileInfo();
        StreamWriter w;
        if (!f.Exists)
        {
            w = f.CreateText();
            ShowInDebugLog("Resetting file");
        }
        else
        {
            f.Delete();
            ShowInDebugLog("Resetting file");
            w = f.CreateText();
            ShowInDebugLog("Resetting file");
        }
        ShowInDebugLog("File reset");
        w.Close();
    }


    void FirstLoad()
    {
        Save(FirstSaveString());
        LoadFile();
    }

    string FirstSaveString()
    {
        /*string result = "";

        foreach (LevelController level in levelPrefabs)
        {
            result += "0;";
        }

        return (result);*/

        string result = "";
        int len = levelGroup1.Count + levelGroup2.Count + levelGroup3.Count;

        for (int i = 0; i < len; i++)
        {
            result += "0;";
        }

        return result;
    }

    void LoadPresetsFromFileString(string info)
    {
        string[] table = info.Split(';');
        
        for (int i = 0; i < table.Length - 1; i++)
        {
           //Debug.Log("table [" + i + "] -> " + table[i]);
           if (i < levelStars.Count)
           {
                table[i].Trim();
                int starsToAdd;
                if (!int.TryParse(table[i], out starsToAdd))
                {
                    Debug.Log("tried to parse " + table[i] + " in starsToAdd");
                }
                levelStars[i] = starsToAdd;
            }
        }
        
    }

    void InitSphere()
    {
        sphereBaseMat = sphereFX.gameObject.GetComponent<MeshRenderer>().material;
    }

    void ResetSphere()
    {
        sphereFX.gameObject.GetComponent<MeshRenderer>().material = sphereBaseMat;
    }
}

