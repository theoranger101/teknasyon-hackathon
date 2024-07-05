using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Core.EventImplementations
{
    public enum StoryEventType
    {
        StorySelected = 0,
        GetPart = 1,
        BlankSpaceEnter = 2,
        StoryCompleted = 3,
        ImageCreated = 4,
        SentenceEnd = 5,
    }

    public class StoryEvent : Event<StoryEvent>
    {
        public int Index;

        public string Text;

        public Texture2D Image;

        public Sentence Sentence;

        public List<string> Options;

        public WordType WordType;

        public bool IsLastPart;

        public static StoryEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }

        public static StoryEvent Get(int index)
        {
            var evt = GetPooledInternal();
            evt.Index = index;
            return evt;
        }

        public static StoryEvent Get(string userInput)
        {
            var evt = GetPooledInternal();
            evt.Text = userInput;
            return evt;
        }

        public static StoryEvent Get(Texture2D image)
        {
            var evt = GetPooledInternal();
            evt.Image = image;
            return evt;
        }

        public static StoryEvent Get(Sentence sentence, int index)
        {
            var evt = GetPooledInternal();
            evt.Sentence = sentence;
            evt.Index = index;

            return evt;
        }

        public static StoryEvent Get(string text, List<string> options, bool isLastPart)
        {
            var evt = GetPooledInternal();
            evt.Text = text;
            evt.Options = options;
            evt.IsLastPart = isLastPart;

            return evt;
        }

        public static StoryEvent Get(string text, WordType wordType, bool isLastPart)
        {
            var evt = GetPooledInternal();
            evt.Text = text;
            evt.WordType = wordType;
            evt.IsLastPart = isLastPart;

            return evt;
        }

        protected override void Reset()
        {
            base.Reset();
            Index = 0;
            Text = "";
            Image = null;
            Sentence = null;
        }
    }
}