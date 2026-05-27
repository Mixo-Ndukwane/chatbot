using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ChatBot_GUI
{
    // ─────────────────────────────────────────────────────────────────────────
    // Bubble — one chat message painted as a rounded rectangle
    // ─────────────────────────────────────────────────────────────────────────
    internal class Bubble : Control
    {
        private static readonly Color UserBg   = Color.FromArgb(0, 120, 215);
        private static readonly Color BotBg    = Color.FromArgb(225, 228, 234);
        private static readonly Color UserFg   = Color.White;
        private static readonly Color BotFg    = Color.FromArgb(20, 20, 20);
        private static readonly Color UserMeta = Color.FromArgb(190, 220, 255);
        private static readonly Color BotMeta  = Color.FromArgb(110, 110, 110);

        private const int Radius  = 14;
        private const int PadH    = 12;
        private const int PadV    = 8;
        private const int MetaGap = 2;

        public readonly bool IsUser;
        private readonly string _meta;
        private readonly string _body;

        private static readonly Font FontMeta = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        private static readonly Font FontBody = new Font("Segoe UI", 10f);

        private int _bubbleW;
        private int _bubbleH;

        public Bubble(string meta, string body, bool isUser)
        {
            IsUser = isUser;
            _meta = meta;
            _body = body;

            // Enable transparent backcolor support first
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            BackColor = Color.Transparent;
            TabStop = false;
        }
        /// <summary>Measures text and sets Width/Height. Must be called before adding to parent.</summary>
        public void Fit(int panelWidth)
        {
            if (panelWidth < 10) panelWidth = 400; // safe fallback

            int maxBubbleW = Math.Max((int)(panelWidth * 0.70), 100);
            int innerW     = maxBubbleW - PadH * 2;

            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Measure at max width first
                SizeF metaSz = g.MeasureString(_meta, FontMeta,
                                   new SizeF(innerW, 9999f), StringFormat.GenericDefault);
                SizeF bodySz = g.MeasureString(_body, FontBody,
                                   new SizeF(innerW, 9999f), StringFormat.GenericDefault);

                // Shrink bubble to content width
                int contentW = (int)Math.Ceiling(Math.Max(metaSz.Width, bodySz.Width));
                int bubbleW  = Math.Min(contentW + PadH * 2 + 6, maxBubbleW);
                bubbleW      = Math.Max(bubbleW, 80);

                // Re-measure body at actual inner width (may wrap differently)
                int ai = bubbleW - PadH * 2;
                bodySz = g.MeasureString(_body, FontBody,
                             new SizeF(ai, 9999f), StringFormat.GenericDefault);
                metaSz = g.MeasureString(_meta, FontMeta,
                             new SizeF(ai, 9999f), StringFormat.GenericDefault);

                int bubbleH = (int)Math.Ceiling(
                    PadV + metaSz.Height + MetaGap + bodySz.Height + PadV) + 4;

                _bubbleW = bubbleW;
                _bubbleH = bubbleH;

                // Control is full panel width so left/right alignment works in OnPaint
                Width  = panelWidth;
                Height = bubbleH;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_bubbleW == 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int bx = IsUser ? Width - _bubbleW - 10 : 10;
            var r  = new Rectangle(bx, 0, _bubbleW - 1, _bubbleH - 1);

            using (var br = new SolidBrush(IsUser ? UserBg : BotBg))
            using (var path = RoundedPath(r, Radius))
                g.FillPath(br, path);

            float tx = bx + PadH;
            float ty = PadV;
            float tw = _bubbleW - PadH * 2;

            // Meta (sender + time)
            using (var mb = new SolidBrush(IsUser ? UserMeta : BotMeta))
            {
                SizeF ms = g.MeasureString(_meta, FontMeta,
                               new SizeF(tw, 9999f), StringFormat.GenericDefault);
                g.DrawString(_meta, FontMeta, mb, new RectangleF(tx, ty, tw, ms.Height));
                ty += ms.Height + MetaGap;
            }

            // Body
            float bodyH = Math.Max(_bubbleH - ty - PadV, 1f);
            using (var tb = new SolidBrush(IsUser ? UserFg : BotFg))
                g.DrawString(_body, FontBody, tb, new RectangleF(tx, ty, tw, bodyH));
        }

        private static GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X,         r.Y,          d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y,          d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d,   0, 90);
            p.AddArc(r.X,         r.Bottom - d, d, d,  90, 90);
            p.CloseFigure();
            return p;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BubblePanel — scrollable chat area, bubbles stacked top-to-bottom
    // ─────────────────────────────────────────────────────────────────────────
    public class BubblePanel : Panel
    {
        private const int Gap = 6;   // px between bubbles
        private int _totalHeight = Gap;

        public BubblePanel()
        {
            AutoScroll  = true;
            BackColor   = Color.FromArgb(245, 247, 250);
            BorderStyle = BorderStyle.None;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint, true);
        }

        public void AddBubble(string sender, string timestamp, string message, bool isUser)
        {
            // Use actual client width; fall back to Width if not yet laid out
            int cw = ClientSize.Width > 10 ? ClientSize.Width : Width;
            if (cw < 10) cw = 400;

            var b = new Bubble($"{sender}  [{timestamp}]", message, isUser);
            b.Fit(cw);

            // Child Top must be the absolute position in the virtual canvas,
            // NOT offset by the current scroll position.
            b.Left = 0;
            b.Top  = _totalHeight;

            _totalHeight += b.Height + Gap;
            AutoScrollMinSize = new Size(0, _totalHeight + Gap);

            Controls.Add(b);
            ScrollToBottom(b);
        }

        // Reposition all bubbles when the panel is resized
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            Reflow();
        }

        private void Reflow()
        {
            int cw = ClientSize.Width;
            if (cw < 10) return;

            SuspendLayout();
            int y = Gap;
            foreach (Control c in Controls)
            {
                if (!(c is Bubble b)) continue;
                b.Fit(cw);
                b.Left = 0;
                b.Top  = y;
                y += b.Height + Gap;
            }
            _totalHeight = y;
            AutoScrollMinSize = new Size(0, _totalHeight + Gap);
            ResumeLayout(false);

            // Keep the view at the bottom after resize
            ScrollToBottom();
        }

        private void ScrollToBottom(Control last = null)
        {
            // ScrollControlIntoView is the reliable way — it handles the
            // AutoScrollPosition offset correctly on all .NET 4.x versions.
            if (last != null)
            {
                ScrollControlIntoView(last);
                return;
            }
            // Fallback: scroll to the very bottom of the virtual canvas
            if (Controls.Count > 0)
                ScrollControlIntoView(Controls[Controls.Count - 1]);
        }
    }
}
