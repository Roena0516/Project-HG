using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    EventInstance eventInstance;

    public float sync;

    public string eventName;

    private SettingsManager settings;
    private LevelEditer levelEditer;
    public GameManager gameManager;

    public static MusicPlayer Instance { get; private set; }

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

    void Start()
    {
        settings = SettingsManager.Instance;

        sync = (settings.sync / 1000f) + 2.6f; // + 2f - 0.1f + 0.7f

        StartCoroutine(StartSong());
    }

    IEnumerator StartSong()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        int timeLinePosition = 0;
        if (!gameManager.isTest)
        {
            eventName = settings.eventName;
        }
        else
        {
            levelEditer = LevelEditer.Instance;
            eventName = levelEditer.eventName;
            timeLinePosition = levelEditer.currentMusicTime;
        }
        eventInstance = RuntimeManager.CreateInstance($"event:/{eventName}");

        Debug.Log($"2{eventName}");

        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        eventInstance.setVolume(0.5f);
        eventInstance.setTimelinePosition(timeLinePosition);

        yield return new WaitForSecondsRealtime(sync);
        eventInstance.start();
    }

    void Update()
    {
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
    }

    void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}
