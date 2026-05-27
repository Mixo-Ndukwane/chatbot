using System.Collections.Generic;
using System.Linq;

namespace ChatBot_GUI
{
    /// <summary>
    /// Tracks user name, every topic they have asked about (with frequency),
    /// and arbitrary key-value pairs for other state.
    /// </summary>
    public class MemoryStore
    {
        // ── basic state ────────────────────────────────────────────────────────
        public string UserName { get; set; }

        // ── topic frequency tracking ───────────────────────────────────────────
        // topic name → number of times the user has asked about it
        private readonly Dictionary<string, int> _topicCounts =
            new Dictionary<string, int>();

        // The topic asked about most often (null until at least one topic is asked)
        public string FavouriteTopic
        {
            get
            {
                if (_topicCounts.Count == 0) return null;
                return _topicCounts.OrderByDescending(kv => kv.Value).First().Key;
            }
            // Setter kept for compatibility — calling it records one visit
            set
            {
                if (!string.IsNullOrEmpty(value))
                    RecordTopic(value);
            }
        }

        // The most recently asked topic (regardless of frequency)
        public string LastTopic { get; private set; }

        // All topics the user has asked about, most-frequent first
        public IEnumerable<string> TopicsAsked =>
            _topicCounts.OrderByDescending(kv => kv.Value).Select(kv => kv.Key);

        // How many distinct topics the user has asked about
        public int UniqueTopicCount => _topicCounts.Count;

        /// <summary>Records that the user asked about a topic.</summary>
        public void RecordTopic(string topic)
        {
            if (string.IsNullOrEmpty(topic)) return;
            LastTopic = topic;
            if (_topicCounts.ContainsKey(topic))
                _topicCounts[topic]++;
            else
                _topicCounts[topic] = 1;
        }

        /// <summary>How many times the user has asked about a specific topic.</summary>
        public int TimesAsked(string topic)
        {
            int count;
            return _topicCounts.TryGetValue(topic, out count) ? count : 0;
        }

        // ── generic key-value store ────────────────────────────────────────────
        private readonly Dictionary<string, string> _store =
            new Dictionary<string, string>();

        public void Store(string key, string value)  => _store[key] = value;
        public string Recall(string key)
        {
            string v;
            return _store.TryGetValue(key, out v) ? v : null;
        }

        // ── personalisation helpers ────────────────────────────────────────────

        /// <summary>
        /// Returns a personalised opener referencing the favourite topic,
        /// e.g. "As someone interested in Phishing, "
        /// Returns empty string if no topic has been asked yet.
        /// </summary>
        public string GetPersonalisedOpener()
        {
            string fav = FavouriteTopic;
            if (string.IsNullOrEmpty(fav)) return "";
            return $"As someone interested in {fav}, ";
        }

        /// <summary>
        /// Returns a contextual reference to the favourite topic for use in
        /// fallback / greeting messages.
        /// </summary>
        public string GetFavouriteTopicHint()
        {
            string fav = FavouriteTopic;
            if (string.IsNullOrEmpty(fav)) return "";

            int count = TimesAsked(fav);
            if (count >= 3)
                return $"I've noticed you're really interested in {fav} — " +
                       $"you've asked about it {count} times. ";
            if (count == 2)
                return $"You seem particularly interested in {fav}. ";
            return $"Based on your interest in {fav}, ";
        }

        /// <summary>
        /// Returns a suggestion to revisit the favourite topic, or empty string.
        /// </summary>
        public string GetTopicSuggestion()
        {
            string fav = FavouriteTopic;
            if (string.IsNullOrEmpty(fav)) return "";
            return $"You might also want to revisit {fav} — " +
                   "type 'explain more' if you'd like a deeper look.";
        }

        /// <summary>
        /// True if the user has asked about more than one topic.
        /// </summary>
        public bool HasMultipleTopics => _topicCounts.Count > 1;

        /// <summary>
        /// Returns the second-most-asked topic, or null.
        /// </summary>
        public string SecondFavouriteTopic
        {
            get
            {
                var ordered = _topicCounts.OrderByDescending(kv => kv.Value).ToList();
                return ordered.Count >= 2 ? ordered[1].Key : null;
            }
        }
    }
}
