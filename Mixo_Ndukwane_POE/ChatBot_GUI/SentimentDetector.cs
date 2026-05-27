using System.Collections.Generic;

namespace ChatBot_GUI
{
    public enum Sentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Happy
    }

    public class SentimentDetector
    {
        // Each sentiment maps to a list of trigger phrases (checked as substrings)
        private readonly Dictionary<Sentiment, List<string>> _triggers =
            new Dictionary<Sentiment, List<string>>
        {
            {
                Sentiment.Worried, new List<string>
                {
                    "worried", "i'm worried", "im worried",
                    "scared",  "i'm scared",  "im scared",
                    "afraid",  "i'm afraid",  "im afraid",
                    "anxious", "i'm anxious", "im anxious",
                    "nervous", "i'm nervous", "im nervous",
                    "unsafe",  "not safe",    "feel unsafe"
                }
            },
            {
                Sentiment.Curious, new List<string>
                {
                    "curious",       "i'm curious",
                    "wondering",     "i was wondering",
                    "interested",    "i'm interested",
                    "want to know",  "i want to know",
                    "how does",      "how do",
                    "can you explain","explain to me"
                }
            },
            {
                Sentiment.Frustrated, new List<string>
                {
                    "frustrated",    "i'm frustrated",
                    "annoyed",       "i'm annoyed",
                    "confused",      "i'm confused",
                    "don't understand", "dont understand",
                    "i don't get",   "i dont get",
                    "makes no sense","this is hard",
                    "too complicated","not clear"
                }
            },
            {
                Sentiment.Happy, new List<string>
                {
                    "great",    "that's great",
                    "awesome",  "that's awesome",
                    "thanks",   "thank you",  "thank u",
                    "helpful",  "very helpful",
                    "love it",  "i love it",
                    "amazing",  "excellent",
                    "perfect",  "well done",
                    "good job", "nice"
                }
            }
        };

        // Standalone responses when the message is purely a sentiment (no topic)
        private readonly Dictionary<Sentiment, List<string>> _responses =
            new Dictionary<Sentiment, List<string>>
        {
            {
                Sentiment.Worried, new List<string>
                {
                    "It's completely understandable to feel worried about online safety. The good news is that awareness is your strongest defence. Ask me about any topic and I'll help you understand how to stay protected.",
                    "Feeling unsafe online is a valid concern — cyber threats are real. But knowing what to look out for makes a huge difference. What would you like to learn about first?",
                    "I hear you. Being anxious about cybersecurity shows you take your safety seriously. Let me help — type 'topics' to see everything I can explain."
                }
            },
            {
                Sentiment.Curious, new List<string>
                {
                    "Curiosity is the best starting point for learning! Ask me anything — type 'topics' to see the full list of cybersecurity topics I can explain.",
                    "I love the curiosity! There's a lot to explore in cybersecurity. What would you like to know about? You can ask by name or type 'topics' for the full list.",
                    "Great mindset! Wanting to understand how things work is exactly how you stay one step ahead of threats. What topic interests you?"
                }
            },
            {
                Sentiment.Frustrated, new List<string>
                {
                    "Cybersecurity can feel overwhelming at first — you're not alone. Let me break it down simply. Which part is confusing you? Ask me anything and I'll explain it clearly.",
                    "No worries at all! These topics can be tricky. Try asking me something specific like 'what is phishing' or 'explain malware' and I'll walk you through it step by step.",
                    "I understand the frustration. Let's slow it down. Type 'topics' to see the list and pick one — I'll give you a clear definition, explanation, and example."
                }
            },
            {
                Sentiment.Happy, new List<string>
                {
                    "That's great to hear! Staying informed about cybersecurity is one of the best things you can do. Is there anything else you'd like to learn about?",
                    "Glad I could help! Knowledge is your best defence online. Feel free to ask about any other topic — type 'topics' to see the full list.",
                    "Awesome! Keep that curiosity going — the more you know, the safer you are online. What else can I help you with?"
                }
            }
        };

        private readonly System.Random _random = new System.Random();

        /// <summary>Detects the dominant sentiment in the input string.</summary>
        public Sentiment Detect(string input)
        {
            string lower = input.ToLowerInvariant();
            foreach (var pair in _triggers)
                foreach (var trigger in pair.Value)
                    if (lower.Contains(trigger))
                        return pair.Key;
            return Sentiment.Neutral;
        }

        /// <summary>
        /// Returns a full standalone response for a pure sentiment message,
        /// personalised with the user's name.
        /// Returns null for Neutral.
        /// </summary>
        public string GetStandaloneResponse(Sentiment sentiment, string userName)
        {
            if (sentiment == Sentiment.Neutral) return null;
            if (!_responses.ContainsKey(sentiment)) return null;

            var list = _responses[sentiment];
            string response = list[_random.Next(list.Count)];

            // Personalise with name if available
            if (!string.IsNullOrEmpty(userName))
                response = $"{userName}, " + char.ToLower(response[0]) + response.Substring(1);

            return response;
        }

        /// <summary>Short prefix prepended before a topic answer when sentiment is detected.</summary>
        public string GetPrefix(Sentiment sentiment, string userName)
        {
            string name = string.IsNullOrEmpty(userName) ? "" : $"{userName}, ";
            switch (sentiment)
            {
                case Sentiment.Worried:
                    return $"I understand your concern, {userName}. Let me help.\n\n";
                case Sentiment.Curious:
                    return $"Great question, {userName}! Here's what I know:\n\n";
                case Sentiment.Frustrated:
                    return $"No worries, {userName} — let me break it down simply.\n\n";
                case Sentiment.Happy:
                    return $"Glad you're enjoying this, {userName}!\n\n";
                default:
                    return "";
            }
        }
    }
}
