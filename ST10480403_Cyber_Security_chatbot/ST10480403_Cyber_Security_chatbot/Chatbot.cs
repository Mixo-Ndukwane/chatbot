using CyberSecurityChatBot;   // Allows access to classes within the same namespace

using System;                // Provides basic system functionality (Console, Math, etc.)
using System.Threading;      // Enables delays (Thread.Sleep)

namespace CyberSecurityChatBot // Groups related classes together
{
    public class ChatBot // Main chatbot class handling interaction and responses
    {
        // Array storing all cybersecurity topics available in the bot
        private readonly string[] _topics = new[]
        {
            "Passwords","Phishing","Malware","Ransomware","Two-Factor Authentication",
            "Social Engineering","Firewalls","VPN","Encryption","Antivirus",
            "Spyware","Adware","Identity Theft","Data Breaches","Public Wi-Fi Safety",
            "Secure Browsing","Email Security","Backup Data","Software Updates",
            "Cloud Security","Cyberbullying","Online Scams","Digital Footprint",
            "Privacy Settings","Safe Downloads"
        };

        // Displays ASCII art for branding/visual appeal
        public void ShowAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Blue; // Set text color

            Console.WriteLine(@"
  ____            _                     _                 
 / ___| _   _ ___| |_ ___ _ __ ___  ___| |_ ___  _ __ ___ 
 \___ \| | | / __| __/ _ \ '__/ __|/ _ \ __/ _ \| '__/ __|
  ___) | |_| \__ \ ||  __/ |  \__ \  __/ || (_) | |  \__ \
 |____/ \__, |___/\__\___|_|  |___/\___|\__\___/|_|  |___/
        |___/                                             
███████╗ █████╗ ███████╗███████╗███╗   ██╗███████╗████████╗
██╔════╝██╔══██╗██╔════╝██╔════╝████╗  ██║██╔════╝╚══██╔══╝
███████╗███████║█████╗  █████╗  ██╔██╗ ██║█████╗     ██║   
╚════██║██╔══██║██╔══╝  ██╔══╝  ██║╚██╗██║██╔══╝     ██║   
███████║██║  ██║██║     ███████╗██║ ╚████║███████╗   ██║   
╚══════╝╚═╝  ╚═╝╚═╝     ╚══════╝╚═╝  ╚═══╝╚══════╝   ╚═╝   

   SAFENET CYBERSECURITY AWARENESS BOT
");

            Console.ResetColor(); // Reset text color
        }

        // Displays topics and handles user input + responses
        public void ShowTopicsAndRespond(User user)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n════════════  TOPICS MENU ════════════\n");
            Console.ResetColor();

            // Loop through topics and display them as a numbered list
            for (int i = 0; i < _topics.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[{i + 1}] ➤ {_topics[i]}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n═══════════════════════════════════════\n");
            Console.ResetColor();

            // Prompt user for input
            Console.Write($"\n {user.Name}, what would you like to know? ");

            string input = (Console.ReadLine() ?? string.Empty).Trim().ToLower();

            // Handle general/basic questions
            if (HandleBasicQuestions(input))
                return;

            // Try to find closest matching topic if user input is incorrect
            string closestMatch = GetClosestMatch(input, _topics);

            // Ask user for confirmation if a close match is found
            if (closestMatch != null && closestMatch.ToLower() != input)
            {
                Console.Write($"Did you mean '{closestMatch}'? (yes/no): ");
                string confirm = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

                if (confirm == "yes" || confirm == "y")
                {
                    input = closestMatch.ToLower(); // Replace input with corrected topic
                }
            }

            int index = -1; // Stores selected topic index

            // Check if input is a number (menu selection)
            if (int.TryParse(input, out int number))
            {
                if (number >= 1 && number <= _topics.Length)
                    index = number - 1;
            }
            else
            {
                // Check if input matches a topic name
                for (int i = 0; i < _topics.Length; i++)
                {
                    if (_topics[i].ToLower() == input)
                    {
                        index = i;
                        break;
                    }
                }
            }

            // Handle invalid input
            if (index == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n Invalid choice.");
                Console.WriteLine("I didn't quite understand that, could you rephrase?");
                Console.ResetColor();
                return;
            }

            // Display selected topic heading
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n════════════  {_topics[index].ToUpper()} ════════════");
            Console.ResetColor();

            // Provide explanation for selected topic
            GiveAnswer(_topics[index]);
        }

        // Handles simple conversational questions
        private bool HandleBasicQuestions(string input)
        {
            if (input.Contains("how are you"))
            {
                Console.WriteLine("I'm just a bot, but I'm here and ready to help you!");
                return true;
            }
            else if (input.Contains("your purpose") || input.Contains("what do you do"))
            {
                Console.WriteLine("My purpose is to help you understand cybersecurity and stay safe online.");
                return true;
            }
            else if (input.Contains("what can i ask"))
            {
                Console.WriteLine("You can ask about passwords, phishing, malware, safe browsing, and more!");
                return true;
            }

            return false; // No match found
        }

        // Finds the closest matching topic using Levenshtein Distance
        private string GetClosestMatch(string input, string[] topics)
        {
            int minDistance = int.MaxValue;
            string closest = null;

            foreach (var topic in topics)
            {
                int distance = LevenshteinDistance(input, topic.ToLower());

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = topic;
                }
            }

            // Only return match if similarity is close enough
            return (minDistance <= 3) ? closest : null;
        }

        // Algorithm to calculate difference between two strings
        private static int LevenshteinDistance(string a, string b)
        {
            int[,] dp = new int[a.Length + 1, b.Length + 1];

            // Initialize base cases
            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

            // Fill matrix
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;

                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }

            return dp[a.Length, b.Length]; // Final distance
        }

        // Provides explanations for each cybersecurity topic
        private void GiveAnswer(string topic)
        {
            switch (topic)
            {
                // Each case gives definition, explanation, and example
                case "Passwords":
                    Console.WriteLine("Definition: Passwords are secret keys used to access accounts.");
                    Console.WriteLine("Explanation: Strong passwords protect your accounts from hackers. They should include letters, numbers, and symbols.");
                    Console.WriteLine("Example: Instead of '123456', use something like 'P@ssw0rd!2026'.");
                    break;

                case "Phishing":
                    Console.WriteLine("Definition: Phishing is a cyber attack that tricks users into giving personal information.");
                    Console.WriteLine("Explanation: Attackers pretend to be trusted sources like banks or apps.");
                    Console.WriteLine("Example: You receive an email saying 'Your bank account is locked, click here to fix it'.");
                    break;

                case "Malware":
                    Console.WriteLine("Definition: Malware is malicious software designed to harm systems.");
                    Console.WriteLine("Explanation: It can steal data, damage files, or spy on users.");
                    Console.WriteLine("Example: Downloading a fake app that installs a virus.");
                    break;

                case "Ransomware":
                    Console.WriteLine("Definition: Ransomware is malware that locks your files.");
                    Console.WriteLine("Explanation: Hackers demand payment to restore access.");
                    Console.WriteLine("Example: A message appears saying 'Pay R500 to unlock your files'.");
                    break;

                case "Two-Factor Authentication":
                    Console.WriteLine("Definition: 2FA is an extra layer of security.");
                    Console.WriteLine("Explanation: It requires a second verification step after your password.");
                    Console.WriteLine("Example: Entering a code sent to your phone after logging in.");
                    break;

                case "Social Engineering":
                    Console.WriteLine("Definition: Social engineering is manipulating people to reveal information.");
                    Console.WriteLine("Explanation: Attackers use trust or fear instead of hacking systems.");
                    Console.WriteLine("Example: Someone calls pretending to be IT support asking for your password.");
                    break;

                case "Firewalls":
                    Console.WriteLine("Definition: Firewalls are security systems that monitor network traffic.");
                    Console.WriteLine("Explanation: They block unauthorized access to your device.");
                    Console.WriteLine("Example: A firewall blocking a suspicious website from connecting.");
                    break;

                case "VPN":
                    Console.WriteLine("Definition: A VPN is a tool that secures your internet connection.");
                    Console.WriteLine("Explanation: It hides your IP address and encrypts your data.");
                    Console.WriteLine("Example: Using a VPN on public Wi-Fi at a café.");
                    break;

                case "Encryption":
                    Console.WriteLine("Definition: Encryption is converting data into a secure format.");
                    Console.WriteLine("Explanation: Only authorized users can read encrypted data.");
                    Console.WriteLine("Example: WhatsApp messages are encrypted so others can't read them.");
                    break;

                case "Antivirus":
                    Console.WriteLine("Definition: Antivirus is software that detects and removes threats.");
                    Console.WriteLine("Explanation: It scans your device for harmful programs.");
                    Console.WriteLine("Example: Windows Defender removing a virus.");
                    break;

                case "Spyware":
                    Console.WriteLine("Definition: Spyware secretly monitors user activity.");
                    Console.WriteLine("Explanation: It collects data without your knowledge.");
                    Console.WriteLine("Example: An app tracking your keystrokes.");
                    break;

                case "Adware":
                    Console.WriteLine("Definition: Adware displays unwanted advertisements.");
                    Console.WriteLine("Explanation: It may track your behavior to show ads.");
                    Console.WriteLine("Example: Pop-up ads appearing constantly on your screen.");
                    break;

                case "Identity Theft":
                    Console.WriteLine("Definition: Identity theft is stealing personal information.");
                    Console.WriteLine("Explanation: Criminals use it for fraud or impersonation.");
                    Console.WriteLine("Example: Someone using your ID to open a bank account.");
                    break;

                case "Data Breaches":
                    Console.WriteLine("Definition: A data breach is unauthorized access to data.");
                    Console.WriteLine("Explanation: Sensitive information gets exposed.");
                    Console.WriteLine("Example: A company leak exposing user passwords.");
                    break;

                case "Public Wi-Fi Safety":
                    Console.WriteLine("Definition: Public Wi-Fi safety refers to using shared networks securely.");
                    Console.WriteLine("Explanation: Public networks are less secure and easy to hack.");
                    Console.WriteLine("Example: Avoid logging into banking apps on free Wi-Fi.");
                    break;

                case "Secure Browsing":
                    Console.WriteLine("Definition: Secure browsing means using the internet safely.");
                    Console.WriteLine("Explanation: Avoid risky websites and use HTTPS.");
                    Console.WriteLine("Example: Checking for a padlock icon in your browser.");
                    break;

                case "Email Security":
                    Console.WriteLine("Definition: Email security protects your inbox from threats.");
                    Console.WriteLine("Explanation: Avoid unknown links and attachments.");
                    Console.WriteLine("Example: Ignoring suspicious emails from unknown senders.");
                    break;

                case "Backup Data":
                    Console.WriteLine("Definition: Backup is copying data to a safe location.");
                    Console.WriteLine("Explanation: It prevents data loss during attacks.");
                    Console.WriteLine("Example: Saving files to Google Drive or external drive.");
                    break;

                case "Software Updates":
                    Console.WriteLine("Definition: Updates improve and secure software.");
                    Console.WriteLine("Explanation: They fix vulnerabilities hackers exploit.");
                    Console.WriteLine("Example: Updating your phone to the latest version.");
                    break;

                case "Cloud Security":
                    Console.WriteLine("Definition: Cloud security protects online stored data.");
                    Console.WriteLine("Explanation: It ensures safe access to cloud services.");
                    Console.WriteLine("Example: Using strong passwords for Google Drive.");
                    break;

                case "Cyberbullying":
                    Console.WriteLine("Definition: Cyberbullying is online harassment.");
                    Console.WriteLine("Explanation: It happens through messages or social media.");
                    Console.WriteLine("Example: Someone sending threatening texts online.");
                    break;

                case "Online Scams":
                    Console.WriteLine("Definition: Online scams trick users into giving money or info.");
                    Console.WriteLine("Explanation: They often look like real offers.");
                    Console.WriteLine("Example: A fake giveaway asking for payment to claim a prize.");
                    break;

                case "Digital Footprint":
                    Console.WriteLine("Definition: Digital footprint is your online activity record.");
                    Console.WriteLine("Explanation: Everything you post leaves a trace.");
                    Console.WriteLine("Example: Old social media posts still visible years later.");
                    break;

                case "Privacy Settings":
                    Console.WriteLine("Definition: Privacy settings control who sees your data.");
                    Console.WriteLine("Explanation: They help protect personal information.");
                    Console.WriteLine("Example: Setting your Instagram account to private.");
                    break;

                case "Safe Downloads":
                    Console.WriteLine("Definition: Safe downloads involve getting files from trusted sources.");
                    Console.WriteLine("Explanation: Unsafe downloads may contain malware.");
                    Console.WriteLine("Example: Downloading apps only from official app stores.");
                    break;

                default:
                    Console.WriteLine("No information available.");
                    break;
            }
        }
        // Creates typing animation effect for text output
        public static void TypeText(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c); // Print character one by one
                Thread.Sleep(25); // Delay to simulate typing
            }
            Console.WriteLine(); // Move to next line after printing
        }
    }
}