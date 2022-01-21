using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace V3UnityFontReader
{
    public partial class Form1 : Form
    {
        Rectangle rectangle;
        FontStructure font;
        GlyphRect rect;
        bool loaded_png = false;
        bool loaded_txt = false;
        int cur_index = 0;
        string png_fn = "";
        string txt_fn = "";
        public Form1()
        {
            InitializeComponent();
            font = new FontStructure();
            rect = new GlyphRect();
            rectangle = new Rectangle();
        }

        void PaintRectangle()
        {
            pictureBox1.Image = new Bitmap(png_fn);
            pictureBox1.Refresh();
            // Y is inverted and does not account for the character itself
            rectangle = new Rectangle(rect.m_X, InterpretY(rect.m_Y), rect.m_Width, rect.m_Height);
            using (Graphics gr = Graphics.FromImage(pictureBox1.Image))
            {
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    gr.DrawRectangle(pen, rectangle);
                }
            }
            pictureBox1.Refresh();
        }

        void ReplaceAllGlyphs()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }
            pictureBox1.Image = new Bitmap(png_fn);
            cur_index = 0;

            /*
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.Clear(Color.Transparent);
            }
            pictureBox1.Refresh();
            */

            for (int j = 0; j < font.m_GlyphTable.Count; j++)
            {
                rectangle = new Rectangle(rect.m_X, InterpretY(rect.m_Y), rect.m_Width, rect.m_Height);
                ReplaceGlyph();
                IncreaseIndex();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        void ReplaceGlyph()
        {
            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                return;
            }

            int index = font.m_GlyphTable[cur_index].m_Index;
            string ext = Path.Combine(Directory.GetCurrentDirectory(), "extracted");
            if(!Directory.Exists(ext))
            {
                MessageBox.Show(ext, "Extracted files folder not found!");
                return;
            }
            string extfile = Path.Combine(ext, Path.GetFileNameWithoutExtension(txt_fn));
            if(!Directory.Exists(extfile))
            {
                MessageBox.Show(extfile, "This file hasn't been extracted yet!");
                return;
            }
            string partial = Path.Combine(extfile, index.ToString()) + ".png";
            if(!File.Exists(partial))
            {
                MessageBox.Show(partial, "File not found!");
                return;
            }
            using Bitmap partial_bmp = new Bitmap(partial);
            if(partial_bmp == null)
            {
                return;
            }

            // For some reason, the opacity might be broken?
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                var bak = g.CompositingMode;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.FillRectangle(Brushes.Transparent, rect.m_X, InterpretY(rect.m_Y), rect.m_Width, rect.m_Height);
                g.CompositingMode = bak;
                g.DrawImageUnscaled(partial_bmp, rect.m_X, InterpretY(rect.m_Y));
            }
            pictureBox1.Refresh();
        }

        void ExtractAllGlyphs()
        {
            if(!loaded_png || !loaded_txt)
            {
                return;
            }

            cur_index = 0;
            for (int j = 0; j < font.m_GlyphTable.Count; j++)
            {
                ExtractGlyph();
                IncreaseIndex();
                PaintRectangle();
                UpdateTextboxString();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        void ExtractGlyph()
        {
            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                return;
            }

            int index = font.m_GlyphTable[cur_index].m_Index;
            using Bitmap full_image = new Bitmap(png_fn);
            using Image portion = full_image.Clone(rectangle, full_image.PixelFormat);
            string ext = Path.Combine(Directory.GetCurrentDirectory(), "extracted");
            Directory.CreateDirectory(ext);
            string extfile = Path.Combine(ext, Path.GetFileNameWithoutExtension(txt_fn));
            Directory.CreateDirectory(extfile);
            string newpng = Path.Combine(extfile, index.ToString()) + ".png";
            portion.Save(newpng, System.Drawing.Imaging.ImageFormat.Png);
            portion.Dispose();
            full_image.Dispose();
        }

        int InterpretY(int y)
        {
            return pictureBox1.Image.Size.Height - y - rect.m_Height;
        }

        int NormalizeY(int y)
        {
            return y + pictureBox1.Image.Size.Height + rect.m_Height;
        }

        TMPCharacter GetCharacterFromIndex(int index)
        {
            TMPCharacter character = new TMPCharacter();
            foreach (TMPCharacter c in font.m_CharacterTable)
            {
                if (c.m_GlyphIndex == index)
                {
                    character = c;
                }
            }
            return character;
        }

        void UpdateTextboxString()
        {
            int index = font.m_GlyphTable[cur_index].m_Index;
            TMPCharacter character = GetCharacterFromIndex(index);
            if (character == new TMPCharacter())
            {
                return;
            }
            char c = (char)character.m_Unicode;
            int x = rect.m_X;
            int y = rect.m_Y;
            textBox1.Text =
                "m_Index: " +
                index +
                ", character: \"" +
                c +
                "\", X: " +
                x +
                ", Y: " +
                y +
                ", InterpretedY: " +
                // Y is inverted and does not account for the character itself
                InterpretY(y) +
                ", cW: " +
                rect.m_Width +
                ", cH: " +
                rect.m_Height
                ;
            textBox1.Update();
            textBox1.Refresh();
        }

        void OpenPNG()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "PNG File|*.png";
            openFileDialog1.Title = "Open .PNG file";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string filename = openFileDialog1.FileName;
            if (filename == null || string.IsNullOrWhiteSpace(filename))
            {
                return;
            }
            png_fn = filename;
            pictureBox1.Image = new Bitmap(png_fn);
            pictureBox1.Refresh();

            loaded_png = true;

            if (loaded_txt)
            {
                rect = font.m_GlyphTable[cur_index].m_GlyphRect;

                PaintRectangle();

                UpdateTextboxString();
            }
        }

        int GetGlyphByIndex(uint index)
        {
            for (int i = 0; i < font.m_GlyphTable.Count; i++)
            {
                if(index == font.m_GlyphTable[i].m_Index)
                {
                    return i;
                }
            }

            return -1;
        }

        void VerifyCharacterTable()
        {
            var table = font.m_CharacterTable;
            List<TMPCharacter> ret = new List<TMPCharacter>();
            int cont = 0;
            foreach(TMPCharacter character in table)
            {
                if(ret.Contains(character))
                {
                    Debug.WriteLine("Found duplicate character: \"" + (char)character.m_Unicode + "\"");
                    continue;
                }

                int glyph = GetGlyphByIndex(character.m_GlyphIndex);
                if(glyph == -1)
                {
                    Debug.WriteLine("No equivalent glyph found for character: \"" + (char)(character.m_Unicode) + "\"");
                    continue;
                }

                if(character.m_Unicode == 10 || character.m_Unicode == 13)
                {
                    Debug.WriteLine("Character is \\r or \\n");
                    continue;
                }

                Debug.WriteLine(cont + ": \"" + (char)character.m_Unicode + "\", unicode: " + (uint)character.m_Unicode);

                ret.Add(character);
                cont++;
            }

            font.m_CharacterTable = ret;
        }

        void OpenTXT()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "TXT File|*.txt";
            openFileDialog1.Title = "Open .TXT file";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string filename = openFileDialog1.FileName;
            if (filename == null || string.IsNullOrWhiteSpace(filename))
            {
                return;
            }
            txt_fn = filename;
            cur_index = 0;
            font = new FontStructure();
            font.m_GlyphTable = new List<Glyph>();
            font.m_CharacterTable = new List<TMPCharacter>();
            font.m_UsedGlyphRects = new List<GlyphRect>();

            string last = "";
            var lines = File.ReadAllLines(txt_fn);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Length == 0)
                {
                    continue;
                }

                if (line.Contains("int size ="))
                {
                    int size = int.Parse(line.Substring(15));
                    int cont = 0;
                    for (int j = 0; j < size; j++)
                    {
                        int base_i = i + 1;

                        if (last.Contains("m_GlyphTable"))
                        {
                            font.m_GlyphTable.Add(new Glyph());
                            font.m_GlyphTable[j] = new Glyph();
                            for (int k = 0; k < 16; k++)
                            {
                                line = lines[base_i + k];
                                font.m_GlyphTable[j].Read(line, k);
                                i++;
                            }
                        }
                        else
                        {
                            if (last.Contains("m_CharacterTable"))
                            {
                                font.m_CharacterTable.Add(new TMPCharacter());
                                font.m_CharacterTable[j] = new TMPCharacter();
                                for (int k = 0; k < 6; k++)
                                {
                                    line = lines[base_i + k];
                                    font.m_CharacterTable[j].Read(line, k);
                                    i++;
                                }
                            }
                            else
                            {
                                if (last.Contains("m_UsedGlyphRects"))
                                {
                                    font.m_UsedGlyphRects.Add(new GlyphRect());
                                    font.m_UsedGlyphRects[j] = new GlyphRect();
                                    for (int k = 0; k < 6; k++)
                                    {
                                        line = lines[base_i + k];
                                        font.m_UsedGlyphRects[j].Read(line, k);
                                        i++;
                                    }
                                }
                            }
                        }
                        cont++;
                    }
                }

                last = line;
            }

            VerifyCharacterTable();

            Debug.WriteLine("GlyphTable.Count: " + font.m_GlyphTable.Count);
            Debug.WriteLine("CharacterTable.Count: " + font.m_CharacterTable.Count);

            loaded_txt = true;

            if (loaded_png)
            {
                rect = font.m_GlyphTable[cur_index].m_GlyphRect;

                PaintRectangle();

                UpdateTextboxString();
            }
        }

        void IncreaseIndex()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            if (cur_index + 1 >= font.m_CharacterTable.Count)
            {
                cur_index = font.m_CharacterTable.Count - 1;
                return;
            }

            cur_index++;
            Debug.WriteLine("cur_index: " + cur_index + ", gTable: " + font.m_GlyphTable.Count + ", cTable: " + font.m_CharacterTable.Count);
            rect = font.m_GlyphTable[cur_index].m_GlyphRect;
        }

        void DecreaseIndex()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            if (cur_index <= 0)
            {
                cur_index = 0;
                return;
            }

            cur_index--;
            rect = font.m_GlyphTable[cur_index].m_GlyphRect;
        }

        void SaveImageAsPNG()
        {
            Image i = pictureBox1.Image;
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = ".PNG file|*.png";
            save.Title = "Save as .PNG";
            if (save.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string filename = save.FileName;
            i.Save(filename);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

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
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IncreaseIndex();
            PaintRectangle();
            UpdateTextboxString();
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
    }
}
