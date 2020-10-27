namespace MM
{
    partial class Main
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.timeLayerBox = new System.Windows.Forms.Panel();
            this.timeLayerValues = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.trackBox = new System.Windows.Forms.GroupBox();
            this.liveCheck = new System.Windows.Forms.CheckBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBarCurLab = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.panel1.SuspendLayout();
            this.timeLayerBox.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.trackBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.BackColor = System.Drawing.Color.Gainsboro;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(3, 3);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(778, 561);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel1.Controls.Add(this.comboBox3);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.timeLayerBox);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(19, 77);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(302, 120);
            this.panel1.TabIndex = 1;
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(76, 58);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(190, 21);
            this.comboBox3.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "SeriesType";
            // 
            // timeLayerBox
            // 
            this.timeLayerBox.Controls.Add(this.timeLayerValues);
            this.timeLayerBox.Controls.Add(this.label4);
            this.timeLayerBox.Location = new System.Drawing.Point(10, 78);
            this.timeLayerBox.Name = "timeLayerBox";
            this.timeLayerBox.Size = new System.Drawing.Size(289, 31);
            this.timeLayerBox.TabIndex = 3;
            // 
            // timeLayerValues
            // 
            this.timeLayerValues.FormattingEnabled = true;
            this.timeLayerValues.Location = new System.Drawing.Point(71, 7);
            this.timeLayerValues.Name = "timeLayerValues";
            this.timeLayerValues.Size = new System.Drawing.Size(56, 21);
            this.timeLayerValues.TabIndex = 1;
            this.timeLayerValues.TabStop = false;
            this.timeLayerValues.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Time  Layer";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.comboBox2);
            this.panel5.Controls.Add(this.label2);
            this.panel5.Location = new System.Drawing.Point(10, 24);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(289, 31);
            this.panel5.TabIndex = 3;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(53, 7);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(233, 21);
            this.comboBox2.TabIndex = 1;
            this.comboBox2.TabStop = false;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(63, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(233, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.TabStop = false;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Solution";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(344, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(682, 193);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            this.richTextBox1.ZoomFactor = 2F;
            this.richTextBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.richTextBox1_MouseDoubleClick);
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel2.Controls.Add(this.checkBox1);
            this.panel2.Controls.Add(this.trackBox);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.richTextBox1);
            this.panel2.Location = new System.Drawing.Point(12, 585);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1029, 200);
            this.panel2.TabIndex = 3;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(275, 35);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(40, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "3D";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // trackBox
            // 
            this.trackBox.Controls.Add(this.liveCheck);
            this.trackBox.Controls.Add(this.trackBar1);
            this.trackBox.Controls.Add(this.trackBarCurLab);
            this.trackBox.Location = new System.Drawing.Point(19, 4);
            this.trackBox.Name = "trackBox";
            this.trackBox.Size = new System.Drawing.Size(233, 67);
            this.trackBox.TabIndex = 3;
            this.trackBox.TabStop = false;
            this.trackBox.Text = "Change Coef";
            // 
            // liveCheck
            // 
            this.liveCheck.AutoSize = true;
            this.liveCheck.Location = new System.Drawing.Point(25, 44);
            this.liveCheck.Name = "liveCheck";
            this.liveCheck.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.liveCheck.Size = new System.Drawing.Size(46, 17);
            this.liveCheck.TabIndex = 2;
            this.liveCheck.Text = "Live";
            this.liveCheck.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(63, 19);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(163, 45);
            this.trackBar1.TabIndex = 1;
            // 
            // trackBarCurLab
            // 
            this.trackBarCurLab.AutoSize = true;
            this.trackBarCurLab.Location = new System.Drawing.Point(22, 20);
            this.trackBarCurLab.Name = "trackBarCurLab";
            this.trackBarCurLab.Size = new System.Drawing.Size(35, 13);
            this.trackBarCurLab.TabIndex = 0;
            this.trackBarCurLab.Text = "label3";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Location = new System.Drawing.Point(787, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(236, 561);
            this.panel3.TabIndex = 4;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.Controls.Add(this.chart1);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Location = new System.Drawing.Point(12, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1026, 567);
            this.panel4.TabIndex = 5;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 797);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Name = "Main";
            this.Text = "Last Of Us";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.timeLayerBox.ResumeLayout(false);
            this.timeLayerBox.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.trackBox.ResumeLayout(false);
            this.trackBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.GroupBox trackBox;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label trackBarCurLab;
        private System.Windows.Forms.CheckBox liveCheck;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Panel timeLayerBox;
        private System.Windows.Forms.ComboBox timeLayerValues;
        private System.Windows.Forms.Label label4;
    }
}

