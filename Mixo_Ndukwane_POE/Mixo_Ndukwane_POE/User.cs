using System;

namespace CyberSecurityChatBot
{
    public class User// Class responsible for storing and handling user information
    {
        public string Name { get; private set; } = string.Empty;// Stores user's name (can only be set inside this class)

        public void PromptName()// Method to ask user for their name
        {
            do
            {
                Console.Write("\nEnter your name: ");// Prompts user to enter name
                Name = Console.ReadLine() ?? string.Empty;// Reads user input and prevents null values

            }
            while (string.IsNullOrWhiteSpace(Name)); // Repeats until a valid (non-empty) name is entered
        }

        // Allow GUI code to set the name programmatically
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            Name = name.Trim();
        }
    }
}


