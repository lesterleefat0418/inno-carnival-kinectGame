using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfigPage : MonoBehaviour
{
    public static ConfigPage Instance = null;
    public ConfigData configData;
    public GameObject kinectController;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.LoadRecords();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Debug.Log("Switch Scene");

            if (this.kinectController != null) {

                if (SceneManager.GetActiveScene().buildIndex == 1) {
                    this.kinectController.transform.localPosition = Vector3.zero;
                    this.changeScene(2);
                }
                else {
                    this.kinectController.transform.localPosition = new Vector3(1000f, 0f, 0f);
                    this.changeScene(1);
                }
            }

        }
    }

    public void LoadRecords()
    {
        if (DataManager.Load() != null)
        {
            this.configData = DataManager.Load();
            //Debug.Log("Load config file: " + configData);

            this.changeScene(1);
        }
        else
        {
            Debug.Log("config file is empty and get data from inspector!");
            this.SaveRecords();
        }
    }

    public void SaveRecords()
    {
        DataManager.Save(this.configData);
        this.LoadRecords();
    }

    public void changeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }


    private void OnDisable()
    {
        this.SaveRecords();
    }


    private void OnApplicationQuit()
    {
        this.SaveRecords();
    }
}


[System.Serializable]
public class ItemMarks 
{ 
    public int type1 = 5;
    public int type2 = 2;
    public int type3 = -1;
}

[System.Serializable]
public class ConfigData
{
    public int gameTime = 60;
    public int successMarkLevel = 60;
    public ItemMarks itemMarks;
}

public static class DataManager
{
    public static string directory = Directory.GetCurrentDirectory();
    public static string fileName = "/config.txt";
    public static void Save(ConfigData sData, bool dataMultipleLines = true)
    {
        string json = JsonUtility.ToJson(sData, dataMultipleLines);
        File.WriteAllText(directory + fileName, json);

        Debug.Log("Saved config file");
    }

    public static ConfigData Load()
    {
        string fullPath = directory + fileName;
        ConfigData loadData = new ConfigData();

        if (File.Exists(fullPath))
        {
            if (new FileInfo(fileName.Replace("/", "")).Length != 0)
            {
                string json = File.ReadAllText(fullPath);
                loadData = JsonUtility.FromJson<ConfigData>(json);
                return loadData;
            }
            else
            {
                UnityEngine.Debug.Log("Empty File");
                return null;
            }
        }
        else
        {
            UnityEngine.Debug.Log("Save File does not exist & create new One");
            var newFile = File.Create(fullPath);
            newFile.Close();
            return null;
        }
    }

}
