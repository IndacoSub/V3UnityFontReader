using System;
using System.Linq;

namespace V3UnityFontReader
{
    public partial class V3UnityFontReader
    {
        private int InterpretY(int y)
        {
            // Used to read "partial glyphs" a.k.a. letters/numbers/symbols
            int ret = PictureBoxImage.Image.Size.Height - y - rect.m_Height;
            return ret < 0 ? 0 : ret;
        }

        private int InterpretY2(int y)
        {
            // Used to read *used* "full glyphs" a.k.a. letters/numbers/symbols PLUS their surrounding area (used in colored CLT)
            int ret = PictureBoxImage.Image.Size.Height - y - font.m_UsedGlyphRects[cur_index].m_Height;
            return ret < 0 ? 0 : ret;
        }

        private int InterpretY3(int y)
        {
            // Used to read *free* glyphs (dumbest feature ever)
            int ret = PictureBoxImage.Image.Size.Height - y - font.m_FreeGlyphRects[cur_index].m_Height;
            return ret < 0 ? 0 : ret;
        }

        private int NormalizeY(int y)
        {
            return Math.Abs(y - PictureBoxImage.Image.Size.Height + rect.m_Height);
        }

        private int NormalizeY2(int y)
        {
            return Math.Abs(y - PictureBoxImage.Image.Size.Height + font.m_UsedGlyphRects[cur_index].m_Height);
        }

        private int NormalizeY3(int y)
        {
            return Math.Abs(y - PictureBoxImage.Image.Size.Height + font.m_FreeGlyphRects[cur_index].m_Height);
        }

        // Remove spaces (https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string)
        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}