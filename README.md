# PA1_Eliza
PA1_Eliza

Below is the text of the original assignment description from CS 4242 Natural Language Processing at the Universeity of Minnesota - Duluth. The source code includes further commentary on the implementation, and the textbook referenced can be found here: https://web.stanford.edu/~jurafsky/slp3/

----

Write an Eliza program in the programming language of your choice.  It will be very helpful if you have read textbook Chapters 2 and 24.1-24.2 AND the original Eliza paper before getting too deeply into this program. See the assigned reading page for details. 

Rather than the traditional role of psychotherapist, your Eliza program should be able to serve as a career counselor for college students who are close to graduation. Your program should start by issuing a short message identifying that Eliza is a program and that you are the author.

Please submit your commented source code plus transcripts of two sessions you conduct with Eliza. These sessions should show Eliza to her best advantage and exercise all of the features that you have included in your program.. Each transcript session should include at least 20 "turns" - that is 20 statements by Eliza and 20 by the user.  You should submit a single pdf file to Canvas where your program source code is first, and then your two transcripts follow. I would suggest that you adopt two different personas for each of your transcript sessions - for example you could have session where the user is a very serious motivated student who has very focused career ambitions, and you could have another where the person is less serious or doesn't have any idea about their future.

Your program should engage in a dialogue with the user, with Eliza playing the role of career counselor. Your program should be able carry out "word spotting", that is it should recognize certain key words and respond simply based on that word being present in the input. It should also be able to transform certain simple sentence forms from statements (from the user) into questions (that Eliza will ask). Also, you should personalize the dialogue by asking and using the user's name and other information about the user that can then be used to extend or change the direction of the dialogue. 

Here's part of an example dialogue...

-->[eliza] Hi, I'm Eliza the Career Counselor. I'm a chatbot written by Ted Pedersen.

-> [eliza] What is your name? (question to get things started) 

=> [Ted] My name is Ted. 

-> [eliza] Hi Ted. What is your major? (personalization)

=> [Ted] My major is  Computer Science.

-> [eliza] Do you like Computer Science? (phrase spotting)

=> [Ted] I think so. 

--> [eliza] You sound uncertain. I am concerned. (phrase spotting)

==> [Ted] I'm sorry. 

-> [eliza] Where are you from? (question to spur conversation along)

=> [Ted] Fergus Falls but  can you help me today? 

-> [eliza]  How can I help you today? (transformation)

=> [Ted] What are the best jobs for for Computer Science majors?.

-> [eliza] What do you think the best jobs for Computer Science majors are?  (transformation)

=> [Ted] I don't know, that's why I'm here.

-> [eliza] Ted, I'm concerned that you don't know. Planning a career is serious work, you need to focus! (phrase spotting and memory)

=> [Ted]  This is stupid. I thought you were supposed to help me?

-> [eliza] I need to ask you to maintain a respectful tone!  ("abusive" language detector)

=> [Ted] I would like to have a better idea of how to find a job.

-> [eliza] Would it please you to have a better idea of how to find a job? (transformation)

==> [Ted] YES. That's why I'm here.

--> [eliza] You sound excited. I like enthusiasm Ted! (word spotting and memory)

==> [Ted] Do you review resumes?

--> [eliza] Why do you ask me about resumes? .(word spotting)

==> [Ted] dfjakfadjldafjafdl

--> [eliza] I'm sorry I don't understand. (gibberish detection)

==> [Ted] My keyboard got stuck. 

--> [eliza] Earlier you said you were from Fergus Falls. Tell me how this affects your career plans. (memory, and question to spur conversation along)

==> [Ted] I'd like to live closer to my family.

--> [eliza] Is your family important to you? (word spotting)

....

Your program should be robust. If the user inputs gibberish or a very complicated question, Eliza should respond in some plausible way (I didn't quite understand, can you say that another way, etc.) Also, Eliza should avoid getting stuck in conversational loops, as in "I don't know", "Why don't you know?", "I said I don't know", "Why don't you know?", over and over again.

Your goal should be to have Eliza engage the user for as long as possible, and to handle as broad a range of responses as is possible. Before you develop your implementation, you should think about questions that would be asked of a career counselor, and have Eliza respond in a semi-plausible way to them, but also allow Eliza to carry out an extended career counseling session without really providing much new or useful information to the student. It is important to remember that your Eliza program should not have any specific information about careers or career planning! 

Your program should rely on regular expressions to identify and alter a user's input. Please make certain to explain the intent of your regular expressions in some detail in your comments. 

You will find it very helpful to use functions or the equivalent in this program. For example, you may frequently want to check if a user has input abusive or offensive language (and respond with a polite reminder that such talk is not acceptable). Rather than repeating such a check over and over again, you should simply call a function to do this after each user response. There will no doubt be other things to check for in each response, and also other things Eliza might say more than once during a session that would be best implemented in a subroutine. In general you should not repeat a regular expression that you are using multiple times, rather include those in a subroutine that can be called multiple times (so that if you need to change the regular expression you only change it in one place).

The specific things I will be looking for in grading the functionality of this program are :

1 point - gibberish dection, abusive language check, and word spotting

1 point - use of transformations to convert student statements into Eliza questions

1 point - reliance on regular expressions for pattern matching

1 point - use of memory to interject topics that arose earlier in conversation

1 points - able to avoid dead ends or loops during a 20 turn conversation

Please make sure you follow the commenting standards as outlined in the syllabus and also the programming assignment grading rubric (found in the files section of this site). You should assume that the reader of your code is not familiar with the assignment in general nor with Eliza in particular.

Please remember that you must work on your assignment individually (as described in both the syllabus and the programming assignment grading rubric). You may use pre-existing modules however, please do not use any pre-existing code that is specific to Eliza, chat bots, or dialogue agents. If you aren't sure about some code you would like to use, please check with me before committing to use it. Make sure your comments make it clear what modules you have used and what functionality those provide. Please do not submit screen shots or cut and pastes of terminal windows for your source code or output. Instead, export your code to pdf. Your code should be on a white background with line numbers, output should be on a white background as well. 
