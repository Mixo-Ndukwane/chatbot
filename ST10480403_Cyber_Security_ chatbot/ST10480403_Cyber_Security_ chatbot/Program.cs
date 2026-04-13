using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Xml.Linq;

namespace CyberSecurityChatBot
{
    internal static class Program
    {
        private static void Main()
        {
            var audio = new SoundPlayerService();
            var user = new User();
            var bot = new ChatBot();

            bot.ShowAsciiArt();
            audio.PlayGreeting();

            user.PromptName();
            Console.WriteLine("\n=====================================");
            ChatBot.TypeText($"Welcome, {user.Name}!");
            ChatBot.TypeText("Cybersecurity Awareness Bot Activated.");
            ChatBot.TypeText("Your safety online is my priority.");
            Console.WriteLine("=====================================\n");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n══════════════════════════════════════");
            Console.WriteLine(" HOW CAN I ASSIST YOU TODAY?");
            Console.WriteLine("══════════════════════════════════════");
            Console.ResetColor();

            bool continueChat = true;

            while (continueChat)
            {
                bot.ShowTopicsAndRespond(user);

                string response;
                do
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\n Do you have another question? (yes/no): ");
                    Console.ResetColor();
                    response = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

                    if (response == "yes" || response == "y")
                    {
                        continueChat = true;
                        break;
                    }
                    else if (response == "no" || response == "n")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nGoodbye, {user.Name}! Stay safe online ");
                        Console.ResetColor();
                        continueChat = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please enter 'yes' or 'no'.");
                    }

                } while (true);
            }
        }
    }
}