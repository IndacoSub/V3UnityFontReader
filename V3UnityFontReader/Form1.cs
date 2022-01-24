using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace V3UnityFontReader
{

    public class MyComparer : IComparer<string>
    {

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern int StrCmpLogicalW(String x, String y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }

    }
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
        int total_red_data = 0;
        int total_red_glyphs = 0;
        int total_red_image = 0;
        int original_txt_glyph_size = 0;
        int original_txt_ctable_size = 0;
        int original_txt_usedglyph_size = 0;
        int original_txt_freeglyph_size = 0;
        string cur_filename = "";
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

        void DeleteUsedFreeGlyphs()
        {
            List<GlyphRect> free_after = new List<GlyphRect>();

            foreach (GlyphRect used in font.m_UsedGlyphRects)
            {
                foreach (GlyphRect free in font.m_FreeGlyphRects)
                {
                    Rectangle usedrect = new Rectangle(used.m_X, used.m_Y, used.m_Width, used.m_Height);
                    Rectangle freerect = new Rectangle(free.m_X, free.m_Y, free.m_Width, free.m_Height);
                    if (usedrect.IntersectsWith(freerect) || freerect.IntersectsWith(usedrect))
                    {
                        continue;
                    }

                    if (!free_after.Contains(free))
                    {
                        free_after.Add(free);
                    }
                }
            }

            font.m_FreeGlyphRects = free_after;
        }

        bool IsSpecial(TMPCharacter ch)
        {
            foreach (SpecialCharacter character in specials)
            {
                if (character.TCharacter.m_GlyphIndex == ch.m_GlyphIndex)
                {
                    return true;
                }
            }

            return false;
        }

        bool CanHaveUsedGlyph(TMPCharacter ch)
        {
            bool is_special = IsSpecial(ch);
            if (is_special)
            {
                Debug.WriteLine("Is special?");
                return false;
            }

            int g = GetGlyphByIndex(ch.m_GlyphIndex);
            if (g == -1)
            {
                Debug.WriteLine("GlyphIndex is -1?: " + ch.m_GlyphIndex);
                return false;
            }

            Glyph gg = font.m_GlyphTable[g];
            bool has_rect_coords = gg.m_GlyphRect.m_Width > 0 && gg.m_GlyphRect.m_Height > 0;
            return has_rect_coords;
        }

        int UsedGlyphRectByCharacter(TMPCharacter ch)
        {
            int ret = -1;

            foreach (GlyphRect glyphRect in font.m_UsedGlyphRects)
            {
                ret++;
                TMPCharacter temp = WhatIsInsideGlyphRect(glyphRect);
                if (temp == null)
                {
                    continue;
                }
                if (temp == new TMPCharacter())
                {
                    continue;
                }
                if (temp.m_GlyphIndex == ch.m_GlyphIndex)
                {
                    return ret;
                }
            }

            return -1;
        }

        TMPCharacter WhatIsInsideGlyphRect(GlyphRect myrect)
        {
            TMPCharacter ret = new TMPCharacter();

            foreach (TMPCharacter ch in font.m_CharacterTable)
            {
                //Debug.WriteLine("ch.Index: " + ch.m_GlyphIndex);
                int glyph_index = GetGlyphByIndex(ch.m_GlyphIndex);
                if (glyph_index == -1)
                {
                    continue;
                }
                Glyph glyph = font.m_GlyphTable[glyph_index];
                if (glyph == null)
                {
                    continue;
                }
                if (glyph.m_GlyphRect.m_X == 0 && glyph.m_GlyphRect.m_Y == 0)
                {
                    continue;
                }
                bool is_inside_x = glyph.m_GlyphRect.m_X > myrect.m_X && glyph.m_GlyphRect.m_X + glyph.m_GlyphRect.m_Width < myrect.m_X + myrect.m_Width;
                bool is_inside_y = glyph.m_GlyphRect.m_Y > myrect.m_Y && glyph.m_GlyphRect.m_Y + glyph.m_GlyphRect.m_Height < myrect.m_Y + myrect.m_Height;
                if (is_inside_x && is_inside_y)
                {
                    ret = ch;
                    break;
                }
            }

            return ret;
        }

        void PaintRectangle()
        {
            pictureBox1.Image = new Bitmap(png_fn);
            pictureBox1.Refresh();
            // Y is inverted and does not account for the character itself

            rectangle = new Rectangle(rect.m_X, InterpretY(rect.m_Y), rect.m_Width, rect.m_Height);

            //rectangle = new Rectangle(font.m_UsedGlyphRects[cur_index].m_X, InterpretY2(font.m_UsedGlyphRects[cur_index].m_Y), font.m_UsedGlyphRects[cur_index].m_Width, font.m_UsedGlyphRects[cur_index].m_Height);
            using (Graphics gr = Graphics.FromImage(pictureBox1.Image))
            {
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    gr.DrawRectangle(pen, rectangle);
                }
            }
            pictureBox1.Refresh();

        }

        void VerifyUsedGlyphTable()
        {
            Debug.WriteLine("Old rects: " + font.m_UsedGlyphRects.Count);

            List<GlyphRect> rects = new List<GlyphRect>();

            foreach (GlyphRect gr in font.m_UsedGlyphRects)
            {
                if (gr.m_Width == 0 || gr.m_Height == 0)
                {
                    continue;
                }
                TMPCharacter ch = new TMPCharacter();
                foreach (Glyph g in font.m_GlyphTable)
                {
                    foreach (TMPCharacter c in font.m_CharacterTable)
                    {
                        /*
                        if (IsSpecial(c))
                        {
                            continue;
                        }
                        */

                        if (c.m_GlyphIndex == g.m_Index)
                        {
                            bool is_x = g.m_GlyphRect.m_X > gr.m_X && g.m_GlyphRect.m_Width + g.m_GlyphRect.m_X < gr.m_X + gr.m_Width;
                            bool is_y = g.m_GlyphRect.m_Y > gr.m_Y && g.m_GlyphRect.m_Height + g.m_GlyphRect.m_Y < gr.m_Y + gr.m_Height;
                            if (is_x && is_y)
                            {
                                if (!rects.Contains(gr))
                                {
                                    rects.Add(gr);
                                }
                            }
                        }
                    }
                }

                rects.Sort((x, y) => WhatIsInsideGlyphRect(x).m_GlyphIndex.CompareTo(WhatIsInsideGlyphRect(y).m_GlyphIndex));
            }

            Debug.WriteLine("New rects: " + rects.Count);

            font.m_UsedGlyphRects = rects;
        }

        void ReplaceAllGlyphs()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }
            pictureBox1.Image = new Bitmap(png_fn);
            cur_index = 0;

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
            string images = Path.Combine(extfile, "images");
            if (!Directory.Exists(images))
            {
                MessageBox.Show(images, "The \"images\" folder couldn't be found!");
                return;
            }

            string[] data_files =
            Directory.GetFiles(data, "*.txt", SearchOption.AllDirectories);

            string[] image_files =
            Directory.GetFiles(images, "*.png", SearchOption.AllDirectories);

            Array.Sort(data_files, new MyComparer());
            Array.Sort(image_files, new MyComparer());

            Debug.WriteLine("Data files count: " + data_files.Length);
            Debug.WriteLine("Image files count: " + image_files.Length);

            //specials.Clear();

            int cont_txt = 0;
            int cont_png = 0;
            total_red_data = 0;
            total_red_image = 0;
            total_red_glyphs = 0;

            font.m_GlyphTable = new List<Glyph>();
            font.m_CharacterTable = new List<TMPCharacter>();
            font.m_UsedGlyphRects = new List<GlyphRect>();

            // If we need to replace/add images we also need to read the data first
            // or else it's going to read the data of the "default" file

            foreach (string f in data_files)
            {
                cur_index = total_red_data;

                ReplaceGlyphData(f);
                cont_txt++;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            // Sort by m_Index / m_GlyphIndex
            font.m_GlyphTable.Sort((x, y) => x.m_Index.CompareTo(y.m_Index));
            font.m_CharacterTable.Sort((x, y) => x.m_GlyphIndex.CompareTo(y.m_GlyphIndex));
            VerifyCharacterTable();
            VerifyUsedGlyphTable();

            foreach (string f in image_files)
            {
                //Debug.WriteLine("ImageF: " + (cont_png + 1) + ", " + f);
                cur_index = cont_png;
                ReplaceGlyphImage(f);
                cont_png++;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Debug.WriteLine("TXT red: " + cont_txt);
            Debug.WriteLine("PNG red: " + cont_png);

            DeleteUsedFreeGlyphs();

            cur_index = 0;
        }

        void ReplaceGlyphData(string path)
        {
            string txtdata = path;
            if (!File.Exists(txtdata))
            {
                MessageBox.Show(txtdata, "File not found!");
                return;
            }

            string[] content = File.ReadAllLines(txtdata);
            if (content == null)
            {
                MessageBox.Show(txtdata, "Null file!");
                return;
            }

            int lines = content.Length;
            if (lines != 22)
            {
                MessageBox.Show(txtdata, "Wrong number of lines (" + lines + ", expected 22)!");
                return;
            }

            font.m_GlyphTable.Add(new Glyph());
            font.m_CharacterTable.Add(new TMPCharacter());

            // Read the .txt "line by line"
            using (StreamReader readtext = new StreamReader(txtdata))
            {
                font.m_GlyphTable[total_red_data].Read(content[0], 2);
                font.m_GlyphTable[total_red_data].Read(content[1], 4);
                font.m_GlyphTable[total_red_data].Read(content[2], 5);
                font.m_GlyphTable[total_red_data].Read(content[3], 6);
                font.m_GlyphTable[total_red_data].Read(content[4], 7);
                font.m_GlyphTable[total_red_data].Read(content[5], 8);
                font.m_GlyphTable[total_red_data].Read(content[6], 10);
                font.m_GlyphTable[total_red_data].Read(content[7], 11);
                font.m_GlyphTable[total_red_data].Read(content[8], 12);
                font.m_GlyphTable[total_red_data].Read(content[9], 13);
                font.m_GlyphTable[total_red_data].Read(content[10], 14);
                font.m_GlyphTable[total_red_data].Read(content[11], 15);

                font.m_CharacterTable[total_red_data].Read(content[13], 2);
                font.m_CharacterTable[total_red_data].Read(content[14], 3);
                font.m_CharacterTable[total_red_data].Read(content[15], 4);
                font.m_CharacterTable[total_red_data].Read(content[16], 5);

                //VerifyCharacterTable();

                //Debug.WriteLine(font.m_CharacterTable[cur_index].m_Unicode);

                // Could be a special case without glyph like a space, \\r or \\n
                if (!content[18].Contains("SPECIAL"))
                {
                    font.m_UsedGlyphRects.Add(new GlyphRect());
                    font.m_UsedGlyphRects[total_red_glyphs].Read(content[18], 2);
                    font.m_UsedGlyphRects[total_red_glyphs].Read(content[19], 3);
                    font.m_UsedGlyphRects[total_red_glyphs].Read(content[20], 4);
                    font.m_UsedGlyphRects[total_red_glyphs].Read(content[21], 5);
                    total_red_glyphs++;
                }
            }

            total_red_data++;
        }

        void ReplaceGlyphImage(string path)
        {

            string partial = path;
            if (!File.Exists(partial))
            {
                MessageBox.Show(partial, "File not found!");
                return;
            }

            using Bitmap partial_bmp = new Bitmap(partial);
            if (partial_bmp == null)
            {
                Debug.WriteLine("partial_bmp is null!");
                return;
            }

            string glyph_index = path.Substring(path.LastIndexOf("\\") + 1);
            //Debug.WriteLine("1: " + glyph_index);
            glyph_index = glyph_index.Substring(0, glyph_index.IndexOf('.'));
            //Debug.WriteLine("2: " + glyph_index);
            int glyph_index_int = int.Parse(glyph_index);
            int pos = GetGlyphByIndex((uint)glyph_index_int);
            if (pos == -1)
            {
                Debug.WriteLine("Couldn't find equivalent glyph with m_Index: " + glyph_index_int);
                return;
            }

            rect = font.m_GlyphTable[pos].m_GlyphRect;
            rectangle = new Rectangle(rect.m_X, InterpretY(rect.m_Y), rect.m_Width, rect.m_Height);

            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                Debug.WriteLine("The rectangle for " + partial + " (glyph: " + font.m_GlyphTable[total_red_image].m_Index + ") is empty!");
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
            total_red_image++;
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

            Debug.WriteLine("Special.size(): " + specials.Count);
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

                writetext.WriteLine("____________________________________________________");

                if (CanHaveUsedGlyph(character))
                {
                    int used_index = UsedGlyphRectByCharacter(character);
                    if (used_index >= font.m_UsedGlyphRects.Count || used_index < 0)
                    {
                        Debug.WriteLine("Used GlyphRect not found for character: " + character.m_Unicode + ", with index: " + character.m_GlyphIndex + ", len: " + font.m_UsedGlyphRects.Count + ", index: " + used_index);
                    }

                    writetext.WriteLine("m_UsedGlyphRects.m_X = " + font.m_UsedGlyphRects[used_index].m_X.ToString());
                    writetext.WriteLine("m_UsedGlyphRects.m_Y = " + font.m_UsedGlyphRects[used_index].m_Y.ToString());
                    writetext.WriteLine("m_UsedGlyphRects.m_Width = " + font.m_UsedGlyphRects[used_index].m_Width.ToString());
                    writetext.WriteLine("m_UsedGlyphRects.m_Height = " + font.m_UsedGlyphRects[used_index].m_Height.ToString());
                }
                else
                {
                    writetext.WriteLine("SPECIAL");
                    writetext.WriteLine("SPECIAL");
                    writetext.WriteLine("SPECIAL");
                    writetext.WriteLine("SPECIAL");
                }
            }

            font.m_CharacterTable = bak_charactertable;
        }

        void ExtractGlyphImage()
        {

            string ext = Path.Combine(Directory.GetCurrentDirectory(), "extracted");
            Directory.CreateDirectory(ext);
            string extfile = Path.Combine(ext, Path.GetFileNameWithoutExtension(txt_fn));
            Directory.CreateDirectory(extfile);
            string images = Path.Combine(extfile, "images");
            Directory.CreateDirectory(images);

            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                return;
            }

            int index = font.m_GlyphTable[cur_index].m_Index;
            using Bitmap full_image = new Bitmap(png_fn);
            using Image portion = full_image.Clone(rectangle, full_image.PixelFormat);
            string newpng = Path.Combine(images, index.ToString()) + ".png";
            portion.Save(newpng, System.Drawing.Imaging.ImageFormat.Png);
            portion.Dispose();
            full_image.Dispose();
        }

        int InterpretY(int y)
        {
            // Used to read "partial glyphs" a.k.a. letters/numbers/symbols
            return pictureBox1.Image.Size.Height - y - rect.m_Height;
        }

        int InterpretY2(int y)
        {
            // Used to read *used* "full glyphs" a.k.a. letters/numbers/symbols PLUS their surrounding area (used in colored CLT)
            return pictureBox1.Image.Size.Height - y - font.m_UsedGlyphRects[cur_index].m_Height;
        }

        int InterpretY3(int y)
        {
            // Used to read *free* glyphs (dumbest feature ever)
            return pictureBox1.Image.Size.Height - y - font.m_FreeGlyphRects[cur_index].m_Height;
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
            //textBox1.Text = "Inside is: " + (char)WhatIsInsideGlyphRect(font.m_UsedGlyphRects[cur_index]).m_Unicode;
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
            //Debug.WriteLine("m_GlyphTable.Count: " + font.m_GlyphTable.Count);
            for (int i = 0; i < font.m_GlyphTable.Count; i++)
            {
                if (index == (uint)font.m_GlyphTable[i].m_Index)
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
                    else
                    {
                        Debug.WriteLine("Duplicate special: " + character.m_Unicode);
                    }

                    Debug.WriteLine("Character is \\r or \\n");
                    continue;
                }

                //Debug.WriteLine(cont + ": \"" + (char)character.m_Unicode + "\", unicode: " + (uint)character.m_Unicode);

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
                                else
                                {
                                    if (last.Contains("m_FreeGlyphRects"))
                                    {
                                        font.m_FreeGlyphRects.Add(new GlyphRect());
                                        font.m_FreeGlyphRects[j] = new GlyphRect();
                                        for (int k = 0; k < 6; k++)
                                        {
                                            line = lines[base_i + k];
                                            txt_lines.Add(line);
                                            font.m_FreeGlyphRects[j].Read(line, k);
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                        cont++;
                    }
                }

                last = line;
            }

            original_txt_glyph_size = font.m_GlyphTable.Count;
            original_txt_ctable_size = font.m_CharacterTable.Count;
            original_txt_usedglyph_size = font.m_UsedGlyphRects.Count;
            original_txt_freeglyph_size = font.m_FreeGlyphRects.Count;

            VerifyCharacterTable();

            Debug.WriteLine("GlyphTable.Count: " + font.m_GlyphTable.Count);
            Debug.WriteLine("CharacterTable.Count: " + font.m_CharacterTable.Count);
            Debug.WriteLine("UsedGlyphRects.Count: " + font.m_UsedGlyphRects.Count);

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

            Debug.WriteLine("Special.size(): " + specials.Count);
            foreach (SpecialCharacter sc in specials)
            {
                font.m_CharacterTable.Insert((int)sc.Position, sc.TCharacter);
            }

            font.m_CharacterTable.Sort((x, y) => x.m_GlyphIndex.CompareTo(y.m_GlyphIndex));

            string last = "";
            int lines_count = txt_lines.Count;
            for (int j = 0; j < lines_count; j++)
            {
                if (txt_lines[j].Contains("int size ="))
                {
                    int size = 0;
                    string before_last = last.Substring(0, last.LastIndexOf("(") + 1);
                    string after_last = " items)";
                    string before_equals = txt_lines[j].Substring(0, txt_lines[j].LastIndexOf("=") + 1 + 1); // Itself *and* space included

                    if (last.Contains("m_GlyphTable"))
                    {
                        size = font.m_GlyphTable.Count;
                        txt_lines[j] = before_equals + size.ToString();
                        txt_lines[j - 1] = before_last + size.ToString() + after_last;
                        txt_lines.RemoveRange(j + 1, 16 * original_txt_glyph_size);
                    }
                    else
                    {
                        if (last.Contains("m_CharacterTable"))
                        {
                            size = font.m_CharacterTable.Count;
                            txt_lines[j] = before_equals + size.ToString();
                            txt_lines[j - 1] = before_last + size.ToString() + after_last;
                            txt_lines.RemoveRange(j + 1, 6 * original_txt_ctable_size);
                        }
                        else
                        {
                            if (last.Contains("m_UsedGlyphRects"))
                            {
                                size = font.m_UsedGlyphRects.Count;
                                txt_lines[j] = before_equals + size.ToString();
                                txt_lines[j - 1] = before_last + size.ToString() + after_last;
                                Debug.WriteLine("Contains! Size: " + font.m_UsedGlyphRects.Count);
                                txt_lines.RemoveRange(j + 1, 6 * original_txt_usedglyph_size);
                            }
                            else
                            {
                                if (last.Contains("m_FreeGlyphRects"))
                                {
                                    size = font.m_FreeGlyphRects.Count;
                                    txt_lines[j] = before_equals + size.ToString();
                                    txt_lines[j - 1] = before_last + size.ToString() + after_last;
                                    txt_lines.RemoveRange(j + 1, 6 * original_txt_freeglyph_size);
                                }
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
                                txt_lines.Insert(j + k, "");
                                txt_lines[j + k] = font.m_GlyphTable[i].Write(k, i);
                            }
                            j += 16;
                        }
                        else
                        {
                            if (last.Contains("m_CharacterTable"))
                            {
                                for (int k = 0; k < 6; k++)
                                {
                                    txt_lines.Insert(j + k, "");
                                    txt_lines[j + k] = font.m_CharacterTable[i].Write(k, i);
                                }
                                j += 6;
                            }
                            else
                            {
                                if (last.Contains("m_UsedGlyphRects"))
                                {
                                    Debug.WriteLine("m_UsedGlyphRects size: " + font.m_UsedGlyphRects.Count);
                                    for (int k = 0; k < 6; k++)
                                    {
                                        txt_lines.Insert(j + k, "");
                                        txt_lines[j + k] = font.m_UsedGlyphRects[i].Write(k, i, false);
                                    }
                                    j += 6;
                                }
                                else
                                {
                                    if (last.Contains("m_FreeGlyphRects"))
                                    {
                                        for (int k = 0; k < 6; k++)
                                        {
                                            txt_lines.Insert(j + k, "");
                                            txt_lines[j + k] = font.m_FreeGlyphRects[i].Write(k, i, true);
                                        }
                                        j += 6;
                                    }
                                }
                            }
                        }
                    }
                }

                if (txt_lines[j].Contains("characterSequence"))
                {
                    string before_equals = txt_lines[j].Substring(0, txt_lines[j].IndexOf("=") + 1 + 1); // Itself *and* space included
                    before_equals += "\"";
                    foreach (TMPCharacter c in bak_charactertable)
                    {
                        before_equals += (char)c.m_Unicode;
                    }
                    before_equals += "\\r\\n";
                    before_equals += "\"";
                    txt_lines[j] = before_equals;
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
