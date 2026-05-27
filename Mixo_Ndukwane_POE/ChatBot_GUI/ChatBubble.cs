using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ChatBot_GUI
{
    /// <summary>
    /// A single chat message rendered as a rounded-rectangle bubble.
    /// User messages align right (blue); bot messages align left (grey).
    /// </summary>
    public class ChatBubble : UserControl
    {
        // ── colours ────────────────────────────────────────────────────────────
        private static readonly Color UserBubbleColor = Color.FromArgb(0, 132, 255);
        private static readonly Color BotBubbleColor  = Color.FromArgb(235, 237, 242);
        private static readonly Color UserTextColor   = Color.White;
        private static readonly Color BotTextColor    = Color.FromArgb(30, 30, 30);
        private static readonly Color TimestampColor  = Color.FromArgb(140, 140, 140);

        // ── layout constants ───────────────────────────────────────────────────
        private const int InnerPad     = 12;   // inner text padding inside bubble
        private const int Radius       = 16;   // corner radius
        private const int MaxBubblePct = 72;   // bubble max width as % of panel width
        private const int SideMargin   = 12;   // gap from panel edge
        private const int BottomGap    = 8;    // gap below each bubble

        // ── state ──────────────────────────────────────────────────────────────
        public bool   IsUser    { get; private set; }
        public string Message   { get; private set; }
        public string Sender    { get; private set; }
        public string Timestamp { get; private set; }

        private readonly Font _msgFont;
        private readonly Font _boldFont;
        private readonly Font _tsFont;

        public ChatBubble(string sender, string message, bool isUser)
        {
            IsUser    = isUser;
            Message   = message;
            Sender    = sender;
            Timestamp = DateTime.Now.ToString("HH:mm");

            _msgFont  = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);
            _boldFont = new Font("Segoe UI", 10f, FontStyle.Bold,    GraphicsUnit.Point);
            _tsFont   = new Font("Segoe UI", 7.5f, FontStyle.Regular, GraphicsUnit.Point);

            SetStyle(ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.UserPaint             |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            BackColor = Color.Transparent;
            Dock      = DockStyle.Top;
        }

        // ── measure ────────────────────────────────────────────────────────────
        public void Measure(int panelWidth)
        {
            int maxBubbleW = (int)(panelWidth * MaxBubblePct / 100.0);

            using (Graphics g = CreateGraphics())
            {
                SizeF senderSz = g.MeasureString(Sender, _boldFont);
                SizeF msgSz    = g.MeasureString(Message, _msgFont,
                                     new SizeF(maxBubbleW - InnerPad * 2, float.MaxValue),
                                     StringFormat.GenericDefault);
                SizeF tsSz     = g.MeasureString(Timestamp, _tsFont);

                int bubbleW = (int)Math.Max(
                                  Math.Max(senderSz.Width, msgSz.Width) + InnerPad * 2,
                                  tsSz.Width + InnerPad * 2);
                bubbleW = Math.Min(bubbleW, maxBubbleW);

                int bubbleH = (int)(senderSz.Height + msgSz.Height + tsSz.Height
                                    + InnerPad * 2 + 6);

                Height = bubbleH + BottomGap + 8;
            }

            Width = panelWidth;
        }

        // ── paint ──────────────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int panelW     = Width;
            int maxBubbleW = (int)(panelW * MaxBubblePct / 100.0);

            SizeF senderSz = g.MeasureString(Sender, _boldFont);
            SizeF msgSz    = g.MeasureString(Message, _msgFont,
                                 new SizeF(maxBubbleW - InnerPad * 2, float.MaxValue),
                                 StringFormat.GenericDefault);
            SizeF tsSz     = g.MeasureString(Timestamp, _tsFont);

            int bubbleW = (int)Math.Max(
                              Math.Max(senderSz.Width, msgSz.Width) + InnerPad * 2,
                              tsSz.Width + InnerPad * 2);
            bubbleW = Math.Min(bubbleW, maxBubbleW);

            int bubbleH = (int)(senderSz.Height + msgSz.Height + tsSz.Height
                                + InnerPad * 2 + 6);

            int bubbleX = IsUser
                ? panelW - bubbleW - SideMargin
                : SideMargin;

            Rectangle bubbleRect = new Rectangle(bubbleX, 4, bubbleW, bubbleH);

            // Shadow
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(18, 0, 0, 0)))
            {
                Rectangle shadowRect = new Rectangle(bubbleRect.X + 2, bubbleRect.Y + 3,
                                                     bubbleRect.Width, bubbleRect.Height);
                FillRoundedRect(g, shadowBrush, shadowRect, Radius);
            }

            // Bubble background
            Color bubbleColor = IsUser ? UserBubbleColor : BotBubbleColor;
            using (SolidBrush bgBrush = new SolidBrush(bubbleColor))
                FillRoundedRect(g, bgBrush, bubbleRect, Radius);

            // Text colours
            Color textColor = IsUser ? UserTextColor : BotTextColor;
            Color tsColor   = IsUser ? Color.FromArgb(200, 230, 255) : TimestampColor;

            float y = bubbleRect.Y + InnerPad;
            float x = bubbleRect.X + InnerPad;
            float w = bubbleW - InnerPad * 2;

            // Sender name (bold)
            using (SolidBrush brush = new SolidBrush(textColor))
                g.DrawString(Sender, _boldFont, brush, new RectangleF(x, y, w, senderSz.Height));
            y += senderSz.Height + 2;

            // Message body
            using (SolidBrush brush = new SolidBrush(textColor))
                g.DrawString(Message, _msgFont, brush, new RectangleF(x, y, w, msgSz.Height));
            y += msgSz.Height + 2;

            // Timestamp — right-aligned inside bubble
            StringFormat tsSf = new StringFormat { Alignment = StringAlignment.Far };
            using (SolidBrush brush = new SolidBrush(tsColor))
                g.DrawString(Timestamp, _tsFont, brush, new RectangleF(x, y, w, tsSz.Height), tsSf);
        }

        // ── helpers ────────────────────────────────────────────────────────────
        private static void FillRoundedRect(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath path = RoundedRectPath(rect, radius))
                g.FillPath(brush, path);
        }

        private static GraphicsPath RoundedRectPath(Rectangle r, int radius)
        {
            int d    = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X,         r.Y,          d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y,          d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d,   0, 90);
            path.AddArc(r.X,         r.Bottom - d, d, d,  90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _msgFont.Dispose();
                _boldFont.Dispose();
                _tsFont.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
