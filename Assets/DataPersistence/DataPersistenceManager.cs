using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private List<string> fileNames = new List<string>() { "save1.game", "save2.game", "save3.game", "save4.game", "save5.game", "save6.game", "save7.game", "save8.game", "save9.game", "save10.game" };
    [SerializeField] private bool useEncryption;

    private string currentFileName;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); 
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // persistence accross scenes
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(currentFileName))
        {
            currentFileName = fileNames[0];
        }
        dataHandler = new FileDataHandler(Application.persistentDataPath, currentFileName, useEncryption);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoadedCallback;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedCallback;
    }

    private void OnSceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame(); 
    }

    public void NewGame()
    {
        gameData = new GameData
        {
            goldEarned = 0,
            firstPlayerPosition = new Vector3(34.8f, 4.2464f, -25.91f),
            secondPlayerPosition = new Vector3(43.8f, 4.2464f, -0.1f),
            unlockedAchievements = new List<string>(),
            levelDuration = 180,
        };
        SaveGame(); 
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
        if (gameData == null)
        {
            NewGame();
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void SetSaveSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < fileNames.Count)
        {
            currentFileName = fileNames[slotIndex];
            dataHandler = new FileDataHandler(Application.persistentDataPath, currentFileName, useEncryption);
        }
        else
        {
            Debug.LogWarning("Invalid save slot index.");
        }
    }

    public GameData GetLoadedData(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= fileNames.Count)
        {
            Debug.LogError("Invalid slot index.");
            return null;
        }

        string slotFileName = fileNames[slotIndex];
        FileDataHandler tempHandler = new FileDataHandler(Application.persistentDataPath, slotFileName, useEncryption);
        return tempHandler.Load();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
