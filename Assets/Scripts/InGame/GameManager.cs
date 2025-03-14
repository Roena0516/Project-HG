using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public bool isLevelEnd;
    public bool isSyncRoom;

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
            SceneManager.LoadScene("InGame");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSyncRoom)
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                SceneManager.LoadScene("FreePlay");
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
        yield return new WaitForSeconds(3f);

        DontDestroyOnLoad(gameObject);

        if (isSyncRoom)
        {
            SceneManager.LoadScene("SyncRoomResult");
        }
        else
        {
            SceneManager.LoadScene("Result");
        }

        yield break;
    }
}
