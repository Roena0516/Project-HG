using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class LevelEditer : MonoBehaviour
{

    private SaveManager saveManager;

    private GameObject beat13;
    private GameObject beat14;
    private GameObject beat16;
    private GameObject beat18;
    private GameObject beat112;
    private GameObject beat116;
    private GameObject beat124;
    private GameObject beat132;
    private GameObject beat148;
    private GameObject beat164;

    public GameObject parentFolder13;
    public GameObject parentFolder14;
    public GameObject parentFolder16;
    public GameObject parentFolder18;
    public GameObject parentFolder112;
    public GameObject parentFolder116;
    public GameObject parentFolder124;
    public GameObject parentFolder132;
    public GameObject parentFolder148;
    public GameObject parentFolder164;

    public GameObject canvas;

    private RectTransform rect13;
    private RectTransform rect14;
    private RectTransform rect16;
    private RectTransform rect18;
    private RectTransform rect112;
    private RectTransform rect116;
    private RectTransform rect124;
    private RectTransform rect132;
    private RectTransform rect148;
    private RectTransform rect164;

    public TMP_Dropdown dropdown;

    private void Awake()
    {
        beat13 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat13");
        beat14 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat14");
        beat16 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat16");
        beat18 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat18");
        beat112 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat112");
        beat116 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat116");
        beat124 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat124");
        beat132 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat132");
        beat148 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat148");
        beat164 = Resources.Load<GameObject>("Prefabs/LevelEditor/Beats/Beat164");
    }

    [System.Obsolete]
    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        rect13 = beat13.GetComponent<RectTransform>();
        rect14 = beat14.GetComponent<RectTransform>();
        rect16 = beat16.GetComponent<RectTransform>();
        rect18 = beat18.GetComponent<RectTransform>();
        rect112 = beat112.GetComponent<RectTransform>();
        rect116 = beat116.GetComponent<RectTransform>();
        rect124 = beat124.GetComponent<RectTransform>();
        rect132 = beat132.GetComponent<RectTransform>();
        rect148 = beat148.GetComponent<RectTransform>();
        rect164 = beat164.GetComponent<RectTransform>();

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        LevelEditorSetter();
    }

    private void LevelEditorSetter()
    {
        canvas.transform.localScale = Vector3.one;

        int beatNum13 = 3;
        float sizeDelta = 640 / beatNum13;
        for (int i = 0; i < 960 / beatNum13; i++)
        {
            GameObject instantiateObject = Instantiate(beat13, new Vector3(0, rect13.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder13.transform);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum13));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 1 Not Founded");
            }
            buttonTransform = instantiateObject.transform.Find("Btn 2");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum13));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 2 Not Founded");
            }
            buttonTransform = instantiateObject.transform.Find("Btn 3");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum13));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 3 Not Founded");
            }
            buttonTransform = instantiateObject.transform.Find("Btn 4");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum13));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 4 Not Founded");
            }
        }

        int beatNum14 = 4;
        sizeDelta = 640 / beatNum14;
        for (int i = 0; i < 960 / beatNum14; i++)
        {
            GameObject instantiateObject = Instantiate(beat14, new Vector3(0, rect13.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder14.transform);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum14));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 1 Not Founded");
            }
            buttonTransform = instantiateObject.transform.Find("Btn 2");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum14));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 2 Not Founded");
            }
            buttonTransform = instantiateObject.transform.Find("Btn 3");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum14));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 3 Not Founded");
            }
            buttonTransform = instantiateObject.transform.Find("Btn 4");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum14));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 4 Not Founded");
            }
        }

        int beatNum16 = 6;
        sizeDelta = 640 / beatNum16;
        for (int i = 0; i < 960 / beatNum16; i++)
        {
            GameObject instantiateObject = Instantiate(beat16, new Vector3(0, rect13.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder16.transform);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum16));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 1 Not Founded");
            }
            buttonTransform = instantiateObject.transform.Find("Btn 2");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum16));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 2 Not Founded");
            }
            buttonTransform = instantiateObject.transform.Find("Btn 3");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum16));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 3 Not Founded");
            }
            buttonTransform = instantiateObject.transform.Find("Btn 4");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum16));
                }
                else
                {
                    Debug.LogError("Button Component Not Founded");
                }
            }
            else
            {
                Debug.LogError("Btn 4 Not Founded");
            }
        }

        Debug.Log($"{rect13.position.y} {rect13.sizeDelta.y} {rect13.position.y + rect13.sizeDelta.y}");
        Debug.Log($"{rect14.position.y} {rect14.sizeDelta.y} {rect14.position.y + rect14.sizeDelta.y}");
    }

    private void ButtonClickHandler(int position, int beat)
    {
        Debug.Log($"Position : {position}, Beat : {beat}");
    }

    private void OnDropdownValueChanged(int index)
    {
        string selectedOption = dropdown.options[index].text;
        DropdownHandler(selectedOption);
    }

    public void DropdownHandler(string option)
    {
        if (option == "1/3")
        {
            parentFolder13.SetActive(true);
            parentFolder14.SetActive(false);
            parentFolder16.SetActive(false);
            parentFolder18.SetActive(false);
            parentFolder112.SetActive(false);
            parentFolder116.SetActive(false);
            parentFolder124.SetActive(false);
            parentFolder132.SetActive(false);
            parentFolder148.SetActive(false);
            parentFolder164.SetActive(false);
        }
        if (option == "1/4")
        {
            parentFolder13.SetActive(false);
            parentFolder14.SetActive(true);
            parentFolder16.SetActive(false);
            parentFolder18.SetActive(false);
            parentFolder112.SetActive(false);
            parentFolder116.SetActive(false);
            parentFolder124.SetActive(false);
            parentFolder132.SetActive(false);
            parentFolder148.SetActive(false);
            parentFolder164.SetActive(false);
        }
        if (option == "1/6")
        {
            parentFolder13.SetActive(false);
            parentFolder14.SetActive(false);
            parentFolder16.SetActive(true);
            parentFolder18.SetActive(false);
            parentFolder112.SetActive(false);
            parentFolder116.SetActive(false);
            parentFolder124.SetActive(false);
            parentFolder132.SetActive(false);
            parentFolder148.SetActive(false);
            parentFolder164.SetActive(false);
        }
    }

    

    //public void LoadFromJson()
    //{
    //    string path = Application.persistentDataPath + "/chartData.json";
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);
    //        NoteData loadedData = JsonUtility.FromJson<NoteData>(json);
    //        notes = new List<Note>(loadedData.notes);
    //    }
    //}

}
