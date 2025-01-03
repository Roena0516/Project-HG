using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private MenuManager menu;

    [System.Obsolete]
    private void Start()
    {
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
            Destroy(menu.gameObject);
            SceneManager.LoadScene("Menu");
        }
    }
}
