using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public class ResponseGenerator
    {
        private List<Intent> _intents;
        private string _defaultResponse;

        public ResponseGenerator()
        {
            _intents = new List<Intent>();
            _defaultResponse = "I didn't quite understand that. Could you rephrase?";
            LoadIntents();
        }

        private void LoadIntents()
        {
            // Start with just ONE intent to test
            _intents.Add(new Intent(
                "Greeting",
                new List<string> { "how are you", "hello", "hi" },
                new List<string> {
                    "I'm doing well! How can I help you today?"
                }
            ));
        }

        public string GenerateResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return _defaultResponse;

            string lowerInput = userInput.ToLower().Trim();

            foreach (Intent intent in _intents)
            {
                if (intent.Matches(lowerInput))
                {
                    return intent.GetResponse();
                }
            }

            return _defaultResponse;
        }

        public bool IsExitCommand(string userInput)
        {
            string lowerInput = userInput.ToLower().Trim();
            return lowerInput == "exit" || lowerInput == "quit" || lowerInput == "bye";
        }

        public List<string> GetAvailableTopics()
        {
            List<string> topics = new List<string>();
            foreach (Intent intent in _intents)
            {
                if (intent.Name != "Greeting")
                {
                    topics.Add(intent.Name);
                }
            }
            return topics;
        }
    }
}
