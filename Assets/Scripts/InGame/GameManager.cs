using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private MenuManager menu;

    public bool isLevelEnd;

    [System.Obsolete]
    private void Start()
    {
        isLevelEnd = false;
        menu = FindObjectOfType<MenuManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("InGame");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("FreePlay");
        }

        if (isLevelEnd)
        {
            StartCoroutine(ChangeToResult());
        }
    }

    IEnumerator ChangeToResult()
    {
        yield return new WaitForSeconds(3f);

        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Result");
    }
}
