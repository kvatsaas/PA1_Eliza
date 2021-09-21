/*
 * ELIZA Career Counselor
 * Created by Kristian Vatsaas
 * September 2021
 * 
 * ELIZA was one of the first chatbot programs, created at MIT. ELIZA imitates real conversation by scanning input with a
 * large number of regular expressions to look for familiar words, phrases, or sentence structures. Then, the program can
 * respond accordingly, either by directly addressing a recognized phrase or simply by reflecting the user's input back as
 * a question. This peculiar method of conversation worked because ELIZA's primary implementation was to emulate a
 * Rogerian psychotherapist, which was chosen because it is one of the few types of conversationalists that do not need to
 * actually know anything about what they are talking about and can stay on topic by simply repeating the other person.
 * 
 * This particular implementation of ELIZA is as a career counselor at a university, although it still takes a Rogerian
 * approach to conversation. ELIZA can remember the user's name, major, and home (if given when asked) and use them
 * later, and has methods of spurring the conversation along if it is in danger of looping. ELIZA also has a language
 * filter (though it is fairly G-rated) and has rudimentary gibberish detection. Below is a brief example of the program's
 * conversational loop, after initial introductions. Output is marked by > and user input by <.
 * 
 * > What can I help you with today?
 * < I need help with my resume.
 * > Kristian, is your resume updated?
 * < Sort of, but it's a little messy.
 * > Do you know anyone who already has a job in computer science that could help you?
 * < I have a couple of friends in the industry.
 * > Why do you have a couple of friends in the industry?
 * 
 * The conversation continues until the user enters "exit" or "quit," though Eliza's unique responses are limited and when
 * they run out, ELIZA must rely on a basic conversational spur if input is not recognized.
 * 
 * This project is split into two files, Eliza.cs (the one you are in now) and ElizaPatterns.cs. ElizaPatterns is a static
 * class that handles regexes, regex operations, and responses related to recognized input (words, phrases, or patterns).
 * Eliza.cs contains the main conversational loop with helper methods for managing input and output. It uses the methods
 * from ElizaPatterns to do so. Eliza.cs also contains some basic responses, such as introductions and conversational spurs,
 * that are not directly related to patterns in user input.
 * 
 * The algorithm has two parts - an introductory phase and the main conversational loop. The former is best defined as a
 * modified version of the latter, so I will define the main loop first.
 *  1. Receive input from the user.
 *  2. If input is "exit" or "quit," display farewell message and exit the loop.
 *  3. Run input through the abuse filter. If the filter catches it, scold the user and return to step 1.
 *  4. Run input through the gibberish filter. If the filter catches it, respond accordingly and return to step 1.
 *  5. Generate and send a response to the input via the steps below, then return to step 1.
 *      a. Scan for recognized keywords. If found, respond with the related response and stop scanning. Otherwise, continue to b.
 *      b. Scan for recognized phrases. If found, respond with the related response and stop scanning.  Otherwise, continue to c.
 *      c. If there are any unique conversational spurs not yet used, respond with a random one.  Otherwise, continue to d.
 *      d. Respond with the basic conversational spur.
 *  
 *  The introductory phase is similar but reacts a little differently to input.
 *  1. Introduce the chatbot.
 *  2. Ask the user for their name, major, or home. If all three have already been asked for, jump to step 6.
 *  3. Run input through abuse and gibberish filters and respond accordingly if caught, then return to step 2.
 *  4. Scan input for name/major/home. If found, store it and return to step 2.
 *  5. Scan input for recognized keywords or phrases. If found generate a response as in step 5 of the main conversational loop
 *      and continue to step 6; if not, return to step 2.
 *  6. If a response was generated in step 5, use it. Otherwise, use the generic conversation starter.
 *  Note that if the user responds to one of these questions with something else, and ELIZA recognizes it, she will respond
 *  accordingly and will not ask the other introductory questions. It should also be noted that this is not actually implemented
 *  as a loop because of slight differences in the execution of each step, but it is best understood as one.
 *  
 *  One final note: to meet assignment requirements, I need printable output, so throughout the program I have a few extra
 *  steps to record the conversation and eventually output it into a file. These are not important to the program itself, but
 *  to leave them unexplained would no doubt be confusing.
 * 
 * This program was created for CS 4242 Natural Language Processing at the University of Minnesota-Duluth.
 * Last updated 9/21/21
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace PA1_ELIZA
{
    static class Eliza
    {
        // variables for I/O and conversational memory
        static string input = "";
        static string lastResponse = "";
        static string name = "";
        static string major = "";
        static string home = "";
        static string log = "";

        // Preloaded basic questions
        static string intro = "Hello! I am Eliza the Career Counselor, a chatbot written by Kristian Vatsaas.";
        static string extro = "Goodbye and good luck in your future endeavors!";
        static string askName = "What is your name?";
        static string askMajor = "What is your major?";
        static string askHome = "Where do you consider home?";
        static string startConversation = "What can I help you with today?";
        static string gibberishResponse = "That doesn't make sense to me.";
        static string abuseResponse = "Please keep a civil tongue and show respect!";

        // Chance of ELIZA addressing user by name is 1/nameChance
        static int nameChance = 5;

        // If input is not matched by any regex, one of these unique conversational spurs will be used instead.
        static List<string> spurs = new List<string>()
        {
            "Have you considered academia?",
            "Would you enjoy working in an office?",
            "Do you have a preferred location?",
            "Are you happy with your major?",
            "Where do you see yourself in five years?"
        };
        // If all unique spurs have been used, this basic spur is used instead.
        static string basicSpur = "Do you have any other questions or concerns?";

        /// <summary>
        /// Gets input from the user and adds it to the conversation log. Then, checks if the user is trying
        /// to exit; otherwise, changes pronouns to reflect the perspective change.
        /// </summary>
        /// <returns>false if input is 'quit' or 'exit', otherwise true</returns>
        static bool GetInput()
        {
            Console.Write("< ");
            input = Console.ReadLine();
            log += "< " + input + "\n";
            if (input.ToLower().Equals("exit") || input.ToLower().Equals("quit"))
                return false;
            input = ElizaPatterns.PronounSwap(input);
            return true;
        }

        /// <summary>
        /// Outputs a given response and adds it to the conversation log. Includes a chance that the response
        /// will be modified to address the user by name.
        /// </summary>
        /// <param name="output">The text to be printed.</param>
        /// <param name="useName">Default is true; should be set to false if user's name should never be used.</param>
        static void Respond(string output, bool useName = true)
        {
            lastResponse = output;  // store output for repeat detection

            // if not disabled, insert name at beginning of response. Changes the first letter to lowercase unless it is 'I'
            if (useName && !name.Equals(""))
            {
                var rand = new Random();
                if (rand.Next(nameChance - 1) == 0)
                {
                    if (output.Substring(0, 1).Equals("I"))
                        output = name + ", " + output;
                    else
                        output = name + ", " + output.Substring(0, 1).ToLower() + output.Substring(1);
                }
            }
            output = "> " + output + "\n";  // add angle bracket to denote speaker and add new line
            Console.Write(output);          // output to console
            log += output;                  // add to conversation log
        }

        /// <summary>
        /// Scans input for keywords and phrases to create a response. If no matches are found, a spur is selected instead.
        /// </summary>
        /// <param name="spur">Denotes whether spurs are allowed for this response.</param>
        /// <returns>The response generated, which is the empty string if none was generated.</returns>
        static string GenerateResponse(bool spur = true)
        {
            string response;
            // scan for keywords first as they will rarely be used if they don't have precedent
            foreach (string pattern in ElizaPatterns.keywords.Keys) // scan for each keyword pattern in order
            {
                response = ElizaPatterns.KeywordSpot(input, pattern);
                if (!response.Equals("") && !response.Equals(lastResponse)) // skip if not unique
                    return response;
            }
            foreach (string pattern in ElizaPatterns.phrases.Keys) // scan for each phrase pattern in order
            {
                response = ElizaPatterns.PhraseSpot(input, pattern);
                if (!response.Equals("") && !response.Equals(lastResponse)) // skip if not unique
                    return response;
            }

            // if no match found, use a spur if allowed
            if (spur)
                return SpurResponse();
            else
                return "";
        }

        /// <summary>
        /// Randomly selects a conversational spur to use as a response.
        /// </summary>
        /// <returns>The selected spur.</returns>
        static string SpurResponse()
        {
            if (spurs.Count == 0)   // return basic spur if all others have been used
                return basicSpur;
            else
            {
                var rand = new Random();
                var index = rand.Next(spurs.Count); // randomly select from list of spurs
                var response = spurs[index];
                spurs.RemoveAt(index);  // remove spur from list once used
                return response;
            }
        }

        /// <summary>
        /// Adds conversational spurs related to the user's major to the list of spurs.
        /// </summary>
        static void AddMajorToSpurs()
        {
            spurs.Add("You mentioned earlier that you are studying " + major + ". Do you like it?");
            spurs.Add("What type of job would you like to have in the field of " + major + "?");
            spurs.Add("Do you know anyone who already has a job in " + major + " that could help you?");
        }

        /// <summary>
        /// Adds conversational spurs related to the user's home to the list of spurs.
        /// </summary>
        static void AddHomeToSpurs()
        {
            spurs.Add("Previously, you said " + home + " is your home. Do you want to stay there?");
            spurs.Add("Did living in " + home + " affect your career planning?");
        }

        /// <summary>
        /// Runs the introductory phase and main conversational loop.
        /// </summary>
        /// <param name="args">Not used.</param>
        static void Main(string[] args)
        {
            var response = "";

            // Introduce chatbot.
            Respond(intro, false);


            // this does not actually loop, but the while provides a convenient structure to break from if needed.
            while (true)
            {
             /* Ask user for their name. If they give it, greet them and continue preloaded prompts.
             * If not, attempt to match the input. If successful, skip preloaded prompts and start conversation using that input.
             * Otherwise, continue preloaded prompts.
             */
                Respond(askName);
                if (!GetInput())
                {
                    Respond(extro, false); // quit if user input so directs
                    return;
                }
                else
                {   // if detected, respond to abuse or gibberish and continue to next question
                    if (ElizaPatterns.IsAbusive(input))
                        Respond(abuseResponse, false);
                    else if (ElizaPatterns.IsGibberish(input))
                        Respond(gibberishResponse, false);
                    else
                    {
                        // if name is detected, store it and greet the user
                        name = ElizaPatterns.NameSpot(input);
                        if (!name.Equals(""))
                            Respond("Hello, " + name + "!", false);
                        // if not, check for other recognized patterns. if found, store the related response and skip rest of intro phase
                        else if (!(response = GenerateResponse()).Equals(""))
                            break;
                    }
                }

                /* Ask user for their major. If they give it, greet them and continue preloaded prompts.
                 * Also add associated prompts to potential conversation spurs.
                 * If not, attempt to match the input. If successful, skip preloaded prompts and start conversation using that input.
                 * Otherwise, continue preloaded prompts.
                 * Skip this step if user attempted to start a different conversation when asked for their name.
                 */

                Respond(askMajor, false);
                if (!GetInput())
                {
                    Respond(extro); // quit if user input so directs
                    return;
                }
                else
                {   // if detected, respond to abuse or gibberish and continue to next question
                    if (ElizaPatterns.IsAbusive(input))
                        Respond(abuseResponse);
                    else if (ElizaPatterns.IsGibberish(input))
                        Respond(gibberishResponse);
                    else
                    {
                        // if major is detected, store it and add the related spurs
                        major = ElizaPatterns.MajorSpot(input);
                        if (!major.Equals(""))
                            AddMajorToSpurs();
                        // if not, check for other recognized patterns. if found, store the related response and skip rest of intro phase
                        else if (!(response = GenerateResponse()).Equals(""))
                            break;
                    }
                }

                /* Ask user for their home. If they give it, greet them and continue preloaded prompts.
                 * Also add associated prompts to potential conversation spurs.
                 * If not, attempt to match the input. If successful, skip preloaded prompts and start conversation using that input.
                 * Otherwise, continue preloaded prompts.
                 * Skip this step if user attempted to start a different conversation when asked for their name or major.
                 */

                Respond(askHome);
                if (!GetInput())
                {
                    Respond(extro); // quit if user input so directs
                    return;
                }
                else
                {   // if detected, respond to abuse or gibberish and continue to next question
                    if (ElizaPatterns.IsAbusive(input))
                        Respond(abuseResponse);
                    else if (ElizaPatterns.IsGibberish(input))
                        Respond(gibberishResponse);
                    else
                    {
                        // if home is detected, store it and add the related spurs
                        home = ElizaPatterns.HomeSpot(input);
                        if (!home.Equals(""))
                            AddHomeToSpurs();
                        // if not, check for other recognized patterns. if found, store the related response and skip rest of intro phase
                        else if (!(response = GenerateResponse()).Equals(""))
                            break;  // technically unnecessary, but easier to follow this way
                    }
                }

                /* If this point is reached, no other response has been created to react to user input, so set the standard conversation
                 * starter as the response. Logically, this could be worked into the previous conditional block, but the logical
                 * progression is easier to follow by having it separate.
                 */
                response = startConversation;
                break;

            }

            Respond(response);  // send whichever response resulted from the introductory phase

            while (true)
            {
                if (!GetInput())
                {
                    Respond(extro); // quit if user input so directs
                    break;
                }
                /* If detected, respond to abuse or gibberish. If the previous input resulted in the same response,
                 * use a spur instead. Either way, proceed to next user input after this response.
                 */
                else if (ElizaPatterns.IsAbusive(input))
                {
                    if (lastResponse.Equals(abuseResponse))
                        Respond(SpurResponse());
                    else
                        Respond(abuseResponse);
                    continue;
                }
                else if (ElizaPatterns.IsGibberish(input))
                {
                    if (lastResponse.Equals(gibberishResponse))
                        Respond(SpurResponse());
                    else
                        Respond(gibberishResponse);
                    continue;
                }
                // If input is not abusive or gibberish, generate and send a response, then proceed to next user input.
                else
                    Respond(GenerateResponse());
            }


            // Quick and dirty method to echo the conversation to a file that I can print for PDF submission.
            File.WriteAllText(@"S:\Downloads\log.txt", log);
        }
    }
}
