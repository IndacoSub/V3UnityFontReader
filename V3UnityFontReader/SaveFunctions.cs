using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace V3UnityFontReader
{
    public partial class Form1
    {
        private void SaveImageAsPNG()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            Image i = PictureBoxImage.Image;
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

        private void SaveFontAsTXT()
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
                if (!font.m_CharacterTable.Contains(sc.TCharacter))
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
                    string before_equals =
                        txt_lines[j].Substring(0, txt_lines[j].LastIndexOf("=") + 1 + 1); // Itself *and* space included

                    int red_size = int.Parse(txt_lines[j].Substring(before_equals.Length));
                    if (red_size <= 0)
                    {
                        last = txt_lines[j];
                        continue;
                    }

                    if (last.Contains("m_GlyphTable"))
                    {
                        size = font.m_GlyphTable.Count;
                        txt_lines[j] = before_equals + size;
                        txt_lines[j - 1] = before_last + size + after_last;
                        txt_lines.RemoveRange(j + 1, 16 * original_txt_glyph_size);
                        lines_count -= 16 * original_txt_glyph_size;
                        j++;
                    }
                    else
                    {
                        if (last.Contains("m_CharacterTable"))
                        {
                            size = font.m_CharacterTable.Count;
                            txt_lines[j] = before_equals + size;
                            txt_lines[j - 1] = before_last + size + after_last;
                            txt_lines.RemoveRange(j + 1, 6 * original_txt_ctable_size);
                            lines_count -= 6 * original_txt_ctable_size;
                            j++;
                        }
                        else
                        {
                            if (last.Contains("m_UsedGlyphRects"))
                            {
                                size = font.m_UsedGlyphRects.Count;
                                txt_lines[j] = before_equals + size;
                                txt_lines[j - 1] = before_last + size + after_last;
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
                                    txt_lines[j] = before_equals + size;
                                    txt_lines[j - 1] = before_last + size + after_last;
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
                    string before_equals =
                        txt_lines[j].Substring(0, txt_lines[j].IndexOf("=") + 1 + 1); // Itself *and* space included
                    txt_lines[j] = before_equals + PictureBoxImage.Image.Size.Width;
                }

                if (txt_lines[j].Contains("characterSequence"))
                {
                    string before_equals =
                        txt_lines[j].Substring(0, txt_lines[j].IndexOf("=") + 1 + 1); // Itself *and* space included
                    before_equals += "\"";

                    /*
                    foreach(TMPCharacter c in bak_charactertable)
                    {
                        Debug.WriteLine("Character: " + (char)c.m_Unicode + "(" + c.m_Unicode + "), num: " + c.m_GlyphIndex);
                    }
                    */

                    foreach (TMPCharacter c in bak_charactertable)
                    {
                        char ch = (char)c.m_Unicode;
                        if (ch <= 0)
                        {
                            Debug.WriteLine("Character is being detected as zero or negative: " + c.m_Unicode);
                            //continue;
                        }

                        if (char.IsWhiteSpace(ch))
                        {
                            Debug.WriteLine("Detected whitespace character: " + c.m_Unicode);
                            //continue;
                        }

                        before_equals += ch;
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
    }
}
