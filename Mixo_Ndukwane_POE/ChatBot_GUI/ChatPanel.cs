using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatBot_GUI
{
    /// <summary>
    /// A vertically-scrolling panel that hosts ChatBubble controls.
    /// Bubbles are stacked oldest-at-top, newest-at-bottom.
    /// </summary>
    public class ChatPanel : Panel
    {
        private static readonly Color ChatBackground = Color.FromArgb(245, 247, 250);

        // Inner panel that holds all bubbles — we manage its height manually
        // so we can stack top-to-bottom without DockStyle fighting us.
        private readonly FlowLayoutPanel _flow;

        public ChatPanel()
        {
            BackColor  = ChatBackground;
            AutoScroll = true;

            _flow = new FlowLayoutPanel
            {
                FlowDirection    = FlowDirection.TopDown,
                WrapContents     = false,
                AutoSize         = true,
                AutoSizeMode     = AutoSizeMode.GrowAndShrink,
                BackColor        = Color.Transparent,
                Padding          = new Padding(0, 6, 0, 6),
                Dock             = DockStyle.Top,
            };

            Controls.Add(_flow);

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint, true);
        }

        /// <summary>Adds a chat bubble and scrolls to the bottom.</summary>
        public void AddBubble(string sender, string message, bool isUser)
        {
            int w = ClientSize.Width > 0 ? ClientSize.Width : Width;

            ChatBubble bubble = new ChatBubble(sender, message, isUser);
            bubble.Measure(w);

            _flow.Controls.Add(bubble);

            // Keep flow panel width in sync
            _flow.Width = w;

            ScrollToBottom();
        }

        // ── layout ─────────────────────────────────────────────────────────────

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            ReflowBubbles();
        }

        private void ReflowBubbles()
        {
            int w = ClientSize.Width;
            if (w <= 0) w = Width;

            _flow.SuspendLayout();
            _flow.Width = w;

            foreach (Control c in _flow.Controls)
            {
                if (c is ChatBubble b)
                    b.Measure(w);
            }

            _flow.ResumeLayout(true);
        }

        private void ScrollToBottom()
        {
            // Scroll to the bottom of the flow panel
            if (_flow.Controls.Count == 0) return;
            AutoScrollPosition = new Point(0, _flow.Height);
        }
    }
}
