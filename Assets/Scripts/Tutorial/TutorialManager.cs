using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        yield return new WaitForSeconds(5f);

        tutorialText.text = "Project-HG는 4개의 버튼을 사용하는 리듬 게임입니다.";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "D, F, J, K를 눌러 노트를 입력할 수 있습니다.";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "노트의 종류로는 일반, 틱, ESC가 있습니다.";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "일반 노트는 해당하는 키를 누르는 것으로 처리할 수 있습니다.";

        yield return new WaitForSeconds(5f);


        tutorialText.text = "틱 노트는 꾹 눌러 처리하는 노트입니다.";

        yield return new WaitForSeconds(5f);


        tutorialText.text = "ESC 노트는 해당하는 키를 떼는 것으로 처리할 수 있습니다.";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "Project-HG의 모든 튜토리얼이 끝났습니다!";

        yield return new WaitForSeconds(5f);

        SceneManager.LoadSceneAsync("Menu");
    }
}
