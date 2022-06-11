using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3UnityFontReader
{
    public partial class Form1
    {
        private void VerifyCharacterTable()
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
                    Debug.WriteLine("No equivalent glyph found for character: \"" + (char)character.m_Unicode + "\"");
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

        private bool AllTheSameAdvance()
        {
            float last = font.m_GlyphTable[0].m_Metrics.m_HorizontalAdvance;
            foreach (Glyph g in font.m_GlyphTable)
            {
                if (last != g.m_Metrics.m_HorizontalAdvance)
                {
                    return false;
                }
            }

            return true;
        }

        private uint GetFirstFreeGlyph()
        {
            uint glyph = 0;

            for (uint j = 1; j < 30000; j++)
            {
                glyph = j;
                bool found = false;
                foreach (Glyph g in font.m_GlyphTable)
                {
                    found |= g.m_Index == (int)glyph;
                }

                if (!found)
                {
                    TMPCharacter ch = new TMPCharacter();
                    ch.m_GlyphIndex = glyph;
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
