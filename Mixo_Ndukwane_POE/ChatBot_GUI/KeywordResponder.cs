using System;
using System.Collections.Generic;

namespace ChatBot_GUI
{
    /// <summary>
    /// Maps keywords found in user input to the canonical topic name.
    /// ProcessInput uses GetTopic() first — if a topic is matched the full
    /// definition/explanation/example is shown via GetAnswerText().
    /// GetQuickTip() is kept for general questions that don't map to a topic.
    /// </summary>
    public class KeywordResponder
    {
        // keyword (lower-case) → canonical topic name (must match _topics array)
        private static readonly Dictionary<string, string> _topicMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "password",                  "Passwords"                  },
            { "passwords",                 "Passwords"                  },
            { "phishing",                  "Phishing"                   },
            { "malware",                   "Malware"                    },
            { "ransomware",                "Ransomware"                 },
            { "ransom",                    "Ransomware"                 },
            { "two-factor",                "Two-Factor Authentication"  },
            { "two factor",                "Two-Factor Authentication"  },
            { "2fa",                       "Two-Factor Authentication"  },
            { "multi-factor",              "Two-Factor Authentication"  },
            { "mfa",                       "Two-Factor Authentication"  },
            { "social engineering",        "Social Engineering"         },
            { "firewall",                  "Firewalls"                  },
            { "firewalls",                 "Firewalls"                  },
            { "vpn",                       "VPN"                        },
            { "virtual private network",   "VPN"                        },
            { "encryption",                "Encryption"                 },
            { "encrypt",                   "Encryption"                 },
            { "antivirus",                 "Antivirus"                  },
            { "anti-virus",                "Antivirus"                  },
            { "spyware",                   "Spyware"                    },
            { "adware",                    "Adware"                     },
            { "identity theft",            "Identity Theft"             },
            { "identity",                  "Identity Theft"             },
            { "data breach",               "Data Breaches"              },
            { "data breaches",             "Data Breaches"              },
            { "breach",                    "Data Breaches"              },
            { "public wi-fi",              "Public Wi-Fi Safety"        },
            { "public wifi",               "Public Wi-Fi Safety"        },
            { "wifi",                      "Public Wi-Fi Safety"        },
            { "wi-fi",                     "Public Wi-Fi Safety"        },
            { "secure browsing",           "Secure Browsing"            },
            { "browsing",                  "Secure Browsing"            },
            { "https",                     "Secure Browsing"            },
            { "email security",            "Email Security"             },
            { "email",                     "Email Security"             },
            { "backup",                    "Backup Data"                },
            { "back up",                   "Backup Data"                },
            { "software update",           "Software Updates"           },
            { "software updates",          "Software Updates"           },
            { "update",                    "Software Updates"           },
            { "patch",                     "Software Updates"           },
            { "cloud security",            "Cloud Security"             },
            { "cloud",                     "Cloud Security"             },
            { "cyberbullying",             "Cyberbullying"              },
            { "cyber bullying",            "Cyberbullying"              },
            { "bullying",                  "Cyberbullying"              },
            { "online scam",               "Online Scams"               },
            { "online scams",              "Online Scams"               },
            { "scam",                      "Online Scams"               },
            { "digital footprint",         "Digital Footprint"          },
            { "footprint",                 "Digital Footprint"          },
            { "privacy settings",          "Privacy Settings"           },
            { "privacy",                   "Privacy Settings"           },
            { "safe downloads",            "Safe Downloads"             },
            { "download",                  "Safe Downloads"             },
            { "downloads",                 "Safe Downloads"             },
        };

        // Quick tips shown when the user asks a general question (not "what is X")
        private static readonly Dictionary<string, string> _quickTips =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Passwords",
              "Use at least 12 characters mixing letters, numbers, and symbols. Never reuse passwords across sites." },
            { "Phishing",
              "Never click suspicious links in emails — go directly to the official website instead." },
            { "Malware",
              "Install reputable antivirus software and avoid downloading files from unofficial websites." },
            { "Ransomware",
              "Regular backups are your best defence. Never pay the ransom — there's no guarantee you'll get your files back." },
            { "Two-Factor Authentication",
              "Use an authenticator app (Google Authenticator, Authy) rather than SMS codes for better security." },
            { "Social Engineering",
              "Legitimate IT support will never ask for your password. Be suspicious of unsolicited requests." },
            { "Firewalls",
              "Keep your operating system's built-in firewall enabled at all times." },
            { "VPN",
              "Always use a VPN on public Wi-Fi to protect your data from eavesdroppers." },
            { "Encryption",
              "Always look for HTTPS (padlock icon) before entering passwords or payment details." },
            { "Antivirus",
              "Keep your antivirus updated and run regular full scans, not just quick scans." },
            { "Spyware",
              "Only install apps from trusted sources and review app permissions carefully." },
            { "Adware",
              "Avoid installing free software bundles — they often include adware." },
            { "Identity Theft",
              "Monitor your credit report regularly and never share your ID number unnecessarily." },
            { "Data Breaches",
              "Use unique passwords per site so a breach on one site doesn't expose all your accounts." },
            { "Public Wi-Fi Safety",
              "Avoid logging into banking or sensitive accounts on public Wi-Fi without a VPN." },
            { "Secure Browsing",
              "Check for HTTPS and a padlock icon before entering any personal information." },
            { "Email Security",
              "Be cautious of unexpected attachments and links from unknown senders." },
            { "Backup Data",
              "Follow the 3-2-1 rule: 3 copies, 2 different media types, 1 stored offsite." },
            { "Software Updates",
              "Enable automatic updates — outdated software is one of the most common attack vectors." },
            { "Cloud Security",
              "Enable 2FA on all cloud accounts and review sharing permissions regularly." },
            { "Cyberbullying",
              "Block and report bullies, save evidence, and talk to a trusted adult or authority." },
            { "Online Scams",
              "If an offer sounds too good to be true, it almost certainly is." },
            { "Digital Footprint",
              "Think before you post — everything you share online can persist for years." },
            { "Privacy Settings",
              "Review and tighten privacy settings on all social media, apps, and devices regularly." },
            { "Safe Downloads",
              "Only download software from official websites or verified app stores." },
        };

        /// <summary>
        /// Returns the canonical topic name if the input contains a known keyword,
        /// otherwise null. Used by ProcessInput to route to GetAnswerText().
        /// </summary>
        public string GetTopic(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            string lower = input.ToLowerInvariant();

            // Check multi-word keywords first (longer match wins)
            string bestKey   = null;
            int    bestLen   = 0;
            foreach (var kv in _topicMap)
            {
                if (lower.Contains(kv.Key.ToLowerInvariant()) && kv.Key.Length > bestLen)
                {
                    bestKey = kv.Key;
                    bestLen = kv.Key.Length;
                }
            }
            return bestKey != null ? _topicMap[bestKey] : null;
        }

        /// <summary>
        /// Returns a single quick-tip string for a topic, or null if not found.
        /// Used as a fallback when the user asks a general question.
        /// </summary>
        public string GetQuickTip(string topic)
        {
            if (topic == null) return null;
            string tip;
            return _quickTips.TryGetValue(topic, out tip) ? tip : null;
        }

        // Legacy method kept so nothing else breaks
        public string GetResponse(string input)
        {
            string topic = GetTopic(input);
            return topic != null ? GetQuickTip(topic) : null;
        }
    }
}
