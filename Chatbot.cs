using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public class Chatbot
    {
        private string _userName;
        private ResponseGenerator _responseGenerator;

        public Chatbot(string userName)
        {
            _userName = userName;
            _responseGenerator = new ResponseGenerator();
        }

        public void StartConversation()
        {
            Console.WriteLine($"\nHello, {_userName}!");
            Console.WriteLine("Type 'exit' to end the conversation.\n");

            ProcessUserInput();
        }

        private void ProcessUserInput()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{_userName}> ");
                Console.ResetColor();

                string userInput = Console.ReadLine();

                // Input validation - check for empty input
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Bot] Please say something!");
                    Console.ResetColor();
                    continue;
                }

                if (_responseGenerator.IsExitCommand(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nGoodbye, {_userName}! Stay safe online!");
                    Console.ResetColor();
                    break;
                }

                string response = _responseGenerator.GenerateResponse(userInput);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[Bot] {response}");
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }
}