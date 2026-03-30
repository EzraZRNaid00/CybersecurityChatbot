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
                Console.Write($"{_userName}> ");
                string userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("[Bot] Please say something!");
                    continue;
                }

                if (_responseGenerator.IsExitCommand(userInput))
                {
                    Console.WriteLine($"\nGoodbye, {_userName}! Stay safe!");
                    break;
                }

                string response = _responseGenerator.GenerateResponse(userInput);
                Console.WriteLine($"[Bot] {response}");
                Console.WriteLine();
            }
        }
    }
}