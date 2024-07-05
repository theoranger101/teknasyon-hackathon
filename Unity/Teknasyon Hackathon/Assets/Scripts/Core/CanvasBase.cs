using UnityEngine;

namespace Core
{
    public class CanvasBase : MonoBehaviour
    {
        protected CanvasGroup m_CanvasGroup;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }
#endif
    }
}