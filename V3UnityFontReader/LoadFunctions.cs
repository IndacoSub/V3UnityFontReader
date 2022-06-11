using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace V3UnityFontReader
{
    public partial class Form1
    {
        private void OpenPNG()
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
            PictureBoxImage.Image = new Bitmap(png_fn);
            PictureBoxImage.Refresh();

            loaded_png = true;

            if (loaded_txt)
            {
                rect = font.m_GlyphTable[cur_index].m_GlyphRect;

                PaintRectangle();

                UpdateTextboxString();
            }
        }

        private void OpenTXT()
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

        private void LoadCustomFromFont(int mouseX, int mouseY)
        {
            string characterin = "";
            var res = ShowInputDialog(ref characterin);
            if (res != DialogResult.OK)
            {
                return;
            }

            if (fm.CurrentFont == null)
            {
                MessageBox.Show("Current font is null!");
                return;
            }

            if (characterin.Length < 1)
            {
                MessageBox.Show("Invalid character!");
                return;
            }

            characterin = RemoveWhitespace(characterin);

            // Graphics part

            int startX = 0, startY = 0;
            int leftmostX = mouseX;
            for (int j = 0; j < characterin.Length; j++)
            {
                if (j == 0)
                {
                    startX = mouseX;
                    startY = mouseY;
                }

                string ch = characterin[j].ToString();
                (int retx, int rety) = AttemptNextXY(ch, startX, startY);
                int newx = startX + 3 * retx + 10; // Arbitrary value just to be sure
                int newy = startY;
                if (newx >= PictureBoxImage.Width ||
                    newx + 6 * retx + 10 >= PictureBoxImage.Width) // Arbitrary value just to be extra sure
                {
                    newx = leftmostX;
                    newy = startY + 3 * rety + 10; // Arbitrary value just to be sure
                }

                startX = newx;
                startY = newy;
            }
        }

        private void LoadCustomFromFiles(int mouseX, int mouseY)
        {
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

            if (urect == null || urect == new GlyphRect())
            {
                Debug.WriteLine("Null rect!");
                return;
            }

            string image = Directory.GetCurrentDirectory();
            image = Path.Combine(image, "extracted", Path.GetFileNameWithoutExtension(txt_fn), "images");
            image = Path.Combine(image, character.m_GlyphIndex + ".png");
            if (!File.Exists(image))
            {
                Debug.WriteLine("PNG image not found: " + image);
                return;
            }

            string uglyph_image = Directory.GetCurrentDirectory();
            uglyph_image = Path.Combine(uglyph_image, "extracted", Path.GetFileNameWithoutExtension(txt_fn),
                "used_glyphs");
            uglyph_image = Path.Combine(uglyph_image, character.m_GlyphIndex + ".png");
            if (!File.Exists(uglyph_image))
            {
                Debug.WriteLine("Used PNG image not found: " + uglyph_image);
                return;
            }

            Image bak = new Bitmap(PictureBoxImage.Image);

            using (Bitmap used_gfx = new Bitmap(uglyph_image))
            {
                urect.m_Width = used_gfx.Width;
                urect.m_Height = used_gfx.Height;
                urect.m_X = mouseX;
                urect.m_Y = mouseY;

                using (Bitmap glyph_gfx = new Bitmap(image))
                {
                    // The +1 is needed, not sure why that is
                    glyph.m_GlyphRect.m_Width = glyph_gfx.Width + 1;
                    glyph.m_GlyphRect.m_Height = glyph_gfx.Height;
                    glyph.m_GlyphRect.m_X = urect.m_X;
                    glyph.m_GlyphRect.m_Y = urect.m_Y;
                    bool is_x_odd = (urect.m_Width - glyph.m_GlyphRect.m_Width) / 2 % 2 != 0;
                    bool is_y_odd = (urect.m_Height - glyph.m_GlyphRect.m_Height) / 2 % 2 != 0;
                    Debug.WriteLine("Is X odd: " + is_x_odd);
                    Debug.WriteLine("Is Y odd: " + is_y_odd);
                    glyph.m_GlyphRect.m_X =
                        urect.m_X + (urect.m_Width - glyph.m_GlyphRect.m_Width) / 2 + (is_x_odd ? 0 : 0);
                    glyph.m_GlyphRect.m_Y = urect.m_Y + (urect.m_Height - glyph.m_GlyphRect.m_Height) / 2 +
                                            (is_y_odd ? 0 : 0);
                    //glyph.m_GlyphRect.m_Y = PictureBoxImage.Image.Size.Height - urect.m_Height - glyph.m_GlyphRect.m_Height;

                    Rectangle uRectangle = new Rectangle(urect.m_X, urect.m_Y, urect.m_Width, urect.m_Height);
                    Rectangle gRectangle = new Rectangle(glyph.m_GlyphRect.m_X, glyph.m_GlyphRect.m_Y,
                        glyph.m_GlyphRect.m_Width, glyph.m_GlyphRect.m_Height);

                    using (Graphics g = Graphics.FromImage(PictureBoxImage.Image))
                    {
                        g.CompositingMode = CompositingMode.SourceCopy;
                        g.DrawImage(used_gfx, uRectangle);
                        g.DrawImage(glyph_gfx, gRectangle);
                    }
                }
            }

            glyph.m_GlyphRect.m_Y = PictureBoxImage.Image.Size.Height - glyph.m_GlyphRect.m_Y - glyph.m_GlyphRect.m_Height;
            urect.m_Y = PictureBoxImage.Image.Size.Height - urect.m_Y - urect.m_Height;

            if (glyph.m_GlyphRect.m_Y < 0)
            {
                PictureBoxImage.Image = new Bitmap(bak);
                PictureBoxImage.Refresh();
                return;
            }

            if (urect.m_Y < 0)
            {
                PictureBoxImage.Image = new Bitmap(bak);
                PictureBoxImage.Refresh();
                return;
            }

            PictureBoxImage.Refresh();

            if (!font.m_GlyphTable.Any(g => g.m_Index == glyph.m_Index))
            {
                font.m_GlyphTable.Add(glyph);
            }

            if (!font.m_CharacterTable.Any(c => c.m_GlyphIndex == character.m_GlyphIndex))
            {
                //Debug.WriteLine("Adding to character table!");
                font.m_CharacterTable.Add(character);
            }

            if (!font.m_UsedGlyphRects.Any(u => u.m_X == urect.m_X && u.m_Y == urect.m_Y))
            {
                font.m_UsedGlyphRects.Add(urect);
            }

            font.m_GlyphTable.Sort((x, y) => x.m_Index.CompareTo(y.m_Index));
            font.m_CharacterTable.Sort((x, y) => x.m_Unicode.CompareTo(y.m_Unicode));
            font.m_UsedGlyphRects.Sort((x, y) =>
                WhatIsInsideGlyphRect(x).m_GlyphIndex.CompareTo(WhatIsInsideGlyphRect(y).m_GlyphIndex));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "*.TXT files|*.txt";
            saveFileDialog.Title = "Save file as .TXT (if you want)";
            saveFileDialog.FileName = txtdata;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (StreamWriter writetext = new StreamWriter(saveFileDialog.FileName))
            {
                writetext.WriteLine("m_Index = " + glyph.m_Index);
                writetext.WriteLine("m_Metrics.m_Width = " + glyph.m_Metrics.m_Width);
                writetext.WriteLine("m_Metrics.m_Height = " + glyph.m_Metrics.m_Height);
                writetext.WriteLine("m_Metrics.m_HorizontalBearingX = " + glyph.m_Metrics.m_HorizontalBearingX);
                writetext.WriteLine("m_Metrics.m_HorizontalBearingY = " + glyph.m_Metrics.m_HorizontalBearingY);
                writetext.WriteLine("m_Metrics.m_HorizontalAdvance = " + glyph.m_Metrics.m_HorizontalAdvance);
                writetext.WriteLine("m_GlyphRect.m_X = " + glyph.m_GlyphRect.m_X);
                writetext.WriteLine("m_GlyphRect.m_Y = " + glyph.m_GlyphRect.m_Y);
                writetext.WriteLine("m_GlyphRect.m_Width = " + glyph.m_GlyphRect.m_Width);
                writetext.WriteLine("m_GlyphRect.m_Height = " + glyph.m_GlyphRect.m_Height);
                writetext.WriteLine("m_Scale = " + glyph.m_Scale);
                writetext.WriteLine("m_AtlasIndex = " + glyph.m_AtlasIndex);

                writetext.WriteLine("____________________________________________________");

                writetext.WriteLine("m_ElementType = " + character.m_ElementType);
                writetext.WriteLine("m_Unicode = " + character.m_Unicode);
                writetext.WriteLine("m_GlyphIndex = " + character.m_GlyphIndex);
                writetext.WriteLine("m_Scale = " + character.m_Scale);

                writetext.WriteLine("____________________________________________________");

                if (!IsSpecial(character))
                {
                    writetext.WriteLine("m_UsedGlyphRects.m_X = " + urect.m_X);
                    writetext.WriteLine("m_UsedGlyphRects.m_Y = " + urect.m_Y);
                    writetext.WriteLine("m_UsedGlyphRects.m_Width = " + urect.m_Width);
                    writetext.WriteLine("m_UsedGlyphRects.m_Height = " + urect.m_Height);
                }
                else
                {
                    writetext.WriteLine("SPECIAL");
                    writetext.WriteLine("SPECIAL");
                    writetext.WriteLine("SPECIAL");
                    writetext.WriteLine("SPECIAL");
                }
            }
        }
    }
}
