using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityChatbot
{
    public class Intent
    {
        public string Name { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Responses { get; set; }

        public Intent()
        {
            Keywords = new List<string>();
            Responses = new List<string>();
        }

        public Intent(string name, List<string> keywords, List<string> responses)
        {
            Name = name;
            Keywords = keywords;
            Responses = responses;
        }

        public bool Matches(string input)
        {
            string lowerInput = input.ToLower();
            return Keywords.Any(keyword => lowerInput.Contains(keyword.ToLower()));
        }

        public string GetResponse()
        {
            if (Responses == null || Responses.Count == 0)
                return "I am unable to give you a valid response";

            Random random = new Random();
            int index = random.Next(Responses.Count);
            return Responses[index];
        }
    }
}