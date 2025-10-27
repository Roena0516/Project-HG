using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Alerter : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Image _alertFade;
    [SerializeField] private GameObject _alertSeparated;

    private RectTransform alert;

    public void SetAlertSeparated(string content, UnityAction onConfirm = null, UnityAction onCancel = null)
    {
        _canvas.transform.localScale = Vector3.one;

        Vector3 position = new(0f, -1400f, 0f);
        Quaternion rotation = new(0f, 0f, 0f, 0f);
        alert = Instantiate(_alertSeparated, position, rotation, _canvas.transform).GetComponent<RectTransform>();
        //alert.DOAnchorPosY(1400f, 0f).SetEase(Ease.OutSine).SetAutoKill(true);

        if (alert)
        {
            Transform containerFolder = alert.gameObject.transform.Find("Container");
            Transform contentFolder = containerFolder.Find("Content");
            Transform text = contentFolder.Find("Text");

            Transform buttonFolder = containerFolder.Find("Button");
            Transform confirmButtonTransform = buttonFolder.Find("Right");
            Transform cancelButtonTransform = buttonFolder.Find("Left");

            TextMeshProUGUI textComponent = text.gameObject.GetComponent<TextMeshProUGUI>();
            textComponent.text = content;

            Button confirmButton = confirmButtonTransform.gameObject.GetComponent<Button>();
            Button cancelButton = cancelButtonTransform.gameObject.GetComponent<Button>();
            confirmButton.onClick.AddListener(onConfirm);
            cancelButton.onClick.AddListener(onCancel);

            alert.DOAnchorPosY(0f, 0.3f).SetEase(Ease.OutSine).SetAutoKill(true);
            _alertFade
                .DOFade(0.4f, 0.3f)
                .SetEase(Ease.OutSine)
                .SetAutoKill(true);
        }
    }

    public void DisableAlert()
    {
        if (alert)
        {
            _canvas.transform.localScale = Vector3.one;

            alert.DOAnchorPosY(1400f, 0.3f).SetEase(Ease.OutSine).SetAutoKill(true);
            _alertFade
                .DOFade(0f, 0.3f)
                .SetEase(Ease.OutSine)
                .OnComplete(() => Destroy(alert.gameObject))
                .SetAutoKill(true);
        }
    }
}
