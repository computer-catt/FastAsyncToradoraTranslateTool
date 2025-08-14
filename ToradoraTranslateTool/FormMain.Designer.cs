namespace ToradoraTranslateTool
{
    partial class FormMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonExtractIso = new System.Windows.Forms.Button();
            this.buttonExtractGame = new System.Windows.Forms.Button();
            this.buttonTranslate = new System.Windows.Forms.Button();
            this.buttonRepackGame = new System.Windows.Forms.Button();
            this.contextMenuStripDebug = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemDebugMode = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonRepackIso = new System.Windows.Forms.Button();
            this.buttonExtractIsoHelp = new System.Windows.Forms.Button();
            this.buttonExtractGameHelp = new System.Windows.Forms.Button();
            this.buttonRepackGameHelp = new System.Windows.Forms.Button();
            this.buttonTranslateHelp = new System.Windows.Forms.Button();
            this.buttonRepackIsoHelp = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelWork = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.IsoProgress = new System.Windows.Forms.ProgressBar();
            this.timerWork = new System.Windows.Forms.Timer(this.components);
            this.labelVersion = new System.Windows.Forms.Label();
            this.contextMenuStripDebug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonExtractIso
            // 
            this.buttonExtractIso.Location = new System.Drawing.Point(11, 43);
            this.buttonExtractIso.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExtractIso.Name = "buttonExtractIso";
            this.buttonExtractIso.Size = new System.Drawing.Size(153, 39);
            this.buttonExtractIso.TabIndex = 0;
            this.buttonExtractIso.Text = "Extract ISO";
            this.buttonExtractIso.UseVisualStyleBackColor = true;
            this.buttonExtractIso.Click += new System.EventHandler(this.buttonExtractIso_Click);
            // 
            // buttonExtractGame
            // 
            this.buttonExtractGame.Enabled = false;
            this.buttonExtractGame.Location = new System.Drawing.Point(11, 90);
            this.buttonExtractGame.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExtractGame.Name = "buttonExtractGame";
            this.buttonExtractGame.Size = new System.Drawing.Size(153, 39);
            this.buttonExtractGame.TabIndex = 1;
            this.buttonExtractGame.Text = "Extract game files";
            this.buttonExtractGame.UseVisualStyleBackColor = true;
            this.buttonExtractGame.Click += new System.EventHandler(this.buttonExtractGame_Click);
            // 
            // buttonTranslate
            // 
            this.buttonTranslate.Enabled = false;
            this.buttonTranslate.Location = new System.Drawing.Point(11, 137);
            this.buttonTranslate.Margin = new System.Windows.Forms.Padding(4);
            this.buttonTranslate.Name = "buttonTranslate";
            this.buttonTranslate.Size = new System.Drawing.Size(153, 39);
            this.buttonTranslate.TabIndex = 2;
            this.buttonTranslate.Text = "Translate strings";
            this.buttonTranslate.UseVisualStyleBackColor = true;
            this.buttonTranslate.Click += new System.EventHandler(this.buttonTranslate_Click);
            // 
            // buttonRepackGame
            // 
            this.buttonRepackGame.ContextMenuStrip = this.contextMenuStripDebug;
            this.buttonRepackGame.Enabled = false;
            this.buttonRepackGame.Location = new System.Drawing.Point(11, 183);
            this.buttonRepackGame.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRepackGame.Name = "buttonRepackGame";
            this.buttonRepackGame.Size = new System.Drawing.Size(153, 39);
            this.buttonRepackGame.TabIndex = 3;
            this.buttonRepackGame.Text = "Repack game files";
            this.buttonRepackGame.UseVisualStyleBackColor = true;
            this.buttonRepackGame.Click += new System.EventHandler(this.buttonRepackGame_Click);
            // 
            // contextMenuStripDebug
            // 
            this.contextMenuStripDebug.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.itemDebugMode });
            this.contextMenuStripDebug.Name = "contextMenuStripDebug";
            this.contextMenuStripDebug.Size = new System.Drawing.Size(167, 28);
            // 
            // itemDebugMode
            // 
            this.itemDebugMode.CheckOnClick = true;
            this.itemDebugMode.Name = "itemDebugMode";
            this.itemDebugMode.Size = new System.Drawing.Size(166, 24);
            this.itemDebugMode.Text = "Debug mode";
            // 
            // buttonRepackIso
            // 
            this.buttonRepackIso.Enabled = false;
            this.buttonRepackIso.Location = new System.Drawing.Point(11, 230);
            this.buttonRepackIso.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRepackIso.Name = "buttonRepackIso";
            this.buttonRepackIso.Size = new System.Drawing.Size(153, 39);
            this.buttonRepackIso.TabIndex = 4;
            this.buttonRepackIso.Text = "Repack ISO";
            this.buttonRepackIso.UseVisualStyleBackColor = true;
            this.buttonRepackIso.Click += new System.EventHandler(this.buttonRepackIso_Click);
            // 
            // buttonExtractIsoHelp
            // 
            this.buttonExtractIsoHelp.Location = new System.Drawing.Point(169, 58);
            this.buttonExtractIsoHelp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExtractIsoHelp.Name = "buttonExtractIsoHelp";
            this.buttonExtractIsoHelp.Size = new System.Drawing.Size(27, 25);
            this.buttonExtractIsoHelp.TabIndex = 5;
            this.buttonExtractIsoHelp.Text = "?";
            this.buttonExtractIsoHelp.UseVisualStyleBackColor = true;
            this.buttonExtractIsoHelp.Click += new System.EventHandler(this.buttonExtractIsoHelp_Click);
            // 
            // buttonExtractGameHelp
            // 
            this.buttonExtractGameHelp.Location = new System.Drawing.Point(169, 105);
            this.buttonExtractGameHelp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExtractGameHelp.Name = "buttonExtractGameHelp";
            this.buttonExtractGameHelp.Size = new System.Drawing.Size(27, 25);
            this.buttonExtractGameHelp.TabIndex = 6;
            this.buttonExtractGameHelp.Text = "?";
            this.buttonExtractGameHelp.UseVisualStyleBackColor = true;
            this.buttonExtractGameHelp.Click += new System.EventHandler(this.buttonExtractGameHelp_Click);
            // 
            // buttonRepackGameHelp
            // 
            this.buttonRepackGameHelp.Location = new System.Drawing.Point(169, 198);
            this.buttonRepackGameHelp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRepackGameHelp.Name = "buttonRepackGameHelp";
            this.buttonRepackGameHelp.Size = new System.Drawing.Size(27, 25);
            this.buttonRepackGameHelp.TabIndex = 8;
            this.buttonRepackGameHelp.Text = "?";
            this.buttonRepackGameHelp.UseVisualStyleBackColor = true;
            this.buttonRepackGameHelp.Click += new System.EventHandler(this.buttonRepackGameHelp_Click);
            // 
            // buttonTranslateHelp
            // 
            this.buttonTranslateHelp.Location = new System.Drawing.Point(169, 151);
            this.buttonTranslateHelp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonTranslateHelp.Name = "buttonTranslateHelp";
            this.buttonTranslateHelp.Size = new System.Drawing.Size(27, 25);
            this.buttonTranslateHelp.TabIndex = 7;
            this.buttonTranslateHelp.Text = "?";
            this.buttonTranslateHelp.UseVisualStyleBackColor = true;
            this.buttonTranslateHelp.Click += new System.EventHandler(this.buttonTranslateHelp_Click);
            // 
            // buttonRepackIsoHelp
            // 
            this.buttonRepackIsoHelp.Location = new System.Drawing.Point(169, 245);
            this.buttonRepackIsoHelp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRepackIsoHelp.Name = "buttonRepackIsoHelp";
            this.buttonRepackIsoHelp.Size = new System.Drawing.Size(27, 25);
            this.buttonRepackIsoHelp.TabIndex = 9;
            this.buttonRepackIsoHelp.Text = "?";
            this.buttonRepackIsoHelp.UseVisualStyleBackColor = true;
            this.buttonRepackIsoHelp.Click += new System.EventHandler(this.buttonRepackIsoHelp_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 6);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(351, 279);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // labelWork
            // 
            this.labelWork.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelWork.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelWork.Location = new System.Drawing.Point(11, 2);
            this.labelWork.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelWork.Name = "labelWork";
            this.labelWork.Size = new System.Drawing.Size(185, 34);
            this.labelWork.TabIndex = 11;
            this.labelWork.Text = "Ready";
            this.labelWork.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.buttonExtractIso);
            this.panel1.Controls.Add(this.IsoProgress);
            this.panel1.Controls.Add(this.labelWork);
            this.panel1.Controls.Add(this.buttonRepackIsoHelp);
            this.panel1.Controls.Add(this.buttonRepackGameHelp);
            this.panel1.Controls.Add(this.buttonTranslateHelp);
            this.panel1.Controls.Add(this.buttonExtractGameHelp);
            this.panel1.Controls.Add(this.buttonExtractIsoHelp);
            this.panel1.Controls.Add(this.buttonRepackIso);
            this.panel1.Controls.Add(this.buttonRepackGame);
            this.panel1.Controls.Add(this.buttonTranslate);
            this.panel1.Controls.Add(this.buttonExtractGame);
            this.panel1.Location = new System.Drawing.Point(372, 6);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(210, 280);
            this.panel1.TabIndex = 12;
            // 
            // IsoProgress
            // 
            this.IsoProgress.Location = new System.Drawing.Point(11, 43);
            this.IsoProgress.Name = "IsoProgress";
            this.IsoProgress.Size = new System.Drawing.Size(153, 39);
            this.IsoProgress.TabIndex = 12;
            this.IsoProgress.Value = 34;
            // 
            // timerWork
            // 
            this.timerWork.Interval = 500;
            this.timerWork.Tick += new System.EventHandler(this.timerWork_Tick);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.labelVersion.Location = new System.Drawing.Point(13, 266);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(43, 17);
            this.labelVersion.TabIndex = 13;
            this.labelVersion.Text = "X.X.X";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 289);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ToradoraTranslateTool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.contextMenuStripDebug.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ProgressBar IsoProgress;

        #endregion

        private System.Windows.Forms.Button buttonExtractIso;
        private System.Windows.Forms.Button buttonExtractGame;
        private System.Windows.Forms.Button buttonTranslate;
        private System.Windows.Forms.Button buttonRepackGame;
        private System.Windows.Forms.Button buttonRepackIso;
        private System.Windows.Forms.Button buttonExtractIsoHelp;
        private System.Windows.Forms.Button buttonExtractGameHelp;
        private System.Windows.Forms.Button buttonRepackGameHelp;
        private System.Windows.Forms.Button buttonTranslateHelp;
        private System.Windows.Forms.Button buttonRepackIsoHelp;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelWork;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer timerWork;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDebug;
        private System.Windows.Forms.ToolStripMenuItem itemDebugMode;
    }
}

