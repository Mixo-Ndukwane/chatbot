using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatBot_GUI
{
    public partial class Form1 : Form
    {
        // ── state ──────────────────────────────────────────────────────────────
        private string _userName     = null;
        private string _lastTopic    = null;
        private bool   _awaitingName = true;

        private readonly string[] _topics = {
            "Passwords","Phishing","Malware","Ransomware","Two-Factor Authentication",
            "Social Engineering","Firewalls","VPN","Encryption","Antivirus",
            "Spyware","Adware","Identity Theft","Data Breaches","Public Wi-Fi Safety",
            "Secure Browsing","Email Security","Backup Data","Software Updates",
            "Cloud Security","Cyberbullying","Online Scams","Digital Footprint",
            "Privacy Settings","Safe Downloads"
        };

        private readonly KeywordResponder   _responder = new KeywordResponder();
        private readonly SentimentDetector  _sentiment = new SentimentDetector();
        private readonly MemoryStore        _memory    = new MemoryStore();
        private readonly SoundPlayerService _audio     = new SoundPlayerService();

        // ── constructor ────────────────────────────────────────────────────────
        public Form1()
        {
            InitializeComponent();
            Task.Run(() => _audio.PlayGreeting());
            BotSay("Hello! What is your name?");
        }

        private void Form1_Load(object sender, EventArgs e) { }

        // ── send ───────────────────────────────────────────────────────────────
        private void btnSend_Click(object sender, EventArgs e)
        {
            var text = inputBox.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            btnSend.Enabled  = false;
            inputBox.Enabled = false;

            try
            {
                UserSay(text);
                inputBox.Clear();

                if (_awaitingName)
                {
                    _userName        = text;
                    _memory.UserName = _userName;
                    _awaitingName    = false;
                    BotSay($"Hi {_userName}, welcome to SafeNet Cybersecurity Bot!\n\n" +
                           "Ask me anything about cybersecurity — passwords, phishing, malware, VPNs, and more. " +
                           "Type 'topics' to see the full topic list.");
                }
                else
                {
                    ProcessInput(text);
                }
            }
            catch (Exception ex)
            {
                BotSay($"Something went wrong: {ex.Message}. Please try again.");
            }
            finally
            {
                btnSend.Enabled  = true;
                inputBox.Enabled = true;
                inputBox.Focus();
            }
        }

        // ── main logic ─────────────────────────────────────────────────────────
        private void ProcessInput(string text)
        {
            var lower = text.ToLowerInvariant();

            // ── "explain more" follow-up ───────────────────────────────────────
            if (IsFollowUp(lower))
            {
                if (_lastTopic != null)
                {
                    int times = _memory.TimesAsked(_lastTopic);
                    string timesNote = times >= 2
                        ? $" (you've asked about {_lastTopic} {OrdinalSuffix(times)} time — great dedication!)"
                        : "";
                    BotSay($"Sure {_userName}, here is a deeper explanation of {_lastTopic}{timesNote}:\n\n" +
                           GetDeepExplanation(_lastTopic));
                }
                else
                    BotSay($"Which topic would you like me to explain further, {_userName}? " +
                           "Just ask about it by name or type 'topics' to see the list.");
                return;
            }

            // ── number selection (1–25) ────────────────────────────────────────
            if (int.TryParse(text.Trim(), out int num) && num >= 1 && num <= _topics.Length)
            {
                var topic = _topics[num - 1];
                _lastTopic = topic;
                _memory.RecordTopic(topic);
                BotSay(PersonalisedTopicResponse(topic));
                return;
            }

            // ── detect sentiment ───────────────────────────────────────────────
            var sentiment  = _sentiment.Detect(lower);
            string sentPfx = _sentiment.GetPrefix(sentiment, _userName);
            _memory.Store("lastSentiment", sentiment.ToString());

            // ── basic conversational phrases ───────────────────────────────────
            var basic = HandleBasicResponse(lower);
            if (basic != null) { BotSay(sentPfx + basic); return; }

            // ── "topics" command ───────────────────────────────────────────────
            if (lower == "topics" || lower == "topic" || lower == "topic list" ||
                lower == "show topics" || lower == "list topics")
            {
                var sb = new StringBuilder();
                sb.AppendLine("Here are all the topics I can help with:\n");
                for (int i = 0; i < _topics.Length; i++)
                    sb.AppendLine($"  {i + 1,2}. {_topics[i]}");
                string fav = _memory.FavouriteTopic;
                if (!string.IsNullOrEmpty(fav))
                    sb.AppendLine($"\nBased on your interest, you might want to revisit {fav}.");
                else
                    sb.AppendLine("\nJust ask about any topic by name or number.");
                BotSay(sb.ToString());
                return;
            }

            // ── keyword → full topic answer ────────────────────────────────────
            var matchedTopic = _responder.GetTopic(lower);
            if (matchedTopic != null)
            {
                _lastTopic = matchedTopic;
                _memory.RecordTopic(matchedTopic);
                BotSay(sentPfx + PersonalisedTopicResponse(matchedTopic));
                return;
            }

            // ── fuzzy topic name match (handles typos) ─────────────────────────
            var fuzzy = GetClosestMatch(lower, _topics);
            if (fuzzy != null)
            {
                _lastTopic = fuzzy;
                _memory.RecordTopic(fuzzy);
                BotSay(sentPfx + PersonalisedTopicResponse(fuzzy));
                return;
            }

            // ── pure sentiment message (no topic found) ────────────────────────
            if (sentiment != Sentiment.Neutral)
            {
                var sentResponse = _sentiment.GetStandaloneResponse(sentiment, _userName);
                if (sentResponse != null)
                {
                    string hint = _memory.GetFavouriteTopicHint();
                    BotSay(string.IsNullOrEmpty(hint)
                        ? sentResponse
                        : sentResponse + "\n\n" + hint);
                    return;
                }
            }

            // ── fallback ───────────────────────────────────────────────────────
            string fallback = $"I'm not sure about that, {_userName}. " +
                              "Try asking about a topic like 'what is phishing' or 'explain malware'. " +
                              "Type 'topics' to see the full topic list.";
            string favHint = _memory.GetFavouriteTopicHint();
            if (!string.IsNullOrEmpty(favHint))
                fallback += $"\n\n{favHint}You can ask me more about it anytime.";
            BotSay(fallback);
        }

        // ── personalised topic response ────────────────────────────────────────
        private string PersonalisedTopicResponse(string topic)
        {
            int times  = _memory.TimesAsked(topic);
            string fav = _memory.FavouriteTopic;
            string second = _memory.SecondFavouriteTopic;

            var sb = new StringBuilder();

            // Acknowledge repeat visits
            if (times >= 2)
                sb.AppendLine($"You've asked about {topic} before — here's a refresher:\n");

            sb.AppendLine(GetAnswerText(topic));
            sb.AppendLine("\nType 'explain more' for a deeper explanation.");

            // After 3+ unique topics, note a connection to their favourite
            if (_memory.UniqueTopicCount >= 3 && fav != null && fav != topic)
                sb.AppendLine($"\nSince you're interested in {fav}, " +
                              $"you might find {topic} connects closely to it.");

            // Suggest second favourite on first visit to a new topic
            if (_memory.HasMultipleTopics && second != null && second != topic && times == 1)
                sb.AppendLine($"\nYou've also shown interest in {second} — " +
                              "feel free to ask about that too.");

            return sb.ToString().TrimEnd();
        }

        // ── ordinal suffix ─────────────────────────────────────────────────────
        private static string OrdinalSuffix(int n)
        {
            if (n % 100 >= 11 && n % 100 <= 13) return n + "th";
            switch (n % 10)
            {
                case 1: return n + "st";
                case 2: return n + "nd";
                case 3: return n + "rd";
                default: return n + "th";
            }
        }

        // ── helpers ────────────────────────────────────────────────────────────
        private bool IsFollowUp(string s) =>
            s.Contains("explain more") || s.Contains("tell me more") ||
            s.Contains("more detail")  || s.Contains("don't understand") ||
            s.Contains("dont understand") || s.Contains("not clear") ||
            s.Contains("elaborate")    || s.Contains("confused") ||
            s.Contains("i don't get")  || s.Contains("what does that mean");

        private string HandleBasicResponse(string lower)
        {
            if (lower.Contains("how are you"))
                return $"I'm doing great and ready to help you stay safe online, {_userName}!";
            if (lower.Contains("your purpose") || lower.Contains("what do you do"))
                return "My purpose is to help you understand cybersecurity and stay safe online.";
            if (lower.Contains("what can i ask") || lower.Contains("what can you do"))
                return "You can ask me about passwords, phishing, malware, VPNs, encryption, and much more! " +
                       "Type 'topics' to see the full topic list.";
            if (lower.Contains("thank"))
                return $"You're welcome, {_userName}! Stay safe online.";
            if (lower == "hi" || lower.StartsWith("hi ") || lower.Contains("hello"))
                return $"Hello again, {_userName}! What cybersecurity topic can I help you with today?";
            return null;
        }

        private string GetClosestMatch(string input, string[] topics)
        {
            int best = int.MaxValue; string closest = null;
            foreach (var t in topics)
            {
                int d = Levenshtein(input, t.ToLower());
                if (d < best) { best = d; closest = t; }
            }
            return best <= 3 ? closest : null;
        }

        private static int Levenshtein(string a, string b)
        {
            var dp = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;
            for (int i = 1; i <= a.Length; i++)
                for (int j = 1; j <= b.Length; j++)
                {
                    int c = a[i - 1] == b[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + c);
                }
            return dp[a.Length, b.Length];
        }

        // ── chat display ───────────────────────────────────────────────────────
        private void BotSay(string message)
        {
            var ts = DateTime.Now.ToString("HH:mm");
            bubblePanel.AddBubble("Bot", ts, message, isUser: false);
        }

        private void UserSay(string message)
        {
            var name = _awaitingName ? "You" : (_userName ?? "You");
            var ts   = DateTime.Now.ToString("HH:mm");
            bubblePanel.AddBubble(name, ts, message, isUser: true);
        }

        // ── topic answers ──────────────────────────────────────────────────────
        private string GetAnswerText(string topic)
        {
            switch (topic)
            {
                case "Passwords":
                    return "PASSWORDS\n\nDefinition: A password is a secret string used to verify your identity.\n\nExplanation: Strong passwords protect your accounts from hackers. They should be at least 12 characters and include uppercase, lowercase, numbers, and symbols.\n\nExample: Instead of '123456', use 'P@ssw0rd!2026' or a passphrase like 'BlueSky$Rain#42'.";
                case "Phishing":
                    return "PHISHING\n\nDefinition: Phishing is a cyber attack that tricks users into revealing personal information.\n\nExplanation: Attackers disguise themselves as trusted sources — banks, social media, or government agencies — to steal passwords or credit card numbers.\n\nExample: You receive an email: 'Your FNB account is locked. Click here to verify.' The link leads to a fake site that steals your login.";
                case "Malware":
                    return "MALWARE\n\nDefinition: Malware is malicious software designed to harm or exploit devices.\n\nExplanation: It includes viruses, worms, trojans, and spyware. It can steal data, slow your device, or give attackers remote control.\n\nExample: You download a free game from an unofficial site. Hidden inside is a keylogger that records everything you type.";
                case "Ransomware":
                    return "RANSOMWARE\n\nDefinition: Ransomware is malware that encrypts your files and demands payment to restore them.\n\nExplanation: Once infected, all your documents and photos become inaccessible. Attackers demand cryptocurrency payment.\n\nExample: A pop-up appears: 'Your files are encrypted. Pay R2000 in Bitcoin within 48 hours or lose everything.'";
                case "Two-Factor Authentication":
                    return "TWO-FACTOR AUTHENTICATION (2FA)\n\nDefinition: 2FA adds a second verification step beyond your password.\n\nExplanation: Even if someone steals your password, they still can't log in without the second factor — usually a code sent to your phone.\n\nExample: You log into Gmail with your password, then receive a 6-digit SMS code that you must also enter.";
                case "Social Engineering":
                    return "SOCIAL ENGINEERING\n\nDefinition: Social engineering manipulates people psychologically to reveal confidential information.\n\nExplanation: Instead of hacking systems, attackers hack people — using trust, urgency, or fear.\n\nExample: Someone calls claiming to be IT support: 'We detected a breach. I need your password to fix it immediately.'";
                case "Firewalls":
                    return "FIREWALLS\n\nDefinition: A firewall monitors and controls incoming and outgoing network traffic.\n\nExplanation: It acts as a barrier between your trusted network and untrusted external networks, blocking suspicious connections.\n\nExample: Your firewall blocks an unknown program from sending your data to a server in another country.";
                case "VPN":
                    return "VPN (Virtual Private Network)\n\nDefinition: A VPN encrypts your internet connection and hides your IP address.\n\nExplanation: It creates a secure tunnel between your device and the internet, protecting your data from eavesdroppers on public Wi-Fi.\n\nExample: At a coffee shop, with a VPN your banking session is encrypted and invisible to anyone on the same network.";
                case "Encryption":
                    return "ENCRYPTION\n\nDefinition: Encryption converts readable data into an unreadable format that only authorised parties can decode.\n\nExplanation: It uses mathematical algorithms and keys. Without the correct key, encrypted data looks like random gibberish.\n\nExample: WhatsApp messages are encrypted end-to-end — only you and the recipient can read them.";
                case "Antivirus":
                    return "ANTIVIRUS\n\nDefinition: Antivirus software detects, prevents, and removes malicious software.\n\nExplanation: It scans files against a database of known threats and uses behaviour analysis to catch new ones.\n\nExample: You download an email attachment. Your antivirus detects a trojan and quarantines the file before it can run.";
                case "Spyware":
                    return "SPYWARE\n\nDefinition: Spyware secretly monitors your activity and sends data to a third party.\n\nExplanation: It can record keystrokes, capture screenshots, and steal passwords — all without your knowledge.\n\nExample: A free app you installed is silently logging every website you visit and sending the data to advertisers.";
                case "Adware":
                    return "ADWARE\n\nDefinition: Adware automatically displays or downloads unwanted advertisements.\n\nExplanation: It slows your device, tracks your behaviour, and can lead to more serious malware.\n\nExample: After installing a free PDF converter, your browser is flooded with pop-up ads and your homepage has changed.";
                case "Identity Theft":
                    return "IDENTITY THEFT\n\nDefinition: Identity theft is when someone steals your personal information to commit fraud.\n\nExplanation: Criminals use your ID number or banking details to open accounts or make purchases in your name.\n\nExample: A hacker uses your leaked ID number to open a store account and run up debt in your name.";
                case "Data Breaches":
                    return "DATA BREACHES\n\nDefinition: A data breach is an incident where sensitive information is accessed without authorisation.\n\nExplanation: Breaches expose passwords, emails, and financial data. The stolen data is often sold on the dark web.\n\nExample: A major retailer is hacked and 10 million customer email addresses and passwords are leaked online.";
                case "Public Wi-Fi Safety":
                    return "PUBLIC WI-FI SAFETY\n\nDefinition: Protecting yourself when using shared, open networks.\n\nExplanation: Public networks are unencrypted. Attackers can perform 'man-in-the-middle' attacks to steal your data.\n\nExample: At an airport, a hacker sets up a hotspot called 'Free Airport WiFi'. Anyone who connects has their traffic monitored.";
                case "Secure Browsing":
                    return "SECURE BROWSING\n\nDefinition: Using the internet in a way that protects your privacy and security.\n\nExplanation: Always use HTTPS sites, avoid suspicious links, and keep your browser updated.\n\nExample: Before entering banking details, check that the URL starts with 'https://' and shows a padlock icon.";
                case "Email Security":
                    return "EMAIL SECURITY\n\nDefinition: Protecting your inbox from threats like phishing, spam, and malware.\n\nExplanation: Be cautious of unexpected attachments, links from unknown senders, and emails creating urgency.\n\nExample: You receive an email from 'support@paypa1.com' (note the '1' instead of 'l') asking you to verify your account.";
                case "Backup Data":
                    return "BACKUP DATA\n\nDefinition: Copying data to a secondary location for recovery purposes.\n\nExplanation: Regular backups protect against ransomware and hardware failure. Follow the 3-2-1 rule: 3 copies, 2 different media, 1 offsite.\n\nExample: You back up files to an external hard drive weekly and also sync to Google Drive automatically.";
                case "Software Updates":
                    return "SOFTWARE UPDATES\n\nDefinition: Patches that fix bugs and close security vulnerabilities.\n\nExplanation: Outdated software is one of the most common attack vectors. Hackers exploit known vulnerabilities in unpatched systems.\n\nExample: The WannaCry ransomware in 2017 exploited a Windows flaw that had already been patched — but many users hadn't updated.";
                case "Cloud Security":
                    return "CLOUD SECURITY\n\nDefinition: Protecting data and services stored in cloud environments.\n\nExplanation: Use strong passwords, enable 2FA, and review sharing permissions regularly.\n\nExample: You store sensitive documents on Google Drive. You enable 2FA and set sharing to 'Only me'.";
                case "Cyberbullying":
                    return "CYBERBULLYING\n\nDefinition: Using digital technology to harass, threaten, or humiliate someone.\n\nExplanation: It can happen via social media, messaging apps, or gaming platforms and has serious mental health impacts.\n\nExample: A classmate repeatedly posts embarrassing photos of someone on Instagram and encourages others to mock them.";
                case "Online Scams":
                    return "ONLINE SCAMS\n\nDefinition: Fraudulent schemes designed to trick people into giving money or personal information.\n\nExplanation: Common types include lottery scams, romance scams, job offer scams, and fake online stores.\n\nExample: 'Congratulations! You've won R50,000. Pay a R500 processing fee to claim your prize.' There is no prize.";
                case "Digital Footprint":
                    return "DIGITAL FOOTPRINT\n\nDefinition: The trail of data you leave behind when using the internet.\n\nExplanation: Every post, search, purchase, and login contributes to your footprint. It can be used by advertisers, employers, or attackers.\n\nExample: A photo you posted 5 years ago is still publicly visible and shows up when someone searches your name.";
                case "Privacy Settings":
                    return "PRIVACY SETTINGS\n\nDefinition: Controls that determine who can see your information online.\n\nExplanation: Regularly review and tighten privacy settings on all social media, apps, and devices.\n\nExample: Your Facebook profile is set to 'Public'. Changing it to 'Friends only' prevents strangers from seeing your posts.";
                case "Safe Downloads":
                    return "SAFE DOWNLOADS\n\nDefinition: Obtaining files only from trusted, verified sources.\n\nExplanation: Malicious files are often disguised as legitimate software. Always verify the source and scan downloads.\n\nExample: Instead of downloading software from a random site, always go to the official website directly.";
                default:
                    return "I don't have specific information on that topic yet. Try asking about passwords, phishing, or malware.";
            }
        }

        // ── deep explanations ──────────────────────────────────────────────────
        private string GetDeepExplanation(string topic)
        {
            switch (topic)
            {
                case "Passwords":
                    return "A strong password needs three things:\n\n" +
                           "1. LENGTH — at least 12 characters. Longer = harder to crack.\n" +
                           "2. COMPLEXITY — mix UPPERCASE, lowercase, numbers, and symbols.\n" +
                           "3. UNIQUENESS — never reuse a password on different sites.\n\n" +
                           "Tip: Use a passphrase — a sentence only you know.\n" +
                           "Example: 'My dog Biscuit loves 3 walks!' becomes 'MdBl3w!'\n\n" +
                           "Tool: Use a free password manager like Bitwarden.";
                case "Phishing":
                    return "How to spot a phishing attempt:\n\n" +
                           "Red flags:\n" +
                           "- Urgent language: 'Act NOW or your account will be closed!'\n" +
                           "- Suspicious sender: 'support@paypa1.com' (fake domain)\n" +
                           "- Generic greeting: 'Dear Customer' instead of your name\n" +
                           "- Unexpected attachments\n\n" +
                           "What to do:\n" +
                           "- Don't click — go directly to the official website instead\n" +
                           "- Report the email as phishing\n" +
                           "- Enable 2FA so even a stolen password can't be used";
                case "Malware":
                    return "Types of malware:\n\n" +
                           "Virus      — attaches to files and spreads when you share them\n" +
                           "Worm       — spreads across networks without user action\n" +
                           "Trojan     — disguises itself as legitimate software\n" +
                           "Ransomware — encrypts your files and demands payment\n" +
                           "Spyware    — secretly monitors your activity\n" +
                           "Adware     — bombards you with unwanted ads\n\n" +
                           "Protection:\n" +
                           "- Install reputable antivirus software\n" +
                           "- Keep your OS and apps updated\n" +
                           "- Only download from official sources";
                case "Two-Factor Authentication":
                    return "The three authentication factors:\n\n" +
                           "1. Something you KNOW  — password or PIN\n" +
                           "2. Something you HAVE  — phone or hardware key\n" +
                           "3. Something you ARE   — fingerprint or face ID\n\n" +
                           "Types of 2FA (least to most secure):\n" +
                           "SMS code          — convenient but can be SIM-swapped\n" +
                           "Authenticator app — more secure (Google Authenticator, Authy)\n" +
                           "Hardware key      — most secure (YubiKey)\n\n" +
                           "Enable 2FA on: email, banking, and social media.";
                case "VPN":
                    return "How a VPN works:\n\n" +
                           "1. You connect to a VPN server\n" +
                           "2. Your device encrypts all outgoing data\n" +
                           "3. The encrypted data travels to the VPN server\n" +
                           "4. The VPN server sends it to the destination\n" +
                           "5. Websites see the VPN server's IP, not yours\n\n" +
                           "Use a VPN on public Wi-Fi and when accessing sensitive accounts away from home.";
                case "Encryption":
                    return "Encryption explained simply:\n\n" +
                           "Imagine you write a letter and lock it in a box. Only the person with the key can open it.\n\n" +
                           "Types:\n" +
                           "Symmetric  — same key to encrypt and decrypt (fast, used for files)\n" +
                           "Asymmetric — public key encrypts, private key decrypts (used in HTTPS)\n\n" +
                           "Where you see it daily:\n" +
                           "HTTPS           — encrypts data between your browser and websites\n" +
                           "WhatsApp/Signal — end-to-end encrypted messages\n" +
                           "BitLocker       — encrypts your entire hard drive";
                default:
                    return GetAnswerText(topic) + "\n\nFor more detail, visit https://staysafeonline.org";
            }
        }
    }
}
