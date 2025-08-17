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
        tutorialText.text = "Project-HG�� ���� ���� ȯ���մϴ�!";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "Project-HG�� 4���� ��ư�� ����ϴ� ���� �����Դϴ�.";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "D, F, J, K�� ���� ��Ʈ�� �Է��� �� �ֽ��ϴ�.";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "��Ʈ�� �����δ� �Ϲ�, ƽ, ESC�� �ֽ��ϴ�.";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "�Ϲ� ��Ʈ�� �ش��ϴ� Ű�� ������ ������ ó���� �� �ֽ��ϴ�.";

        yield return new WaitForSeconds(5f);


        tutorialText.text = "ƽ ��Ʈ�� �� ���� ó���ϴ� ��Ʈ�Դϴ�.";

        yield return new WaitForSeconds(5f);


        tutorialText.text = "ESC ��Ʈ�� �ش��ϴ� Ű�� ���� ������ ó���� �� �ֽ��ϴ�.";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "Project-HG�� ��� Ʃ�丮���� �������ϴ�!";

        yield return new WaitForSeconds(5f);

        SceneManager.LoadSceneAsync("Menu");
    }
}
