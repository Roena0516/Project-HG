using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using FMODUnity;

public class CompleteLoading : MonoBehaviour
{
    private SettingsManager settingsManager;

    [SerializeField] private GetUser getUser;

    private string baseUrl = "https://prod.windeath44.wiki/api";
    private string accessToken;

    private async void Start()
    {
#if UNITY_STANDALONE || UNITY_EDITOR  
        Player player = GetUser();
        if (player == null)
        {
            return;
        }
#elif UNITY_WEBGL
        Player player = await GetUserInWebGL();
#endif

        accessToken = player.accessToken;

        GetMyRatingResponse myRating = await getUser.GetUserRatingAPI(baseUrl, accessToken, onSuccess: (res) =>
        {
            Debug.Log($"get rating: {res.data.rating} ranking: {res.data.ranking}");
        }, onError: (err) =>
        {
            Debug.LogWarning("get rating failed: " + err);
        });

        if (myRating != null)
        {
            player = new()
            {
                id = myRating.playerId,
                accessToken = accessToken,
                playerName = myRating.playerId,
                rating = myRating.rating,
                ranking = myRating.ranking,
                createdAt = myRating.createdAt,
                updatedAt = myRating.updatedAt
            };
        }

        settingsManager = SettingsManager.Instance;
#if UNITY_STANDALONE || UNITY_EDITOR
        settingsManager.LoadSettings();
#else
        await settingsManager.LoadSettingsInWebGL();
#endif
        settingsManager.SetPlayerData(player);

        DontDestroyOnLoad(settingsManager.gameObject);

        if (settingsManager.settings.isFirstStart)
        {
            settingsManager.SetFirstStart(false);

            StartTutorial();
        }
        else
        {
            await SceneManager.LoadSceneAsync("Menu");
        }
    }

    private Player GetUser()
    {
        string[] userDataFile = Directory.GetFiles(Application.streamingAssetsPath, "userData.json");

        if (userDataFile.Length == 0)
        {
            Debug.LogError("User Data is not found");
            return null;
        }

        string json = File.ReadAllText(userDataFile[0]);

        Player foundPlayer = JsonUtility.FromJson<Player>(json);

        return foundPlayer;
    }

    private async Task<Player> GetUserInWebGL()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "userData.json");

        using (UnityWebRequest req = UnityWebRequest.Get(path))
        {
            var op = req.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[WebGL] Failed to load userData.json: {req.error}");
                return null;
            }

            string json = req.downloadHandler.text;
            Player foundPlayer = null;
            try
            {
                foundPlayer = JsonUtility.FromJson<Player>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[WebGL] JSON Parse Error: {e.Message}");
            }

            return foundPlayer;
        }
    }

    private void StartTutorial()
    {
        string[] directory = Directory.GetDirectories(Application.streamingAssetsPath, "system"); // system ???? ????????
        if (directory.Length == 0)
        {
            Debug.LogWarning($"system ?????? ???? ??????????.");
        }

        string[] jsonFiles = Directory.GetFiles(directory[0], "tutorial.roena"); // ???????? ???? ???? ????????
        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning($"???????? ?????? ???????? ????????.");
        }

        settingsManager.SetFileName(jsonFiles[0]); // LoadManager???? ?????? ???? ????
        SceneManager.LoadSceneAsync("Tutorial"); // ???????? ?????? ????
    }
}
