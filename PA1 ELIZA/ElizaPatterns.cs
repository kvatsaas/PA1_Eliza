using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PA1_ELIZA
{
    static class ElizaPatterns
    {

        /*
         * Any of the following will be considered gibberish:
         * -An alphanumeric string with four consecutive vowels
         * -An alphanumeric string with two separate instances of three consecutive vowels
         * -An alphanumeric string with five consecutive consonants
         * -An alphanumeric string with two separate instances of four consecutive consonants
         * -An alphanumeric string with twenty or more characters
         * This is obviously not particularly robust, but seems a decent rudimentary approach without requiring an extremely long or complex regex.
         */
        static readonly string rx_gibberish = @"\b[aeiou]{4,}\b|\b[aeiou]{3,}\w*[aeiou]{3,}\b|\b[bcdfghijklmnpqrstvwxyz]{5,}\b|\b[bcdfghijklmnpqrstvwxyz]{4,}\w*[bcdfghijklmnpqrstvwxyz]{4,}\b|\b[a-z]{20,}\b";

        /* Elementary regex that defines an elementary-age filter. More of a proof-of-concept to avoid turning in a program full of
         * slurs, so I didn't bother trying to capture all the compound words while avoiding words like 'scrap' or 'buttress'
         * as this filter is already only identifying a fairly arbitrary selection.
         */
        static readonly string rx_abuse = @"\bpoop\b|\bbutt\b|\bdummy\b|\bstupid\b|\bcrap\b|\bidiot\b";

        static readonly string rx_name = @"(Your name( is|'s)|(You am( called)?)) (?<name>[\w'-]+)";
        static readonly string rx_major = @"(Your (major|field|degree) is( in)?|You( am|'m) (in|studying)) (?<major>[\w' -]+)";
        static readonly string rx_home = @"You ((lived?|grew up) in|am (from|living in)) (?<home>[\w' -\.]+\b)";

        // Regex pattern for pronouns
        static readonly string rx_pronouns = @"\bi'm\b|\bi'll\b|\bi\b|\bme\b|\bmy\b|\byou are\b|\byou're\b|\byour\b|\byou\b|\bwe\b|\bour\b";

        // Dictionary for swapping pronouns. All keys are lowercase for simplified matching - candidates will be converted to lower.
        static readonly Dictionary<string, string> pronouns = new Dictionary<string, string>()
        {
            {"i", "you"},
            {"i'm", "you am"},
            {"i'll", "you'll"},
            {"me", "you"},
            {"my", "your"},
            {"you", "I"},
            {"you are", "I are"},
            {"you're", "I are"},
            {"your", "my"},
            {"we", "you"},
            {"our", "your"}
        };

        public static readonly Dictionary<string, string> keywords = new Dictionary<string, string>()
        {
            {@"\bresumes?\b", "Is your resume updated?"},
            {@"\bcareers?\b|\bjobs?\b", "Do you have a career path in mind?"},
            {@"\b(grad|graduate) (program|degree|school|applications?|studies?)", "Do you want to pursue graduate studies?"},
            {@"\bclass(es)?\b|\bcourses?\b|\bgrades?\b", "Are your classes going well right now?"},
            {@"research", "Are you interested in doing research?"}
        };

        public static readonly Dictionary<string, string> phrases = new Dictionary<string, string>()
        {
            {@"^You(( would|'d) like|want) to (?<clause>[\w -']+)[!\?\.].*", "Would it further your goals to ${clause}?"},
            {@"^(can|can't|could|will|would|won't|do|don't) I (?<clause>[\w -']+)[!\?\.].*", "Do you think if I ${clause}, it will help your career planning?"},
            {@"^(?<option1>What|Why|Where) (?<option2>is|are) (?<word>[\w-']+) (?<clause>[\w -']+)[!\?\.].*", "First, ask yourself this: ${option1} do you think ${word} ${option2} ${clause}?"},
            {@"^You do(n't| not) (?<option>know|think) (?<clause>[\w -']+)[!\?\.].*", "Why don't you ${option} ${clause}?"},
            {@"^You am (?<clause>[\w -']+)[!\?\.].*", "Why are you ${clause}?"},
            {@"^You am not (?<clause>[\w -']+)[!\?\.].*", "Why aren't you ${clause}?"},
            {@"^You (?<option>can|cannot|can't|will|will not|won't|did|did not|didn't|do|do not|don't) (?<clause>[\w -']+)[!\?\.].*", "Why ${option} you ${clause}?"},
            {@"^You (?<clause>[\w -']+)[!\?\.].*", "Why do you ${clause}?"},
            {@"^I (?<clause>[\w -']+)[!\?\.].*", "Why do you think I ${clause}?"}
        };


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
            var pattern = @"(?<phrase>\b\w+\b";
            for (int i = 0; i < max; i++)
            {
                pattern += @"([ -]\b\w+\b)?";
            }
            pattern += ")";
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
            return Regex.IsMatch(input, rx_abuse, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static bool IsGibberish(string input)
        {
            return Regex.IsMatch(input, rx_gibberish, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static string PronounSwap(string input)
        {
            return Regex.Replace(input, rx_pronouns, m => pronouns[m.Value.ToLower()], RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static string NameSpot(string input)
        {
            Match match;
            if ((match = Regex.Match(input, rx_name, RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["name"].Value;
            else if ((match = Regex.Match(input, WholeInputPattern(MaxWords(2)), RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["phrase"].Value;
            else
                return "";
        }

        public static string MajorSpot(string input)
        {
            Match match;
            if ((match = Regex.Match(input, rx_major, RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["major"].Value;
            else if ((match = Regex.Match(input, WholeInputPattern(MaxWords(4)), RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["phrase"].Value;
            else
                return "";
        }

        public static string HomeSpot(string input)
        {
            Match match;
            if ((match = Regex.Match(input, rx_home, RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["home"].Value;
            else if ((match = Regex.Match(input, WholeInputPattern(MaxWords(3)), RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["phrase"].Value;
            else
                return "";
        }

        public static string KeywordSpot(string input, string pattern)
        {
            if (Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled).Success)
                return keywords[pattern];
            else
                return "";
        }

        public static string PhraseSpot(string input, string pattern)
        {
            if (Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled).Success)
                return Regex.Replace(input, pattern, phrases[pattern], RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            else
                return "";
        }
    }
}
