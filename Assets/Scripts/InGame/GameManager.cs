using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public bool isLevelEnd;
    public bool isSyncRoom;
    public bool isTest;

    public static GameManager Instance { get; private set; }

    private LevelEditer levelEditor;

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

    [System.Obsolete]
    private void Start()
    {
        isLevelEnd = false;
        isTest = SceneManager.GetSceneByName("LevelEditor").isLoaded;
        isSyncRoom = SceneManager.GetSceneByName("SyncRoom").isLoaded;

        if (isTest)
        {
            EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
            if (eventSystems.Length > 1)
            {
                for (int i = 1; i < eventSystems.Length; i++)
                {
                    Destroy(eventSystems[i].gameObject);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isTest)
        {
            SceneManager.LoadSceneAsync("InGame");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSyncRoom)
            {
                SceneManager.LoadSceneAsync("Menu");
            }
            if (isTest)
            {
                levelEditor = LevelEditer.Instance;
                levelEditor.canvas.SetActive(true);
                foreach (NoteClass note in levelEditor.saveManager.notes)
                {
                    note.isInputed = false;
                }
                Scene editorScene = SceneManager.GetSceneByName("LevelEditor");
                if (editorScene.IsValid() && editorScene.isLoaded)
                {
                    SceneManager.SetActiveScene(editorScene);
                }
                SceneManager.UnloadSceneAsync("InGame");
                return;
            }
            SceneManager.LoadSceneAsync("FreePlay");
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

        yield break;
    }
}
