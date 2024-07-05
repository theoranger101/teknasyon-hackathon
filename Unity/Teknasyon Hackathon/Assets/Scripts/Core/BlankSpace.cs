using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public enum WordType
    {
        Adjective = 0,
        Adverb = 1,
        Noun = 2,
        Pronoun = 3,
        Verb = 4
    }

    [Serializable]
    public class BlankSpace
    {
        public WordType Type;

        [NonSerialized]
        public string Text;

        public List<string> Options = new();
        public List<Sprite> PremadeSprites = new();
    }

    [Serializable]
    public class ConnectedBlankSpace : BlankSpace
    {
        public BlankSpace Connected;
        
    }
}