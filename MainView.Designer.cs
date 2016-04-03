namespace BigData
{
    partial class MainView
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
            this.cbLow = new System.Windows.Forms.CheckBox();
            this.nudStocksCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbClose = new System.Windows.Forms.CheckBox();
            this.cbOpen = new System.Windows.Forms.CheckBox();
            this.cbHigh = new System.Windows.Forms.CheckBox();
            this.nudDays = new System.Windows.Forms.NumericUpDown();
            this.nudK = new System.Windows.Forms.NumericUpDown();
            this.pic5 = new System.Windows.Forms.PictureBox();
            this.pic4 = new System.Windows.Forms.PictureBox();
            this.pic3 = new System.Windows.Forms.PictureBox();
            this.pic2 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pic1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudStocksCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbLow
            // 
            this.cbLow.AutoSize = true;
            this.cbLow.Checked = true;
            this.cbLow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLow.Location = new System.Drawing.Point(134, 97);
            this.cbLow.Name = "cbLow";
            this.cbLow.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbLow.Size = new System.Drawing.Size(50, 17);
            this.cbLow.TabIndex = 0;
            this.cbLow.Text = "נמוך";
            this.cbLow.UseVisualStyleBackColor = true;
            // 
            // nudStocksCount
            // 
            this.nudStocksCount.Location = new System.Drawing.Point(111, 15);
            this.nudStocksCount.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudStocksCount.Name = "nudStocksCount";
            this.nudStocksCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.nudStocksCount.Size = new System.Drawing.Size(55, 20);
            this.nudStocksCount.TabIndex = 1;
            this.nudStocksCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "מספר מניות";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "מספר אשכולות";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "מספר ימים לאחור";
            // 
            // cbClose
            // 
            this.cbClose.AutoSize = true;
            this.cbClose.Checked = true;
            this.cbClose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbClose.Location = new System.Drawing.Point(190, 97);
            this.cbClose.Name = "cbClose";
            this.cbClose.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbClose.Size = new System.Drawing.Size(57, 17);
            this.cbClose.TabIndex = 6;
            this.cbClose.Text = "סגירה";
            this.cbClose.UseVisualStyleBackColor = true;
            // 
            // cbOpen
            // 
            this.cbOpen.AutoSize = true;
            this.cbOpen.Checked = true;
            this.cbOpen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOpen.Location = new System.Drawing.Point(12, 97);
            this.cbOpen.Name = "cbOpen";
            this.cbOpen.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbOpen.Size = new System.Drawing.Size(59, 17);
            this.cbOpen.TabIndex = 7;
            this.cbOpen.Text = "פתיחה";
            this.cbOpen.UseVisualStyleBackColor = true;
            // 
            // cbHigh
            // 
            this.cbHigh.AutoSize = true;
            this.cbHigh.Checked = true;
            this.cbHigh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHigh.Location = new System.Drawing.Point(77, 97);
            this.cbHigh.Name = "cbHigh";
            this.cbHigh.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbHigh.Size = new System.Drawing.Size(51, 17);
            this.cbHigh.TabIndex = 8;
            this.cbHigh.Text = "גבוה";
            this.cbHigh.UseVisualStyleBackColor = true;
            // 
            // nudDays
            // 
            this.nudDays.Location = new System.Drawing.Point(111, 41);
            this.nudDays.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudDays.Name = "nudDays";
            this.nudDays.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.nudDays.Size = new System.Drawing.Size(55, 20);
            this.nudDays.TabIndex = 9;
            this.nudDays.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // nudK
            // 
            this.nudK.Location = new System.Drawing.Point(111, 67);
            this.nudK.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudK.Name = "nudK";
            this.nudK.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.nudK.Size = new System.Drawing.Size(55, 20);
            this.nudK.TabIndex = 10;
            this.nudK.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // pic5
            // 
            this.pic5.Image = global::BigData.Properties.Resources.loading;
            this.pic5.Location = new System.Drawing.Point(203, 137);
            this.pic5.Name = "pic5";
            this.pic5.Size = new System.Drawing.Size(40, 40);
            this.pic5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic5.TabIndex = 21;
            this.pic5.TabStop = false;
            this.pic5.Click += new System.EventHandler(this.OpenResultsWin);
            // 
            // pic4
            // 
            this.pic4.Image = global::BigData.Properties.Resources.Dis_graph;
            this.pic4.Location = new System.Drawing.Point(157, 137);
            this.pic4.Name = "pic4";
            this.pic4.Size = new System.Drawing.Size(40, 40);
            this.pic4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic4.TabIndex = 20;
            this.pic4.TabStop = false;
            // 
            // pic3
            // 
            this.pic3.Image = global::BigData.Properties.Resources.Dis_brain;
            this.pic3.Location = new System.Drawing.Point(111, 137);
            this.pic3.Name = "pic3";
            this.pic3.Size = new System.Drawing.Size(40, 40);
            this.pic3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic3.TabIndex = 19;
            this.pic3.TabStop = false;
            // 
            // pic2
            // 
            this.pic2.Enabled = false;
            this.pic2.Image = global::BigData.Properties.Resources.Dis_connect;
            this.pic2.Location = new System.Drawing.Point(65, 137);
            this.pic2.Name = "pic2";
            this.pic2.Size = new System.Drawing.Size(40, 40);
            this.pic2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic2.TabIndex = 18;
            this.pic2.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = global::BigData.Properties.Resources.go;
            this.pictureBox7.Location = new System.Drawing.Point(178, 17);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(65, 65);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 17;
            this.pictureBox7.TabStop = false;
            this.pictureBox7.Click += new System.EventHandler(this.Action);
            // 
            // pic1
            // 
            this.pic1.Enabled = false;
            this.pic1.Image = global::BigData.Properties.Resources.upDown;
            this.pic1.Location = new System.Drawing.Point(19, 137);
            this.pic1.Name = "pic1";
            this.pic1.Size = new System.Drawing.Size(40, 40);
            this.pic1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic1.TabIndex = 11;
            this.pic1.TabStop = false;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 124);
            this.Controls.Add(this.pic5);
            this.Controls.Add(this.pic4);
            this.Controls.Add(this.pic3);
            this.Controls.Add(this.pic2);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.pic1);
            this.Controls.Add(this.nudK);
            this.Controls.Add(this.nudDays);
            this.Controls.Add(this.cbHigh);
            this.Controls.Add(this.cbOpen);
            this.Controls.Add(this.cbClose);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudStocksCount);
            this.Controls.Add(this.cbLow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainView";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudStocksCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbLow;
        private System.Windows.Forms.NumericUpDown nudStocksCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbClose;
        private System.Windows.Forms.CheckBox cbOpen;
        private System.Windows.Forms.CheckBox cbHigh;
        private System.Windows.Forms.NumericUpDown nudDays;
        private System.Windows.Forms.NumericUpDown nudK;
        private System.Windows.Forms.PictureBox pic1;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pic2;
        private System.Windows.Forms.PictureBox pic3;
        private System.Windows.Forms.PictureBox pic4;
        private System.Windows.Forms.PictureBox pic5;
    }
}

