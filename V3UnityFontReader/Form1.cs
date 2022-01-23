﻿using System;
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
    public class SpecialCharacter
    {
        public TMPCharacter TCharacter = new TMPCharacter();
        public uint Position = 0;
    };
    public partial class Form1 : Form
    {
        Rectangle rectangle;
        FontStructure font;
        GlyphRect rect;
        bool loaded_png = false;
        bool loaded_txt = false;
        List<string> txt_lines = new List<string>();
        int cur_index = 0;
        string png_fn = "";
        string txt_fn = "";
        List<SpecialCharacter> specials = new List<SpecialCharacter>();
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
                // If we need to replace/add images we also need to read the data first
                // or it's going to read the data of the "default" file
                ReplaceGlyphData();
                ReplaceGlyphImage();
                IncreaseIndex();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            cur_index = 0;
        }

        void ReplaceGlyphData()
        {
            int index = font.m_GlyphTable[cur_index].m_Index;
            string ext = Path.Combine(Directory.GetCurrentDirectory(), "extracted");
            if (!Directory.Exists(ext))
            {
                MessageBox.Show(ext, "Extracted files folder not found!");
                return;
            }
            string extfile = Path.Combine(ext, Path.GetFileNameWithoutExtension(txt_fn));
            if (!Directory.Exists(extfile))
            {
                MessageBox.Show(extfile, "This file hasn't been extracted yet!");
                return;
            }
            string data = Path.Combine(extfile, "data");
            if (!Directory.Exists(data))
            {
                MessageBox.Show(data, "The \"data\" folder couldn't be found!");
                return;
            }
            string txtdata = Path.Combine(data, index.ToString()) + ".txt";
            if (!File.Exists(txtdata))
            {
                MessageBox.Show(txtdata, "File not found!");
                return;
            }

            string[] content = File.ReadAllLines(txtdata);
            int lines = content.Length;
            if (lines != 17)
            {
                MessageBox.Show(txtdata, "Wrong number of lines (" + lines + ", expected 17)!");
                return;
            }

            using (StreamReader readtext = new StreamReader(txtdata))
            {
                font.m_GlyphTable[cur_index].Read(content[0], 2);
                font.m_GlyphTable[cur_index].Read(content[1], 4);
                font.m_GlyphTable[cur_index].Read(content[2], 5);
                font.m_GlyphTable[cur_index].Read(content[3], 6);
                font.m_GlyphTable[cur_index].Read(content[4], 7);
                font.m_GlyphTable[cur_index].Read(content[5], 8);
                font.m_GlyphTable[cur_index].Read(content[6], 10);
                font.m_GlyphTable[cur_index].Read(content[7], 11);
                font.m_GlyphTable[cur_index].Read(content[8], 12);
                font.m_GlyphTable[cur_index].Read(content[9], 13);
                font.m_GlyphTable[cur_index].Read(content[10], 14);
                font.m_GlyphTable[cur_index].Read(content[11], 15);

                font.m_CharacterTable[cur_index].Read(content[13], 2);
                font.m_CharacterTable[cur_index].Read(content[14], 3);
                font.m_CharacterTable[cur_index].Read(content[15], 4);
                font.m_CharacterTable[cur_index].Read(content[16], 5);

                //Debug.WriteLine(font.m_CharacterTable[cur_index].m_Unicode);
            }
        }

        void ReplaceGlyphImage()
        {
            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                return;
            }

            int index = font.m_GlyphTable[cur_index].m_Index;
            string ext = Path.Combine(Directory.GetCurrentDirectory(), "extracted");
            if (!Directory.Exists(ext))
            {
                MessageBox.Show(ext, "Extracted files folder not found!");
                return;
            }
            string extfile = Path.Combine(ext, Path.GetFileNameWithoutExtension(txt_fn));
            if (!Directory.Exists(extfile))
            {
                MessageBox.Show(extfile, "This file hasn't been extracted yet!");
                return;
            }
            string images = Path.Combine(extfile, "images");
            if (!Directory.Exists(images))
            {
                MessageBox.Show(images, "The \"images\" folder couldn't be found!");
                return;
            }
            string partial = Path.Combine(images, index.ToString()) + ".png";
            if (!File.Exists(partial))
            {
                MessageBox.Show(partial, "File not found!");
                return;
            }
            using Bitmap partial_bmp = new Bitmap(partial);
            if (partial_bmp == null)
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
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            cur_index = 0;
            for (int j = 0; j < font.m_GlyphTable.Count; j++)
            {
                ExtractGlyphImage();
                ExtractGlyphData();
                IncreaseIndex();
                PaintRectangle();
                UpdateTextboxString();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            cur_index = 0;
        }

        void ExtractGlyphData()
        {
            int index = font.m_GlyphTable[cur_index].m_Index;
            string ext = Path.Combine(Directory.GetCurrentDirectory(), "extracted");
            Directory.CreateDirectory(ext);
            string extfile = Path.Combine(ext, Path.GetFileNameWithoutExtension(txt_fn));
            Directory.CreateDirectory(extfile);
            string data = Path.Combine(extfile, "data");
            Directory.CreateDirectory(data);
            string newtxt = Path.Combine(data, index.ToString()) + ".txt";

            var bak_charactertable = new List<TMPCharacter>(font.m_CharacterTable);

            foreach (SpecialCharacter sc in specials)
            {
                font.m_CharacterTable.Insert((int)sc.Position, sc.TCharacter);
            }

            using (StreamWriter writetext = new StreamWriter(newtxt))
            {
                writetext.WriteLine("m_Index = " + font.m_GlyphTable[cur_index].m_Index.ToString());
                writetext.WriteLine("m_Metrics.m_Width = " + font.m_GlyphTable[cur_index].m_Metrics.m_Width.ToString());
                writetext.WriteLine("m_Metrics.m_Height = " + font.m_GlyphTable[cur_index].m_Metrics.m_Height.ToString());
                writetext.WriteLine("m_Metrics.m_HorizontalBearingX = " + font.m_GlyphTable[cur_index].m_Metrics.m_HorizontalBearingX.ToString());
                writetext.WriteLine("m_Metrics.m_HorizontalBearingY = " + font.m_GlyphTable[cur_index].m_Metrics.m_HorizontalBearingY.ToString());
                writetext.WriteLine("m_Metrics.m_HorizontalAdvance = " + font.m_GlyphTable[cur_index].m_Metrics.m_HorizontalAdvance.ToString());
                writetext.WriteLine("m_GlyphRect.m_X = " + font.m_GlyphTable[cur_index].m_GlyphRect.m_X.ToString());
                writetext.WriteLine("m_GlyphRect.m_Y = " + font.m_GlyphTable[cur_index].m_GlyphRect.m_Y.ToString());
                writetext.WriteLine("m_GlyphRect.m_Width = " + font.m_GlyphTable[cur_index].m_GlyphRect.m_Width.ToString());
                writetext.WriteLine("m_GlyphRect.m_Height = " + font.m_GlyphTable[cur_index].m_GlyphRect.m_Height.ToString());
                writetext.WriteLine("m_Scale = " + font.m_GlyphTable[cur_index].m_Scale.ToString());
                writetext.WriteLine("m_AtlasIndex = " + font.m_GlyphTable[cur_index].m_AtlasIndex.ToString());

                writetext.WriteLine("____________________________________________________");

                TMPCharacter character = GetCharacterFromIndex(font.m_GlyphTable[cur_index].m_Index);

                writetext.WriteLine("m_ElementType = " + character.m_ElementType.ToString());
                writetext.WriteLine("m_Unicode = " + character.m_Unicode.ToString());
                writetext.WriteLine("m_GlyphIndex = " + character.m_GlyphIndex.ToString());
                writetext.WriteLine("m_Scale = " + character.m_Scale.ToString());
            }

            font.m_CharacterTable = bak_charactertable;
        }

        void ExtractGlyphImage()
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
            string images = Path.Combine(extfile, "images");
            Directory.CreateDirectory(images);
            string newpng = Path.Combine(images, index.ToString()) + ".png";
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
                if (index == font.m_GlyphTable[i].m_Index)
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
            uint real_cont = 0;
            foreach (TMPCharacter character in table)
            {
                real_cont++;
                if (ret.Contains(character))
                {
                    Debug.WriteLine("Found duplicate character: \"" + (char)character.m_Unicode + "\"");
                    continue;
                }

                int glyph = GetGlyphByIndex(character.m_GlyphIndex);
                if (glyph == -1)
                {
                    Debug.WriteLine("No equivalent glyph found for character: \"" + (char)(character.m_Unicode) + "\"");
                    continue;
                }

                if (character.m_Unicode == 10 || character.m_Unicode == 13)
                {
                    SpecialCharacter sc = new SpecialCharacter();
                    sc.Position = real_cont - 1;
                    sc.TCharacter = character;
                    if (!specials.Contains(sc))
                    {
                        specials.Add(sc);
                    }

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
            txt_lines = new List<string>();
            font = new FontStructure();
            font.m_GlyphTable = new List<Glyph>();
            font.m_CharacterTable = new List<TMPCharacter>();
            font.m_UsedGlyphRects = new List<GlyphRect>();

            string last = "";
            var lines = File.ReadAllLines(txt_fn);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                txt_lines.Add(line);
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
                                txt_lines.Add(line);
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
                                    txt_lines.Add(line);
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
                                        txt_lines.Add(line);
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
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

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

        void SaveFontAsTXT()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = ".TXT file|*.txt";
            save.Title = "Save as .TXT";
            if (save.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string filename = save.FileName;

            var bak_charactertable = new List<TMPCharacter>(font.m_CharacterTable);

            foreach (SpecialCharacter sc in specials)
            {
                font.m_CharacterTable.Insert((int)sc.Position, sc.TCharacter);
            }

            string last = "";
            for (int j = 0; j < txt_lines.Count; j++)
            {
                if (txt_lines[j].Contains("int size ="))
                {
                    int size = 0;
                    string before_equals = txt_lines[j].Substring(0, txt_lines[j].LastIndexOf("=") + 1 + 1); // Itself *and* space included
                    if (last.Contains("m_GlyphTable"))
                    {
                        size = font.m_GlyphTable.Count;
                        txt_lines[j] = before_equals + size.ToString();
                    }
                    else
                    {
                        if (last.Contains("m_CharacterTable"))
                        {
                            size = font.m_CharacterTable.Count;
                            txt_lines[j] = before_equals + size.ToString();
                        }
                        else
                        {
                            if (last.Contains("m_UsedGlyphRects"))
                            {
                                size = font.m_UsedGlyphRects.Count;
                                txt_lines[j] = before_equals + size.ToString();
                            }
                        }
                    }

                    j++;

                    for (int i = 0; i < size; i++)
                    {
                        if (last.Contains("m_GlyphTable"))
                        {
                            for (int k = 0; k < 16; k++)
                            {
                                txt_lines[j + k] = font.m_GlyphTable[i].Write(txt_lines[j + k], k, i);
                            }
                            j += 16;
                        }
                        else
                        {
                            if (last.Contains("m_CharacterTable"))
                            {
                                for (int k = 0; k < 6; k++)
                                {
                                    txt_lines[j + k] = font.m_CharacterTable[i].Write(txt_lines[j + k], k, i);
                                }
                                j += 6;
                            }
                            else
                            {
                                if (last.Contains("m_UsedGlyphRects"))
                                {
                                    for (int k = 0; k < 6; k++)
                                    {
                                        txt_lines[j + k] = font.m_UsedGlyphRects[i].Write(txt_lines[j + k], k, i);
                                    }
                                    j += 6;
                                }
                            }
                        }
                    }
                }
                last = txt_lines[j];
            }

            font.m_CharacterTable = bak_charactertable;

            File.WriteAllLines(filename, txt_lines);
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

        private void button8_Click(object sender, EventArgs e)
        {
            SaveFontAsTXT();
        }
    }
}
