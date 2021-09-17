using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PA1_ELIZA
{
    class ElizaPatterns
    {
        /* Elementary regex that defines an elementary-age filter. More of a proof-of-concept to avoid turning in a program full of
         * slurs, so I didn't bother trying to capture all the compound words while avoiding words like 'scrap' or 'buttress'
         * as this filter is already only identifying a fairly arbitrary selection.
         */
        static readonly string rx_abuse = @"\bpoop\b|\bbutt\b|\bdummy\b|\bstupid\b|\bcrap\b|\bidiot\b";

        // Regex pattern for pronouns
        static readonly string rx_pronouns = @"\bI\b|\bme\b|\bmy\b|\byou\b|\byour\b|\bwe\b|\bour\b";

        // Dictionary for swapping pronouns. All keys are lowercase for simplified matching - candidates will be converted to lower.
        static readonly Dictionary<string, string> pronouns = new Dictionary<string, string>()
        {
            {"i", "you"},
            {"me", "you"},
            {"my", "your"},
            {"you", "I"},
            {"your", "my"},
            {"we", "you"},
            {"our", "your"}
        };

        static readonly Dictionary<string, string> keywords = new Dictionary<string, string>()
        {
            {@"\bresumes?\b", "Is your resume updated?"},
            {@"\bcareers?\b|\bjobs?\b", "Do you have a career path in mind?"},
            {@"\bclass(es)?\b|\bcourses?\b|\bschool\b|\bgrades?\b", "Are your classes going well right now?"},
            {@"research", "Are you interested in doing research?"},
            {@"\b(grad|graduate) (program|degree|school|applications?|studies?)", "Do you want to pursue graduate studies?"}
        };

        static readonly Dictionary<string, string> phrases = new Dictionary<string, string>()
        {
            {@"You (would like|want)", "Would <@1> further your goals?"},
            {@"(can('t)?|could|will|would|won't) I", "Do you think if I <@1>, it will help your career planning?"},
            {@"(What|Why) (is|are)", "I think it's more important to ask <@1> you think <@2> <@3>"},
            {@"You [do(n't| not) know", "What is keeping you from finding out?"},
            {@"", ""},
            {@"", ""},
            {@"", ""},
            {@"", ""},
            {@"", ""},
            {@"", ""}
        };

        static readonly string rx_name = @"(My name( is|'s)|((I am|I'm)( called)?)) (?<name>\b\w+\b)([ -]\b\w +\b)?";
        static readonly string rx_major = @"(My (major|field|degree) is( in)?|(I am|I'm) (in|studying)) (?<major>[^\p{P}]*\b)";
        static readonly string rx_home = @"I( (lived?|grew up) in|('m| am) (from|living in)) (?<home>(\b\w+\b)( \b\w+\b)?( \b\w+\b)?)";
        static readonly string rx_oneWord = @"(?<phrase>\b\w+\b)";

        private static string WholeInputPattern(string input)
        {
            return "^" + input + "$";
        }

        private static string EndInputPattern(string input)
        {
            return input + "$";
        }

        private static string StartInputPattern(string input)
        {
            return "^" + input;
        }

        private static string MaxWords(int max)
        {
            var pattern = rx_oneWord;
            for (int i = 0; i < max; i++)
            {
                pattern += @"([ -]\b\w +\b)?";
            }
            return pattern;
        }

        private static string CombinePatterns(string separator, string firstPattern, params string[] patterns)
        {
            var newPattern = firstPattern;
            foreach (string pattern in patterns)
                newPattern += separator + pattern;
            return newPattern;
        }

        public static bool IsAbusive(string input)
        {
            return Regex.IsMatch(input, rx_abuse, RegexOptions.IgnoreCase);
        }

        public static string PronounSwap(string input)
        {
            return Regex.Replace(input, rx_pronouns, m => pronouns[m.Value.ToLower()], RegexOptions.IgnoreCase);
        }

        public static string NameSpot(string input)
        {
            Match match;
            if ((match = Regex.Match(input, rx_name, RegexOptions.IgnoreCase)).Success)
                return match.Groups["name"].Value;
            else if ((match = Regex.Match(input, MaxWords(2), RegexOptions.IgnoreCase)).Success)
                return match.Groups["phrase"].Value;
            else
                return "";
        }

        public static string MajorSpot(string input)
        {
            Match match;
            if ((match = Regex.Match(input, rx_major, RegexOptions.IgnoreCase)).Success)
                return match.Groups["major"].Value;
            else if ((match = Regex.Match(input, MaxWords(4), RegexOptions.IgnoreCase)).Success)
                return match.Groups["phrase"].Value;
            else
                return "";
        }

        public static string HomeSpot(string input)
        {
            Match match;
            if ((match = Regex.Match(input, rx_home, RegexOptions.IgnoreCase)).Success)
                return match.Groups["home"].Value;
            else if ((match = Regex.Match(input, MaxWords(3), RegexOptions.IgnoreCase)).Success)
                return match.Groups["phrase"].Value;
            else
                return "";
        }

        public static string PhraseSpot(string input, string pattern)
        {
            Match match;
            if ((match = Regex.Match(input, rx_name, RegexOptions.IgnoreCase)).Success)
                return match.Groups["home"].Value;
            else
                return "";
        }

        public static string KeywordSpot(string input, string pattern)
        {
            Match match;
            if ((match = Regex.Match(input, rx_name, RegexOptions.IgnoreCase)).Success)
                return match.Groups["home"].Value;
            else
                return "";
        }
    }
}
