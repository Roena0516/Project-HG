using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsAnimation : MonoBehaviour
{
    [SerializeField] private Fader _fader;

    private void Awake()
    {
        _fader.fadeImage.color = _fader.fadeImage.color.SetAlpha(1f);
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
