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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            buttonExtractIso = new System.Windows.Forms.Button();
            buttonExtractGame = new System.Windows.Forms.Button();
            buttonTranslate = new System.Windows.Forms.Button();
            buttonRepackGame = new System.Windows.Forms.Button();
            contextMenuStripDebug = new System.Windows.Forms.ContextMenuStrip(components);
            itemDebugMode = new System.Windows.Forms.ToolStripMenuItem();
            buttonRepackIso = new System.Windows.Forms.Button();
            buttonExtractIsoHelp = new System.Windows.Forms.Button();
            buttonExtractGameHelp = new System.Windows.Forms.Button();
            buttonRepackGameHelp = new System.Windows.Forms.Button();
            buttonTranslateHelp = new System.Windows.Forms.Button();
            buttonRepackIsoHelp = new System.Windows.Forms.Button();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            labelWork = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            DeleteGenRes = new System.Windows.Forms.Button();
            IsoProgress = new System.Windows.Forms.ProgressBar();
            timerWork = new System.Windows.Forms.Timer(components);
            labelVersion = new System.Windows.Forms.Label();
            dragbar = new System.Windows.Forms.Button();
            Exit = new System.Windows.Forms.Button();
            contextMenuStripDebug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // buttonExtractIso
            // 
            buttonExtractIso.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            buttonExtractIso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonExtractIso.Location = new System.Drawing.Point(11, 54);
            buttonExtractIso.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonExtractIso.Name = "buttonExtractIso";
            buttonExtractIso.Size = new System.Drawing.Size(153, 49);
            buttonExtractIso.TabIndex = 0;
            buttonExtractIso.Text = "Extract ISO";
            buttonExtractIso.UseVisualStyleBackColor = true;
            buttonExtractIso.Click += buttonExtractIso_Click;
            // 
            // buttonExtractGame
            // 
            buttonExtractGame.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            buttonExtractGame.Enabled = false;
            buttonExtractGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonExtractGame.Location = new System.Drawing.Point(11, 112);
            buttonExtractGame.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonExtractGame.Name = "buttonExtractGame";
            buttonExtractGame.Size = new System.Drawing.Size(153, 49);
            buttonExtractGame.TabIndex = 1;
            buttonExtractGame.Text = "Extract game files";
            buttonExtractGame.UseVisualStyleBackColor = true;
            buttonExtractGame.Click += buttonExtractGame_Click;
            // 
            // buttonTranslate
            // 
            buttonTranslate.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            buttonTranslate.Enabled = false;
            buttonTranslate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonTranslate.Location = new System.Drawing.Point(11, 171);
            buttonTranslate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonTranslate.Name = "buttonTranslate";
            buttonTranslate.Size = new System.Drawing.Size(153, 49);
            buttonTranslate.TabIndex = 2;
            buttonTranslate.Text = "Translate strings";
            buttonTranslate.UseVisualStyleBackColor = true;
            buttonTranslate.Click += buttonTranslate_Click;
            // 
            // buttonRepackGame
            // 
            buttonRepackGame.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            buttonRepackGame.ContextMenuStrip = contextMenuStripDebug;
            buttonRepackGame.Enabled = false;
            buttonRepackGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRepackGame.Location = new System.Drawing.Point(11, 229);
            buttonRepackGame.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonRepackGame.Name = "buttonRepackGame";
            buttonRepackGame.Size = new System.Drawing.Size(153, 49);
            buttonRepackGame.TabIndex = 3;
            buttonRepackGame.Text = "Repack game files";
            buttonRepackGame.UseVisualStyleBackColor = true;
            buttonRepackGame.Click += buttonRepackGame_Click;
            // 
            // contextMenuStripDebug
            // 
            contextMenuStripDebug.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenuStripDebug.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { itemDebugMode });
            contextMenuStripDebug.Name = "contextMenuStripDebug";
            contextMenuStripDebug.Size = new System.Drawing.Size(167, 28);
            // 
            // itemDebugMode
            // 
            itemDebugMode.CheckOnClick = true;
            itemDebugMode.Name = "itemDebugMode";
            itemDebugMode.Size = new System.Drawing.Size(166, 24);
            itemDebugMode.Text = "Debug mode";
            // 
            // buttonRepackIso
            // 
            buttonRepackIso.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            buttonRepackIso.Enabled = false;
            buttonRepackIso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRepackIso.Location = new System.Drawing.Point(11, 288);
            buttonRepackIso.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonRepackIso.Name = "buttonRepackIso";
            buttonRepackIso.Size = new System.Drawing.Size(153, 49);
            buttonRepackIso.TabIndex = 4;
            buttonRepackIso.Text = "Repack ISO";
            buttonRepackIso.UseVisualStyleBackColor = true;
            buttonRepackIso.Click += buttonRepackIso_Click;
            // 
            // buttonExtractIsoHelp
            // 
            buttonExtractIsoHelp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            buttonExtractIsoHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonExtractIsoHelp.Location = new System.Drawing.Point(169, 72);
            buttonExtractIsoHelp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonExtractIsoHelp.Name = "buttonExtractIsoHelp";
            buttonExtractIsoHelp.Size = new System.Drawing.Size(27, 31);
            buttonExtractIsoHelp.TabIndex = 5;
            buttonExtractIsoHelp.Text = "?";
            buttonExtractIsoHelp.UseVisualStyleBackColor = true;
            buttonExtractIsoHelp.Click += buttonExtractIsoHelp_Click;
            // 
            // buttonExtractGameHelp
            // 
            buttonExtractGameHelp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            buttonExtractGameHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonExtractGameHelp.Location = new System.Drawing.Point(169, 131);
            buttonExtractGameHelp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonExtractGameHelp.Name = "buttonExtractGameHelp";
            buttonExtractGameHelp.Size = new System.Drawing.Size(27, 31);
            buttonExtractGameHelp.TabIndex = 6;
            buttonExtractGameHelp.Text = "?";
            buttonExtractGameHelp.UseVisualStyleBackColor = true;
            buttonExtractGameHelp.Click += buttonExtractGameHelp_Click;
            // 
            // buttonRepackGameHelp
            // 
            buttonRepackGameHelp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            buttonRepackGameHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRepackGameHelp.Location = new System.Drawing.Point(169, 248);
            buttonRepackGameHelp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonRepackGameHelp.Name = "buttonRepackGameHelp";
            buttonRepackGameHelp.Size = new System.Drawing.Size(27, 31);
            buttonRepackGameHelp.TabIndex = 8;
            buttonRepackGameHelp.Text = "?";
            buttonRepackGameHelp.UseVisualStyleBackColor = true;
            buttonRepackGameHelp.Click += buttonRepackGameHelp_Click;
            // 
            // buttonTranslateHelp
            // 
            buttonTranslateHelp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            buttonTranslateHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonTranslateHelp.Location = new System.Drawing.Point(169, 189);
            buttonTranslateHelp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonTranslateHelp.Name = "buttonTranslateHelp";
            buttonTranslateHelp.Size = new System.Drawing.Size(27, 31);
            buttonTranslateHelp.TabIndex = 7;
            buttonTranslateHelp.Text = "?";
            buttonTranslateHelp.UseVisualStyleBackColor = true;
            buttonTranslateHelp.Click += buttonTranslateHelp_Click;
            // 
            // buttonRepackIsoHelp
            // 
            buttonRepackIsoHelp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            buttonRepackIsoHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRepackIsoHelp.Location = new System.Drawing.Point(169, 306);
            buttonRepackIsoHelp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonRepackIsoHelp.Name = "buttonRepackIsoHelp";
            buttonRepackIsoHelp.Size = new System.Drawing.Size(27, 31);
            buttonRepackIsoHelp.TabIndex = 9;
            buttonRepackIsoHelp.Text = "?";
            buttonRepackIsoHelp.UseVisualStyleBackColor = true;
            buttonRepackIsoHelp.Click += buttonRepackIsoHelp_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            pictureBox1.BackgroundImage = ((System.Drawing.Image)resources.GetObject("pictureBox1.BackgroundImage"));
            pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pictureBox1.Location = new System.Drawing.Point(13, 34);
            pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(351, 348);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 10;
            pictureBox1.TabStop = false;
            // 
            // labelWork
            // 
            labelWork.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            labelWork.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)204));
            labelWork.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            labelWork.Location = new System.Drawing.Point(11, 2);
            labelWork.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelWork.Name = "labelWork";
            labelWork.Size = new System.Drawing.Size(185, 42);
            labelWork.TabIndex = 11;
            labelWork.Text = "Ready";
            labelWork.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Right));
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(DeleteGenRes);
            panel1.Controls.Add(buttonExtractIso);
            panel1.Controls.Add(IsoProgress);
            panel1.Controls.Add(labelWork);
            panel1.Controls.Add(buttonRepackIsoHelp);
            panel1.Controls.Add(buttonRepackGameHelp);
            panel1.Controls.Add(buttonTranslateHelp);
            panel1.Controls.Add(buttonExtractGameHelp);
            panel1.Controls.Add(buttonExtractIsoHelp);
            panel1.Controls.Add(buttonRepackIso);
            panel1.Controls.Add(buttonRepackGame);
            panel1.Controls.Add(buttonTranslate);
            panel1.Controls.Add(buttonExtractGame);
            panel1.Location = new System.Drawing.Point(373, 34);
            panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(210, 350);
            panel1.TabIndex = 12;
            // 
            // DeleteGenRes
            // 
            DeleteGenRes.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            DeleteGenRes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            DeleteGenRes.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
            DeleteGenRes.ForeColor = System.Drawing.Color.Red;
            DeleteGenRes.Location = new System.Drawing.Point(11, 112);
            DeleteGenRes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            DeleteGenRes.Name = "DeleteGenRes";
            DeleteGenRes.Size = new System.Drawing.Size(153, 49);
            DeleteGenRes.TabIndex = 13;
            DeleteGenRes.Text = "Delete generated resources";
            DeleteGenRes.UseVisualStyleBackColor = true;
            DeleteGenRes.Visible = false;
            DeleteGenRes.Click += DeleteGenRes_Click;
            // 
            // IsoProgress
            // 
            IsoProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            IsoProgress.Location = new System.Drawing.Point(11, 54);
            IsoProgress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            IsoProgress.Name = "IsoProgress";
            IsoProgress.Size = new System.Drawing.Size(153, 49);
            IsoProgress.TabIndex = 12;
            IsoProgress.Value = 34;
            // 
            // timerWork
            // 
            timerWork.Interval = 500;
            timerWork.Tick += timerWork_Tick;
            // 
            // labelVersion
            // 
            labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            labelVersion.AutoSize = true;
            labelVersion.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)170)), ((int)((byte)170)), ((int)((byte)170)));
            labelVersion.Location = new System.Drawing.Point(14, 358);
            labelVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new System.Drawing.Size(42, 20);
            labelVersion.TabIndex = 13;
            labelVersion.Text = "X.X.X";
            labelVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dragbar
            // 
            dragbar.Dock = System.Windows.Forms.DockStyle.Top;
            dragbar.Enabled = false;
            dragbar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            dragbar.Location = new System.Drawing.Point(0, 0);
            dragbar.Name = "dragbar";
            dragbar.Size = new System.Drawing.Size(595, 26);
            dragbar.TabIndex = 14;
            dragbar.UseVisualStyleBackColor = true;
            // 
            // Exit
            // 
            Exit.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Exit.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
            Exit.ForeColor = System.Drawing.Color.Red;
            Exit.Location = new System.Drawing.Point(563, 0);
            Exit.Name = "Exit";
            Exit.Size = new System.Drawing.Size(32, 26);
            Exit.TabIndex = 15;
            Exit.Text = "X";
            Exit.UseVisualStyleBackColor = true;
            Exit.Click += Exit_click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(595, 396);
            Controls.Add(Exit);
            Controls.Add(dragbar);
            Controls.Add(labelVersion);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = ((System.Drawing.Icon)resources.GetObject("$this.Icon"));
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MaximizeBox = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ToradoraTranslateTool";
            FormClosing += FormMain_FormClosing;
            contextMenuStripDebug.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button DeleteGenRes;

        private System.Windows.Forms.Button Exit;

        private System.Windows.Forms.Button dragbar;

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