using System;
using UnityEngine;

public class InGameAnimation : MonoBehaviour
{
    [SerializeField] private Fader _fader;

    public void FadeIn(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeIn(duration, onComplete);
    }

    public void FadeOut(float duration = 0.5f, Action onComplete = null)
    {
        _fader.FadeOut(duration, onComplete);
    }
}
