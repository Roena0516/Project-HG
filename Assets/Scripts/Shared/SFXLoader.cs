using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SFXLoader : MonoBehaviour
{
    public static SFXLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(string fileName)
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
        PlaySFXInStandAlone(fileName);
#elif UNITY_WEBGL
        StartCoroutine(PlaySFXInWebGL(fileName));
#endif
    }

    private void PlaySFXInStandAlone(string fileName)
    {
        string path = $"{Application.streamingAssetsPath}/SFX/{fileName}";

        // FMOD에서 직접 경로를 통해 재생
        FMOD.RESULT result = RuntimeManager.CoreSystem.createSound(
            path,
            FMOD.MODE.DEFAULT,
            out FMOD.Sound sound
        );

        if (result == FMOD.RESULT.OK)
        {
            sound.setMode(FMOD.MODE.LOOP_OFF);
            RuntimeManager.CoreSystem.playSound(sound, default(FMOD.ChannelGroup), false, out _);
        }
        else
        {
            Debug.LogError($"Failed to load sound: {path}");
        }
    }

    private IEnumerator PlaySFXInWebGL(string fileName)
    {
        string path = $"{Application.streamingAssetsPath}/SFX/{fileName}";
        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[WebGL] Failed to load sound: {req.error}");
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
