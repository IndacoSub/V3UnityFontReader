using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace V3UnityFontReader
{
    public partial class V3UnityFontReader
    {
        private void ReplaceAllGlyphs()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            PictureBoxImage.Image = new Bitmap(png_fn);
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

            using (Graphics g = Graphics.FromImage(PictureBoxImage.Image))
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

        private void ReplaceUsedGlyphImage(string path)
        {
            string partial = path;
            if (!File.Exists(partial))
            {
                MessageBox.Show(partial, "File not found!");
                return;
            }

            using Bitmap partial_bmp = new Bitmap(partial);

            string glyph_index = path.Substring(path.LastIndexOf("\\") + 1);
            //Debug.WriteLine("Initial glyph index: " + glyph_index);
            glyph_index = glyph_index.Substring(0, glyph_index.IndexOf('.'));
            //Debug.WriteLine("After substring " + glyph_index);
            Debug.WriteLine("Glyph index: " + glyph_index);
            int glyph_index_int = int.Parse(glyph_index);
            int pos = GetGlyphByIndex((uint)glyph_index_int);
            if (pos == -1)
            {
                Debug.WriteLine("Couldn't find equivalent glyph with m_Index: " + glyph_index_int);
                return;
            }

            Glyph gg = font.m_GlyphTable[pos]; // Table
            TMPCharacter ch = GetCharacterFromIndex(gg.m_Index); // Character
            int u_pos = UsedGlyphRectByCharacter(ch); // Used (pos)
            if (u_pos == -1)
            {
                Debug.WriteLine("Couldn't find corresponding used glyph for character: " + ch.m_Unicode + " (" +
                                (char)ch.m_Unicode + "), index: " + ch.m_GlyphIndex +
                                " (a.k.a. probably wrong coordinates?)");
                return;
            }

            GlyphRect used = font.m_UsedGlyphRects[u_pos];

            rect = used;
            // Using (Interpret)Y(1) here is fine as we just declared that rect = used
            rectangle = new Rectangle(rect.m_X, InterpretY(rect.m_Y), rect.m_Width, rect.m_Height);

            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                return;
            }

            // For some reason, the opacity might be broken?
            using (Graphics g = Graphics.FromImage(PictureBoxImage.Image))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                // Using Y1 here is fine as we just declared that rect = used
                g.DrawImageUnscaled(partial_bmp, rect.m_X, InterpretY(rect.m_Y));
            }

            PictureBoxImage.Refresh();
            total_red_image++;
        }

        private void ReplaceGlyphData(string path)
        {
            string txtdata = path;
            if (!File.Exists(txtdata))
            {
                MessageBox.Show(txtdata, "File not found!");
                return;
            }

            string[] content = File.ReadAllLines(txtdata);

            int lines = content.Length;
            if (lines != 22)
            {
                MessageBox.Show(txtdata, "Wrong number of lines (" + lines + ", expected 22)!");
                return;
            }

            font.m_GlyphTable.Add(new Glyph());
            font.m_CharacterTable.Add(new TMPCharacter());

            // Read the .txt "line by line"
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
            // Line 12 = empty?
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
                // Line 17 = empty?
                font.m_UsedGlyphRects[total_red_glyphs].Read(content[18], 2);
                font.m_UsedGlyphRects[total_red_glyphs].Read(content[19], 3);
                font.m_UsedGlyphRects[total_red_glyphs].Read(content[20], 4);
                font.m_UsedGlyphRects[total_red_glyphs].Read(content[21], 5);

                total_red_glyphs++;
            }

            total_red_data++;
        }

        private void ReplaceGlyphImage(string path)
        {
            string partial = path;
            if (!File.Exists(partial))
            {
                MessageBox.Show(partial, "File not found!");
                return;
            }

            using Bitmap partial_bmp = new Bitmap(partial);

            string glyph_index = path.Substring(path.LastIndexOf("\\") + 1);
            //Debug.WriteLine("Initial glyph index: " + glyph_index);
            glyph_index = glyph_index.Substring(0, glyph_index.IndexOf('.'));
            //Debug.WriteLine("After substring: " + glyph_index);
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
                Debug.WriteLine("The rectangle for " + partial + " (glyph: " +
                                font.m_GlyphTable[total_red_image].m_Index + ") is empty!");
                return;
            }

            // For some reason, the opacity might be broken?
            using (Graphics g = Graphics.FromImage(PictureBoxImage.Image))
            {
                // TODO: Is this intentional? Above it's SourceOver (commented out)
                g.CompositingMode = CompositingMode.SourceCopy;
                //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.DrawImageUnscaled(partial_bmp, rect.m_X, InterpretY(rect.m_Y));
            }

            PictureBoxImage.Refresh();
            total_red_image++;
        }
    }
}