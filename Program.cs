// See https://aka.ms/new-console-template for more information
using System;

namespace CybersecurityChatbot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Bot";

            // Get user name
            Console.Write("Please enter your name: ");
            string userName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = "User";
            }

            // Capitalize first letter
            userName = char.ToUpper(userName[0]) + userName.Substring(1).ToLower();


        }
    }
}
