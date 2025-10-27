using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class RankingAPI : MonoBehaviour
{
    public async Task<CursorPagePlayerResponse> GetRankingsAPI(
        string baseUrl,
        int size,
        long? cursorRanking,
        Action<ResponseEntity_CursorPagePlayerResponse> onSuccess,
        Action<string> onError,
        int timeOutSec = 15)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            onError?.Invoke("GetRankingsAPI: Base URL is empty");
            return null;
        }

        string url = $"{baseUrl}/game/players/ranking?size={size}";
        if (cursorRanking.HasValue)
        {
            url += $"&cursorRanking={cursorRanking.Value}";
        }

        using (var req = UnityWebRequest.Get(url))
        {
            req.timeout = timeOutSec;

            var op = req.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(req.error);
                return null;
            }

            string json = req.downloadHandler.text;

            ResponseEntity_CursorPagePlayerResponse parsed = null;
            try
            {
                parsed = JsonUtility.FromJson<ResponseEntity_CursorPagePlayerResponse>(json);
            }
            catch (Exception e)
            {
                onError?.Invoke($"JSON parse error: {e.Message}");
                return null;
            }

            if (parsed == null || parsed.data == null)
            {
                onError?.Invoke("Empty response or invalid shape");
                return null;
            }

            onSuccess?.Invoke(parsed);
            return parsed.data;
        }
    }
}
