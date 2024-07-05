using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace Core
{
    [Serializable]
    public class Sentence
    {
        public string Prompt;
        
        public string Text;
        [ReadOnly]
        public List<string> Parts = new();
        public List<BlankSpace> BlankSpaces = new();

        public void CreateParts()
        {
            Parts = Text.Split("_").ToList();
        }
    }
}