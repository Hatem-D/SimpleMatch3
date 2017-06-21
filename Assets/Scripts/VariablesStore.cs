using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;




public class VariablesStore : MonoBehaviour{

	public Text debugLog;
	string fileName;
    
    public enum GameType { scoreAttack }
    public GameType currentGameType = GameType.scoreAttack;

    FileInfo f;

    [HideInInspector]
    public string file;
    
    // Use this for initialization
	void Awake(){
		if (Application.platform == RuntimePlatform.Android) {
			fileName = Application.persistentDataPath;
			if (currentGameType == GameType.scoreAttack) fileName += "/scoreatk.txt";
		} else {
			Debug.Log ("Not Android");
			fileName = Application.persistentDataPath + "\\" + "scoreatk.txt";
		}
		Debug.Log (fileName);
        Load();
    }

    void Start () {
    }

	FileInfo GetFileInfo(){
		//Debug.Log ("Getting file info");
		ShowInDebugLog ("Getting file info");
		if (Application.platform == RuntimePlatform.WindowsEditor) {			
			return (new FileInfo (fileName));
		} else {
			ShowInDebugLog ("Getting file info reckon android platform ");
            if (currentGameType == GameType.scoreAttack)
                return (new FileInfo(Application.persistentDataPath + "/scoreatk.txt"));
            else return (new FileInfo(Application.persistentDataPath + "/scoreatk.txt"));
		}
	}

	void ShowInDebugLog(string textToShow){
		Text androidDebugLog = GameObject.Find("AndroidDebugLogText").GetComponent<Text>();

		if (androidDebugLog != null) {			
			androidDebugLog.text = textToShow;
		} 
	}

    public void Save(string textToSave)
    {
		f = GetFileInfo ();
        StreamWriter w;
        if (!f.Exists)
        {
            w = f.CreateText();			
			ShowInDebugLog ("Creating : " +textToSave);
        }
        else
        {
            w = f.AppendText();			
			ShowInDebugLog ("Appending : " +textToSave);
        }
		ShowInDebugLog (textToSave);
        w.WriteLine(textToSave);
        w.Close();
    }

	public void ResetFile(){
		f = GetFileInfo ();
		StreamWriter w;
		if (!f.Exists)
		{
			w = f.CreateText();
			ShowInDebugLog ("Reseting file");
		}
		else
		{
			f.Delete ();
			ShowInDebugLog ("Reseting file");
			w = f.CreateText ();
			ShowInDebugLog ("Reseting file");
		}
		ShowInDebugLog ("File reset");
		w.Close();
	}

    void Load()
    {
		StreamReader r;
		try {
			ShowInDebugLog("loading...platform : "+Application.platform.ToString()+" - datapath : "+Application.persistentDataPath + "/scoreatk.txt");
            if (currentGameType == GameType.scoreAttack)
            {
                r = File.OpenText(Application.persistentDataPath + "/scoreatk.txt");
            }
            else
            {
                r = File.OpenText(Application.persistentDataPath + "/scoreatk.txt");
            }
            
            if (r != null) ShowInDebugLog("Reader null") ;
            if (r.ToString().Length > 0) ShowInDebugLog("Reader Empty");
            ShowInDebugLog("loading...platform : "+Application.platform.ToString()+" - datapath : "+Application.persistentDataPath + "/scoreatk.txt" + r.ToString());
			if (Application.platform == RuntimePlatform.WindowsEditor){
				r.Close();
				r = File.OpenText(fileName);
			}

			string info = r.ReadToEnd();
			ShowInDebugLog (info);
			r.Close();
			LoadPresetsFromFileString(info);
		}catch {
			ShowInDebugLog ("Exception Caught");
			FirstLoad ();
		}
	}

	

	void SaveAllTables ()
    {
		ResetFile ();

        Load ();
    }

	void FirstLoad ()
	{
        if (currentGameType == GameType.scoreAttack) Save("0;0;0");
        else Save("0");

		Load ();
	}

	void LoadPresetsFromFileString(string info)
	{
        //Debug.Log("loading presets : " + info);
        file = info;
	}

}

