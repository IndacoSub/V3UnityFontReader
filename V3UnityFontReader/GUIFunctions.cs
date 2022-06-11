using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace V3UnityFontReader
{
    public partial class Form1
    {
        // Ask input (https://stackoverflow.com/questions/97097/what-is-the-c-sharp-version-of-vb-nets-inputdialog)
        public static DialogResult ShowInputDialog(ref string input)
        {
            Size size = new Size(200, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Name";

            TextBox textBox = new TextBox();
            textBox.Size = new Size(size.Width - 10, 23);
            textBox.Location = new Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        private void PictureBoxImage_Click(object sender, MouseEventArgs e)
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            if (e.X > PictureBoxImage.Image.Size.Width)
            {
                MessageBox.Show("Invalid mouse-X!");
                return;
            }

            if (e.Y > PictureBoxImage.Image.Size.Height)
            {
                MessageBox.Show("Invalid mouse-Y!");
                return;
            }

            if (fm.LoadedFont)
            {
                LoadCustomFromFont(e.X, e.Y);
            }
            else
            {
                LoadCustomFromFiles(e.X, e.Y);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenPNG();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenTXT();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DecreaseIndex();
            PaintRectangle();
            UpdateTextboxString();
            TextboxNormalY.Text = "";
            TextboxTableY.Text = "";
            TextboxUsedY.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IncreaseIndex();
            PaintRectangle();
            //PaintRectangleUsed();
            UpdateTextboxString();
            TextboxNormalY.Text = "";
            TextboxTableY.Text = "";
            TextboxUsedY.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ExtractAllGlyphs();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ReplaceAllGlyphs();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SaveImageAsPNG();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SaveFontAsTXT();
        }

        private void LabelTableY_Click(object sender, EventArgs e)
        {
        }

        private void LabelUsedY_Click(object sender, EventArgs e)
        {
        }

        private void ButtonConvert_Click(object sender, EventArgs e)
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            bool is_1_empty = TextboxNormalY.Text.Length == 0;
            bool is_2_empty = TextboxTableY.Text.Length == 0;
            bool is_3_empty = TextboxUsedY.Text.Length == 0;
            if (is_1_empty && is_2_empty && is_3_empty)
            {
                return;
            }

            if (is_2_empty && is_3_empty)
            {
                // Normal
                if (TextboxNormalY.Text.Length > 0 && int.TryParse(TextboxNormalY.Text, out int a) && a > 0)
                {
                    int toint = int.Parse(TextboxNormalY.Text);
                    TextboxTableY.Text = InterpretY(toint).ToString();
                    TextboxUsedY.Text = InterpretY2(toint).ToString();
                    //TextboxUsedY.Text = (PictureBoxImage.Size.Height - toint).ToString();
                }
                else
                {
                    TextboxTableY.Text = "";
                    TextboxUsedY.Text = "";
                }
            }

            if (is_1_empty && is_3_empty)
            {
                // Table
                if (TextboxTableY.Text.Length > 0 && int.TryParse(TextboxTableY.Text, out int a) && a > 0)
                {
                    int toint = int.Parse(TextboxTableY.Text);
                    int res1 = NormalizeY(toint);
                    TextboxNormalY.Text = res1.ToString();
                    TextboxUsedY.Text = InterpretY2(res1).ToString();
                }
                else
                {
                    TextboxNormalY.Text = "";
                    TextboxUsedY.Text = "";
                }
            }

            if (is_1_empty && is_2_empty)
            {
                // Used
                if (TextboxUsedY.Text.Length > 0 && int.TryParse(TextboxUsedY.Text, out int a) && a > 0)
                {
                    int toint = int.Parse(TextboxUsedY.Text);
                    int res1 = NormalizeY2(toint);
                    TextboxNormalY.Text = res1.ToString();
                    TextboxTableY.Text = InterpretY(res1).ToString();
                }
                else
                {
                    TextboxNormalY.Text = "";
                    TextboxTableY.Text = "";
                }
            }
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            TextboxNormalY.Text = "";
            TextboxTableY.Text = "";
            TextboxUsedY.Text = "";
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (TextboxChar.Text.Length > 1)
            {
                return;
            }

            if (TextboxChar.Text.Length <= 0)
            {
                return;
            }

            char c = TextboxChar.Text[0];
            TextboxNumber.Text = "";
            TextboxNumber.Text += ((uint)c).ToString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "TrueType Font files (*.ttf)|*.ttf|All files (*.*)|*.*";
            var res = ofd.ShowDialog();
            if (res != DialogResult.OK)
            {
                return;
            }

            string fn = ofd.FileName;
            if (!File.Exists(fn))
            {
                // Invalid file
                return;
            }

            int size = int.Parse(TextboxFontSize.Text);

            fm = new FontManager(fn);
            fm.LoadCurrentFont();
            fm.SetCurrentFontSize(size); // TODO: editable at runtime

            LabelFontName.Text = "Font: " + Path.GetFileNameWithoutExtension(fn);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}