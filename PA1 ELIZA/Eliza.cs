using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PA1_ELIZA
{
    class Eliza
    {
        static string input, name, major, home;
        static string log = "";

        static void GetInput()
        {
            input = Console.ReadLine();
            log += input + "\n";
        }

        static void Respond(string output, bool lastPrompt = true)
        {
            output = "> " + output + "\n";
            if (lastPrompt)
                output += "< ";
            Console.Write(output);
            log += output;
        }

        static void AskName()
        {
            Respond("What is your name?");
            GetInput();
            name = ElizaPatterns.NameSpot(input);
        }

        static void Main(string[] args)
        {
            // This flag is set when ELIZA responds with something like "I don't know" to avoid doing so twice
            var loopWarning = false;

            // Regex patterns to be used later
            var i_my = @"I \b\w+\b my [^\p{P}]+";
            var you_your = @"You \b\w+\b your [^\p{P}]+";
            var we_our = @"We \b\w+\b our [^\p{P}]+";
            Match match;
                        
            // Greeting
            Respond("Hello! I am Eliza the Career Counselor, created by Kristian Vatsaas.", false);

            // Attempt to get user's name
            AskName();
            if (name == "")
                Respond("That doesn't look like a name.");
            else
                Respond("Hello, " + name + "!");
            
            while (true)
            {
                GetInput();

                if (input.ToLower().Equals("exit"))
                    break;
                if (ElizaPatterns.IsAbusive(input))
                {
                    Respond("Please be respectful and mind your manners.");
                    continue;
                }

                input = ElizaPatterns.PronounSwap(input);

                // Once one of these works, the condition is met and it stops trying, and the match is stored in match.Value
                if ((match = Regex.Match(input, i_my, RegexOptions.IgnoreCase)).Success ||
                    (match = Regex.Match(input, you_your, RegexOptions.IgnoreCase)).Success ||
                    (match = Regex.Match(input, we_our, RegexOptions.IgnoreCase)).Success)
                    Respond("Why do you say " + match.Value + "?");
                else
                    Respond("Tell me more.");
            }

            // Quick and dirty method to echo the conversation to a file that I can print for PDF submission.
            // File.WriteAllText(@"S:\Downloads\log.txt", log);
        }
    }
}
