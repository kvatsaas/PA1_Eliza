/* Patterns for ELIZA Career Counselor
 * Created by Kristian Vatsaas
 * September 2021
 * 
 * This static class manages all regex operations and contains all regexes along with data tightly coupled with those regexes.
 * The class is built to serve Eliza.cs and so that line between the two (regex vs. non-regex) may appear thin at points.
 * A few methods are currently unused, but are left in for potential use in the future.
 * 
 * A couple of other notes are necessary to understand the use of regular expressions in this library. First, one must
 * understand the C# implementation of regexes. Rather than rewrite the documentation myself, I will simply provide a link.
 * The methods used in this program are quite readable, but to explain their function much further would result in some long comments.
 * .NET regular expression syntax can be found here:
 * https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions
 * The documentation for the Regex class (System.Text.RegularExpressions) is located here:
 * https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=net-5.0
 * 
 * The other thing to make note of is the way the regexes are written for the 'phrases' dictionary. Because of the way the
 * Regex.Replace method works, these expressions need to match the entire input or odd results occur. Essentially,
 * basic use of Replace takes three arguments: the input, the regex pattern, and a replacement pattern. The matched
 * part of the input is replaced with the replacement pattern, but unmatched sections are untouched. So, if only part of the
 * string is matched and the goal is to form a question, the result of the input "Sometimes, I don't like my major. But I'm sticking it out."
 * might be something strange like "Sometimes Why don't you like your major?. But you're sticking it out." To avoid this,
 * these patterns are created to match everything on either side of the intended pattern. There are certainly other ways
 * to accomplish this, but this seemed the most straightforward approach.
 * 
 * This program was created for CS 4242 Natural Language Processing at the University of Minnesota-Duluth.
 * Last updated 9/21/21
 */

using System.Collections.Generic;
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
         * -An alphanumeric string with sixteen or more characters
         * -A substring of four or more characters repeated twice in the same word
         * -A substring of three or more characters repeated three times in the same word
         * This is obviously not particularly robust, but seems a decent rudimentary approach without requiring an extremely long or complex regex.
         */
        static readonly string rx_gibberish = @"\b[aeiou]{4,}\b|\b[aeiou]{3,}\w*[aeiou]{3,}\b|\b[bcdfghjklmnpqrstvwxyz]{5,}\b|\b[bcdfghjklmnpqrstvwxyz]{4,}\w*[bcdfghjklmnpqrstvwxyz]{4,}\b|\w{16,}|\w*(?<three>\w{3,})\w*\k<three>\w*\k<three>\w*\b|\w*(?<four>\w{4,5})\w*\k<four>\w*";

        /* Elementary regex that defines an elementary-age filter. More of a proof-of-concept to avoid turning in a program full of
         * slurs, so I didn't bother trying to capture all the compound words while avoiding words like 'scrap' or 'buttress'
         * as this filter is already only identifying a fairly arbitrary selection.
         */
        static readonly string rx_abuse = @"\bpoop\b|\bbutt\b|\bdummy\b|\bstupid\b|\bcrap\b|\bidiot\b";

        static readonly string rx_name = @"(Your name( is|'s)|(You are( called)?)) (?<name>[\w'-]+)";
        static readonly string rx_major = @"(Your (major|field|degree) is( in)?|You( are|'re) (in|studying)) (?<major>[\w' -]+)";
        static readonly string rx_home = @"You( (lived?|grew up) (in|on)|( are|'re) (from|living (in|on))) (?<home>[\w' -\.]+\b)";

        // Regex pattern for pronouns. Also includes the most common conjugations, though they are not technically pronouns. Each is listed individually for readability.
        static readonly string rx_pronouns = @"\bi'm\b|\bi am\b|\bi'll\b|\bi\b|\bme\b|\bmy\b|\byou are\b|\byou're\b|\byour\b|\byou\b|\bwe\b|\bour\b";

        // Dictionary for swapping pronouns, matching those in the regex above. All keys are lowercase for simplified matching - candidates will be converted to lower.
        static readonly Dictionary<string, string> pronouns = new Dictionary<string, string>()
        {
            {"i", "you"},
            {"i'm", "you are"},
            {"i am", "you are"},
            {"i'll", "you'll"},
            {"me", "you"},
            {"my", "your"},
            {"you", "I"},
            {"you are", "I am"},
            {"you're", "I am"},
            {"your", "my"},
            {"we", "you"},
            {"our", "your"}
        };

        /* This dictionary matches patterns that represent certain keywords to a related tuple. The tuple contains a response and a bool that indicates
         * whether this keyword has previously been detected (and presumably responded to). It should be noted that this is not necessarily the best
         * solution - it shouldn't really be static - but in this case it doesn't matter with the structure of the program. If it mattered, we could
         * make this non-static and actually instantiate ElizaPatterns as an object, or we could provide a method to reset it. But if the structure
         * needed to be different, the whole design would be different as well, so it's not worth worrying about since frankly, this was the final
         * band-aid on the program, and modifying a lot of things for one relatively minor improvement was not appealing.
         */
        public static Dictionary<string, (string, bool)> keywords = new Dictionary<string, (string, bool)>()
        {
            {@"\bresumes?\b", ("Is your resume updated?", false) },
            {@"\bcareers?\b|\bjobs?\b", ("Do you have a career path in mind?", false) },
            {@"\b(grad|graduate) (program|degree|school|applications?|studies?)", ("Do you want to pursue graduate studies?", false) },
            {@"\bclass(es)?\b|\bcourses?\b|\bgrades?\b", ("Are your classes going well right now?", false) },
            {@"research", ("Are you interested in doing research?", false) }
        };

        /*This dictionary matches patterns that represent particular phrases or sentence structures to replacement patterns that
         * use substitution to form a relevant reply. Unlike keywords, previous use is not tracked.
         */
        public static readonly Dictionary<string, string> phrases = new Dictionary<string, string>()
        {
            {@".*You(( would|'d) like|want|need) to (?<clause>[\w '\-]+)[!\?\.]?.*", "Would it further your goals to ${clause}?"},
            {@".*(can|can't|could|will|would|won't|do|don't) I (?<clause>[\w '\-]+)[!\?\.]?.*", "Do you think if I ${clause}, it will help your career planning?"},
            {@".*(?<option1>What|Why|Where) (?<option2>is|are) (?<word>[\w'\-]+) (?<clause>[\w '\-]+)[!\?\.]?.*", "First, ask yourself this: ${option1} do you think ${word} ${option2} ${clause}?"},
            {@".*You do(n't| not) (?<option>know|think) (?<clause>[\w '\-]+)[!\?\.]?.*", "Why don't you ${option} ${clause}?"},
            {@".*You are (?<clause>[\w '\-]+)[!\?\.]?.*", "Why are you ${clause}?"},
            {@".*You are not (?<clause>[\w '\-]+)[!\?\.]?.*", "Why aren't you ${clause}?"},
            {@".*You (?<option>can|cannot|can't|will|will not|won't|did|did not|didn't|do|do not|don't) (?<clause>[\w '\-]+)[!\?\.]?.*", "Why ${option} you ${clause}?"},
            {@".*You (?<clause>[\w '\-]+)[!\?\.]?.*", "Why do you ${clause}?"},
            {@".*I (?<clause>[\w '\-]+)[!\?\.]?.*", "Why do you think I ${clause}?"}
        };

        /// <summary>
        /// Modifies a pattern so that it only matches whole inputs.
        /// </summary>
        /// <param name="pattern">The pattern to be modified.</param>
        /// <returns>The modified pattern.</returns>
        private static string WholeInputPattern(string pattern)
        {
            return "^" + pattern + "$";
        }

        /// <summary>
        /// Modifies a pattern so that it only matches the end of input.
        /// </summary>
        /// <param name="pattern">The pattern to be modified.</param>
        /// <returns>The modified pattern.</returns>
        private static string EndInputPattern(string pattern)
        {
            return pattern + "$";
        }

        /// <summary>
        /// Modifies a pattern so that it only matches the start of input.
        /// </summary>
        /// <param name="pattern">The pattern to be modified.</param>
        /// <returns>The modified pattern.</returns>
        private static string StartInputPattern(string pattern)
        {
            return "^" + pattern;
        }

        /// <summary>
        /// Dynamically creates a pattern that matches up to a given maximum number of words. Words may be separated by the ' ', '-', or ''' characters.
        /// </summary>
        /// <param name="max">The maximum number of words.</param>
        /// <returns>The requested pattern.</returns>
        private static string MaxWords(int max)
        {
            var pattern = @"(?<phrase>\b\w+\b";
            for (int i = 0; i < max; i++)
            {
                pattern += @"(([ '-]|(, ))\b\w+\b)?";
            }
            pattern += ")";
            return pattern;
        }

        /// <summary>
        /// Concatenates two or more patterns using the given separator.
        /// </summary>
        /// <param name="separator">The character(s) to put between the patterns.</param>
        /// <param name="firstPattern">The first pattern to be combined.</param>
        /// <param name="patterns">The rest of the patterns to be combined.</param>
        /// <returns></returns>
        private static string CombinePatterns(string separator, string firstPattern, params string[] patterns)
        {
            var newPattern = firstPattern;
            foreach (string pattern in patterns)
                newPattern += separator + pattern;
            return newPattern;
        }

        /// <summary>
        /// Checks for matches to rx_abuse on the given input.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>True if any matches are found, otherwise false.</returns>
        public static bool IsAbusive(string input)
        {
            return Regex.IsMatch(input, rx_abuse, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Checks for matches to rx_gibberish on the given input.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>True if any matches are found, otherwise false.</returns>
        public static bool IsGibberish(string input)
        {
            return Regex.IsMatch(input, rx_gibberish, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Performs pronoun swapping on input as specified by rx_pronouns and the related dictionary.
        /// </summary>
        /// <param name="input">The input to pronoun swap.</param>
        /// <returns>The pronoun-swapped input.</returns>
        public static string PronounSwap(string input)
        {
            return Regex.Replace(input, rx_pronouns, m => pronouns[m.Value.ToLower()], RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Looks for a name in the given input, either as part of a sentence or by itself.
        /// </summary>
        /// <param name="input">The input to search.</param>
        /// <returns>The name if found, or the empty string if not.</returns>
        public static string NameSpot(string input)
        {
            Match match;
            // search for a match to rx_name and return the 'name' capture group if successful
            if ((match = Regex.Match(input, rx_name, RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["name"].Value;
            // match up to two words by themselves and treat as the name if successful
            else if ((match = Regex.Match(input, WholeInputPattern(MaxWords(2)), RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["phrase"].Value;
            else
                return "";
        }

        /// <summary>
        /// Looks for a major in the given input, either as part of a sentence or by itself.
        /// </summary>
        /// <param name="input">The input to search.</param>
        /// <returns>The major if found, or the empty string if not.</returns>
        public static string MajorSpot(string input)
        {
            Match match;
            // search for a match to rx_major and return the 'major' capture group if successful
            if ((match = Regex.Match(input, rx_major, RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["major"].Value;
            // match up to two words by themselves and treat as the major if successful
            else if ((match = Regex.Match(input, WholeInputPattern(MaxWords(2)), RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["phrase"].Value;
            else
                return "";
        }

        /// <summary>
        /// Looks for a major in the given input, either as part of a sentence or by itself.
        /// </summary>
        /// <param name="input">The input to search.</param>
        /// <returns>The major if found, or the empty string if not.</returns>
        public static string HomeSpot(string input)
        {
            Match match;
            // search for a home to rx_major and return the 'home' capture group if successful
            if ((match = Regex.Match(input, rx_home, RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["home"].Value;
            // match up to two words by themselves and treat as the home if successful
            else if ((match = Regex.Match(input, WholeInputPattern(MaxWords(3)), RegexOptions.IgnoreCase | RegexOptions.Compiled)).Success)
                return match.Groups["phrase"].Value;
            else
                return "";
        }

        /// <summary>
        /// Checks the given input for matches to the keyword(s) in the given pattern and returns the matching response.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <param name="pattern">The pattern of keyword(s) to look for.</param>
        /// <returns>The response for the keyword, or the empty string if non was found</returns>
        public static string KeywordSpot(string input, string pattern)
        {
            // ensure pattern has not previously been successfully used
            if (!keywords[pattern].Item2 && Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled).Success)
            {
                var output = keywords[pattern].Item1;   // get response
                keywords[pattern] = (output, true);     // reassign related tuple to mark this keyword response as used
                return keywords[pattern].Item1;         // return response
            }
            else
                return "";
        }

        /// <summary>
        /// Checks the given input for matches to the phrase(s) in the given pattern.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <param name="pattern">The pattern of phrase(s) to look for.</param>
        /// <returns>The response generated for the phrase, or the empty string if non was found</returns>
        public static string PhraseSpot(string input, string pattern)
        {
            if (Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled).Success)
                return Regex.Replace(input, pattern, phrases[pattern], RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            else
                return "";
        }
    }
}
