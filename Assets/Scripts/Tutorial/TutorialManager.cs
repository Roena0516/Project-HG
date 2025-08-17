using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    private void Start()
    {
        StartCoroutine(StartTutorial());
    }

    private IEnumerator StartTutorial()
    {
        tutorialText.text = "Project-HG에 오신 것을 환영합니다!";

        yield break;
    }
}
