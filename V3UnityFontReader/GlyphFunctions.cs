using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace V3UnityFontReader
{
    public partial class V3UnityFontReader
    {
        private void DeleteUsedFreeGlyphs()
        {
            List<GlyphRect> free_after = new List<GlyphRect>();

            foreach (GlyphRect used in font.m_UsedGlyphRects)
            {
                foreach (GlyphRect free in font.m_FreeGlyphRects)
                {
                    Rectangle usedrect = new Rectangle(used.m_X,
                        PictureBoxImage.Image.Size.Height - used.m_Y - used.m_Height, used.m_Width, used.m_Height);
                    Rectangle freerect = new Rectangle(free.m_X,
                        PictureBoxImage.Image.Size.Height - free.m_Y - free.m_Height, free.m_Width, free.m_Height);

                    if (usedrect.IntersectsWith(freerect))
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

        // Actually used but commented out
        private void VerifyUsedGlyphTable()
        {
            Debug.WriteLine("Old rects: " + font.m_UsedGlyphRects.Count);

            List<GlyphRect> rects = new List<GlyphRect>();

            foreach (GlyphRect gr in font.m_UsedGlyphRects)
            {
                // X/Y can be 0 but not width or height
                if (gr.m_Width == 0 || gr.m_Height == 0)
                {
                    continue;
                }

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
                            bool is_x = g.m_GlyphRect.m_X > gr.m_X &&
                                        g.m_GlyphRect.m_Width + g.m_GlyphRect.m_X < gr.m_X + gr.m_Width;
                            bool is_y = g.m_GlyphRect.m_Y > gr.m_Y &&
                                        g.m_GlyphRect.m_Height + g.m_GlyphRect.m_Y < gr.m_Y + gr.m_Height;
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

            rects.Sort((x, y) =>
                WhatIsInsideGlyphRect(x).m_GlyphIndex.CompareTo(WhatIsInsideGlyphRect(y).m_GlyphIndex));

            Debug.WriteLine("New rects: " + rects.Count);

            font.m_UsedGlyphRects = rects;
        }

        // Get glyph from table (not used)
        private int GetGlyphByIndex(uint index)
        {
            //Debug.WriteLine("Index: " + index);
            //Debug.WriteLine("m_GlyphTable.Count: " + font.m_GlyphTable.Count);

            int ret = font.m_GlyphTable.FindIndex(gl => gl.m_Index == index);

            return ret;
        }
    }
}