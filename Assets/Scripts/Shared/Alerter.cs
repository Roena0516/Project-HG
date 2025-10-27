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

    private SettingsManager _settings;
    private RectTransform _alert;
    private UnityAction _onConfirm;
    private UnityAction _onCancel;
    private bool _isSet;

    private void Awake()
    {
        _settings = SettingsManager.Instance;
    }

    private void Update()
    {
        if (_isSet)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _onConfirm?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _onCancel?.Invoke();
            }
        }
    }

    public void SetAlertSeparated(string content, UnityAction onConfirm = null, UnityAction onCancel = null)
    {
        _canvas.transform.localScale = Vector3.one;

        _settings.IsBlocked = true;

        Vector3 position = new(0f, -1400f, 0f);
        Quaternion rotation = new(0f, 0f, 0f, 0f);
        _alert = Instantiate(_alertSeparated, position, rotation, _canvas.transform).GetComponent<RectTransform>();
        //alert.DOAnchorPosY(1400f, 0f).SetEase(Ease.OutSine).SetAutoKill(true);

        if (_alert)
        {
            _onConfirm = onConfirm;
            _onCancel = onCancel;

            Transform containerFolder = _alert.gameObject.transform.Find("Container");
            Transform contentFolder = containerFolder.Find("Content");
            Transform text = contentFolder.Find("Text");

            Transform buttonFolder = containerFolder.Find("Button");
            Transform confirmButtonTransform = buttonFolder.Find("Right");
            Transform cancelButtonTransform = buttonFolder.Find("Left");

            TextMeshProUGUI textComponent = text.gameObject.GetComponent<TextMeshProUGUI>();
            textComponent.text = content;

            Button confirmButton = confirmButtonTransform.gameObject.GetComponent<Button>();
            Button cancelButton = cancelButtonTransform.gameObject.GetComponent<Button>();
            confirmButton.onClick.AddListener(_onConfirm);
            cancelButton.onClick.AddListener(_onCancel);

            _alert
                .DOAnchorPosY(0f, 0.3f)
                .SetEase(Ease.OutSine)
                .OnComplete(() => _isSet = true)
                .SetAutoKill(true);
            _alertFade
                .DOFade(0.4f, 0.3f)
                .SetEase(Ease.OutSine)
                .SetAutoKill(true);
        }
    }

    public void DisableAlert()
    {
        _settings.IsBlocked = false;

        if (_alert)
        {
            _canvas.transform.localScale = Vector3.one;

            _alert.DOAnchorPosY(1400f, 0.3f).SetEase(Ease.OutSine).SetAutoKill(true);
            _alertFade
                .DOFade(0f, 0.3f)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    Destroy(_alert.gameObject);
                    _isSet = false;
                })
                .SetAutoKill(true);
        }
    }
}
