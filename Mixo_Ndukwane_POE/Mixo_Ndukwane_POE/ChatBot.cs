using System;
using System.Text;
using System.Threading;

namespace CyberSecurityChatBot
{
    public class ChatBot
    {
        private readonly string[] _topics = new[]
        {
            "Passwords","Phishing","Malware","Ransomware","Two-Factor Authentication",
            "Social Engineering","Firewalls","VPN","Encryption","Antivirus",
            "Spyware","Adware","Identity Theft","Data Breaches","Public Wi-Fi Safety",
            "Secure Browsing","Email Security","Backup Data","Software Updates",
            "Cloud Security","Cyberbullying","Online Scams","Digital Footprint",
            "Privacy Settings","Safe Downloads"
        };

        public void ShowAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(GetAsciiArt());
            Console.ResetColor();
        }

        public string GetAsciiArt()
        {
            return @"  ____            _                     _                 
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
╚══════╝╚═╝  ╚═╝╚═╝     ╚══════╝╚═╝  ╚═══╝╚══════╝   ╚═╝   ";
        }

        public string[] GetTopics() => _topics;

        public string GetTopicsMenuString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Topics Menu:");
            for (int i = 0; i < _topics.Length; i++)
                sb.AppendLine($"[{i + 1}] ➤ {_topics[i]}");
            return sb.ToString();
        }

        public string HandleBasicQuestionsResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            var lower = input.ToLowerInvariant();
            if (lower.Contains("how are you"))
                return "I'm just a bot, but I'm here and ready to help you!";
            if (lower.Contains("your purpose") || lower.Contains("what do you do"))
                return "My purpose is to help you understand cybersecurity and stay safe online.";
            if (lower.Contains("what can i ask"))
                return "You can ask about passwords, phishing, malware, safe browsing, and more!";
            return null;
        }

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
            return (minDistance <= 3) ? closest : null;
        }

        private static int LevenshteinDistance(string a, string b)
        {
            var dp = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                }
            }
            return dp[a.Length, b.Length];
        }

        public string GetAnswerText(string topic)
        {
            switch (topic)
            {
                case "Passwords":
                    return "Definition: Passwords are secret keys used to access accounts.\nExplanation: Strong passwords protect your accounts from hackers. They should include letters, numbers, and symbols.\nExample: Instead of '123456', use something like 'P@ssw0rd!2026'.";
                case "Phishing":
                    return "Definition: Phishing is a cyber attack that tricks users into giving personal information.\nExplanation: Attackers pretend to be trusted sources like banks or apps.\nExample: You receive an email saying 'Your bank account is locked, click here to fix it'.";
                case "Malware":
                    return "Definition: Malware is malicious software designed to harm systems.\nExplanation: It can steal data, damage files, or spy on users.\nExample: Downloading a fake app that installs a virus.";
                case "Ransomware":
                    return "Definition: Ransomware is malware that locks your files.\nExplanation: Hackers demand payment to restore access.\nExample: A message appears saying 'Pay R500 to unlock your files'.";
                case "Two-Factor Authentication":
                    return "Definition: 2FA is an extra layer of security.\nExplanation: It requires a second verification step after your password.\nExample: Entering a code sent to your phone after logging in.";
                case "Social Engineering":
                    return "Definition: Social engineering is manipulating people to reveal information.\nExplanation: Attackers use trust or fear instead of hacking systems.\nExample: Someone calls pretending to be IT support asking for your password.";
                case "Firewalls":
                    return "Definition: Firewalls are security systems that monitor network traffic.\nExplanation: They block unauthorized access to your device.\nExample: A firewall blocking a suspicious website from connecting.";
                case "VPN":
                    return "Definition: A VPN is a tool that secures your internet connection.\nExplanation: It hides your IP address and encrypts your data.\nExample: Using a VPN on public Wi-Fi at a café.";
                case "Encryption":
                    return "Definition: Encryption is converting data into a secure format.\nExplanation: Only authorized users can read encrypted data.\nExample: WhatsApp messages are encrypted so others can't read them.";
                case "Antivirus":
                    return "Definition: Antivirus is software that detects and removes threats.\nExplanation: It scans your device for harmful programs.\nExample: Windows Defender removing a virus.";
                case "Spyware":
                    return "Definition: Spyware secretly monitors user activity.\nExplanation: It collects data without your knowledge.\nExample: An app tracking your keystrokes.";
                case "Adware":
                    return "Definition: Adware displays unwanted advertisements.\nExplanation: It may track your behavior to show ads.\nExample: Pop-up ads appearing constantly on your screen.";
                case "Identity Theft":
                    return "Definition: Identity theft is stealing personal information.\nExplanation: Criminals use it for fraud or impersonation.\nExample: Someone using your ID to open a bank account.";
                case "Data Breaches":
                    return "Definition: A data breach is unauthorized access to data.\nExplanation: Sensitive information gets exposed.\nExample: A company leak exposing user passwords.";
                case "Public Wi-Fi Safety":
                    return "Definition: Public Wi-Fi safety refers to using shared networks securely.\nExplanation: Public networks are less secure and easy to hack.\nExample: Avoid logging into banking apps on free Wi-Fi.";
                case "Secure Browsing":
                    return "Definition: Secure browsing means using the internet safely.\nExplanation: Avoid risky websites and use HTTPS.\nExample: Checking for a padlock icon in your browser.";
                case "Email Security":
                    return "Definition: Email security protects your inbox from threats.\nExplanation: Avoid unknown links and attachments.\nExample: Ignoring suspicious emails from unknown senders.";
                case "Backup Data":
                    return "Definition: Backup is copying data to a safe location.\nExplanation: It prevents data loss during attacks.\nExample: Saving files to Google Drive or external drive.";
                case "Software Updates":
                    return "Definition: Updates improve and secure software.\nExplanation: They fix vulnerabilities hackers exploit.\nExample: Updating your phone to the latest version.";
                case "Cloud Security":
                    return "Definition: Cloud security protects online stored data.\nExplanation: It ensures safe access to cloud services.\nExample: Using strong passwords for Google Drive.";
                case "Cyberbullying":
                    return "Definition: Cyberbullying is online harassment.\nExplanation: It happens through messages or social media.\nExample: Someone sending threatening texts online.";
                case "Online Scams":
                    return "Definition: Online scams trick users into giving money or info.\nExplanation: They often look like real offers.\nExample: A fake giveaway asking for payment to claim a prize.";
                case "Digital Footprint":
                    return "Definition: Digital footprint is your online activity record.\nExplanation: Everything you post leaves a trace.\nExample: Old social media posts still visible years later.";
                case "Privacy Settings":
                    return "Definition: Privacy settings control who sees your data.\nExplanation: They help protect personal information.\nExample: Setting your Instagram account to private.";
                case "Safe Downloads":
                    return "Definition: Safe downloads involve getting files from trusted sources.\nExplanation: Unsafe downloads may contain malware.\nExample: Downloading apps only from official app stores.";
                default:
                    return "No information available.";
            }
        }

        public void ShowTopicsAndRespond(User user)
        {
            ShowAsciiArt();
            Console.WriteLine(GetTopicsMenuString());
            Console.Write($"\n{user.Name}, what would you like to know? ");
            var input = (Console.ReadLine() ?? string.Empty).Trim();
            var basic = HandleBasicQuestionsResponse(input);
            if (!string.IsNullOrEmpty(basic))
            {
                Console.WriteLine(basic);
                return;
            }
            var match = GetClosestMatch(input.ToLower(), _topics);
            if (match != null)
            {
                Console.WriteLine(GetAnswerText(match));
            }
            else
            {
                Console.WriteLine("I didn't understand. Try a topic from the list.");
            }
        }

        public static void TypeText(string text)
        {
            foreach (var c in text)
            {
                Console.Write(c);
                Thread.Sleep(25);
            }
            Console.WriteLine();
        }
    }
}
