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
    public partial class Form1 : Form
    {
        FontStructure font;
        GlyphRect rect;
        bool loaded_png = false;
        bool loaded_txt = false;
        int cur_index = 0;
        string png_fn = "";
        public Form1()
        {
            InitializeComponent();
            font = new FontStructure();
            rect = new GlyphRect();
        }

        void PaintRectangle(int x, int y, int lenx, int leny)
        {
            pictureBox1.Image = new Bitmap(png_fn);
            pictureBox1.Refresh();
            // Y is inverted and does not account for the character itself
            Rectangle ee = new Rectangle(x, pictureBox1.Image.Size.Height - y - leny, lenx, leny);
            Graphics gr = Graphics.FromImage(pictureBox1.Image);
            using (Pen pen = new Pen(Color.Red, 1))
            {
                gr.DrawRectangle(pen, ee);
            }
            pictureBox1.Refresh();
        }

        TMPCharacter GetCharacterFromIndex(int index)
        {
            TMPCharacter character = new TMPCharacter();
            foreach(TMPCharacter c in font.m_CharacterTable)
            {
                if(c.m_GlyphIndex == index)
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
            if(character == new TMPCharacter())
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
                (pictureBox1.Image.Size.Height - rect.m_Y - rect.m_Height) +
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

                PaintRectangle(
                    rect.m_X,
                    rect.m_Y,
                    rect.m_Width,
                    rect.m_Height
                );

                UpdateTextboxString();
            }
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

            font = new FontStructure();
            font.m_GlyphTable = new List<Glyph>();
            font.m_CharacterTable = new List<TMPCharacter>();
            font.m_UsedGlyphRects = new List<GlyphRect>();

            string last = "";
            var lines = File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if(line.Length == 0)
                {
                    continue;
                }

                if(line.Contains("int size ="))
                {
                    int size = int.Parse(line.Substring(15));
                    int cont = 0;
                    for(int j = 0; j < size; j++)
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
                        } else
                        {
                            if(last.Contains("m_CharacterTable"))
                            {
                                font.m_CharacterTable.Add(new TMPCharacter());
                                font.m_CharacterTable[j] = new TMPCharacter();
                                for (int k = 0; k < 6; k++)
                                {
                                    line = lines[base_i + k];
                                    font.m_CharacterTable[j].Read(line, k);
                                    i++;
                                }
                            } else
                            {
                                if(last.Contains("m_UsedGlyphRects"))
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

            loaded_txt = true;

            if (loaded_png)
            {
                rect = font.m_GlyphTable[cur_index].m_GlyphRect;

                PaintRectangle(
                    rect.m_X,
                    rect.m_Y,
                    rect.m_Width,
                    rect.m_Height
                );

                UpdateTextboxString();
            }
        }

        void IncreaseIndex()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            if (cur_index + 1 >= font.m_UsedGlyphRects.Count)
            {
                cur_index = font.m_UsedGlyphRects.Count - 1;
                return;
            }

            cur_index++;
            rect = font.m_GlyphTable[cur_index].m_GlyphRect;

            PaintRectangle(
                rect.m_X,
                rect.m_Y,
                rect.m_Width,
                rect.m_Height
            );

            UpdateTextboxString();
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

            PaintRectangle(
                rect.m_X,
                rect.m_Y,
                rect.m_Width,
                rect.m_Height
            );

            UpdateTextboxString();
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
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IncreaseIndex();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
