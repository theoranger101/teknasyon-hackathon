using System.Collections.Generic;

namespace Core
{
    public class PromptGenerator
    {
        public static string ExtractNounsAdjectives(string text)
        {
            string[] words = text.Split(' ');
            List<string> importantWords = new List<string>();

            foreach (string word in words)
            {
                string trimmedWord = word.Trim().ToLower();
                if (IsNoun(trimmedWord) || IsAdjective(trimmedWord))
                {
                    importantWords.Add(trimmedWord);
                }
            }

            return string.Join(" ", importantWords);
        }

        private static bool IsNoun(string word)
        {
            // Replace with your preferred noun part-of-speech (POS) tagger library
            // This is a basic example using string comparisons
            return (word.EndsWith("s") || word.EndsWith("ed") || word.EndsWith("ing")) && word.Length > 3;
        }

        private static bool IsAdjective(string word)
        {
            // Replace with your preferred adjective part-of-speech (POS) tagger library
            // This is a basic example using string comparisons
            return word.Length > 3 && (word.EndsWith("ful") || word.EndsWith("able") || word.EndsWith("ible"));
        }
    }
}