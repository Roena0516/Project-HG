using System.Collections;
using System.IO;
using UnityEngine;

public class GetResults : MonoBehaviour
{
    public ResultsContainer GetResultsAPI()
    {
        string[] results = Directory.GetFiles(Application.streamingAssetsPath, "result.json");
        string resultsJson = File.ReadAllText(results[0]);
        ResultsContainer resultsContainer = JsonUtility.FromJson<ResultsContainer>(resultsJson);

        return resultsContainer;
    }

    //public IEnumerator GetResultAPI()
    //{
        
    //}
}
