using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace V3UnityFontReader
{
    public partial class V3UnityFontReader
    {
        private void ExtractAllGlyphs()
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
                rectangle = new Rectangle(font.m_UsedGlyphRects[j].m_X, InterpretY2(font.m_UsedGlyphRects[j].m_Y),
                    font.m_UsedGlyphRects[j].m_Width, font.m_UsedGlyphRects[j].m_Height);
                rect = font.m_UsedGlyphRects[j];
                ExtractUsedGlyph(j);
                PaintRectangleUsed();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            cur_index = 0;
        }

        private void ExtractUsedGlyph(int cont)
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

            if (ch == new TMPCharacter())
            {
                Debug.WriteLine("Invalid character!");
                return;
            }

            int gl_index = GetGlyphByIndex(ch.m_GlyphIndex);
            if (gl_index == -1)
            {
                Debug.WriteLine("Couldn't find corresponding glyph to" + ch.m_GlyphIndex + " !");
                return;
            }

            string newpng = Path.Combine(uglyphs, ch.m_GlyphIndex.ToString()) + ".png";
            Glyph g = font.m_GlyphTable[gl_index];
            Rectangle glyph_rect = new Rectangle(g.m_GlyphRect.m_X, g.m_GlyphRect.m_Y, g.m_GlyphRect.m_Width,
                g.m_GlyphRect.m_Height);
            Rectangle actual_rect = new Rectangle
            {
                X = glyph_rect.X - gr.m_X,
                Y = glyph_rect.Y - gr.m_Y,
                Width = glyph_rect.Width,
                Height = glyph_rect.Height - 1 // For some reason it doesn't work correctly without this -1???
            };

            using (Bitmap full_image = new Bitmap(png_fn))
            {
                using (Bitmap portion = full_image.Clone(rectangle, full_image.PixelFormat))
                {
                    using (Graphics graphics = Graphics.FromImage(portion))
                    {
                        graphics.SetClip(actual_rect);
                        graphics.Clear(Color.Transparent);
                    }

                    portion.Save(newpng, ImageFormat.Png);
                    portion.Dispose();
                    full_image.Dispose();
                }
            }
        }

        private void ExtractGlyphData()
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
                writetext.WriteLine("m_Index = " + font.m_GlyphTable[cur_index].m_Index);
                writetext.WriteLine("m_Metrics.m_Width = " + font.m_GlyphTable[cur_index].m_Metrics.m_Width);
                writetext.WriteLine("m_Metrics.m_Height = " + font.m_GlyphTable[cur_index].m_Metrics.m_Height);
                writetext.WriteLine("m_Metrics.m_HorizontalBearingX = " +
                                    font.m_GlyphTable[cur_index].m_Metrics.m_HorizontalBearingX);
                writetext.WriteLine("m_Metrics.m_HorizontalBearingY = " +
                                    font.m_GlyphTable[cur_index].m_Metrics.m_HorizontalBearingY);
                writetext.WriteLine("m_Metrics.m_HorizontalAdvance = " +
                                    font.m_GlyphTable[cur_index].m_Metrics.m_HorizontalAdvance);
                writetext.WriteLine("m_GlyphRect.m_X = " + font.m_GlyphTable[cur_index].m_GlyphRect.m_X);
                writetext.WriteLine("m_GlyphRect.m_Y = " + font.m_GlyphTable[cur_index].m_GlyphRect.m_Y);
                writetext.WriteLine("m_GlyphRect.m_Width = " + font.m_GlyphTable[cur_index].m_GlyphRect.m_Width);
                writetext.WriteLine("m_GlyphRect.m_Height = " + font.m_GlyphTable[cur_index].m_GlyphRect.m_Height);
                writetext.WriteLine("m_Scale = " + font.m_GlyphTable[cur_index].m_Scale);
                writetext.WriteLine("m_AtlasIndex = " + font.m_GlyphTable[cur_index].m_AtlasIndex);

                writetext.WriteLine("____________________________________________________");

                TMPCharacter character = GetCharacterFromIndex(font.m_GlyphTable[cur_index].m_Index);

                writetext.WriteLine("m_ElementType = " + character.m_ElementType);
                writetext.WriteLine("m_Unicode = " + character.m_Unicode);
                writetext.WriteLine("m_GlyphIndex = " + character.m_GlyphIndex);
                writetext.WriteLine("m_Scale = " + character.m_Scale);

                writetext.WriteLine("____________________________________________________");

                if (CanHaveUsedGlyph(character))
                {
                    int used_index = UsedGlyphRectByCharacter(character);
                    if (used_index >= font.m_UsedGlyphRects.Count || used_index < 0)
                    {
                        Debug.WriteLine("Used GlyphRect not found for character: " + character.m_Unicode +
                                        ", with index: " + character.m_GlyphIndex + ", len: " +
                                        font.m_UsedGlyphRects.Count + ", index: " + used_index);
                    }
                    else
                    {
                        writetext.WriteLine("m_UsedGlyphRects.m_X = " + font.m_UsedGlyphRects[used_index].m_X);
                        writetext.WriteLine("m_UsedGlyphRects.m_Y = " + font.m_UsedGlyphRects[used_index].m_Y);
                        writetext.WriteLine("m_UsedGlyphRects.m_Width = " + font.m_UsedGlyphRects[used_index].m_Width);
                        writetext.WriteLine("m_UsedGlyphRects.m_Height = " + font.m_UsedGlyphRects[used_index].m_Height);
                    }
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

        private void ExtractGlyphImage()
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
            portion.Save(newpng, ImageFormat.Png);
            portion.Dispose();
            full_image.Dispose();
        }
    }
}