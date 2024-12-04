using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    EventInstance eventInstance;

    public float sync = 0f;

    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance("event:/umiyurikaiteitan 3");

        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        eventInstance.setVolume(1.0f);

        StartCoroutine(StartSong());
    }

    IEnumerator StartSong()
    {
        yield return new WaitForSeconds(1.0f + sync);
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
