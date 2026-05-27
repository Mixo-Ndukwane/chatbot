namespace ChatBot_GUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.headerPanel = new System.Windows.Forms.Panel();
            this.headerLabel = new System.Windows.Forms.Label();
            this.bubblePanel = new ChatBot_GUI.BubblePanel();
            this.inputBox    = new System.Windows.Forms.TextBox();
            this.btnSend     = new System.Windows.Forms.Button();
            this.panelInput  = new System.Windows.Forms.Panel();

            this.headerPanel.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();

            // ── headerPanel ────────────────────────────────────────────────────
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.headerPanel.Controls.Add(this.headerLabel);
            this.headerPanel.Dock      = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height    = 52;
            this.headerPanel.Name      = "headerPanel";
            this.headerPanel.TabIndex  = 0;

            // ── headerLabel ────────────────────────────────────────────────────
            this.headerLabel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.headerLabel.Font      = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.headerLabel.ForeColor = System.Drawing.Color.White;
            this.headerLabel.Name      = "headerLabel";
            this.headerLabel.TabIndex  = 0;
            this.headerLabel.Text      = "SafeNet Cybersecurity Awareness Bot";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── bubblePanel ────────────────────────────────────────────────────
            this.bubblePanel.AutoScroll = true;
            this.bubblePanel.BackColor  = System.Drawing.Color.FromArgb(245, 247, 250);
            this.bubblePanel.Dock       = System.Windows.Forms.DockStyle.Fill;
            this.bubblePanel.Name       = "bubblePanel";
            this.bubblePanel.TabIndex   = 1;

            // ── panelInput ─────────────────────────────────────────────────────
            this.panelInput.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.panelInput.Controls.Add(this.inputBox);
            this.panelInput.Controls.Add(this.btnSend);
            this.panelInput.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.panelInput.Height    = 55;
            this.panelInput.Name      = "panelInput";
            this.panelInput.Padding   = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.panelInput.TabIndex  = 2;

            // ── inputBox ───────────────────────────────────────────────────────
            this.inputBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputBox.Dock        = System.Windows.Forms.DockStyle.Fill;
            this.inputBox.Font        = new System.Drawing.Font("Segoe UI", 11F);
            this.inputBox.Name        = "inputBox";
            this.inputBox.TabIndex    = 0;

            // ── btnSend ────────────────────────────────────────────────────────
            this.btnSend.BackColor                 = System.Drawing.Color.SteelBlue;
            this.btnSend.Dock                      = System.Windows.Forms.DockStyle.Right;
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle                 = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font                      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSend.ForeColor                 = System.Drawing.Color.White;
            this.btnSend.Name                      = "btnSend";
            this.btnSend.TabIndex                  = 1;
            this.btnSend.Text                      = "Send";
            this.btnSend.UseVisualStyleBackColor   = false;
            this.btnSend.Width                     = 90;
            this.btnSend.Click                    += new System.EventHandler(this.btnSend_Click);

            // ── Form1 ──────────────────────────────────────────────────────────
            this.AcceptButton        = this.btnSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(860, 620);
            this.Controls.Add(this.bubblePanel);
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize         = new System.Drawing.Size(500, 420);
            this.Name                = "Form1";
            this.Text                = "SafeNet Cybersecurity Bot";
            this.Load               += new System.EventHandler(this.Form1_Load);

            this.headerPanel.ResumeLayout(false);
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel   headerPanel;
        private System.Windows.Forms.Label   headerLabel;
        private BubblePanel                  bubblePanel;
        private System.Windows.Forms.TextBox inputBox;
        private System.Windows.Forms.Button  btnSend;
        private System.Windows.Forms.Panel   panelInput;
    }
}
