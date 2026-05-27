
using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Xml.Linq;

namespace CyberSecurityChatBot   // Defines the namespace for the application
{
    internal static class Program // Main class of the application
    {
        private static void Main() // Entry point of the program
        {
            // Create instances of required classes
            var audio = new SoundPlayerService(); // Handles audio playback
            var user = new User();                // Handles user-related data (e.g., name)
            var bot = new ChatBot();              // Handles chatbot logic and responses

            bot.ShowAsciiArt();   // Display ASCII art for branding/visual appeal
            audio.PlayGreeting(); // Play greeting sound when the app starts

            user.PromptName();    // Ask the user to enter their name

            // Display welcome message section
            Console.WriteLine("\n=====================================");
            ChatBot.TypeText($"Welcome, {user.Name}!"); // Display user's name
            ChatBot.TypeText("Cybersecurity Awareness Bot Activated."); // Bot intro
            ChatBot.TypeText("Your safety online is my priority.");     // Purpose message
            Console.WriteLine("=====================================\n");

            // Display heading for user interaction
            Console.ForegroundColor = ConsoleColor.Blue; // Change text color to blue
            Console.WriteLine("\n══════════════════════════════════════");
            Console.WriteLine(" HOW CAN I ASSIST YOU TODAY?");
            Console.WriteLine("══════════════════════════════════════");
            Console.ResetColor(); // Reset text color to default

            bool continueChat = true; // Controls whether the chat continues

            // Main chat loop
            while (continueChat)
            {
                bot.ShowTopicsAndRespond(user); // Show topics and respond based on user input

                string response; // Variable to store user's decision

                // Loop to validate user input (yes/no)
                do
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\n Do you have another question? (yes/no): ");
                    Console.ResetColor();

                    // Read and clean user input
                    response = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

                    if (response == "yes" || response == "y")
                    {
                        continueChat = true; // Continue chat
                        break;
                    }
                    else if (response == "no" || response == "n")
                    {
                        // Exit message when user ends chat
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nGoodbye, {user.Name}! Stay safe online ");
                        Console.ResetColor();

                        continueChat = false; // Stop chat loop
                        break;
                    }
                    else
                    {
                        // Handle invalid input
                        Console.WriteLine("Please enter 'yes' or 'no'.");
                    }

                } while (true); // Repeat until valid input is entered
            }
        }
    }
}

