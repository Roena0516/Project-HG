using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public bool isLevelEnd;
    public bool isSyncRoom;
    public bool isTest;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isLevelEnd = false;
        isSyncRoom = SceneManager.GetSceneByName("SyncRoom").isLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync("InGame");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSyncRoom)
            {
                SceneManager.LoadSceneAsync("Menu");
            }
            else
            {
                SceneManager.LoadSceneAsync("FreePlay");
            }
        }

        if (isLevelEnd)
        {
            isLevelEnd = false;
            StartCoroutine(ChangeToResult());
        }
    }

    IEnumerator ChangeToResult()
    {
        yield return new WaitForSeconds(5f);

        if (!isTest)
        {
            DontDestroyOnLoad(gameObject);

            if (isSyncRoom)
            {
                SceneManager.LoadSceneAsync("SyncRoomResult");
            }
            else
            {
                SceneManager.LoadSceneAsync("Result");
            }
        }
        else
        {
            SceneManager.UnloadSceneAsync("LevelEditorTest");
        }

        yield break;
    }
}
