using System.Collections;
using System.Collections.Generic;
using Core.EventImplementations;
using DG.Tweening;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class StoryUIController : CanvasBase
    {
        [SerializeField]
        private TextMeshProUGUI m_StoryText;

        [SerializeField]
        private TMP_InputField m_InputField;

        [SerializeField]
        private RectTransform m_OptionsPanel;

        [SerializeField]
        private List<Button> m_OptionButtons = new();

        [SerializeField]
        private RawImage m_RawImage;

        private GeneralSettings S_GeneralSettings;

        private void Awake()
        {
            S_GeneralSettings = GeneralSettings.Get();

            m_StoryText.text = "";

            m_InputField.text = "";
            m_InputField.transform.localScale = new Vector3(0f, 1f, 1f);
            m_InputField.gameObject.SetActive(false);

            m_InputField.onEndEdit.AddListener(OnEndInputEdit);

            m_OptionsPanel.transform.localScale = Vector3.zero;
            m_OptionsPanel.gameObject.SetActive(false);

            for (var i = 0; i < m_OptionButtons.Count; i++)
            {
                var index = i;
                m_OptionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
            }

            GEM.AddListener<StoryEvent>(OnStorySelected, channel: (int)StoryEventType.StorySelected);
            GEM.AddListener<StoryEvent>(OnGetPart, channel: (int)StoryEventType.GetPart);
            GEM.AddListener<StoryEvent>(OnSentenceEnd, channel: (int)StoryEventType.SentenceEnd);
            GEM.AddListener<StoryEvent>(OnImageCreated, channel: (int)StoryEventType.ImageCreated);
            GEM.AddListener<StoryEvent>(OnStoryCompleted, channel: (int)StoryEventType.StoryCompleted);
        }

        private void OnStorySelected(StoryEvent evt)
        {
            m_CanvasGroup.Toggle(true);
        }

        private void OnStoryCompleted(StoryEvent evt)
        {
            m_CanvasGroup.Toggle(false);

            m_RawImage.texture = null;
            m_StoryText.text = "";
        }

        private void OnGetPart(StoryEvent evt)
        {
            StartCoroutine(TypeWriter(evt.Text, evt.IsLastPart));

            if (evt.IsLastPart)
                return;

            if (S_GeneralSettings.FreeVersion)
            {
                for (var i = 0; i < m_OptionButtons.Count; i++)
                {
                    m_OptionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                        evt.Options[i];
                }
            }
            else
            {
                m_InputField.placeholder.transform.GetComponent<TextMeshProUGUI>().text =
                    evt.WordType.ToString();
            }
        }

        private IEnumerator TypeWriter(string text, bool stopInput)
        {
            for (var i = 0; i < text.Length; i++)
            {
                m_StoryText.text += text[i];
                yield return new WaitForSeconds(0.025f);
            }

            yield return null;

            if (stopInput)
            {
                using var evt = StoryEvent.Get().SendGlobal((int)StoryEventType.BlankSpaceEnter);
                yield break;
            }

            if (S_GeneralSettings.FreeVersion)
            {
                SetOptionsPanelPosition();
            }
            else
            {
                SetInputFieldPosition();
            }
        }

        private void SetOptionsPanelPosition()
        {
            var bottomLeft = m_StoryText.textInfo.characterInfo[^1]
                .bottomLeft;
            var worldBottomLeft = m_StoryText.transform.TransformPoint(bottomLeft);

            m_OptionsPanel.transform.position = new Vector3(worldBottomLeft.x + 20f, worldBottomLeft.y, 0f);
            m_OptionsPanel.gameObject.SetActive(true);
            m_OptionsPanel.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.OutBack);
        }

        private void SetInputFieldPosition()
        {
            var bottomLeft = m_StoryText.textInfo.characterInfo[^1]
                .bottomLeft;
            var worldBottomLeft = m_StoryText.transform.TransformPoint(bottomLeft);

            m_InputField.transform.position = new Vector3(worldBottomLeft.x + 20f, worldBottomLeft.y, 0f);
            StartCoroutine(ToggleInputField(true));
        }

        private IEnumerator ToggleInputField(bool visible)
        {
            m_InputField.text = "";
            m_InputField.gameObject.SetActive(true);

            var duration = 0.2f;
            var startScale = m_InputField.transform.localScale;
            var endScale = new Vector3(visible ? 1f : 0f, startScale.y, startScale.z);

            var timeElapsed = 0f;
            while (timeElapsed < duration)
            {
                var progress = timeElapsed / duration;
                m_InputField.transform.localScale = Vector3.Lerp(startScale, endScale, progress);

                yield return null;

                timeElapsed += Time.deltaTime;
            }

            m_InputField.transform.localScale = endScale;
            m_InputField.gameObject.SetActive(visible);
        }

        private void OnEndInputEdit(string input)
        {
            StartCoroutine(ToggleInputField(false));

            m_StoryText.text += input;
            using var evt = StoryEvent.Get(input).SendGlobal((int)StoryEventType.BlankSpaceEnter);
        }

        private void OnOptionSelected(int index)
        {
            m_OptionsPanel.transform.DOScale(new Vector3(0f, 0f, 0f), 0.2f).SetEase(Ease.OutBack)
                .OnComplete(() => { m_OptionsPanel.gameObject.SetActive(false); });

            var input = m_OptionButtons[index].GetComponentInChildren<TextMeshProUGUI>().text;

            m_StoryText.text += $"<color=#FF0000> {input} </color>";
            using var evt = StoryEvent.Get(input).SendGlobal((int)StoryEventType.BlankSpaceEnter);
        }

        private void OnImageCreated(StoryEvent evt)
        {
            m_RawImage.texture = evt.Image;
            m_RawImage.DOFade(1f, 0.25f);
        }

        private void OnSentenceEnd(StoryEvent evt)
        {
            StartCoroutine(ClearText());
        }

        private IEnumerator ClearText()
        {
            var font = m_StoryText.font;
            var size = m_StoryText.fontSize;
            var go = m_StoryText.gameObject;

            Destroy(m_StoryText);

            yield return null;

            m_StoryText = go.AddComponent<TextMeshProUGUI>();
            m_StoryText.font = font;
            m_StoryText.fontSize = size;
        }
    }
}