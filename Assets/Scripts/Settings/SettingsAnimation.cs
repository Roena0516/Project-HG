using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsAnimation : MonoBehaviour
{
    [SerializeField] private Fader _fader;
    [SerializeField] private Alerter _alerter;

    [SerializeField] private RectTransform _settingsDescriptionPanel;
    [SerializeField] private RectTransform _settingsPanel;

    private void Awake()
    {
        _fader.fadeImage.color = _fader.fadeImage.color.SetAlpha(1f);
    }

    public void SetPanels()
    {
        SetSettingsDescription();
        SetSettings();
    }

    private void SetSettingsDescription()
    {
        _settingsDescriptionPanel.DOAnchorPosX(-216, 0).SetAutoKill(true);
        _settingsDescriptionPanel.DOAnchorPosX(216, 0.5f).SetEase(Ease.OutSine).SetAutoKill(true);
    }

    private void SetSettings()
    {
        _settingsPanel.DOAnchorPosX(552, 0).SetAutoKill(true);
        _settingsPanel.DOAnchorPosX(-552, 0.5f).SetEase(Ease.OutSine).SetAutoKill(true);
    }

    public void SetAlertSeparated(string content, UnityAction onConfirm = null, UnityAction onCancel = null)
    {
        _alerter.SetAlertSeparated(content, onConfirm, onCancel);
    }

    public void DisableAlert()
    {
        _alerter.DisableAlert();
    }

    public void FadeIn(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeIn(duration, onComplete);
    }

    public void FadeOut(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeOut(duration, onComplete);
    }
}
