using System.Windows.Forms;

namespace V3UnityFontReader
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonLoadPNG = new System.Windows.Forms.Button();
            this.ButtonLoadTXT = new System.Windows.Forms.Button();
            this.ButtonPrevious = new System.Windows.Forms.Button();
            this.ButtonNext = new System.Windows.Forms.Button();
            this.InfoTextbox = new System.Windows.Forms.TextBox();
            this.ButtonExtractAll = new System.Windows.Forms.Button();
            this.ButtonReplaceAll = new System.Windows.Forms.Button();
            this.ButtonSavePNG = new System.Windows.Forms.Button();
            this.ButtonSaveTXT = new System.Windows.Forms.Button();
            this.TextboxNormalY = new System.Windows.Forms.TextBox();
            this.LabelConversion1 = new System.Windows.Forms.Label();
            this.TextboxTableY = new System.Windows.Forms.TextBox();
            this.TextboxUsedY = new System.Windows.Forms.TextBox();
            this.LabelConversion2 = new System.Windows.Forms.Label();
            this.LabelNormalY = new System.Windows.Forms.Label();
            this.LabelTableY = new System.Windows.Forms.Label();
            this.LabelUsedY = new System.Windows.Forms.Label();
            this.ButtonConvert = new System.Windows.Forms.Button();
            this.ButtonReset = new System.Windows.Forms.Button();
            this.TextboxChar = new System.Windows.Forms.TextBox();
            this.TextboxNumber = new System.Windows.Forms.TextBox();
            this.ImagePanel = new System.Windows.Forms.Panel();
            this.PictureBoxImage = new System.Windows.Forms.PictureBox();
            this.ButtonOpenFont = new System.Windows.Forms.Button();
            this.LabelFontName = new System.Windows.Forms.Label();
            this.CheckboxAddX = new System.Windows.Forms.CheckBox();
            this.TextboxFontSize = new System.Windows.Forms.TextBox();
            this.LabelFontSize = new System.Windows.Forms.Label();
            this.TextboxX = new System.Windows.Forms.TextBox();
            this.LabelX = new System.Windows.Forms.Label();
            this.ImagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxImage)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonLoadPNG
            // 
            this.ButtonLoadPNG.Location = new System.Drawing.Point(18, 583);
            this.ButtonLoadPNG.Name = "ButtonLoadPNG";
            this.ButtonLoadPNG.Size = new System.Drawing.Size(69, 34);
            this.ButtonLoadPNG.TabIndex = 1;
            this.ButtonLoadPNG.Text = "Load PNG";
            this.ButtonLoadPNG.UseVisualStyleBackColor = true;
            this.ButtonLoadPNG.Click += new System.EventHandler(this.button1_Click);
            // 
            // ButtonLoadTXT
            // 
            this.ButtonLoadTXT.Location = new System.Drawing.Point(18, 619);
            this.ButtonLoadTXT.Name = "ButtonLoadTXT";
            this.ButtonLoadTXT.Size = new System.Drawing.Size(69, 36);
            this.ButtonLoadTXT.TabIndex = 2;
            this.ButtonLoadTXT.Text = "Load TXT";
            this.ButtonLoadTXT.UseVisualStyleBackColor = true;
            this.ButtonLoadTXT.Click += new System.EventHandler(this.button2_Click);
            // 
            // ButtonPrevious
            // 
            this.ButtonPrevious.Location = new System.Drawing.Point(93, 583);
            this.ButtonPrevious.Name = "ButtonPrevious";
            this.ButtonPrevious.Size = new System.Drawing.Size(68, 34);
            this.ButtonPrevious.TabIndex = 3;
            this.ButtonPrevious.Text = "<---";
            this.ButtonPrevious.UseVisualStyleBackColor = true;
            this.ButtonPrevious.Click += new System.EventHandler(this.button3_Click);
            // 
            // ButtonNext
            // 
            this.ButtonNext.Location = new System.Drawing.Point(167, 583);
            this.ButtonNext.Name = "ButtonNext";
            this.ButtonNext.Size = new System.Drawing.Size(71, 34);
            this.ButtonNext.TabIndex = 4;
            this.ButtonNext.Text = "--->";
            this.ButtonNext.UseVisualStyleBackColor = true;
            this.ButtonNext.Click += new System.EventHandler(this.button4_Click);
            // 
            // InfoTextbox
            // 
            this.InfoTextbox.Location = new System.Drawing.Point(330, 588);
            this.InfoTextbox.Name = "InfoTextbox";
            this.InfoTextbox.ReadOnly = true;
            this.InfoTextbox.Size = new System.Drawing.Size(681, 23);
            this.InfoTextbox.TabIndex = 5;
            this.InfoTextbox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // ButtonExtractAll
            // 
            this.ButtonExtractAll.Location = new System.Drawing.Point(93, 619);
            this.ButtonExtractAll.Name = "ButtonExtractAll";
            this.ButtonExtractAll.Size = new System.Drawing.Size(68, 36);
            this.ButtonExtractAll.TabIndex = 6;
            this.ButtonExtractAll.Text = "ExtractAll";
            this.ButtonExtractAll.UseVisualStyleBackColor = true;
            this.ButtonExtractAll.Click += new System.EventHandler(this.button5_Click);
            // 
            // ButtonReplaceAll
            // 
            this.ButtonReplaceAll.Location = new System.Drawing.Point(167, 619);
            this.ButtonReplaceAll.Name = "ButtonReplaceAll";
            this.ButtonReplaceAll.Size = new System.Drawing.Size(71, 36);
            this.ButtonReplaceAll.TabIndex = 7;
            this.ButtonReplaceAll.Text = "ReplaceAll";
            this.ButtonReplaceAll.UseVisualStyleBackColor = true;
            this.ButtonReplaceAll.Click += new System.EventHandler(this.button6_Click);
            // 
            // ButtonSavePNG
            // 
            this.ButtonSavePNG.Location = new System.Drawing.Point(244, 583);
            this.ButtonSavePNG.Name = "ButtonSavePNG";
            this.ButtonSavePNG.Size = new System.Drawing.Size(71, 34);
            this.ButtonSavePNG.TabIndex = 8;
            this.ButtonSavePNG.Text = "Save .PNG";
            this.ButtonSavePNG.UseVisualStyleBackColor = true;
            this.ButtonSavePNG.Click += new System.EventHandler(this.button7_Click);
            // 
            // ButtonSaveTXT
            // 
            this.ButtonSaveTXT.Location = new System.Drawing.Point(244, 619);
            this.ButtonSaveTXT.Name = "ButtonSaveTXT";
            this.ButtonSaveTXT.Size = new System.Drawing.Size(71, 36);
            this.ButtonSaveTXT.TabIndex = 9;
            this.ButtonSaveTXT.Text = "Save .TXT";
            this.ButtonSaveTXT.UseVisualStyleBackColor = true;
            this.ButtonSaveTXT.Click += new System.EventHandler(this.button8_Click);
            // 
            // TextboxNormalY
            // 
            this.TextboxNormalY.Location = new System.Drawing.Point(428, 627);
            this.TextboxNormalY.Name = "TextboxNormalY";
            this.TextboxNormalY.Size = new System.Drawing.Size(95, 23);
            this.TextboxNormalY.TabIndex = 10;
            // 
            // LabelConversion1
            // 
            this.LabelConversion1.AutoSize = true;
            this.LabelConversion1.Location = new System.Drawing.Point(528, 629);
            this.LabelConversion1.Name = "LabelConversion1";
            this.LabelConversion1.Size = new System.Drawing.Size(31, 15);
            this.LabelConversion1.TabIndex = 11;
            this.LabelConversion1.Text = "<=>";
            // 
            // TextboxTableY
            // 
            this.TextboxTableY.Location = new System.Drawing.Point(567, 627);
            this.TextboxTableY.Name = "TextboxTableY";
            this.TextboxTableY.Size = new System.Drawing.Size(112, 23);
            this.TextboxTableY.TabIndex = 12;
            // 
            // TextboxUsedY
            // 
            this.TextboxUsedY.Location = new System.Drawing.Point(723, 627);
            this.TextboxUsedY.Name = "TextboxUsedY";
            this.TextboxUsedY.Size = new System.Drawing.Size(118, 23);
            this.TextboxUsedY.TabIndex = 13;
            // 
            // LabelConversion2
            // 
            this.LabelConversion2.AutoSize = true;
            this.LabelConversion2.Location = new System.Drawing.Point(683, 629);
            this.LabelConversion2.Name = "LabelConversion2";
            this.LabelConversion2.Size = new System.Drawing.Size(31, 15);
            this.LabelConversion2.TabIndex = 14;
            this.LabelConversion2.Text = "<=>";
            // 
            // LabelNormalY
            // 
            this.LabelNormalY.AutoSize = true;
            this.LabelNormalY.Location = new System.Drawing.Point(444, 611);
            this.LabelNormalY.Name = "LabelNormalY";
            this.LabelNormalY.Size = new System.Drawing.Size(57, 15);
            this.LabelNormalY.TabIndex = 15;
            this.LabelNormalY.Text = "Normal Y";
            // 
            // LabelTableY
            // 
            this.LabelTableY.AutoSize = true;
            this.LabelTableY.Location = new System.Drawing.Point(576, 611);
            this.LabelTableY.Name = "LabelTableY";
            this.LabelTableY.Size = new System.Drawing.Size(89, 15);
            this.LabelTableY.TabIndex = 16;
            this.LabelTableY.Text = "Table Rect Y (C)";
            this.LabelTableY.Click += new System.EventHandler(this.LabelTableY_Click);
            // 
            // LabelUsedY
            // 
            this.LabelUsedY.AutoSize = true;
            this.LabelUsedY.Location = new System.Drawing.Point(723, 611);
            this.LabelUsedY.Name = "LabelUsedY";
            this.LabelUsedY.Size = new System.Drawing.Size(96, 15);
            this.LabelUsedY.TabIndex = 17;
            this.LabelUsedY.Text = "Used Glyph Y (C)";
            this.LabelUsedY.Click += new System.EventHandler(this.LabelUsedY_Click);
            // 
            // ButtonConvert
            // 
            this.ButtonConvert.Location = new System.Drawing.Point(845, 611);
            this.ButtonConvert.Name = "ButtonConvert";
            this.ButtonConvert.Size = new System.Drawing.Size(72, 23);
            this.ButtonConvert.TabIndex = 18;
            this.ButtonConvert.Text = "Convert";
            this.ButtonConvert.UseVisualStyleBackColor = true;
            this.ButtonConvert.Click += new System.EventHandler(this.ButtonConvert_Click);
            // 
            // ButtonReset
            // 
            this.ButtonReset.Location = new System.Drawing.Point(845, 638);
            this.ButtonReset.Name = "ButtonReset";
            this.ButtonReset.Size = new System.Drawing.Size(72, 23);
            this.ButtonReset.TabIndex = 19;
            this.ButtonReset.Text = "Reset";
            this.ButtonReset.UseVisualStyleBackColor = true;
            this.ButtonReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // TextboxChar
            // 
            this.TextboxChar.Location = new System.Drawing.Point(922, 613);
            this.TextboxChar.Name = "TextboxChar";
            this.TextboxChar.Size = new System.Drawing.Size(100, 23);
            this.TextboxChar.TabIndex = 20;
            this.TextboxChar.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // TextboxNumber
            // 
            this.TextboxNumber.Location = new System.Drawing.Point(922, 638);
            this.TextboxNumber.Name = "TextboxNumber";
            this.TextboxNumber.ReadOnly = true;
            this.TextboxNumber.Size = new System.Drawing.Size(100, 23);
            this.TextboxNumber.TabIndex = 21;
            // 
            // ImagePanel
            // 
            this.ImagePanel.Controls.Add(this.PictureBoxImage);
            this.ImagePanel.Location = new System.Drawing.Point(1, 1);
            this.ImagePanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ImagePanel.Name = "ImagePanel";
            this.ImagePanel.Size = new System.Drawing.Size(1021, 577);
            this.ImagePanel.TabIndex = 22;
            // 
            // PictureBoxImage
            // 
            this.PictureBoxImage.Location = new System.Drawing.Point(0, 0);
            this.PictureBoxImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PictureBoxImage.Name = "PictureBoxImage";
            this.PictureBoxImage.Size = new System.Drawing.Size(999, 559);
            this.PictureBoxImage.TabIndex = 0;
            this.PictureBoxImage.TabStop = false;
            this.PictureBoxImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBoxImage_Click);
            // 
            // ButtonOpenFont
            // 
            this.ButtonOpenFont.Location = new System.Drawing.Point(330, 619);
            this.ButtonOpenFont.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ButtonOpenFont.Name = "ButtonOpenFont";
            this.ButtonOpenFont.Size = new System.Drawing.Size(82, 36);
            this.ButtonOpenFont.TabIndex = 23;
            this.ButtonOpenFont.Text = "OpenFont";
            this.ButtonOpenFont.UseVisualStyleBackColor = true;
            this.ButtonOpenFont.Click += new System.EventHandler(this.button11_Click);
            // 
            // LabelFontName
            // 
            this.LabelFontName.AutoSize = true;
            this.LabelFontName.Location = new System.Drawing.Point(712, 654);
            this.LabelFontName.Name = "LabelFontName";
            this.LabelFontName.Size = new System.Drawing.Size(66, 15);
            this.LabelFontName.TabIndex = 24;
            this.LabelFontName.Text = "Font: None";
            // 
            // CheckboxAddX
            // 
            this.CheckboxAddX.AutoSize = true;
            this.CheckboxAddX.Location = new System.Drawing.Point(869, 674);
            this.CheckboxAddX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CheckboxAddX.Name = "CheckboxAddX";
            this.CheckboxAddX.Size = new System.Drawing.Size(142, 19);
            this.CheckboxAddX.TabIndex = 25;
            this.CheckboxAddX.Text = "Font \"+x\" height hack";
            this.CheckboxAddX.UseVisualStyleBackColor = true;
            this.CheckboxAddX.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // TextboxFontSize
            // 
            this.TextboxFontSize.Location = new System.Drawing.Point(741, 672);
            this.TextboxFontSize.Name = "TextboxFontSize";
            this.TextboxFontSize.PlaceholderText = "29";
            this.TextboxFontSize.Size = new System.Drawing.Size(100, 23);
            this.TextboxFontSize.TabIndex = 26;
            this.TextboxFontSize.Text = "29";
            // 
            // LabelFontSize
            // 
            this.LabelFontSize.AutoSize = true;
            this.LabelFontSize.Location = new System.Drawing.Point(678, 675);
            this.LabelFontSize.Name = "LabelFontSize";
            this.LabelFontSize.Size = new System.Drawing.Size(57, 15);
            this.LabelFontSize.TabIndex = 27;
            this.LabelFontSize.Text = "Font Size:";
            // 
            // TextboxX
            // 
            this.TextboxX.Location = new System.Drawing.Point(567, 672);
            this.TextboxX.Name = "TextboxX";
            this.TextboxX.PlaceholderText = "4.0";
            this.TextboxX.Size = new System.Drawing.Size(100, 23);
            this.TextboxX.TabIndex = 28;
            this.TextboxX.Text = "4.0";
            // 
            // LabelX
            // 
            this.LabelX.AutoSize = true;
            this.LabelX.Location = new System.Drawing.Point(544, 675);
            this.LabelX.Name = "LabelX";
            this.LabelX.Size = new System.Drawing.Size(17, 15);
            this.LabelX.TabIndex = 29;
            this.LabelX.Text = "X:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 703);
            this.Controls.Add(this.LabelX);
            this.Controls.Add(this.TextboxX);
            this.Controls.Add(this.LabelFontSize);
            this.Controls.Add(this.TextboxFontSize);
            this.Controls.Add(this.CheckboxAddX);
            this.Controls.Add(this.LabelFontName);
            this.Controls.Add(this.ButtonOpenFont);
            this.Controls.Add(this.ImagePanel);
            this.Controls.Add(this.TextboxNumber);
            this.Controls.Add(this.TextboxChar);
            this.Controls.Add(this.ButtonReset);
            this.Controls.Add(this.ButtonConvert);
            this.Controls.Add(this.LabelUsedY);
            this.Controls.Add(this.LabelTableY);
            this.Controls.Add(this.LabelNormalY);
            this.Controls.Add(this.LabelConversion2);
            this.Controls.Add(this.TextboxUsedY);
            this.Controls.Add(this.TextboxTableY);
            this.Controls.Add(this.LabelConversion1);
            this.Controls.Add(this.TextboxNormalY);
            this.Controls.Add(this.ButtonSaveTXT);
            this.Controls.Add(this.ButtonSavePNG);
            this.Controls.Add(this.ButtonReplaceAll);
            this.Controls.Add(this.ButtonExtractAll);
            this.Controls.Add(this.InfoTextbox);
            this.Controls.Add(this.ButtonNext);
            this.Controls.Add(this.ButtonPrevious);
            this.Controls.Add(this.ButtonLoadTXT);
            this.Controls.Add(this.ButtonLoadPNG);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ImagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ButtonLoadPNG;
        private System.Windows.Forms.Button ButtonLoadTXT;
        private System.Windows.Forms.Button ButtonPrevious;
        private System.Windows.Forms.Button ButtonNext;
        private System.Windows.Forms.TextBox InfoTextbox;
        private System.Windows.Forms.Button ButtonExtractAll;
        private System.Windows.Forms.Button ButtonReplaceAll;
        private System.Windows.Forms.Button ButtonSavePNG;
        private System.Windows.Forms.Button ButtonSaveTXT;
        private System.Windows.Forms.TextBox TextboxNormalY;
        private System.Windows.Forms.Label LabelConversion1;
        private System.Windows.Forms.TextBox TextboxTableY;
        private System.Windows.Forms.TextBox TextboxUsedY;
        private System.Windows.Forms.Label LabelConversion2;
        private System.Windows.Forms.Label LabelNormalY;
        private System.Windows.Forms.Label LabelTableY;
        private System.Windows.Forms.Label LabelUsedY;
        private System.Windows.Forms.Button ButtonConvert;
        private System.Windows.Forms.Button ButtonReset;
        private System.Windows.Forms.TextBox TextboxChar;
        private System.Windows.Forms.TextBox TextboxNumber;
        private Panel ImagePanel;
        private PictureBox PictureBoxImage;
        private Button ButtonOpenFont;
        private Label LabelFontName;
        private CheckBox CheckboxAddX;
        private TextBox TextboxFontSize;
        private Label LabelFontSize;
        private TextBox TextboxX;
        private Label LabelX;
    }
}
