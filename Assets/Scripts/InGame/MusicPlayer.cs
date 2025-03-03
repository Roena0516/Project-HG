using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    EventInstance eventInstance;

    public float sync;

    public string eventName;

    private MenuManager menu;

    [System.Obsolete]
    void Start()
    {
        menu = FindObjectOfType<MenuManager>();

        sync = menu.sync + 2f - 0.1f + 0.7f;

        StartCoroutine(StartSong());
    }

    IEnumerator StartSong()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        eventName = menu.eventName;
        eventInstance = RuntimeManager.CreateInstance($"event:/{eventName}");

        Debug.Log($"2{eventName}");

        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        eventInstance.setVolume(0.5f);

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
