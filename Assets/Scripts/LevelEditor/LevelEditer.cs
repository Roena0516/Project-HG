using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
//using SFB;

public class LevelEditer : MonoBehaviour
{

    private SaveManager saveManager;

    public GameObject normalPrefab;
    public GameObject holdPrefab;
    public GameObject upPrefab;

    public GameObject notesFolder;
    public GameObject gridFolder;

    public GameObject addIndicator;
    public GameObject removeIndicator;

    public int madi;
    public int madi2;

    private bool isRemoving;

    private float scrollSpeed;

    public float BPM;
    public string artist;
    public string title;
    public string fileName;
    public string eventName;

    //private string[] paths;

    public string noteType;

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

    public List<GameObject> beats13;
    public List<GameObject> beats14;
    public List<GameObject> beats16;
    public List<GameObject> beats18;
    public List<GameObject> beats112;
    public List<GameObject> beats116;
    public List<GameObject> beats124;
    public List<GameObject> beats132;

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

        noteType = "Normal";

        isRemoving = false;

        scrollSpeed = 10f;

        madi = 192;
        madi2 = 288;

        LevelEditorSetter();
    }

    private void LevelEditorSetter()
    {
        canvas.transform.localScale = Vector3.one;

        int beatNum13 = 3;
        float sizeDelta = 640f / (float)beatNum13;
        for (int i = 0; i < madi * beatNum13; i++)
        {
            GameObject instantiateObject = Instantiate(beat13, new Vector3(0, rect13.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder13.transform);
            beats13.Add(instantiateObject);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum13, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum13, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum13, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum13, instantiateObject.transform));
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
        sizeDelta = 640f / (float)beatNum14;
        for (int i = 0; i < madi * beatNum14; i++)
        {
            GameObject instantiateObject = Instantiate(beat14, new Vector3(0, rect14.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder14.transform);
            beats14.Add(instantiateObject);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum14, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum14, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum14, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum14, instantiateObject.transform));
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
        sizeDelta = 640f / (float)beatNum16;
        for (int i = 0; i < madi * beatNum16; i++)
        {
            GameObject instantiateObject = Instantiate(beat16, new Vector3(0, rect16.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder16.transform);
            beats16.Add(instantiateObject);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum16, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum16, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum16, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum16, instantiateObject.transform));
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

        int beatNum18 = 8;
        sizeDelta = 640f / (float)beatNum18;
        for (int i = 0; i < madi * beatNum18; i++)
        {
            GameObject instantiateObject = Instantiate(beat18, new Vector3(0, rect18.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder18.transform);
            beats18.Add(instantiateObject);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum18, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum18, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum18, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum18, instantiateObject.transform));
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

        int beatNum112 = 12;
        sizeDelta = 640f / (float)beatNum112;
        for (int i = 0; i < madi * beatNum112; i++)
        {
            GameObject instantiateObject = Instantiate(beat112, new Vector3(0, rect112.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder112.transform);
            beats112.Add(instantiateObject);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum112, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum112, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum112, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum112, instantiateObject.transform));
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

        int beatNum116 = 16;
        sizeDelta = 640f / (float)beatNum116;
        for (int i = 0; i < madi * beatNum116; i++)
        {
            GameObject instantiateObject = Instantiate(beat116, new Vector3(0, rect116.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder116.transform);
            beats116.Add(instantiateObject);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum116, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum116, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum116, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum116, instantiateObject.transform));
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

        int beatNum124 = 24;
        sizeDelta = 640f / (float)beatNum124;
        for (int i = 0; i < madi * beatNum124; i++)
        {
            GameObject instantiateObject = Instantiate(beat16, new Vector3(0, rect124.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder124.transform);
            beats124.Add(instantiateObject);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum124, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum124, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum124, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum124, instantiateObject.transform));
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

        int beatNum132 = 32;
        sizeDelta = 640f / (float)beatNum132;
        for (int i = 0; i < madi * beatNum132; i++)
        {
            GameObject instantiateObject = Instantiate(beat132, new Vector3(0, rect132.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder132.transform);
            beats132.Add(instantiateObject);
            Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
            if (buttonTransform != null)
            {
                Button buttonComponent = buttonTransform.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum132, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum132, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum132, instantiateObject.transform));
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
                    buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum132, instantiateObject.transform));
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

        //int beatNum148 = 48;
        //sizeDelta = 640 / beatNum148;
        //for (int i = 0; i < madi * beatNum148; i++)
        //{
        //    GameObject instantiateObject = Instantiate(beat148, new Vector3(0, rect148.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder148.transform);
        //    Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
        //    if (buttonTransform != null)
        //    {
        //        Button buttonComponent = buttonTransform.GetComponent<Button>();
        //        if (buttonComponent != null)
        //        {
        //            buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum148, instantiateObject.transform));
        //        }
        //        else
        //        {
        //            Debug.LogError("Button Component Not Founded");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Btn 1 Not Founded");
        //    }
        //    buttonTransform = instantiateObject.transform.Find("Btn 2");
        //    if (buttonTransform != null)
        //    {
        //        Button buttonComponent = buttonTransform.GetComponent<Button>();
        //        if (buttonComponent != null)
        //        {
        //            buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum148, instantiateObject.transform));
        //        }
        //        else
        //        {
        //            Debug.LogError("Button Component Not Founded");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Btn 2 Not Founded");
        //    }
        //    buttonTransform = instantiateObject.transform.Find("Btn 3");
        //    if (buttonTransform != null)
        //    {
        //        Button buttonComponent = buttonTransform.GetComponent<Button>();
        //        if (buttonComponent != null)
        //        {
        //            buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum148, instantiateObject.transform));
        //        }
        //        else
        //        {
        //            Debug.LogError("Button Component Not Founded");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Btn 3 Not Founded");
        //    }
        //    buttonTransform = instantiateObject.transform.Find("Btn 4");
        //    if (buttonTransform != null)
        //    {
        //        Button buttonComponent = buttonTransform.GetComponent<Button>();
        //        if (buttonComponent != null)
        //        {
        //            buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum148, instantiateObject.transform));
        //        }
        //        else
        //        {
        //            Debug.LogError("Button Component Not Founded");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Btn 4 Not Founded");
        //    }
        //}

        //int beatNum164 = 64;
        //sizeDelta = 640 / beatNum164;
        //for (int i = 0; i < madi * beatNum164; i++)
        //{
        //    GameObject instantiateObject = Instantiate(beat164, new Vector3(0, rect164.position.y + (sizeDelta * i), 0), Quaternion.identity, parentFolder164.transform);
        //    Transform buttonTransform = instantiateObject.transform.Find("Btn 1");
        //    if (buttonTransform != null)
        //    {
        //        Button buttonComponent = buttonTransform.GetComponent<Button>();
        //        if (buttonComponent != null)
        //        {
        //            buttonComponent.onClick.AddListener(() => ButtonClickHandler(1, beatNum164, instantiateObject.transform));
        //        }
        //        else
        //        {
        //            Debug.LogError("Button Component Not Founded");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Btn 1 Not Founded");
        //    }
        //    buttonTransform = instantiateObject.transform.Find("Btn 2");
        //    if (buttonTransform != null)
        //    {
        //        Button buttonComponent = buttonTransform.GetComponent<Button>();
        //        if (buttonComponent != null)
        //        {
        //            buttonComponent.onClick.AddListener(() => ButtonClickHandler(2, beatNum164, instantiateObject.transform));
        //        }
        //        else
        //        {
        //            Debug.LogError("Button Component Not Founded");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Btn 2 Not Founded");
        //    }
        //    buttonTransform = instantiateObject.transform.Find("Btn 3");
        //    if (buttonTransform != null)
        //    {
        //        Button buttonComponent = buttonTransform.GetComponent<Button>();
        //        if (buttonComponent != null)
        //        {
        //            buttonComponent.onClick.AddListener(() => ButtonClickHandler(3, beatNum164, instantiateObject.transform));
        //        }
        //        else
        //        {
        //            Debug.LogError("Button Component Not Founded");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Btn 3 Not Founded");
        //    }
        //    buttonTransform = instantiateObject.transform.Find("Btn 4");
        //    if (buttonTransform != null)
        //    {
        //        Button buttonComponent = buttonTransform.GetComponent<Button>();
        //        if (buttonComponent != null)
        //        {
        //            buttonComponent.onClick.AddListener(() => ButtonClickHandler(4, beatNum164, instantiateObject.transform));
        //        }
        //        else
        //        {
        //            Debug.LogError("Button Component Not Founded");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Btn 4 Not Founded");
        //    }
        //}

        //Debug.Log($"{rect13.position.y} {rect13.sizeDelta.y} {rect13.position.y + rect13.sizeDelta.y}");
        //Debug.Log($"{rect14.position.y} {rect14.sizeDelta.y} {rect14.position.y + rect14.sizeDelta.y}");
    }

    private void ButtonClickHandler(int position, int beat, Transform buttonT)
    {
        canvas.transform.localScale = Vector3.one;

        float realBeat = 0f;

        if (beat == 3)
        {
            int index = beats13.IndexOf(buttonT.gameObject);
            if (index != -1)
            {
                realBeat = (2f / (float)beat) * (index);
            }
        }
        if (beat == 4)
        {
            int index = beats14.IndexOf(buttonT.gameObject);
            if (index != -1)
            {
                realBeat = (2f / (float)beat) * (index);
            }
        }
        if (beat == 6)
        {
            int index = beats16.IndexOf(buttonT.gameObject);
            if (index != -1)
            {
                realBeat = (2f / (float)beat) * (index);
            }
        }
        if (beat == 8)
        {
            int index = beats18.IndexOf(buttonT.gameObject);
            if (index != -1)
            {
                realBeat = (2f / (float)beat) * (index);
            }
        }
        if (beat == 12)
        {
            int index = beats112.IndexOf(buttonT.gameObject);
            if (index != -1)
            {
                realBeat = (2f / (float)beat) * (index);
            }
        }
        if (beat == 16)
        {
            int index = beats116.IndexOf(buttonT.gameObject);
            if (index != -1)
            {
                realBeat = (2f / (float)beat) * (index);
            }
        }
        if (beat == 24)
        {
            int index = beats124.IndexOf(buttonT.gameObject);
            if (index != -1)
            {
                realBeat = (2f / (float)beat) * (index);
            }
        }
        if (beat == 32)
        {
            int index = beats132.IndexOf(buttonT.gameObject);
            if (index != -1)
            {
                realBeat = (2f / (float)beat) * (index);
            }
        }

        Debug.Log($"Position : {position}, Beat : {realBeat} 1/{beat}, Type : {noteType}");

        if (isRemoving)
        {
            saveManager.notes.Remove(saveManager.notes.Find(note => note.beat == realBeat && note.position == position));
            foreach (Transform child in notesFolder.transform)
            {
                LevelEditerNoteManager levelEditerNoteManager = child.GetComponent<LevelEditerNoteManager>();
                if (levelEditerNoteManager.noteClass.beat == realBeat && levelEditerNoteManager.noteClass.position == position)
                {
                    Destroy(child.gameObject);
                }
            }
            return;
        }

        float positionX = 0f;
        if (position == 1)
        {
            positionX = -158f;
        }
        if (position == 2)
        {
            positionX = 158f / 3f * -1f;
        }
        if (position == 3f)
        {
            positionX = 158f / 3f;
        }
        if (position == 4f)
        {
            positionX = 158f;
        }

        float positionY = buttonT.position.y;

        //Debug.Log(positionY);

        if (noteType == "Normal")
        {
            GameObject instantiateObject = Instantiate(normalPrefab, new Vector3(positionX, positionY, 0f), Quaternion.identity, notesFolder.transform);
            LevelEditerNoteManager levelEditerNoteManager = instantiateObject.GetComponent<LevelEditerNoteManager>();
            levelEditerNoteManager.noteClass.position = position;
            levelEditerNoteManager.noteClass.beat = realBeat;
            levelEditerNoteManager.noteClass.type = "normal";
            saveManager.notes.Add(levelEditerNoteManager.noteClass);
        }
        if (noteType == "Hold")
        {
            GameObject instantiateObject = Instantiate(holdPrefab, new Vector3(positionX, positionY, 0f), Quaternion.identity, notesFolder.transform);
            LevelEditerNoteManager levelEditerNoteManager = instantiateObject.GetComponent<LevelEditerNoteManager>();
            levelEditerNoteManager.noteClass.position = position;
            levelEditerNoteManager.noteClass.beat = realBeat;
            levelEditerNoteManager.noteClass.type = "hold";
            saveManager.notes.Add(levelEditerNoteManager.noteClass);
        }
        if (noteType == "Up")
        {
            GameObject instantiateObject = Instantiate(upPrefab, new Vector3(positionX, positionY, 0f), Quaternion.identity, notesFolder.transform);
            LevelEditerNoteManager levelEditerNoteManager = instantiateObject.GetComponent<LevelEditerNoteManager>();
            levelEditerNoteManager.noteClass.position = position;
            levelEditerNoteManager.noteClass.beat = realBeat;
            levelEditerNoteManager.noteClass.type = "up";
            saveManager.notes.Add(levelEditerNoteManager.noteClass);
        }
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
        if (option == "1/8")
        {
            parentFolder13.SetActive(false);
            parentFolder14.SetActive(false);
            parentFolder16.SetActive(false);
            parentFolder18.SetActive(true);
            parentFolder112.SetActive(false);
            parentFolder116.SetActive(false);
            parentFolder124.SetActive(false);
            parentFolder132.SetActive(false);
            parentFolder148.SetActive(false);
            parentFolder164.SetActive(false);
        }
        if (option == "1/12")
        {
            parentFolder13.SetActive(false);
            parentFolder14.SetActive(false);
            parentFolder16.SetActive(false);
            parentFolder18.SetActive(false);
            parentFolder112.SetActive(true);
            parentFolder116.SetActive(false);
            parentFolder124.SetActive(false);
            parentFolder132.SetActive(false);
            parentFolder148.SetActive(false);
            parentFolder164.SetActive(false);
        }
        if (option == "1/16")
        {
            parentFolder13.SetActive(false);
            parentFolder14.SetActive(false);
            parentFolder16.SetActive(false);
            parentFolder18.SetActive(false);
            parentFolder112.SetActive(false);
            parentFolder116.SetActive(true);
            parentFolder124.SetActive(false);
            parentFolder132.SetActive(false);
            parentFolder148.SetActive(false);
            parentFolder164.SetActive(false);
        }
        if (option == "1/24")
        {
            parentFolder13.SetActive(false);
            parentFolder14.SetActive(false);
            parentFolder16.SetActive(false);
            parentFolder18.SetActive(false);
            parentFolder112.SetActive(false);
            parentFolder116.SetActive(false);
            parentFolder124.SetActive(true);
            parentFolder132.SetActive(false);
            parentFolder148.SetActive(false);
            parentFolder164.SetActive(false);
        }
        if (option == "1/32")
        {
            parentFolder13.SetActive(false);
            parentFolder14.SetActive(false);
            parentFolder16.SetActive(false);
            parentFolder18.SetActive(false);
            parentFolder112.SetActive(false);
            parentFolder116.SetActive(false);
            parentFolder124.SetActive(false);
            parentFolder132.SetActive(true);
            parentFolder148.SetActive(false);
            parentFolder164.SetActive(false);
        }
        if (option == "1/48")
        {
            parentFolder13.SetActive(false);
            parentFolder14.SetActive(false);
            parentFolder16.SetActive(false);
            parentFolder18.SetActive(false);
            parentFolder112.SetActive(false);
            parentFolder116.SetActive(false);
            parentFolder124.SetActive(false);
            parentFolder132.SetActive(false);
            parentFolder148.SetActive(true);
            parentFolder164.SetActive(false);
        }
        if (option == "1/64")
        {
            parentFolder13.SetActive(false);
            parentFolder14.SetActive(false);
            parentFolder16.SetActive(false);
            parentFolder18.SetActive(false);
            parentFolder112.SetActive(false);
            parentFolder116.SetActive(false);
            parentFolder124.SetActive(false);
            parentFolder132.SetActive(false);
            parentFolder148.SetActive(false);
            parentFolder164.SetActive(true);
        }

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.E))
            {
                gridFolder.transform.Translate(2f * scrollSpeed * Time.deltaTime * Vector2.down);
            }
            else
            {
                gridFolder.transform.Translate(scrollSpeed * Time.deltaTime * Vector2.down);
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.E))
            {
                gridFolder.transform.Translate(2f * scrollSpeed * Time.deltaTime * Vector2.up);
            }
            else
            {
                gridFolder.transform.Translate(scrollSpeed * Time.deltaTime * Vector2.up);
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SaveLevel();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetIsRemoving("Add");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetIsRemoving("Remove");
        }
    }

    public void SaveLevel()
    {
        saveManager.SaveToJson(Path.Combine(Application.streamingAssetsPath, $"{artist}-{title}.roena"), BPM, artist, title, eventName);
    }

    public void LoadLevel()
    {
        LoadFromJson(Path.Combine(Application.streamingAssetsPath, $"{fileName}.roena"));
    }

    private void LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            string decrypted = EncryptionHelper.Decrypt(json);

            NotesContainer container = JsonUtility.FromJson<NotesContainer>(decrypted);

            saveManager.notes = container.notes;

            Debug.Log("Chart loaded successfully!");

            PlaceNotesFromLoadedFile();
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }
    }

    private void PlaceNotesFromLoadedFile()
    {
        foreach (NoteClass note in saveManager.notes)
        {
            canvas.transform.localScale = Vector3.one;

            float positionX = 0f;
            if (note.position == 1)
            {
                positionX = -158f;
            }
            if (note.position == 2)
            {
                positionX = 158f / 3f * -1f;
            }
            if (note.position == 3f)
            {
                positionX = 158f / 3f;
            }
            if (note.position == 4f)
            {
                positionX = 158f;
            }

            float positionY = -211f + (320f * note.beat);

            if (note.type == "normal")
            {
                GameObject instantiateObject = Instantiate(normalPrefab, new Vector3(positionX, positionY, 0f), Quaternion.identity, notesFolder.transform);
                LevelEditerNoteManager levelEditerNoteManager = instantiateObject.GetComponent<LevelEditerNoteManager>();
                levelEditerNoteManager.noteClass.position = note.position;
                levelEditerNoteManager.noteClass.beat = note.beat;
                levelEditerNoteManager.noteClass.type = "normal";
            }
            if (note.type == "hold")
            {
                GameObject instantiateObject = Instantiate(holdPrefab, new Vector3(positionX, positionY, 0f), Quaternion.identity, notesFolder.transform);
                LevelEditerNoteManager levelEditerNoteManager = instantiateObject.GetComponent<LevelEditerNoteManager>();
                levelEditerNoteManager.noteClass.position = note.position;
                levelEditerNoteManager.noteClass.beat = note.beat;
                levelEditerNoteManager.noteClass.type = "hold";
            }
            if (note.type == "up")
            {
                GameObject instantiateObject = Instantiate(upPrefab, new Vector3(positionX, positionY, 0f), Quaternion.identity, notesFolder.transform);
                LevelEditerNoteManager levelEditerNoteManager = instantiateObject.GetComponent<LevelEditerNoteManager>();
                levelEditerNoteManager.noteClass.position = note.position;
                levelEditerNoteManager.noteClass.beat = note.beat;
                levelEditerNoteManager.noteClass.type = "up";
            }
        }
    }

    [System.Serializable]
    private class NotesContainer
    {
        public SongInfoClass info;
        public List<NoteClass> notes;
    }

    public void SetFileName(string inputed)
    {
        fileName = inputed;
    }

    public void SetBPM(string inputed)
    {
        float.TryParse(inputed, out BPM);
    }

    public void SetNoteType(string type)
    {
        noteType = type;
    }

    public void SetArtist(string inputed)
    {
        artist = inputed;
    }

    public void SetTitle(string inputed)
    {
        title = inputed;
    }

    public void SetEventName(string inputed)
    {
        eventName = inputed;
    }

    //public void SetFilePath()
    //{
    //    paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);
    //    if (paths.Length > 0)
    //    {
    //        Debug.Log(paths[0]);
    //    }
    //}

    public void SetIsRemoving(string inputed)
    {
        if (inputed == "Remove")
        {
            isRemoving = true;
            removeIndicator.SetActive(true);
            addIndicator.SetActive(false);
        }

        if (inputed == "Add")
        {
            isRemoving = false;
            removeIndicator.SetActive(false);
            addIndicator.SetActive(true);
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
