using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu]
    public class StoryData : ScriptableObject
    {
        public string Prompt;
        public List<Sentence> Sentences = new();

        public List<BlankSpace> TotalBlankSpaces = new();

        private void OnValidate()
        {
            for (var i = 0; i < Sentences.Count; i++)
            {
                Sentences[i].CreateParts();
            }
        }
    }
}