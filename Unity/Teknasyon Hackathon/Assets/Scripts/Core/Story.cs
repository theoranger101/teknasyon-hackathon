using System.Collections;
using System.Linq;
using Core.EventImplementations;
using Events;
using UnityEngine;
using UnityEngine.Networking;

namespace Core
{
    public class Story : MonoBehaviour
    {
        [SerializeField]
        private StoryData m_Data;

        private int m_CurrentSentenceIndex = 0;
        private int m_CurrentPartIndex = 0;

        private GeneralSettings s_GeneralSettings;

        public void SetStory(StoryData data)
        {
            s_GeneralSettings = GeneralSettings.Get();

            m_Data = data;

            m_Data.TotalBlankSpaces.Clear();
            for (var i = 0; i < m_Data.Sentences.Count; i++)
            {
                m_Data.TotalBlankSpaces = m_Data.TotalBlankSpaces.Concat(m_Data.Sentences[i].BlankSpaces).ToList();
            }

            DissectStory();
            GEM.AddListener<StoryEvent>(OnBlankFilled, channel: (int)StoryEventType.BlankSpaceEnter);
        }

        private void DissectStory()
        {
            var text = string.Format(m_Data.Sentences[m_CurrentSentenceIndex].Parts[m_CurrentPartIndex],
                m_Data.TotalBlankSpaces[0].Text);
            var isLastPart = m_Data.Sentences[m_CurrentSentenceIndex].Parts.Count - 1 <= m_CurrentPartIndex;

            if (s_GeneralSettings.FreeVersion)
            {
                using var evt = StoryEvent.Get(text,
                        !isLastPart
                            ? m_Data.Sentences[m_CurrentSentenceIndex].BlankSpaces[m_CurrentPartIndex].Options
                            : null, isLastPart)
                    .SendGlobal((int)StoryEventType.GetPart);
            }
            else
            {
                using var evt = StoryEvent.Get(text,
                        !isLastPart
                            ? m_Data.Sentences[m_CurrentSentenceIndex].BlankSpaces[m_CurrentPartIndex].Type
                            : WordType.Verb, isLastPart)
                    .SendGlobal((int)StoryEventType.GetPart);
            }
        }

        private void OnBlankFilled(StoryEvent evt)
        {
            if (evt.Text == "")
            {
                StartCoroutine(OnPartComplete());
                return;
            }

            m_Data.Sentences[m_CurrentSentenceIndex].BlankSpaces[m_CurrentPartIndex].Text = evt.Text;

            StartCoroutine(OnPartComplete());
        }

        private IEnumerator OnPartComplete()
        {
            m_CurrentPartIndex++;

            if (m_Data.Sentences[m_CurrentSentenceIndex].Parts.Count <= m_CurrentPartIndex)
            {
                OnSentenceComplete();

                m_CurrentSentenceIndex++;
                m_CurrentPartIndex = 0;

                yield break;
            }

            if (m_CurrentSentenceIndex >= m_Data.Sentences.Count)
            {
                StartCoroutine(WaitAndComplete());
                yield break;
            }

            yield return null;
            DissectStory();
        }

        private void OnSentenceComplete()
        {
            var text = Reconstruct();
            //  text = PromptGenerator.ExtractNounsAdjectives(text);
            StartCoroutine(SendRequest(text));
        }

        IEnumerator SendRequest(string prompt)
        {
            string url = "http://127.0.0.1:5000/generate-image";
            WWWForm form = new WWWForm();
            form.AddField("prompt", prompt);

            UnityWebRequest www = UnityWebRequest.Post(url, form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                byte[] imageBytes = www.downloadHandler.data;
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);

                using var evt1 = StoryEvent.Get(texture).SendGlobal((int)StoryEventType.ImageCreated);
                using var evt2 = StoryEvent.Get().SendGlobal((int)StoryEventType.SentenceEnd);

                yield return null;
                yield return null;
                yield return null;

                if (m_CurrentSentenceIndex >= m_Data.Sentences.Count)
                {
                    StartCoroutine(WaitAndComplete());
                    yield break;
                }

                DissectStory();
            }
        }

        private string Reconstruct()
        {
            var currentSentence = m_Data.Sentences[m_CurrentSentenceIndex];
            var completedSentence = m_Data.Prompt;

            for (var i = 0; i < currentSentence.BlankSpaces.Count; i++)
            {
                completedSentence += currentSentence.Parts[i];
                completedSentence += currentSentence.BlankSpaces[i].Text;
            }

            completedSentence += currentSentence.Parts[^1];

            completedSentence = string.Format(completedSentence, m_Data.TotalBlankSpaces[0].Text);

            completedSentence += currentSentence.Prompt;

            return completedSentence;
        }

        private void OnStoryCompleted()
        {
            using var evt = StoryEvent.Get().SendGlobal((int)StoryEventType.StoryCompleted);

            GEM.RemoveListener<StoryEvent>(OnBlankFilled, channel: (int)StoryEventType.BlankSpaceEnter);
            
            m_Data = null;
            m_CurrentPartIndex = 0;
            m_CurrentSentenceIndex = 0;
        }

        private IEnumerator WaitAndComplete()
        {
            yield return new WaitForSeconds(5f);
            OnStoryCompleted();
        }

        /*
        private void Awake()
        {
            StoryText.text = " ";
            StartCoroutine(DissectData(0));
        }

        public IEnumerator DissectData(int index)
        {
            SubmitButton.onClick.AddListener(OnSubmitBlank);

            m_CurrentSentence = Data.Sentences[index];
            // Debug.Log(sentence.Text);
            StoryText.text += m_CurrentSentence.Text;

            for (var i = 0; i < m_CurrentSentence.BlankSpaces.Count; i++)
            {
                m_Submitted = false;
                m_CurrentBlankSpaceIndex = i;
                FillBlankSpace(i);

                while (!m_Submitted)
                {
                    yield return null;
                }
            }

            Reconstruct();
            StoryText.text = m_CurrentSentence.Text;
        }

        private void FillBlankSpace(int index)
        {
            // Debug.Log($"Enter a {sentence.BlankSpaces[i].Type}: ");
            InputField.text = $"Enter a {m_CurrentSentence.BlankSpaces[index].Type}: ";
        }

        private void OnSubmitBlank()
        {
            if (m_Submitted)
                return;

            m_Submitted = true;
            m_CurrentSentence.BlankSpaces[m_CurrentBlankSpaceIndex].Text = InputField.text;
            InputField.text = " ";
        }

        private void Reconstruct()
        {
            // Define the placeholder
            string placeholder = "__";
            int placeholderIndex = 0;

            while (m_CurrentSentence.Text.Contains(placeholder) &&
                   placeholderIndex < m_CurrentSentence.BlankSpaces.Count)
            {
                m_CurrentSentence.Text = m_CurrentSentence.Text.Replace(placeholder,
                    m_CurrentSentence.BlankSpaces[placeholderIndex].Text, StringComparison.Ordinal);
                placeholderIndex++;
            }
        }
        */
    }
}