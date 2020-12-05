namespace dotNetUi
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.RecordingText = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.UpdateRecordingButton = new System.Windows.Forms.Button();
            this.ScriptTextBox = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ScreenShotButton = new System.Windows.Forms.Button();
            this.LineNumberDisplay = new System.Windows.Forms.NumericUpDown();
            this.LoadScriptButton = new System.Windows.Forms.Button();
            this.RunScriptButton = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.AutoClickCandyCheckBox = new System.Windows.Forms.CheckBox();
            this.AutoClickCandyPeriod = new System.Windows.Forms.NumericUpDown();
            this.LoadCandyImageButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.CandyImagesPanel = new System.Windows.Forms.Panel();
            this.OutputTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoClickCandyMatchPercent = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LineNumberDisplay)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AutoClickCandyPeriod)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AutoClickCandyMatchPercent)).BeginInit();
            this.SuspendLayout();
            // 
            // RecordingText
            // 
            this.RecordingText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecordingText.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RecordingText.Location = new System.Drawing.Point(0, 0);
            this.RecordingText.MaxLength = 0;
            this.RecordingText.Multiline = true;
            this.RecordingText.Name = "RecordingText";
            this.RecordingText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.RecordingText.Size = new System.Drawing.Size(203, 235);
            this.RecordingText.TabIndex = 1;
            this.RecordingText.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 419);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(566, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(39, 17);
            this.StatusLabel.Text = "Status";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer2);
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(566, 419);
            this.panel1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            this.splitContainer2.Panel1.Controls.Add(this.panel4);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.OutputTextBox);
            this.splitContainer2.Size = new System.Drawing.Size(566, 395);
            this.splitContainer2.SplitterDistance = 293;
            this.splitContainer2.TabIndex = 9;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 32);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.RecordingText);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ScriptTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Size = new System.Drawing.Size(566, 261);
            this.splitContainer1.SplitterDistance = 203;
            this.splitContainer1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.UpdateRecordingButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 235);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(203, 26);
            this.panel2.TabIndex = 4;
            // 
            // UpdateRecordingButton
            // 
            this.UpdateRecordingButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateRecordingButton.Location = new System.Drawing.Point(46, 3);
            this.UpdateRecordingButton.Name = "UpdateRecordingButton";
            this.UpdateRecordingButton.Size = new System.Drawing.Size(107, 23);
            this.UpdateRecordingButton.TabIndex = 3;
            this.UpdateRecordingButton.Text = "Update Recording";
            this.UpdateRecordingButton.UseVisualStyleBackColor = true;
            this.UpdateRecordingButton.Click += new System.EventHandler(this.UpdateRecordingButton_Click);
            // 
            // ScriptTextBox
            // 
            this.ScriptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScriptTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScriptTextBox.Location = new System.Drawing.Point(0, 0);
            this.ScriptTextBox.MaxLength = 0;
            this.ScriptTextBox.Multiline = true;
            this.ScriptTextBox.Name = "ScriptTextBox";
            this.ScriptTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ScriptTextBox.Size = new System.Drawing.Size(359, 235);
            this.ScriptTextBox.TabIndex = 4;
            this.ScriptTextBox.WordWrap = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ScreenShotButton);
            this.panel3.Controls.Add(this.LineNumberDisplay);
            this.panel3.Controls.Add(this.LoadScriptButton);
            this.panel3.Controls.Add(this.RunScriptButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 235);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(359, 26);
            this.panel3.TabIndex = 7;
            // 
            // ScreenShotButton
            // 
            this.ScreenShotButton.Location = new System.Drawing.Point(196, 3);
            this.ScreenShotButton.Name = "ScreenShotButton";
            this.ScreenShotButton.Size = new System.Drawing.Size(75, 23);
            this.ScreenShotButton.TabIndex = 8;
            this.ScreenShotButton.Text = "Screen Shot";
            this.ScreenShotButton.UseVisualStyleBackColor = true;
            this.ScreenShotButton.Click += new System.EventHandler(this.ScreenShotButton_Click);
            // 
            // LineNumberDisplay
            // 
            this.LineNumberDisplay.Location = new System.Drawing.Point(302, 3);
            this.LineNumberDisplay.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.LineNumberDisplay.Name = "LineNumberDisplay";
            this.LineNumberDisplay.Size = new System.Drawing.Size(52, 20);
            this.LineNumberDisplay.TabIndex = 7;
            this.LineNumberDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LoadScriptButton
            // 
            this.LoadScriptButton.Location = new System.Drawing.Point(3, 3);
            this.LoadScriptButton.Name = "LoadScriptButton";
            this.LoadScriptButton.Size = new System.Drawing.Size(90, 23);
            this.LoadScriptButton.TabIndex = 6;
            this.LoadScriptButton.Text = "Load Script";
            this.LoadScriptButton.UseVisualStyleBackColor = true;
            this.LoadScriptButton.Click += new System.EventHandler(this.LoadScriptButton_Click);
            // 
            // RunScriptButton
            // 
            this.RunScriptButton.Location = new System.Drawing.Point(100, 3);
            this.RunScriptButton.Name = "RunScriptButton";
            this.RunScriptButton.Size = new System.Drawing.Size(90, 23);
            this.RunScriptButton.TabIndex = 5;
            this.RunScriptButton.Text = "Run Script";
            this.RunScriptButton.UseVisualStyleBackColor = true;
            this.RunScriptButton.Click += new System.EventHandler(this.RunScriptButton_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Controls.Add(this.CandyImagesPanel);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(566, 32);
            this.panel4.TabIndex = 8;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label3);
            this.panel6.Controls.Add(this.AutoClickCandyMatchPercent);
            this.panel6.Controls.Add(this.AutoClickCandyCheckBox);
            this.panel6.Controls.Add(this.AutoClickCandyPeriod);
            this.panel6.Controls.Add(this.LoadCandyImageButton);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(32, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(534, 32);
            this.panel6.TabIndex = 14;
            // 
            // AutoClickCandyCheckBox
            // 
            this.AutoClickCandyCheckBox.AutoSize = true;
            this.AutoClickCandyCheckBox.Location = new System.Drawing.Point(7, 8);
            this.AutoClickCandyCheckBox.Name = "AutoClickCandyCheckBox";
            this.AutoClickCandyCheckBox.Size = new System.Drawing.Size(74, 17);
            this.AutoClickCandyCheckBox.TabIndex = 0;
            this.AutoClickCandyCheckBox.Text = "Auto-Click";
            this.AutoClickCandyCheckBox.UseVisualStyleBackColor = true;
            // 
            // AutoClickCandyPeriod
            // 
            this.AutoClickCandyPeriod.Location = new System.Drawing.Point(86, 6);
            this.AutoClickCandyPeriod.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.AutoClickCandyPeriod.Name = "AutoClickCandyPeriod";
            this.AutoClickCandyPeriod.Size = new System.Drawing.Size(52, 20);
            this.AutoClickCandyPeriod.TabIndex = 8;
            this.AutoClickCandyPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.AutoClickCandyPeriod.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // LoadCandyImageButton
            // 
            this.LoadCandyImageButton.Location = new System.Drawing.Point(292, 5);
            this.LoadCandyImageButton.Name = "LoadCandyImageButton";
            this.LoadCandyImageButton.Size = new System.Drawing.Size(114, 23);
            this.LoadCandyImageButton.TabIndex = 12;
            this.LoadCandyImageButton.Text = "Load Candy Images";
            this.LoadCandyImageButton.UseVisualStyleBackColor = true;
            this.LoadCandyImageButton.Click += new System.EventHandler(this.LoadCandyImageButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(139, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "seconds";
            // 
            // CandyImagesPanel
            // 
            this.CandyImagesPanel.AutoSize = true;
            this.CandyImagesPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.CandyImagesPanel.Location = new System.Drawing.Point(0, 0);
            this.CandyImagesPanel.MinimumSize = new System.Drawing.Size(32, 32);
            this.CandyImagesPanel.Name = "CandyImagesPanel";
            this.CandyImagesPanel.Size = new System.Drawing.Size(32, 32);
            this.CandyImagesPanel.TabIndex = 13;
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputTextBox.Location = new System.Drawing.Point(0, 0);
            this.OutputTextBox.MaxLength = 0;
            this.OutputTextBox.Multiline = true;
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.ReadOnly = true;
            this.OutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputTextBox.Size = new System.Drawing.Size(566, 98);
            this.OutputTextBox.TabIndex = 8;
            this.OutputTextBox.WordWrap = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(566, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // AutoClickCandyMatchPercent
            // 
            this.AutoClickCandyMatchPercent.DecimalPlaces = 1;
            this.AutoClickCandyMatchPercent.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.AutoClickCandyMatchPercent.Location = new System.Drawing.Point(192, 6);
            this.AutoClickCandyMatchPercent.Name = "AutoClickCandyMatchPercent";
            this.AutoClickCandyMatchPercent.Size = new System.Drawing.Size(47, 20);
            this.AutoClickCandyMatchPercent.TabIndex = 13;
            this.AutoClickCandyMatchPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.AutoClickCandyMatchPercent.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(239, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "% match";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 441);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Hey, Sexy!";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LineNumberDisplay)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AutoClickCandyPeriod)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AutoClickCandyMatchPercent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox RecordingText;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Button UpdateRecordingButton;
        private System.Windows.Forms.Button RunScriptButton;
        private System.Windows.Forms.TextBox ScriptTextBox;
        private System.Windows.Forms.Button LoadScriptButton;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.NumericUpDown LineNumberDisplay;
        private System.Windows.Forms.TextBox OutputTextBox;
        private System.Windows.Forms.Button ScreenShotButton;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button LoadCandyImageButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown AutoClickCandyPeriod;
        private System.Windows.Forms.CheckBox AutoClickCandyCheckBox;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel CandyImagesPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown AutoClickCandyMatchPercent;
    }
}

