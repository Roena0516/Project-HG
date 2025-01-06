using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private bool isMenuSet;
    private bool isSet;

    public GameObject fileNameInputField;
    public GameObject syncInputField;
    public GameObject speedInputField;

    public string fileName;
    public float sync;
    public float speed;

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
                syncInputField.SetActive(true);
                speedInputField.SetActive(true);
                isMenuSet = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuSet && isSet)
            {
                fileNameInputField.SetActive(false);
                syncInputField.SetActive(false);
                speedInputField.SetActive(false);
                isMenuSet = false;
            }
        }
    }

    public void SetFileName(string inputed)
    {
        fileName = inputed;
    }

    public void SetSync(string inputed)
    {
        float.TryParse(inputed, out sync);
    }

    public void SetSpeed(string inputed)
    {
        float.TryParse(inputed, out speed);
    }
}
