using System.Collections.Generic;
using Core.EventImplementations;
using Events;
using UnityEngine;

namespace Core
{
    public class StoryManager : MonoBehaviour
    {
        public List<StoryData> StoryDatas = new();
        public Story Story;

        private void Awake()
        {
            GEM.AddListener<StoryEvent>(OnStorySelected, channel: (int)StoryEventType.StorySelected);
            GEM.AddListener<StoryEvent>(OnStoryCompleted, channel: (int)StoryEventType.StoryCompleted);
        }
        
        private void OnStorySelected(StoryEvent evt)
        {
            var story = StoryDatas[evt.Index];
            Story.SetStory(story);
        }
        
        private void OnStoryCompleted(StoryEvent evt)
        {
            
        }
    }
}