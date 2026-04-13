
using System;

namespace CyberSecurityChatBot
{
    public class User
    {
        public string Name { get; private set; } = string.Empty;

        public void PromptName()
        {
            do
            {
                Console.Write("\nEnter your name: ");
                Name = Console.ReadLine() ?? string.Empty;
            }
            while (string.IsNullOrWhiteSpace(Name));
        }
    }
}
