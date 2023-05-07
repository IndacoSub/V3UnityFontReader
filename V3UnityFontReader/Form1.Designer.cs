using System.Windows.Forms;

namespace V3UnityFontReader
{
	partial class V3UnityFontReader
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
			ButtonLoadPNG = new Button();
			ButtonLoadTXT = new Button();
			ButtonPrevious = new Button();
			ButtonNext = new Button();
			InfoTextbox = new TextBox();
			ButtonExtractAll = new Button();
			ButtonReplaceAll = new Button();
			ButtonSavePNG = new Button();
			ButtonSaveTXT = new Button();
			TextboxNormalY = new TextBox();
			LabelConversion1 = new Label();
			TextboxTableY = new TextBox();
			TextboxUsedY = new TextBox();
			LabelConversion2 = new Label();
			LabelNormalY = new Label();
			LabelTableY = new Label();
			LabelUsedY = new Label();
			ButtonConvert = new Button();
			ButtonReset = new Button();
			TextboxChar = new TextBox();
			TextboxNumber = new TextBox();
			ImagePanel = new Panel();
			PictureBoxImage = new PictureBox();
			ButtonOpenFont = new Button();
			LabelFontName = new Label();
			CheckboxAddX = new CheckBox();
			TextboxFontSize = new TextBox();
			LabelFontSize = new Label();
			TextboxX = new TextBox();
			LabelX = new Label();
			ImagePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)PictureBoxImage).BeginInit();
			SuspendLayout();
			// 
			// ButtonLoadPNG
			// 
			ButtonLoadPNG.Location = new System.Drawing.Point(18, 583);
			ButtonLoadPNG.Name = "ButtonLoadPNG";
			ButtonLoadPNG.Size = new System.Drawing.Size(69, 34);
			ButtonLoadPNG.TabIndex = 1;
			ButtonLoadPNG.Text = "Load PNG";
			ButtonLoadPNG.UseVisualStyleBackColor = true;
			ButtonLoadPNG.Click += button1_Click;
			// 
			// ButtonLoadTXT
			// 
			ButtonLoadTXT.Location = new System.Drawing.Point(18, 619);
			ButtonLoadTXT.Name = "ButtonLoadTXT";
			ButtonLoadTXT.Size = new System.Drawing.Size(69, 36);
			ButtonLoadTXT.TabIndex = 2;
			ButtonLoadTXT.Text = "Load TXT";
			ButtonLoadTXT.UseVisualStyleBackColor = true;
			ButtonLoadTXT.Click += button2_Click;
			// 
			// ButtonPrevious
			// 
			ButtonPrevious.Location = new System.Drawing.Point(93, 583);
			ButtonPrevious.Name = "ButtonPrevious";
			ButtonPrevious.Size = new System.Drawing.Size(68, 34);
			ButtonPrevious.TabIndex = 3;
			ButtonPrevious.Text = "<---";
			ButtonPrevious.UseVisualStyleBackColor = true;
			ButtonPrevious.Click += button3_Click;
			// 
			// ButtonNext
			// 
			ButtonNext.Location = new System.Drawing.Point(167, 583);
			ButtonNext.Name = "ButtonNext";
			ButtonNext.Size = new System.Drawing.Size(71, 34);
			ButtonNext.TabIndex = 4;
			ButtonNext.Text = "--->";
			ButtonNext.UseVisualStyleBackColor = true;
			ButtonNext.Click += button4_Click;
			// 
			// InfoTextbox
			// 
			InfoTextbox.Location = new System.Drawing.Point(330, 588);
			InfoTextbox.Name = "InfoTextbox";
			InfoTextbox.ReadOnly = true;
			InfoTextbox.Size = new System.Drawing.Size(681, 23);
			InfoTextbox.TabIndex = 5;
			InfoTextbox.TextChanged += textBox1_TextChanged;
			// 
			// ButtonExtractAll
			// 
			ButtonExtractAll.Location = new System.Drawing.Point(93, 619);
			ButtonExtractAll.Name = "ButtonExtractAll";
			ButtonExtractAll.Size = new System.Drawing.Size(68, 36);
			ButtonExtractAll.TabIndex = 6;
			ButtonExtractAll.Text = "ExtractAll";
			ButtonExtractAll.UseVisualStyleBackColor = true;
			ButtonExtractAll.Click += button5_Click;
			// 
			// ButtonReplaceAll
			// 
			ButtonReplaceAll.Location = new System.Drawing.Point(167, 619);
			ButtonReplaceAll.Name = "ButtonReplaceAll";
			ButtonReplaceAll.Size = new System.Drawing.Size(71, 36);
			ButtonReplaceAll.TabIndex = 7;
			ButtonReplaceAll.Text = "ReplaceAll";
			ButtonReplaceAll.UseVisualStyleBackColor = true;
			ButtonReplaceAll.Click += button6_Click;
			// 
			// ButtonSavePNG
			// 
			ButtonSavePNG.Location = new System.Drawing.Point(244, 583);
			ButtonSavePNG.Name = "ButtonSavePNG";
			ButtonSavePNG.Size = new System.Drawing.Size(71, 34);
			ButtonSavePNG.TabIndex = 8;
			ButtonSavePNG.Text = "Save .PNG";
			ButtonSavePNG.UseVisualStyleBackColor = true;
			ButtonSavePNG.Click += button7_Click;
			// 
			// ButtonSaveTXT
			// 
			ButtonSaveTXT.Location = new System.Drawing.Point(244, 619);
			ButtonSaveTXT.Name = "ButtonSaveTXT";
			ButtonSaveTXT.Size = new System.Drawing.Size(71, 36);
			ButtonSaveTXT.TabIndex = 9;
			ButtonSaveTXT.Text = "Save .TXT";
			ButtonSaveTXT.UseVisualStyleBackColor = true;
			ButtonSaveTXT.Click += button8_Click;
			// 
			// TextboxNormalY
			// 
			TextboxNormalY.Location = new System.Drawing.Point(428, 627);
			TextboxNormalY.Name = "TextboxNormalY";
			TextboxNormalY.Size = new System.Drawing.Size(95, 23);
			TextboxNormalY.TabIndex = 10;
			// 
			// LabelConversion1
			// 
			LabelConversion1.AutoSize = true;
			LabelConversion1.Location = new System.Drawing.Point(528, 629);
			LabelConversion1.Name = "LabelConversion1";
			LabelConversion1.Size = new System.Drawing.Size(31, 15);
			LabelConversion1.TabIndex = 11;
			LabelConversion1.Text = "<=>";
			// 
			// TextboxTableY
			// 
			TextboxTableY.Location = new System.Drawing.Point(567, 627);
			TextboxTableY.Name = "TextboxTableY";
			TextboxTableY.Size = new System.Drawing.Size(112, 23);
			TextboxTableY.TabIndex = 12;
			// 
			// TextboxUsedY
			// 
			TextboxUsedY.Location = new System.Drawing.Point(723, 627);
			TextboxUsedY.Name = "TextboxUsedY";
			TextboxUsedY.Size = new System.Drawing.Size(118, 23);
			TextboxUsedY.TabIndex = 13;
			// 
			// LabelConversion2
			// 
			LabelConversion2.AutoSize = true;
			LabelConversion2.Location = new System.Drawing.Point(683, 629);
			LabelConversion2.Name = "LabelConversion2";
			LabelConversion2.Size = new System.Drawing.Size(31, 15);
			LabelConversion2.TabIndex = 14;
			LabelConversion2.Text = "<=>";
			// 
			// LabelNormalY
			// 
			LabelNormalY.AutoSize = true;
			LabelNormalY.Location = new System.Drawing.Point(444, 611);
			LabelNormalY.Name = "LabelNormalY";
			LabelNormalY.Size = new System.Drawing.Size(57, 15);
			LabelNormalY.TabIndex = 15;
			LabelNormalY.Text = "Normal Y";
			// 
			// LabelTableY
			// 
			LabelTableY.AutoSize = true;
			LabelTableY.Location = new System.Drawing.Point(576, 611);
			LabelTableY.Name = "LabelTableY";
			LabelTableY.Size = new System.Drawing.Size(89, 15);
			LabelTableY.TabIndex = 16;
			LabelTableY.Text = "Table Rect Y (C)";
			LabelTableY.Click += LabelTableY_Click;
			// 
			// LabelUsedY
			// 
			LabelUsedY.AutoSize = true;
			LabelUsedY.Location = new System.Drawing.Point(723, 611);
			LabelUsedY.Name = "LabelUsedY";
			LabelUsedY.Size = new System.Drawing.Size(96, 15);
			LabelUsedY.TabIndex = 17;
			LabelUsedY.Text = "Used Glyph Y (C)";
			LabelUsedY.Click += LabelUsedY_Click;
			// 
			// ButtonConvert
			// 
			ButtonConvert.Location = new System.Drawing.Point(845, 611);
			ButtonConvert.Name = "ButtonConvert";
			ButtonConvert.Size = new System.Drawing.Size(72, 23);
			ButtonConvert.TabIndex = 18;
			ButtonConvert.Text = "Convert";
			ButtonConvert.UseVisualStyleBackColor = true;
			ButtonConvert.Click += ButtonConvert_Click;
			// 
			// ButtonReset
			// 
			ButtonReset.Location = new System.Drawing.Point(845, 638);
			ButtonReset.Name = "ButtonReset";
			ButtonReset.Size = new System.Drawing.Size(72, 23);
			ButtonReset.TabIndex = 19;
			ButtonReset.Text = "Reset";
			ButtonReset.UseVisualStyleBackColor = true;
			ButtonReset.Click += ButtonReset_Click;
			// 
			// TextboxChar
			// 
			TextboxChar.Location = new System.Drawing.Point(922, 613);
			TextboxChar.Name = "TextboxChar";
			TextboxChar.Size = new System.Drawing.Size(100, 23);
			TextboxChar.TabIndex = 20;
			TextboxChar.TextChanged += textBox5_TextChanged;
			// 
			// TextboxNumber
			// 
			TextboxNumber.Location = new System.Drawing.Point(922, 638);
			TextboxNumber.Name = "TextboxNumber";
			TextboxNumber.ReadOnly = true;
			TextboxNumber.Size = new System.Drawing.Size(100, 23);
			TextboxNumber.TabIndex = 21;
			// 
			// ImagePanel
			// 
			ImagePanel.Controls.Add(PictureBoxImage);
			ImagePanel.Location = new System.Drawing.Point(1, 1);
			ImagePanel.Margin = new Padding(3, 2, 3, 2);
			ImagePanel.Name = "ImagePanel";
			ImagePanel.Size = new System.Drawing.Size(1021, 577);
			ImagePanel.TabIndex = 22;
			// 
			// PictureBoxImage
			// 
			PictureBoxImage.Location = new System.Drawing.Point(0, 0);
			PictureBoxImage.Margin = new Padding(3, 2, 3, 2);
			PictureBoxImage.Name = "PictureBoxImage";
			PictureBoxImage.Size = new System.Drawing.Size(999, 559);
			PictureBoxImage.TabIndex = 0;
			PictureBoxImage.TabStop = false;
			PictureBoxImage.MouseClick += PictureBoxImage_Click;
			// 
			// ButtonOpenFont
			// 
			ButtonOpenFont.Location = new System.Drawing.Point(330, 619);
			ButtonOpenFont.Margin = new Padding(3, 2, 3, 2);
			ButtonOpenFont.Name = "ButtonOpenFont";
			ButtonOpenFont.Size = new System.Drawing.Size(82, 36);
			ButtonOpenFont.TabIndex = 23;
			ButtonOpenFont.Text = "OpenFont";
			ButtonOpenFont.UseVisualStyleBackColor = true;
			ButtonOpenFont.Click += button11_Click;
			// 
			// LabelFontName
			// 
			LabelFontName.AutoSize = true;
			LabelFontName.Location = new System.Drawing.Point(712, 654);
			LabelFontName.Name = "LabelFontName";
			LabelFontName.Size = new System.Drawing.Size(66, 15);
			LabelFontName.TabIndex = 24;
			LabelFontName.Text = "Font: None";
			// 
			// CheckboxAddX
			// 
			CheckboxAddX.AutoSize = true;
			CheckboxAddX.Location = new System.Drawing.Point(847, 673);
			CheckboxAddX.Margin = new Padding(3, 2, 3, 2);
			CheckboxAddX.Name = "CheckboxAddX";
			CheckboxAddX.Size = new System.Drawing.Size(150, 19);
			CheckboxAddX.TabIndex = 25;
			CheckboxAddX.Text = "Font \"+x\" height (hack)";
			CheckboxAddX.UseVisualStyleBackColor = true;
			CheckboxAddX.CheckedChanged += checkBox1_CheckedChanged;
			// 
			// TextboxFontSize
			// 
			TextboxFontSize.Location = new System.Drawing.Point(741, 672);
			TextboxFontSize.Name = "TextboxFontSize";
			TextboxFontSize.PlaceholderText = "29";
			TextboxFontSize.Size = new System.Drawing.Size(100, 23);
			TextboxFontSize.TabIndex = 26;
			TextboxFontSize.Text = "29";
			// 
			// LabelFontSize
			// 
			LabelFontSize.AutoSize = true;
			LabelFontSize.Location = new System.Drawing.Point(678, 675);
			LabelFontSize.Name = "LabelFontSize";
			LabelFontSize.Size = new System.Drawing.Size(57, 15);
			LabelFontSize.TabIndex = 27;
			LabelFontSize.Text = "Font Size:";
			// 
			// TextboxX
			// 
			TextboxX.Location = new System.Drawing.Point(567, 672);
			TextboxX.Name = "TextboxX";
			TextboxX.PlaceholderText = "4.0";
			TextboxX.Size = new System.Drawing.Size(100, 23);
			TextboxX.TabIndex = 28;
			TextboxX.Text = "4.0";
			// 
			// LabelX
			// 
			LabelX.AutoSize = true;
			LabelX.Location = new System.Drawing.Point(544, 675);
			LabelX.Name = "LabelX";
			LabelX.Size = new System.Drawing.Size(17, 15);
			LabelX.TabIndex = 29;
			LabelX.Text = "X:";
			// 
			// V3UnityFontReader
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(1023, 703);
			Controls.Add(LabelX);
			Controls.Add(TextboxX);
			Controls.Add(LabelFontSize);
			Controls.Add(TextboxFontSize);
			Controls.Add(CheckboxAddX);
			Controls.Add(LabelFontName);
			Controls.Add(ButtonOpenFont);
			Controls.Add(ImagePanel);
			Controls.Add(TextboxNumber);
			Controls.Add(TextboxChar);
			Controls.Add(ButtonReset);
			Controls.Add(ButtonConvert);
			Controls.Add(LabelUsedY);
			Controls.Add(LabelTableY);
			Controls.Add(LabelNormalY);
			Controls.Add(LabelConversion2);
			Controls.Add(TextboxUsedY);
			Controls.Add(TextboxTableY);
			Controls.Add(LabelConversion1);
			Controls.Add(TextboxNormalY);
			Controls.Add(ButtonSaveTXT);
			Controls.Add(ButtonSavePNG);
			Controls.Add(ButtonReplaceAll);
			Controls.Add(ButtonExtractAll);
			Controls.Add(InfoTextbox);
			Controls.Add(ButtonNext);
			Controls.Add(ButtonPrevious);
			Controls.Add(ButtonLoadTXT);
			Controls.Add(ButtonLoadPNG);
			MinimumSize = new System.Drawing.Size(1039, 742);
			Name = "V3UnityFontReader";
			Text = "V3UnityFontReader";
			ImagePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)PictureBoxImage).EndInit();
			ResumeLayout(false);
			PerformLayout();
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
