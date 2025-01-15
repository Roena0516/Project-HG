using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private bool isSet;

    public string fileName;
    public float sync;
    public float speed;
    public string eventName;

    private void Start()
    {
        isSet = true;
        speed = 4.0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isSet)
            {
                isSet = false;
                DontDestroyOnLoad(gameObject);
                SceneManager.LoadScene("FreePlay");
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
