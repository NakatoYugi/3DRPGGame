using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [Tooltip("��ѡ�����Բ��Ӳ˵�ҳ����ʼ������ʼ��Ϸ")] 
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool disableDataPersistence = false;
    [Tooltip("��ѡ����ʹ�ò��Ե������Ե����ڵ��ļ���Ϊ���õ�testSelectedProfileId")]
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    [Header("Auto Saving Configuration")]
    [SerializeField] private float autoSaveTimeSeconds = 60f;

    private string selectedProfileId = "";

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private Coroutine autoSaveCoroutine;
    public static DataPersistenceManager instance {get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(instance);
        }

        DontDestroyOnLoad(gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        InitializeSelectedProfileId();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSave());
    }

    public void DeleteProfileData(string profileId)
    {
        // delete the data for this profile id
        dataHandler.Delete(profileId);
        // initialize the selected profile id
        InitializeSelectedProfileId();
        // reload the game so that our data matches the newly selected profile id
        LoadGame();
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading
        this.selectedProfileId = newProfileId;
        //����gameDataΪѡ���profileId��Ӧ�Ĵ浵
        LoadGame();
    }
    private void InitializeSelectedProfileId()
    {
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
        }
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        if (disableDataPersistence)
        {
            return;
        }

        gameData = dataHandler.Load(selectedProfileId);

        // start a new game if the data is null and we're configured to initialize data for debugging purposes
        if (this.gameData == null && initializeDataIfNull)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        // if no data can be loaded, don't continue
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }

        //�����ص����ݴ��ݸ�����ʵ����IDataPersistence�ӿڵ��࣬�����ǿ���ѡ�����������Ҫ������
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        if (disableDataPersistence)
        {
            return;
        }

        // if we don't have any data to save, log a warning here
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }

        //�����ݴ��ݸ�����ʵ����IDataPersistence�ӿڵ��࣬�����ǿ��Ը�������
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        //���¸����ݵ�������ʱ��
        gameData.lastUpdated = System.DateTime.Now.ToBinary();
        //�����ºõ����ݱ��浽�ļ�
        dataHandler.Save(gameData, selectedProfileId);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> _dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(_dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame();
            Debug.Log("Auto Saved Game");
        }
    }
}
