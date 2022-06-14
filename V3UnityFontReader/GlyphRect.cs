using System;
using System.Diagnostics;

namespace V3UnityFontReader
{
    internal class GlyphRect
    {
        public Int32 m_X = 0;
        public Int32 m_Y = 0;
        public Int32 m_Width = 0;
        public Int32 m_Height = 0;

        public void Read(string str, int index)
        {
            string after_equal = str.Substring(str.LastIndexOf("=") + 1);

            switch (index)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    m_X = Int32.Parse(after_equal);
                    break;
                case 3:
                    m_Y = Int32.Parse(after_equal);
                    break;
                case 4:
                    m_Width = Int32.Parse(after_equal);
                    break;
                case 5:
                    m_Height = Int32.Parse(after_equal);
                    break;
                default:
                    Debug.WriteLine("(R) Unexpected case in GlyphRect!");
                    break;
            }
        }

        public string Write(int index, int param, bool free)
        {
            string ret = "";
            string before;
            string after;

            switch (index)
            {
                case 0:
                    before = "  [";
                    after = "]";
                    ret = before + param + after;
                    break;
                case 1:
                    ret = free ? "   0 GlyphRect m_FreeGlyphRects" : "   0 GlyphRect m_UsedGlyphRects";
                    break;
                case 2:
                    before = "    0 int m_X = ";
                    ret = before + m_X;
                    break;
                case 3:
                    before = "    0 int m_Y = ";
                    ret = before + m_Y;
                    break;
                case 4:
                    before = "    0 int m_Width = ";
                    ret = before + m_Width;
                    break;
                case 5:
                    before = "    0 int m_Height = ";
                    ret = before + m_Height;
                    break;
                default:
                    Debug.WriteLine("(W) Unexpected case in GlyphRect!");
                    break;
            }

            return ret;
        }
    }
}