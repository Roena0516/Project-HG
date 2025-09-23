using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLoading : MonoBehaviour
{
    private SettingsManager settingsManager;

    [SerializeField] private GetUser getUser;

    private string baseUrl = "https://prod.windeath44.wiki/api";
    private string accessToken;

    private async void Start()
    {
        Player player = GetUser();
        if (player == null)
        {
            return;
        }

        accessToken = player.accessToken;

        GetMyRatingResponse myRating = await getUser.GetUserRatingAPI(baseUrl, accessToken, onSuccess: (res) =>
        {
            Debug.Log($"get rating: {res.data.rating} ranking: {res.data.ranking}");
        }, onError: (err) =>
        {
            Debug.LogWarning("get rating failed: " + err);
        });

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

        settingsManager = SettingsManager.Instance;
        settingsManager.LoadSettings(); // ???? ???? ????
        settingsManager.SetPlayerData(player);

        DontDestroyOnLoad(settingsManager.gameObject);

        if (settingsManager.settings.isFirstStart) // ?? ?????? ?? ???????? ????
        {
            settingsManager.SetFirstStart(false); // ???? ?? ?????? ????

            StartTutorial(); // ???????? ????
        }
        else // ?????? ???????? ????
        {
            SceneManager.LoadSceneAsync("Menu"); // ???? ?????? ????
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
