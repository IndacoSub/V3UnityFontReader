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
                    Rectangle usedrect = new Rectangle(used.m_X, pictureBox1.Image.Size.Height - used.m_Y - used.m_Height, used.m_Width, used.m_Height);
                    Rectangle freerect = new Rectangle(free.m_X, pictureBox1.Image.Size.Height - free.m_Y - free.m_Height, free.m_Width, free.m_Height);
                    if (usedrect.IntersectsWith(freerect) || freerect.IntersectsWith(usedrect))
                    {
                        Debug.WriteLine("Intersects!");
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
                //Debug.WriteLine("Is special?");
                return false;
            }

            int g = GetGlyphByIndex(ch.m_GlyphIndex);
            if (g == -1)
            {
                //Debug.WriteLine("GlyphIndex is -1?: " + ch.m_GlyphIndex);
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
                int glyph_index = GetGlyphByIndex(ch.m_GlyphIndex);
                if (glyph_index == -1)
                {
                    Debug.WriteLine("Couldn't find glyph!");
                    continue;
                }
                Glyph glyph = font.m_GlyphTable[glyph_index];
                if (glyph == null)
                {
                    Debug.WriteLine("Null glyph!");
                    continue;
                }
                if (glyph.m_GlyphRect.m_X == 0 && glyph.m_GlyphRect.m_Y == 0)
                {
                    continue;
                }
                bool is_inside_x = (glyph.m_GlyphRect.m_X > myrect.m_X) && (glyph.m_GlyphRect.m_X + glyph.m_GlyphRect.m_Width < myrect.m_X + myrect.m_Width);
                bool is_inside_y = (glyph.m_GlyphRect.m_Y > myrect.m_Y) && (glyph.m_GlyphRect.m_Y + glyph.m_GlyphRect.m_Height < myrect.m_Y + myrect.m_Height);
                if (is_inside_x && is_inside_y)
                {
                    ret = ch;
                    break;
                } else
                {
                    const bool debug = false;
                    const int DESIRED_GRECT_X = 0;
                    const int DESIRED_URECT_X = 0;
                    const int DESIRED_URECT_Y = 0;
                    if(debug && glyph.m_GlyphRect.m_X == DESIRED_GRECT_X && myrect.m_X == DESIRED_URECT_X && myrect.m_Y == DESIRED_URECT_Y)
                    if(!is_inside_x)
                    {
                        Debug.WriteLine("Wrong X math!");
                        Debug.WriteLine("glyph.m_GlyphRect.m_X (" + glyph.m_GlyphRect.m_X + ") > myrect.m_X (" +
                            myrect.m_X + ")    &&    glyph.m_GlyphRect.m_X + glyph.m_GlyphRect.m_Width (" +
                            glyph.m_GlyphRect.m_Width + ") < myrect.m_X + myrect.m_Width (" + myrect.m_Width + ") = (" +
                            (glyph.m_GlyphRect.m_X + glyph.m_GlyphRect.m_Width) + " VS " + (myrect.m_X + myrect.m_Width) + ")"
                            );
                    } else
                    {
                        if(!is_inside_y)
                        {
                                Debug.WriteLine("Wrong Y math!");
                                Debug.WriteLine("glyph.m_GlyphRect.m_Y (" + glyph.m_GlyphRect.m_Y + ") > myrect.m_Y (" +
                                    myrect.m_Y + ")    &&    glyph.m_GlyphRect.m_Y (" + glyph.m_GlyphRect.m_Y + ") + glyph.m_GlyphRect.m_Height (" +
                                    glyph.m_GlyphRect.m_Height + ") < myrect.m_Y (" + myrect.m_Y + ") + myrect.m_Height (" + myrect.m_Height + ") = (" +
                                    (glyph.m_GlyphRect.m_Y + glyph.m_GlyphRect.m_Height) + " VS " + (myrect.m_Y + myrect.m_Height) + ")"
                                    );
                                //Debug.WriteLine("Suggestion?: myrect.m_Y could be: " + (glyph.m_GlyphRect.m_Y + (glyph.m_GlyphRect.m_Height / 2)));
                        }
                    }
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
            }

            rects.Sort((x, y) => WhatIsInsideGlyphRect(x).m_GlyphIndex.CompareTo(WhatIsInsideGlyphRect(y).m_GlyphIndex));

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
            string uglyph = Path.Combine(extfile, "used_glyphs");
            if (!Directory.Exists(images))
            {
                MessageBox.Show(images, "The \"used_glyphs\" folder couldn't be found!");
                return;
            }

            string[] data_files =
            Directory.GetFiles(data, "*.txt", SearchOption.AllDirectories);

            string[] image_files =
            Directory.GetFiles(images, "*.png", SearchOption.AllDirectories);

            string[] uglyph_files =
            Directory.GetFiles(uglyph, "*.png", SearchOption.AllDirectories);

            Array.Sort(data_files, new MyComparer());
            Array.Sort(image_files, new MyComparer());
            Array.Sort(uglyph_files, new MyComparer());

            Debug.WriteLine("Data files count: " + data_files.Length);
            Debug.WriteLine("Image files count: " + image_files.Length);
            Debug.WriteLine("Used glyphs files count: " + uglyph_files.Length);

            //specials.Clear();

            int cont_txt = 0;
            int cont_png = 0;
            int cont_uglyph_png = 0;

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
            //VerifyUsedGlyphTable();

            cur_index = 0;

            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.Clear(Color.Transparent);
                g.ResetClip();
            }

            foreach (string f in uglyph_files)
            {
                cur_index = cont_uglyph_png;
                ReplaceUsedGlyphImage(f);
                cont_uglyph_png++;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            cur_index = 0;

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

        void ReplaceUsedGlyphImage(string path)
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
            Debug.WriteLine("Glyph index: " + glyph_index);
            int glyph_index_int = int.Parse(glyph_index);
            int pos = GetGlyphByIndex((uint)glyph_index_int);
            if (pos == -1)
            {
                Debug.WriteLine("Couldn't find equivalent glyph with m_Index: " + glyph_index_int);
                return;
            }
            Glyph gg = font.m_GlyphTable[pos];
            TMPCharacter ch = GetCharacterFromIndex(gg.m_Index);
            int u_pos = UsedGlyphRectByCharacter(ch);
            if (u_pos == -1)
            {
                Debug.WriteLine("Couldn't find corresponding used glyph for character: " + ch.m_Unicode + " (" + (char)ch.m_Unicode + "), index: " + ch.m_GlyphIndex + " (a.k.a. probably wrong coordinates?)");
                return;
            }
            GlyphRect used = font.m_UsedGlyphRects[u_pos];

            rect = used;
            // Using Y1 here is fine as we just declared that rect = used
            rectangle = new Rectangle(rect.m_X, InterpretY(rect.m_Y), rect.m_Width, rect.m_Height);

            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                return;
            }

            // For some reason, the opacity might be broken?
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                // Using Y1 here is fine as we just declared that rect = used
                g.DrawImageUnscaled(partial_bmp, rect.m_X, InterpretY(rect.m_Y));
            }
            pictureBox1.Refresh();
            total_red_image++;
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
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
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

            rectangle = new Rectangle(rect.m_X, InterpretY(rect.m_Y), rect.m_Width, rect.m_Height);

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

            //Debug.WriteLine("Now doing glyphs");

            for (int j = 0; j < font.m_UsedGlyphRects.Count; j++)
            {
                cur_index = j;
                rectangle = new Rectangle(font.m_UsedGlyphRects[j].m_X, InterpretY2(font.m_UsedGlyphRects[j].m_Y), font.m_UsedGlyphRects[j].m_Width, font.m_UsedGlyphRects[j].m_Height);
                rect = font.m_UsedGlyphRects[j];
                ExtractUsedGlyph(j);
                PaintRectangleUsed();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            cur_index = 0;
        }

        void ExtractUsedGlyph(int cont)
        {
            string ext = Path.Combine(Directory.GetCurrentDirectory(), "extracted");
            Directory.CreateDirectory(ext);
            string extfile = Path.Combine(ext, Path.GetFileNameWithoutExtension(txt_fn));
            Directory.CreateDirectory(extfile);
            string uglyphs = Path.Combine(extfile, "used_glyphs");
            Directory.CreateDirectory(uglyphs);

            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                Debug.WriteLine("Invalid shape!");
                return;
            }

            GlyphRect gr = font.m_UsedGlyphRects[cont];
            TMPCharacter ch = WhatIsInsideGlyphRect(gr);
            if (ch == null)
            {
                Debug.WriteLine("Null character!");
                return;
            }
            if (ch == new TMPCharacter())
            {
                Debug.WriteLine("Invalid character!");
                return;
            }
            int gl = GetGlyphByIndex(ch.m_GlyphIndex);
            if (gl == -1)
            {
                Debug.WriteLine("Couldn't find corresponding glyph!");
                return;
            }
            string newpng = Path.Combine(uglyphs, ch.m_GlyphIndex.ToString()) + ".png";
            Glyph g = font.m_GlyphTable[gl];
            Rectangle glyph_rect = new Rectangle(g.m_GlyphRect.m_X, g.m_GlyphRect.m_Y, g.m_GlyphRect.m_Width, g.m_GlyphRect.m_Height);
            Rectangle actual_rect = new Rectangle();
            actual_rect.X = glyph_rect.X - gr.m_X;
            actual_rect.Y = glyph_rect.Y - gr.m_Y;
            actual_rect.Width = glyph_rect.Width;
            // For some reason it doesn't work correctly without this -1???
            actual_rect.Height = glyph_rect.Height - 1;
            using (Bitmap full_image = new Bitmap(png_fn))
            {

                /*
                if(ch.m_Unicode == 'j')
                {
                    Debug.WriteLine("GlyphRect X: " + glyph_rect.X + ", Y: " + glyph_rect.Y + ", W: " + rectangle.Width + ", H: " + rectangle.Height);
                    Debug.WriteLine("Rectangle X: " + rectangle.X + ", Y: " + rectangle.Y + ", W: " + rectangle.Width + ", H: " + rectangle.Height);
                    Debug.WriteLine("ActualRectX: " + actual_rect.X + ", Y: " + actual_rect.Y + ", W: " + actual_rect.Width + ", H: " + actual_rect.Height);
                }
                */

                using (Bitmap portion = full_image.Clone(rectangle, full_image.PixelFormat))
                {
                    using (Graphics graphics = Graphics.FromImage(portion))
                    {
                        graphics.SetClip(actual_rect);
                        graphics.Clear(Color.Transparent);
                    }
                    portion.Save(newpng, System.Drawing.Imaging.ImageFormat.Png);
                    portion.Dispose();
                    full_image.Dispose();
                }
            }
        }

        void PaintRectangleUsed()
        {
            pictureBox1.Image = new Bitmap(png_fn);
            pictureBox1.Refresh();

            //Debug.WriteLine("Width: " + font.m_UsedGlyphRects[cur_index].m_Width + ", Height: " + font.m_UsedGlyphRects[cur_index].m_Height);

            // Y is inverted and does not account for the character itself
            rectangle = new Rectangle(font.m_UsedGlyphRects[cur_index].m_X,
                InterpretY2(font.m_UsedGlyphRects[cur_index].m_Y),
                font.m_UsedGlyphRects[cur_index].m_Width,
                font.m_UsedGlyphRects[cur_index].m_Height);

            using (Graphics gr = Graphics.FromImage(pictureBox1.Image))
            {
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    gr.DrawRectangle(pen, rectangle);
                }
            }
            pictureBox1.Refresh();
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

            //Debug.WriteLine("Special.size(): " + specials.Count);
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
            int ret = pictureBox1.Image.Size.Height - y - rect.m_Height;
            return ret < 0 ? 0 : ret;
        }

        int InterpretY2(int y)
        {
            // Used to read *used* "full glyphs" a.k.a. letters/numbers/symbols PLUS their surrounding area (used in colored CLT)
            int ret = pictureBox1.Image.Size.Height - y - font.m_UsedGlyphRects[cur_index].m_Height;
            return ret < 0 ? 0 : ret;
        }

        int InterpretY3(int y)
        {
            // Used to read *free* glyphs (dumbest feature ever)
            int ret = pictureBox1.Image.Size.Height - y - font.m_FreeGlyphRects[cur_index].m_Height;
            return ret < 0 ? 0 : ret;
        }

        int NormalizeY(int y)
        {
            return Math.Abs(y - pictureBox1.Image.Size.Height + rect.m_Height);
        }

        int NormalizeY2(int y)
        {
            return Math.Abs(y - pictureBox1.Image.Size.Height + font.m_UsedGlyphRects[cur_index].m_Height);
        }

        int NormalizeY3(int y)
        {
            return Math.Abs(y - pictureBox1.Image.Size.Height + font.m_FreeGlyphRects[cur_index].m_Height);
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
                "\" (" +
                character.m_Unicode +
                ")" +
                ", X: " +
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
            //textBox1.Text = "Index: " + cur_index + ", inside is: " + (char)WhatIsInsideGlyphRect(font.m_UsedGlyphRects[cur_index]).m_Unicode + " (" + WhatIsInsideGlyphRect(font.m_UsedGlyphRects[cur_index]).m_Unicode + "), X: " + font.m_UsedGlyphRects[cur_index].m_X + ", Y: " + InterpretY2(font.m_UsedGlyphRects[cur_index].m_Y);
            textBox1.Update();
            textBox1.Refresh();

            label4.Text = "Table Rect Y (" + c + ")";
            label5.Text = "Used Glyph Y (" + c + ")";
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
            //Debug.WriteLine("Index: " + index);
            //Debug.WriteLine("m_GlyphTable.Count: " + font.m_GlyphTable.Count);
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
            //Debug.WriteLine("cur_index: " + cur_index + ", gTable: " + font.m_GlyphTable.Count + ", cTable: " + font.m_CharacterTable.Count);
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
                if(!font.m_CharacterTable.Contains(sc.TCharacter))
                {
                    font.m_CharacterTable.Insert((int)sc.Position, sc.TCharacter);
                }
            }

            font.m_CharacterTable.Sort((x, y) => x.m_Unicode.CompareTo(y.m_Unicode));
            font.m_CharacterTable = font.m_CharacterTable.Distinct().ToList();

            font.m_GlyphTable = font.m_GlyphTable.Distinct().ToList();

            font.m_UsedGlyphRects = font.m_UsedGlyphRects.Distinct().ToList();

            DeleteUsedFreeGlyphs();

            string last = "";
            int lines_count = txt_lines.Count;
            for (int j = 0; j < lines_count; j++)
            {
                //Debug.WriteLine("j: " + j + ", lines_count: " + lines_count);
                if (txt_lines[j].Contains("int size ="))
                {
                    int size = 0;
                    string before_last = last.Substring(0, last.LastIndexOf("(") + 1);
                    string after_last = " items)";
                    string before_equals = txt_lines[j].Substring(0, txt_lines[j].LastIndexOf("=") + 1 + 1); // Itself *and* space included

                    int red_size = int.Parse(txt_lines[j].Substring(before_equals.Length));
                    if(red_size <= 0)
                    {
                        last = txt_lines[j];
                        continue;
                    }

                    if (last.Contains("m_GlyphTable"))
                    {
                        size = font.m_GlyphTable.Count;
                        txt_lines[j] = before_equals + size.ToString();
                        txt_lines[j - 1] = before_last + size.ToString() + after_last;
                        txt_lines.RemoveRange(j + 1, 16 * original_txt_glyph_size);
                        lines_count -= 16 * original_txt_glyph_size;
                        j++;
                    }
                    else
                    {
                        if (last.Contains("m_CharacterTable"))
                        {
                            size = font.m_CharacterTable.Count;
                            txt_lines[j] = before_equals + size.ToString();
                            txt_lines[j - 1] = before_last + size.ToString() + after_last;
                            txt_lines.RemoveRange(j + 1, 6 * original_txt_ctable_size);
                            lines_count -= 6 * original_txt_ctable_size;
                            j++;
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
                                lines_count -= 6 * original_txt_usedglyph_size;
                                j++;
                            }
                            else
                            {
                                if (last.Contains("m_FreeGlyphRects"))
                                {
                                    size = font.m_FreeGlyphRects.Count;
                                    txt_lines[j] = before_equals + size.ToString();
                                    txt_lines[j - 1] = before_last + size.ToString() + after_last;
                                    txt_lines.RemoveRange(j + 1, 6 * original_txt_freeglyph_size);
                                    lines_count -= 6 * original_txt_freeglyph_size;
                                    j++;
                                }
                            }
                        }
                    }

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
                            lines_count += 16;
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
                                lines_count += 6;
                            }
                            else
                            {
                                if (last.Contains("m_UsedGlyphRects"))
                                {
                                    //Debug.WriteLine("m_UsedGlyphRects size: " + font.m_UsedGlyphRects.Count);
                                    for (int k = 0; k < 6; k++)
                                    {
                                        txt_lines.Insert(j + k, "");
                                        txt_lines[j + k] = font.m_UsedGlyphRects[i].Write(k, i, false);
                                    }
                                    j += 6;
                                    lines_count += 6;
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
                                        lines_count += 6;
                                    }
                                }
                            }
                        }
                    }
                }

                //Debug.WriteLine(txt_lines[j]);

                if (txt_lines[j].Contains("tlasWidth = ") && txt_lines[j].Contains("int"))
                {
                    Debug.WriteLine("A");
                    string before_equals = txt_lines[j].Substring(0, txt_lines[j].IndexOf("=") + 1 + 1); // Itself *and* space included
                    txt_lines[j] = before_equals + pictureBox1.Image.Size.Width.ToString();
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

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            if(!loaded_png || !loaded_txt)
            {
                return;
            }

            if(e.X > pictureBox1.Image.Size.Width)
            {
                return;
            }

            if(e.Y > pictureBox1.Image.Size.Height)
            {
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.TXT files|*.txt";
            ofd.Multiselect = false;
            ofd.Title = "Open .TXT file";
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string txtdata = ofd.FileName;
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

            TMPCharacter character = new TMPCharacter();
            Glyph glyph = new Glyph();
            GlyphRect urect = new GlyphRect();

            // Read the .txt "line by line"
            using (StreamReader readtext = new StreamReader(txtdata))
            {
                glyph.Read(content[0], 2);
                glyph.Read(content[1], 4);
                glyph.Read(content[2], 5);
                glyph.Read(content[3], 6);
                glyph.Read(content[4], 7);
                glyph.Read(content[5], 8);
                glyph.Read(content[6], 10);
                glyph.Read(content[7], 11);
                glyph.Read(content[8], 12);
                glyph.Read(content[9], 13);
                glyph.Read(content[10], 14);
                glyph.Read(content[11], 15);

                character.Read(content[13], 2);
                character.Read(content[14], 3);
                character.Read(content[15], 4);
                character.Read(content[16], 5);

                //VerifyCharacterTable();

                //Debug.WriteLine(font.m_CharacterTable[cur_index].m_Unicode);

                // Could be a special case without glyph like a space, \\r or \\n
                if (!content[18].Contains("SPECIAL"))
                {
                    urect.Read(content[18], 2);
                    urect.Read(content[19], 3);
                    urect.Read(content[20], 4);
                    urect.Read(content[21], 5);
                }
            }

            if(urect == null || urect == new GlyphRect())
            {
                Debug.WriteLine("Null rect!");
                return;
            }

            string image = Directory.GetCurrentDirectory();
            image = Path.Combine(image, "extracted", Path.GetFileNameWithoutExtension(txt_fn), "images");
            image = Path.Combine(image, character.m_GlyphIndex + ".png");
            if(!File.Exists(image))
            {
                Debug.WriteLine("PNG image not found: " + image);
                return;
            }

            string uglyph_image = Directory.GetCurrentDirectory();
            uglyph_image = Path.Combine(uglyph_image, "extracted", Path.GetFileNameWithoutExtension(txt_fn), "used_glyphs");
            uglyph_image = Path.Combine(uglyph_image, character.m_GlyphIndex + ".png");
            if(!File.Exists(uglyph_image))
            {
                Debug.WriteLine("Used PNG image not found: " + uglyph_image);
                return;
            }

            using (Bitmap used_gfx = new Bitmap(uglyph_image))
            {
                urect.m_Width = used_gfx.Width;
                urect.m_Height = used_gfx.Height;
                urect.m_X = e.X;
                urect.m_Y = e.Y;

                using (Bitmap glyph_gfx = new Bitmap(image))
                {
                    // The +1 is needed, not sure why that is
                    glyph.m_GlyphRect.m_Width = glyph_gfx.Width + 1;
                    glyph.m_GlyphRect.m_Height = glyph_gfx.Height;
                    glyph.m_GlyphRect.m_X = urect.m_X;
                    glyph.m_GlyphRect.m_Y = urect.m_Y;
                    bool is_x_odd = ((urect.m_Width - glyph.m_GlyphRect.m_Width) / 2) % 2 != 0;
                    bool is_y_odd = ((urect.m_Height - glyph.m_GlyphRect.m_Height) / 2) % 2 != 0;
                    Debug.WriteLine("Is X odd: " + is_x_odd);
                    Debug.WriteLine("Is Y odd: " + is_y_odd);
                    glyph.m_GlyphRect.m_X = (urect.m_X + (urect.m_Width - glyph.m_GlyphRect.m_Width) / 2) + (is_x_odd ? 0 : 0);
                    glyph.m_GlyphRect.m_Y = (urect.m_Y + (urect.m_Height - glyph.m_GlyphRect.m_Height) / 2) + (is_y_odd ? 0 : 0);
                    //glyph.m_GlyphRect.m_Y = pictureBox1.Image.Size.Height - urect.m_Height - glyph.m_GlyphRect.m_Height;

                    Rectangle uRectangle = new Rectangle(urect.m_X, urect.m_Y, urect.m_Width, urect.m_Height);
                    Rectangle gRectangle = new Rectangle(glyph.m_GlyphRect.m_X, glyph.m_GlyphRect.m_Y, glyph.m_GlyphRect.m_Width, glyph.m_GlyphRect.m_Height);

                    using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                    {
                        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                        g.DrawImage(used_gfx, uRectangle);
                        g.DrawImage(glyph_gfx, gRectangle);
                    }
                    pictureBox1.Refresh();
                }
            }

            glyph.m_GlyphRect.m_Y = pictureBox1.Image.Size.Height - glyph.m_GlyphRect.m_Y - glyph.m_GlyphRect.m_Height;
            urect.m_Y = pictureBox1.Image.Size.Height - urect.m_Y - urect.m_Height;

            if(!font.m_CharacterTable.Contains(character))
            {
                font.m_CharacterTable.Add(character);
            }

            if(!font.m_GlyphTable.Contains(glyph))
            {
                font.m_GlyphTable.Add(glyph);
            }

            if (!font.m_UsedGlyphRects.Contains(urect))
            {
                font.m_UsedGlyphRects.Add(urect);
            }

            font.m_GlyphTable.Sort((x, y) => x.m_Index.CompareTo(y.m_Index));
            font.m_CharacterTable.Sort((x, y) => x.m_Unicode.CompareTo(y.m_Unicode));
            font.m_UsedGlyphRects.Sort((x, y) => WhatIsInsideGlyphRect(x).m_GlyphIndex.CompareTo(WhatIsInsideGlyphRect(y).m_GlyphIndex));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "*.TXT files|*.txt";
            saveFileDialog.Title = "Save file as .TXT (if you want)";
            saveFileDialog.FileName = txtdata;
            if(saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (StreamWriter writetext = new StreamWriter(saveFileDialog.FileName))
            {
                writetext.WriteLine("m_Index = " + glyph.m_Index.ToString());
                writetext.WriteLine("m_Metrics.m_Width = " + glyph.m_Metrics.m_Width.ToString());
                writetext.WriteLine("m_Metrics.m_Height = " + glyph.m_Metrics.m_Height.ToString());
                writetext.WriteLine("m_Metrics.m_HorizontalBearingX = " + glyph.m_Metrics.m_HorizontalBearingX.ToString());
                writetext.WriteLine("m_Metrics.m_HorizontalBearingY = " + glyph.m_Metrics.m_HorizontalBearingY.ToString());
                writetext.WriteLine("m_Metrics.m_HorizontalAdvance = " + glyph.m_Metrics.m_HorizontalAdvance.ToString());
                writetext.WriteLine("m_GlyphRect.m_X = " + glyph.m_GlyphRect.m_X.ToString());
                writetext.WriteLine("m_GlyphRect.m_Y = " + glyph.m_GlyphRect.m_Y.ToString());
                writetext.WriteLine("m_GlyphRect.m_Width = " + glyph.m_GlyphRect.m_Width.ToString());
                writetext.WriteLine("m_GlyphRect.m_Height = " + glyph.m_GlyphRect.m_Height.ToString());
                writetext.WriteLine("m_Scale = " + glyph.m_Scale.ToString());
                writetext.WriteLine("m_AtlasIndex = " + glyph.m_AtlasIndex.ToString());

                writetext.WriteLine("____________________________________________________");

                writetext.WriteLine("m_ElementType = " + character.m_ElementType.ToString());
                writetext.WriteLine("m_Unicode = " + character.m_Unicode.ToString());
                writetext.WriteLine("m_GlyphIndex = " + character.m_GlyphIndex.ToString());
                writetext.WriteLine("m_Scale = " + character.m_Scale.ToString());

                writetext.WriteLine("____________________________________________________");

                if (!IsSpecial(character))
                {
                    writetext.WriteLine("m_UsedGlyphRects.m_X = " + urect.m_X.ToString());
                    writetext.WriteLine("m_UsedGlyphRects.m_Y = " + urect.m_Y.ToString());
                    writetext.WriteLine("m_UsedGlyphRects.m_Width = " + urect.m_Width.ToString());
                    writetext.WriteLine("m_UsedGlyphRects.m_Height = " + urect.m_Height.ToString());
                } else
                {
                    writetext.WriteLine("SPECIAL");
                    writetext.WriteLine("SPECIAL");
                    writetext.WriteLine("SPECIAL");
                    writetext.WriteLine("SPECIAL");
                }
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
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IncreaseIndex();
            PaintRectangle();
            //PaintRectangleUsed();
            UpdateTextboxString();
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            bool is_1_empty = textBox2.Text.Length == 0;
            bool is_2_empty = textBox3.Text.Length == 0;
            bool is_3_empty = textBox4.Text.Length == 0;
            if (is_1_empty && is_2_empty && is_3_empty)
            {
                return;
            }

            if (is_2_empty && is_3_empty)
            {
                // Normal
                if (textBox2.Text.Length > 0 && int.TryParse(textBox2.Text, out int a) && a > 0)
                {
                    int toint = int.Parse(textBox2.Text);
                    textBox3.Text = InterpretY(toint).ToString();
                    textBox4.Text = InterpretY2(toint).ToString();
                    //textBox4.Text = (pictureBox1.Size.Height - toint).ToString();
                }
                else
                {
                    textBox3.Text = "";
                    textBox4.Text = "";
                }
            }

            if (is_1_empty && is_3_empty)
            {
                // Table
                if (textBox3.Text.Length > 0 && int.TryParse(textBox3.Text, out int a) && a > 0)
                {
                    int toint = int.Parse(textBox3.Text);
                    int res1 = NormalizeY(toint);
                    textBox2.Text = res1.ToString();
                    textBox4.Text = InterpretY2(res1).ToString();
                }
                else
                {
                    textBox2.Text = "";
                    textBox4.Text = "";
                }
            }

            if (is_1_empty && is_2_empty)
            {
                // Used
                if (textBox4.Text.Length > 0 && int.TryParse(textBox4.Text, out int a) && a > 0)
                {
                    int toint = int.Parse(textBox4.Text);
                    int res1 = NormalizeY2(toint);
                    textBox2.Text = res1.ToString();
                    textBox3.Text = InterpretY(res1).ToString();
                }
                else
                {
                    textBox2.Text = "";
                    textBox3.Text = "";
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if(textBox5.Text.Length > 1)
            {
                return;
            }

            if(textBox5.Text.Length <= 0)
            {
                return;
            }

            char c = textBox5.Text[0];
            textBox6.Text = "";
            textBox6.Text += ((uint)c).ToString();
        }
    }
}
