using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using TextPortCore.Models;
using TextPortCore.Data;

namespace TextPortCore.Components.Censor
{
    public class Censor
    {
        public IList<CensoredWord> CensoredWords { get; private set; }
        //public bool TextWasCensored = false;

        public Censor()
        {
            using (TextPortDA da = new TextPortDA())
            {
                CensoredWords = da.GetCensoredWords();
            }
        }

        public string CensorText(string text)
        {
            if (text != null)
            {
                string censoredText = text;

                foreach (CensoredWord censoredWord in CensoredWords)
                {
                    string regularExpression = ToRegexPattern(censoredWord.Word);

                    censoredText = Regex.Replace(censoredText, regularExpression, StarCensoredMatch, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                }

                return censoredText;
            }
            return null;
        }

        private static string StarCensoredMatch(Match m)
        {
            string word = m.Captures[0].Value;

            return new string('*', word.Length);
        }

        private string ToRegexPattern(string wildcardSearch)
        {
            string regexPattern = Regex.Escape(wildcardSearch);

            regexPattern = regexPattern.Replace(@"\*", ".*?");
            regexPattern = regexPattern.Replace(@"\?", ".");

            if (regexPattern.StartsWith(".*?"))
            {
                regexPattern = regexPattern.Substring(3);
                regexPattern = @"(^\b)*?" + regexPattern;
            }

            regexPattern = @"\b" + regexPattern + @"\b";

            return regexPattern;
        }
    }
}