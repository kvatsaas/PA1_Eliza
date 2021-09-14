using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PA1_ELIZA
{
    class Eliza
    {
        static string input;
        static string log = "";
        static string abuseFilter = @"\bpoop\b|\bbutt\b|\bdummy\b|\bstupid\b|\bcrap\b|\bidiot\b";
        // Regex pattern for pronouns
        static string pronouns = @"\bI\b|\bme\b|\bmy\b|\byou\b|\byour\b|\bwe\b|\bour\b";
        // Dictionary for swapping pronouns. All keys are lowercase for simplified matching - candidates will be converted to lower.
        static Dictionary<string, string> pronounsDict = new Dictionary<string, string>()
        {
            {"i", "you"},
            {"me", "you"},
            {"my", "your"},
            {"you", "I"},
            {"your", "my"},
            {"we", "you"},
            {"our", "your"}
        };

        static bool IsAbusive(string input)
        {
            return Regex.IsMatch(input, abuseFilter, RegexOptions.IgnoreCase);
        }

        static string PronounSwap(string input)
        {
            return Regex.Replace(input, pronouns, m => pronounsDict[m.Value.ToLower()], RegexOptions.IgnoreCase);
        }

        static void GetInput()
        {
            input = Console.ReadLine();
            log += input + "\n";
        }

        static void Respond(string output)
        {
            output = "> " + output + "\n< ";
            Console.Write(output);
            log += output;
        }

        static void Main(string[] args)
        {
            // Regex patterns to be used later
            var i_my = @"I \b\w+\b my [^\p{P}]+";
            var you_your = @"You \b\w+\b your [^\p{P}]+";
            var we_our = @"We \b\w+\b our [^\p{P}]+";
            Match match;
                        
            Respond("Hello! I am Eliza the Career Counselor, created by Kristian Vatsaas. How can I help you?");
            
            while (true)
            {
                GetInput();

                if (input.ToLower().Equals("exit"))
                    break;
                if (IsAbusive(input))
                {
                    Respond("Please be respectful and mind your manners.");
                    continue;
                }

                input = PronounSwap(input);

                // Once one of these works, the condition is met and it stops trying, and the match is stored in match.Value
                if ((match = Regex.Match(input, i_my, RegexOptions.IgnoreCase)).Success ||
                    (match = Regex.Match(input, you_your, RegexOptions.IgnoreCase)).Success ||
                    (match = Regex.Match(input, we_our, RegexOptions.IgnoreCase)).Success)
                    Respond("Why do you say " + match.Value + "?");
                else
                    Respond("Tell me more.");
            }

            // Quick and dirty method to echo the conversation to a file that I can print for PDF submission.
            File.WriteAllText(@"S:\Downloads\log.txt", log);
        }
    }
}
