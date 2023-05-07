using System.Collections.Generic;
using System.Diagnostics;

namespace V3UnityFontReader
{
    public partial class V3UnityFontReader
    {
        private void VerifyCharacterTable()
        {
            var table = font.m_CharacterTable;
            List<TMPCharacter> ret = new List<TMPCharacter>();
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
                    Debug.WriteLine("No equivalent glyph found for character: \"" + (char)character.m_Unicode + "\"");
                    continue;
                }

                if (character.m_Unicode == 10 || character.m_Unicode == 13) // \n or \r?
                {
                    SpecialCharacter sc = new SpecialCharacter
                    {
                        Position = real_cont - 1,
                        TCharacter = character
                    };

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
            }

            font.m_CharacterTable = ret;
        }

        private bool AllTheSameAdvance()
        {
            float last = font.m_GlyphTable[0].m_Metrics.m_HorizontalAdvance;

            return font.m_GlyphTable.TrueForAll(g => g.m_Metrics.m_HorizontalAdvance == last);
        }

        private uint GetFirstFreeGlyph()
        {
            uint glyph = 0;

            // 30000 just because
            const int max = 30000;

            // 0 is probably taken
            for (uint j = 1; j < max; j++)
            {
                glyph = j;
                bool found = false;
                foreach (Glyph g in font.m_GlyphTable)
                {
                    found |= g.m_Index == (int)glyph;
                }

                if (!found)
                {
                    TMPCharacter ch = new TMPCharacter
                    {
                        m_GlyphIndex = glyph
                    };
                    if (IsSpecial(ch))
                    {
                        continue;
                    }

                    break;
                }
            }

            return glyph;
        }
    }
}