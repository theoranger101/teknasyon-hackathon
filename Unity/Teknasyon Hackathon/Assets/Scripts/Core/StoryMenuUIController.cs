using System.Collections.Generic;
using System.Linq;
using Core.EventImplementations;
using Events;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class StoryMenuUIController : CanvasBase
    {
        [SerializeField]
        private List<Button> m_Buttons = new();

        [SerializeField]
        private Toggle m_FreeToggle;

        private GeneralSettings s_GeneralSettings;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_Buttons = GetComponentsInChildren<Button>().ToList();
        }
#endif

        private void Awake()
        {
            s_GeneralSettings = GeneralSettings.Get();

            GEM.AddListener<StoryEvent>(OnStoryCompleted, channel: (int)StoryEventType.StoryCompleted);

            for (var i = 0; i < m_Buttons.Count; i++)
            {
                var index = i;
                m_Buttons[i].onClick.AddListener(() => OnButtonSelected(index));
            }

            m_FreeToggle.onValueChanged.AddListener(OnToggleValueChange);
        }

        private void OnButtonSelected(int index)
        {
            using var evt = StoryEvent.Get(index).SendGlobal((int)StoryEventType.StorySelected);
            m_CanvasGroup.Toggle(false);
        }

        private void OnStoryCompleted(StoryEvent evt)
        {
            m_CanvasGroup.Toggle(true);
        }

        private void OnToggleValueChange(bool isOn)
        {
            s_GeneralSettings.FreeVersion = isOn;
        }
    }
}