using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private bool isMenuSet;
    private bool isSet;

    public GameObject fileNameInputField;

    public string fileName;

    private void Start()
    {
        isMenuSet = false;
        isSet = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isMenuSet && isSet)
            {
                isSet = false;
                DontDestroyOnLoad(gameObject);
                SceneManager.LoadScene("InGame");
            }
            if (!isMenuSet && isSet)
            {
                fileNameInputField.SetActive(true);
                isMenuSet = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuSet && isSet)
            {
                fileNameInputField.SetActive(true);
                isMenuSet = false;
            }
        }
    }

    public void SetFileName(string inputed)
    {
        fileName = inputed;
    }
}
