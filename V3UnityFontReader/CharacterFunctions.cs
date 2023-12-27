#define DEBUG

using System.Diagnostics;
using System.Linq;

namespace V3UnityFontReader
{
    public partial class V3UnityFontReader
    {
        private bool IsSpecial(TMPCharacter ch)
        {
            return specials.Any(sp => sp.TCharacter.m_GlyphIndex == ch.m_GlyphIndex);
        }

        private bool CanHaveUsedGlyph(TMPCharacter ch)
        {
            bool is_special = IsSpecial(ch);
            if (is_special)
            {
                //Debug.WriteLine("Is special?");
                return false;
            }

            int g_index = GetGlyphByIndex(ch.m_GlyphIndex);
            if (g_index == -1)
            {
                //Debug.WriteLine("GlyphIndex is -1?: " + ch.m_GlyphIndex);
                return false;
            }

            Glyph gg = font.m_GlyphTable[g_index];
            bool has_rect_coords = gg.m_GlyphRect.m_Width > 0 && gg.m_GlyphRect.m_Height > 0;
            // Needs to have coordinates?
            return has_rect_coords;
        }

        private int UsedGlyphRectByCharacter(TMPCharacter ch)
        {
            if (ch == new TMPCharacter())
            {
                return -1;
            }

            int ret = font.m_UsedGlyphRects.FindIndex(gr =>
                WhatIsInsideGlyphRect(gr).m_GlyphIndex == ch.m_GlyphIndex);

            return ret;
        }

        private TMPCharacter GetCharacterFromIndex(int index)
        {
            TMPCharacter res = font.m_CharacterTable.Find(ch => ch.m_GlyphIndex == index) ?? new TMPCharacter();

            return res;
        }

        private TMPCharacter WhatIsInsideGlyphRect(GlyphRect myrect)
        {
            TMPCharacter ret = new TMPCharacter();

            foreach (TMPCharacter ch in font.m_CharacterTable)
            {

				// Useful for debugging
                bool is_specific_char = false;

				int glyph_index = GetGlyphByIndex(ch.m_GlyphIndex);
                if (glyph_index == -1)
                {
                    Debug.WriteLine("Couldn't find glyph!");
                    continue;
                }

                Glyph glyph = font.m_GlyphTable[glyph_index];

                if (glyph.m_GlyphRect.m_X == 0 && glyph.m_GlyphRect.m_Y == 0)
                {
                    continue;
                }

                bool greater_x = glyph.m_GlyphRect.m_X > myrect.m_X;
                bool weird_math_x = glyph.m_GlyphRect.m_X + glyph.m_GlyphRect.m_Width < myrect.m_X + myrect.m_Width;
				bool is_inside_x = greater_x &&
                                   weird_math_x;
                bool greater_y = glyph.m_GlyphRect.m_Y > myrect.m_Y;
                bool weird_math_y = glyph.m_GlyphRect.m_Y + glyph.m_GlyphRect.m_Height < myrect.m_Y + myrect.m_Height;
				bool is_inside_y = greater_y &&
                                   weird_math_y;

                if (is_inside_x && is_inside_y)
                {
                    ret = ch;
                    break;
                }

#if DEBUG
                const int DESIRED_GRECT_X = 0;
                const int DESIRED_URECT_X = 0;
                const int DESIRED_URECT_Y = 0;

                if (!is_inside_x && !is_inside_y && is_specific_char)
                {
                    //Debug.WriteLine("\"" + (char)ch.m_Unicode + "\" (" + ch.m_Unicode + "): It's not within X or Y!");
                    if(!greater_x)
                    {
						//Debug.WriteLine("glyph.m_GlyphRect.m_X (" + glyph.m_GlyphRect.m_X + ") > myrect.m_X (" + myrect.m_X + ")");
					}
					if (!weird_math_x)
                    {
						//Debug.WriteLine("glyph.m_GlyphRect.m_X (" + glyph.m_GlyphRect.m_X + ") + glyph.m_GlyphRect.m_Width (" + glyph.m_GlyphRect.m_Width + ") < myrect.m_X (" + myrect.m_X + ") + myrect.m_Width (" + myrect.m_Width + ")");
					}
					if (!greater_y)
                    {
						//Debug.WriteLine("glyph.m_GlyphRect.m_Y (" + glyph.m_GlyphRect.m_Y + ") > myrect.m_Y (" + myrect.m_Y + ")");
					}
					if (!weird_math_y)
					{
						//Debug.WriteLine("glyph.m_GlyphRect.m_Y (" + glyph.m_GlyphRect.m_Y + ") + glyph.m_GlyphRect.m_Height (" + glyph.m_GlyphRect.m_Height + ") < myrect.m_Y (" + myrect.m_Y + ") + myrect.m_Height (" + myrect.m_Height + ")");
					}
				}
                else
                {
                    if (glyph.m_GlyphRect.m_X == DESIRED_GRECT_X && myrect.m_X == DESIRED_URECT_X &&
                        myrect.m_Y == DESIRED_URECT_Y)
                        if (!is_inside_x)
                        {
                            Debug.WriteLine("Wrong X math!");
                            Debug.WriteLine("glyph.m_GlyphRect.m_X (" + glyph.m_GlyphRect.m_X + ") > myrect.m_X (" +
                                            myrect.m_X +
                                            ")    &&    glyph.m_GlyphRect.m_X + glyph.m_GlyphRect.m_Width (" +
                                            glyph.m_GlyphRect.m_Width + ") < myrect.m_X + myrect.m_Width (" +
                                            myrect.m_Width + ") = (" +
                                            (glyph.m_GlyphRect.m_X + glyph.m_GlyphRect.m_Width) + " VS " +
                                            (myrect.m_X + myrect.m_Width) + ")"
                            );
                        }
                        else
                        {
                            if (!is_inside_y)
                            {
                                Debug.WriteLine("Wrong Y math!");
                                Debug.WriteLine("glyph.m_GlyphRect.m_Y (" + glyph.m_GlyphRect.m_Y + ") > myrect.m_Y (" +
                                                myrect.m_Y + ")    &&    glyph.m_GlyphRect.m_Y (" +
                                                glyph.m_GlyphRect.m_Y +
                                                ") + glyph.m_GlyphRect.m_Height (" +
                                                glyph.m_GlyphRect.m_Height + ") < myrect.m_Y (" + myrect.m_Y +
                                                ") + myrect.m_Height (" + myrect.m_Height + ") = (" +
                                                (glyph.m_GlyphRect.m_Y + glyph.m_GlyphRect.m_Height) + " VS " +
                                                (myrect.m_Y + myrect.m_Height) + ")"
                                );
                                //Debug.WriteLine("Suggestion?: myrect.m_Y could be: " + (glyph.m_GlyphRect.m_Y + (glyph.m_GlyphRect.m_Height / 2)));
                            }
                        }
                }
#endif
            }

            return ret;
        }
    }
}