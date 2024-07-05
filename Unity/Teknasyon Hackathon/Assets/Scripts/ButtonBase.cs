using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonBase : MonoBehaviour
{
    protected Button m_Button;

#if UNITY_EDITOR
    public virtual void OnValidate()
    {
        m_Button = GetComponent<Button>();
    }
#endif

    private void Awake()
    {
        m_Button.onClick.AddListener(PlayTween);
    }

    private void PlayTween()
    {
        var parent = transform.parent;
        
        var sequence = DOTween.Sequence()
            .Append(parent.DOScale(new Vector3(0.35f, 0.35f, 0.35f), 0.15f).SetEase(Ease.OutBack))
            .Append(parent.DOScale(new Vector3(1f, 1f, 1f), 0.15f).SetEase(Ease.OutBack));
    }
}