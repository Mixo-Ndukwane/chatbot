using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ChatBot_GUI
{
    /// <summary>
    /// Renders one chat message.
    /// User  → blue rounded bubble, right-aligned.
    /// Bot   → plain left-aligned text with a light grey pill, left-aligned.
    /// Height is self-calculated; Width must be set to the container width before adding.
    /// </summary>
    public class ChatMessage : Control
    {
        // ── colours ────────────────────────────────────────────────────────────
        private static readonly Color UserBg    = Color.FromArgb(0, 120, 215);
        private static readonly Color UserFg    = Color.White;
        private static readonly Color BotBg     = Color.FromArgb(233, 236, 240);
        private static readonly Color BotFg     = Color.FromArgb(25, 25, 25);
        private static readonly Color TsColor   = Color.FromArgb(150, 150, 150);
        private static readonly Color UserTsClr = Color.FromArgb(190, 220, 255);

        // ── layout ─────────────────────────────────────────────────────────────
        private const int Radius    = 14;
        private const int PadH      = 12;   // horizontal inner padding
        private const int PadV      = 8;    // vertical inner padding
        private const int MaxWidthPc = 68;  // bubble max % of control width
        private const int SideGap   = 10;   // gap from edge of control
        private const int RowGap    = 6;    // space below each message row

        // ── data ───────────────────────────────────────────────────────────────
        private readonly bool   _isUser;
        private readonly string _text;
        private readonly string _timestamp;

        private static readonly Font FontMsg  = new Font("Segoe UI", 10f);
        private static readonly Font FontTs   = new Font("Segoe UI",  7.5f);

        public ChatMessage(string text, bool isUser)
        {
            _text      = text;
            _isUser    = isUser;
            _timestamp = DateTime.Now.ToString("HH:mm");

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            BackColor = Color.Transparent;
            TabStop   = false;
        }

        // ── public: call after setting Width ───────────────────────────────────
        public void RecalcHeight()
        {
            Height = MeasureHeight(Width);
        }

        private int MeasureHeight(int controlWidth)
        {
            int maxBubbleW = (int)(controlWidth * MaxWidthPc / 100.0) - SideGap * 2;
            if (maxBubbleW < 60) maxBubbleW = 60;

            using (Graphics g = CreateGraphics())
            {
                SizeF msgSz = g.MeasureString(_text, FontMsg,
                                  new SizeF(maxBubbleW - PadH * 2, float.MaxValue),
                                  StringFormat.GenericDefault);
                SizeF tsSz  = g.MeasureString(_timestamp, FontTs);

                int h = (int)(PadV + msgSz.Height + 2 + tsSz.Height + PadV);
                return h + RowGap + 4;   // +4 for shadow
            }
        }

        // ── paint ──────────────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int ctrlW      = Width;
            int maxBubbleW = (int)(ctrlW * MaxWidthPc / 100.0) - SideGap * 2;
            if (maxBubbleW < 60) maxBubbleW = 60;

            SizeF msgSz = g.MeasureString(_text, FontMsg,
                              new SizeF(maxBubbleW - PadH * 2, float.MaxValue),
                              StringFormat.GenericDefault);
            SizeF tsSz  = g.MeasureString(_timestamp, FontTs);

            int bubbleW = (int)Math.Max(msgSz.Width + PadH * 2,
                                        tsSz.Width  + PadH * 2);
            bubbleW = Math.Min(bubbleW, maxBubbleW);

            int bubbleH = (int)(PadV + msgSz.Height + 2 + tsSz.Height + PadV);

            int bubbleX = _isUser
                ? ctrlW - bubbleW - SideGap
                : SideGap;

            Rectangle rect = new Rectangle(bubbleX, 2, bubbleW, bubbleH);

            // shadow
            using (SolidBrush sb = new SolidBrush(Color.FromArgb(22, 0, 0, 0)))
            {
                Rectangle sr = new Rectangle(rect.X + 2, rect.Y + 3, rect.Width, rect.Height);
                FillRounded(g, sb, sr, Radius);
            }

            // bubble background
            Color bg = _isUser ? UserBg : BotBg;
            using (SolidBrush sb = new SolidBrush(bg))
                FillRounded(g, sb, rect, Radius);

            // message text
            Color fg = _isUser ? UserFg : BotFg;
            float tx = rect.X + PadH;
            float ty = rect.Y + PadV;
            float tw = bubbleW - PadH * 2;

            using (SolidBrush sb = new SolidBrush(fg))
                g.DrawString(_text, FontMsg, sb,
                             new RectangleF(tx, ty, tw, msgSz.Height));

            // timestamp — right-aligned inside bubble
            Color tsClr = _isUser ? UserTsClr : TsColor;
            StringFormat tsf = new StringFormat { Alignment = StringAlignment.Far };
            using (SolidBrush sb = new SolidBrush(tsClr))
                g.DrawString(_timestamp, FontTs, sb,
                             new RectangleF(tx, ty + msgSz.Height + 2, tw, tsSz.Height),
                             tsf);
        }

        // ── helpers ────────────────────────────────────────────────────────────
        private static void FillRounded(Graphics g, Brush brush, Rectangle r, int radius)
        {
            using (GraphicsPath path = RoundedPath(r, radius))
                g.FillPath(brush, path);
        }

        private static GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            int d = radius * 2;
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.X,         r.Y,          d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y,          d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d,   0, 90);
            p.AddArc(r.X,         r.Bottom - d, d, d,  90, 90);
            p.CloseFigure();
            return p;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
