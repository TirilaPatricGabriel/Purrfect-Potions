using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private List<string> fileNames = new List<string>() { "save1.game", "save2.game", "save3.game", "save4.game", "save5.game" };
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
            Destroy(this.gameObject); // Destroy duplicate instance
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // Persist across scenes
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(currentFileName))
        {
            currentFileName = fileNames[0]; // Default to the first save slot
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
        dataPersistenceObjects = FindAllDataPersistenceObjects(); // Register new scene objects
        LoadGame(); // Load game data into new objects
    }

    public void NewGame()
    {
        gameData = new GameData
        {
            goldEarned = 0,
            firstPlayerPosition = new Vector3(34.8f, 4.2464f, -25.91f),
            secondPlayerPosition = new Vector3(43.8f, 4.2464f, -0.1f),
            unlockedAchievements = new List<string>()
        };
        SaveGame(); // Immediately save the new game state
        Debug.Log("New game started and saved.");
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
        if (gameData == null)
        {
            Debug.Log("No data found. Starting a new game.");
            NewGame(); // Start a new game if no data is found
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Game data loaded successfully.");
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log($"Saving Data: Gold Earned = {gameData.goldEarned}, First Player Position = {gameData.firstPlayerPosition}");
        dataHandler.Save(gameData);
        Debug.Log("Game successfully saved to file.");
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        Debug.Log($"Found {dataPersistenceObjects.Count()} data persistence objects.");
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void SetSaveSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < fileNames.Count)
        {
            currentFileName = fileNames[slotIndex];
            dataHandler = new FileDataHandler(Application.persistentDataPath, currentFileName, useEncryption);
            Debug.Log($"Switched to save slot {slotIndex + 1} with file name: {currentFileName}");
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
        SaveGame(); // Save the game when the application quits
    }
}
