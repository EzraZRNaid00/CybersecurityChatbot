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
            // Existing Greeting intent
            _intents.Add(new Intent(
                "Greeting",
                new List<string> { "how are you", "hello", "hi" },
                new List<string> {
            "I'm doing well! How can I help you today?",
            "Hello! Ready to learn about cybersecurity?"
                }
            ));

            // Add Purpose intent
            _intents.Add(new Intent(
                "Purpose",
                new List<string> { "purpose", "what do you do", "what can you do" },
                new List<string> {
            "I'm here to help you stay safe online! I can teach you about passwords, phishing, and safe browsing."
                }
            ));

            // Add Password intent
            _intents.Add(new Intent(
                "Password",
                new List<string> { "password", "pass" },
                new List<string> {
            "🔐 Password Safety:\n• Use a unique password for each account\n• Make passwords at least 12 characters long\n• Use a mix of uppercase, lowercase, numbers, and symbols"
                }
            ));

            // Add Phishing intent
            _intents.Add(new Intent(
                "Phishing",
                new List<string> { "phish", "phishing", "scam" },
                new List<string> {
            "🎣 Phishing Awareness:\n• Never click suspicious links\n• Check sender email addresses carefully\n• Legitimate companies won't ask for passwords via email"
                }
            ));

            // Add Safe Browsing intent
            _intents.Add(new Intent(
                "Browsing",
                new List<string> { "brows", "browsing", "safe online" },
                new List<string> {
            "🌐 Safe Browsing Tips:\n• Look for 'https://' in the address bar\n• Avoid downloading from untrusted sources\n• Keep your browser updated"
                }
            ));

            // Add Goodbye intent
            _intents.Add(new Intent(
                "Goodbye",
                new List<string> { "exit", "quit", "bye", "goodbye" },
                new List<string> {
            "Thank you for chatting! Stay safe online!"
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
