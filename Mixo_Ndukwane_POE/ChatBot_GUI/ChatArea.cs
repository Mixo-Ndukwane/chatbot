using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatBot_GUI
{
    /// <summary>
    /// Scrollable chat area. Messages stack top-to-bottom; newest is always
    /// scrolled into view. Handles window resize by re-measuring all messages.
    /// </summary>
    public class ChatArea : Panel
    {
        private int _nextY = 8;   // y-position for the next message

        public ChatArea()
        {
            AutoScroll  = true;
            BackColor   = Color.FromArgb(245, 247, 250);
            BorderStyle = BorderStyle.None;

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint, true);
        }

        // ── public API ─────────────────────────────────────────────────────────

        public void AddMessage(string text, bool isUser)
        {
            ChatMessage msg = new ChatMessage(text, isUser);

            // Width must be set before RecalcHeight so MeasureString is accurate
            msg.Width = ClientSize.Width;
            msg.RecalcHeight();
            msg.Top  = _nextY;
            msg.Left = 0;

            _nextY += msg.Height;

            // Keep AutoScrollMinSize in sync so the scrollbar appears
            AutoScrollMinSize = new Size(0, _nextY + 8);

            Controls.Add(msg);
            ScrollToBottom();
        }

        // ── resize: re-layout all messages ─────────────────────────────────────
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            Reflow();
        }

        private void Reflow()
        {
            int y = 8;
            int w = ClientSize.Width;
            if (w <= 0) return;

            SuspendLayout();
            foreach (Control c in Controls)
            {
                if (c is ChatMessage msg)
                {
                    msg.Width = w;
                    msg.RecalcHeight();
                    msg.Top  = y;
                    msg.Left = 0;
                    y += msg.Height;
                }
            }
            _nextY = y;
            AutoScrollMinSize = new Size(0, _nextY + 8);
            ResumeLayout(false);
        }

        private void ScrollToBottom()
        {
            // Move scroll position to the very bottom
            AutoScrollPosition = new Point(0, AutoScrollMinSize.Height);
        }
    }
}
