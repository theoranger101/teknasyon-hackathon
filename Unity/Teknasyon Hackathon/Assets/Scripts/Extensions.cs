using DG.Tweening;
using UnityEngine;

public static class Extensions
{
    public static void Toggle(this CanvasGroup canvasGroup, bool visible)
    {
        canvasGroup.DOFade(visible ? 1f : 0f, 1f).SetDelay(0.5f);
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}