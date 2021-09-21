using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PA1_ELIZA
{
    static class Eliza
    {
        // Various self-explanatory variable creation
        static string input = "";
        static string name = "";
        static string major = "";
        static string home = "";
        static string log = "";
        static string lastResponse = "";

        // Preloaded basic questions
        static string intro = "Hello! I am Eliza the Career Counselor, a chatbot written by Kristian Vatsaas.";
        static string extro = "Goodbye and good luck in your future endeavors!";
        static string askName = "What is your name?";
        static string askMajor = "What is your major?";
        static string askHome = "Where do you consider home?";
        static string startConversation = "What can I help you with today?";
        static string gibberishResponse = "I don't understand.";
        static string abuseResponse = "Please keep a civil tongue and show respect!";
        static int nameChance = 5;

        static List<string> spurs = new List<string>()
        {
            "Have you considered academia?",
            "Would you enjoy working in an office?",
            "Do you have a preferred location?",
            "Are you happy with your major?",
            "Where do you see yourself in five years?"
        };
        static string basicSpur = "Do you have any other questions or concerns?";

        static bool GetInput()
        {
            input = Console.ReadLine();
            log += input + "\n";
            if (input.ToLower().Equals("exit") || input.ToLower().Equals("quit"))
                return false;
            input = ElizaPatterns.PronounSwap(input);
            return true;
        }

        static void Respond(string output, bool lastPrompt = true, bool useName = true)
        {
            lastResponse = output;
            if (useName && !name.Equals(""))
            {
                var rand = new Random();
                if (rand.Next(nameChance - 1) == 0)
                    output = name + ", " + output.Substring(0, 1).ToLower() + output.Substring(1);
            }
            output = "> " + output + "\n";
            if (lastPrompt)
                output += "< ";
            Console.Write(output);
            log += output;
        }

        static string GenerateResponse(bool spur = false)
        {
            string response;
            foreach (string pattern in ElizaPatterns.keywords.Keys)
            {
                response = ElizaPatterns.KeywordSpot(input, pattern);
                if (!response.Equals("") && !response.Equals(lastResponse))
                    return response;
            }
            foreach (string pattern in ElizaPatterns.phrases.Keys)
            {
                response = ElizaPatterns.PhraseSpot(input, pattern);
                if (!response.Equals("") && !response.Equals(lastResponse))
                    return response;
            }

            if (spur)
                return SpurResponse();
            else return "";
        }

        static string SpurResponse()
        {
            if (spurs.Count == 0)
                return basicSpur;
            else
            {
                var rand = new Random();
                var index = rand.Next(spurs.Count);
                var response = spurs[index];
                spurs.RemoveAt(index);
                return response;
            }
        }

        static void AddMajorToSpurs()
        {
            spurs.Add("You mentioned earlier that you are studying " + major + ". Do you like it?");
            spurs.Add("What type of job would you like to have in the field of " + major + "?");
            spurs.Add("Do you know anyone who already has a job in " + major + " that could help you?");
        }

        static void AddHomeToSpurs()
        {
            spurs.Add("Previously, you said " + home + " is your home. Do you want to stay there?");
            spurs.Add("Did living in " + home + " affect your career planning?");
        }

        static void Main(string[] args)
        {
            var response = "";
            bool skipPreload = false;

            // Introduce chatbot.
            Respond(intro, false, false);

            /*
             * Ask user for their name. If they give it, greet them and continue preloaded prompts.
             * If not, attempt to match the input. If successful, skip preloaded prompts and start conversation using that input.
             * Otherwise, continue preloaded prompts.
             */
            Respond(askName);
            if (!GetInput())
            {
                Respond(extro, false, false);
                return;
            }
            else
            {
                if (ElizaPatterns.IsAbusive(input))
                    Respond(abuseResponse, true, false);
                else if (ElizaPatterns.IsGibberish(input))
                    Respond(gibberishResponse, true, false);
                else
                {
                    name = ElizaPatterns.NameSpot(input);
                    if (!name.Equals(""))
                        Respond("Hello, " + name + "!", false, false);
                    else if (!(response = GenerateResponse()).Equals(""))
                        skipPreload = true;
                }
            }

            /*
             * Ask user for their major. If they give it, greet them and continue preloaded prompts.
             * Also add associated prompts to potential conversation spurs.
             * If not, attempt to match the input. If successful, skip preloaded prompts and start conversation using that input.
             * Otherwise, continue preloaded prompts.
             * Skip this step if user attempted to start a different conversation when asked for their name.
             */

            if (!skipPreload)
            {
                Respond(askMajor);
                if (!GetInput())
                {
                    Respond(extro, false, true);
                    return;
                }
                else
                {
                    if (ElizaPatterns.IsAbusive(input))
                        Respond(abuseResponse, true, false);
                    else if (ElizaPatterns.IsGibberish(input))
                        Respond(gibberishResponse, true, false);
                    else
                    {
                        major = ElizaPatterns.MajorSpot(input);
                        if (!major.Equals(""))
                            AddMajorToSpurs();
                        else if (!(response = GenerateResponse()).Equals(""))
                            skipPreload = true;
                    }
                }
            }

            /*
             * Ask user for their home. If they give it, greet them and continue preloaded prompts.
             * Also add associated prompts to potential conversation spurs.
             * If not, attempt to match the input. If successful, skip preloaded prompts and start conversation using that input.
             * Otherwise, continue preloaded prompts.
             * Skip this step if user attempted to start a different conversation when asked for their name or major.
             */

            if (!skipPreload)
            {
                Respond(askHome);
                if (!GetInput())
                {
                    Respond(extro, false, true);
                    return;
                }
                else
                {
                    if (ElizaPatterns.IsAbusive(input))
                        Respond(abuseResponse, true, false);
                    else if (ElizaPatterns.IsGibberish(input))
                        Respond(gibberishResponse, true, false);
                    else
                    {
                        home = ElizaPatterns.HomeSpot(input);
                        if (!home.Equals(""))
                            AddHomeToSpurs();
                        else if (!(response = GenerateResponse()).Equals(""))
                            skipPreload = true;
                    }
                }
            }

            /*
             * If response has already been generated from previous input, use that. 
             * Otherwise, use the standard conversation starting prompt.
             */
            if (!skipPreload)
                response = startConversation;
            Respond(response);

            while (true)
            {
                if (!GetInput())
                {
                    Respond(extro, false, true);
                    break;
                }
                else if (ElizaPatterns.IsAbusive(input))
                {
                    if (lastResponse.Equals(abuseResponse))
                        Respond(SpurResponse());
                    else
                        Respond(abuseResponse, true, false);
                    continue;
                }
                else if (ElizaPatterns.IsGibberish(input))
                {
                    if (lastResponse.Equals(gibberishResponse))
                        Respond(SpurResponse());
                    else
                        Respond(gibberishResponse, true, false);
                    continue;
                }
                else
                    Respond(GenerateResponse());
            }


            // Quick and dirty method to echo the conversation to a file that I can print for PDF submission.
            // File.WriteAllText(@"S:\Downloads\log.txt", log);
        }
    }
}
