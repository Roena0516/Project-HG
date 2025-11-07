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
    public LineInputChecker line;

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

        sync = (settings.settings.sync / 1000f) + 0.8f;
    }

    void OnEnable()
    {
        line.OnPlay.AddListener(OnPlayDetected);
    }

    void OnDisable()
    {
        line.OnPlay.RemoveListener(OnPlayDetected);
    }

    IEnumerator StartSong()
    {
        yield return new WaitForSeconds(sync);

        Debug.Log($"song is started, currentTime: {line.currentTime}");
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

    void OnPlayDetected()
    {
        // Play가 실행되었을 때 동작

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

        Debug.Log($"{eventName}, sync: {sync}, currentTime: {line.currentTime}");

        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        eventInstance.setVolume(0.5f * (settings.settings.musicVolume / 10f));
        eventInstance.setTimelinePosition(timeLinePosition);

        StartCoroutine(StartSong());
    }
}
