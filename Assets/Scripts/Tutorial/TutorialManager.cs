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
        tutorialText.text = "Project-HG�� ���� ���� ȯ���մϴ�!";

        yield break;
    }
}
