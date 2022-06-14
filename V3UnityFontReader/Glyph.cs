using System;
using System.Diagnostics;

namespace V3UnityFontReader
{
    internal class Glyph
    {
        public Int32 m_Index = 0;
        public GlyphMetrics m_Metrics = new GlyphMetrics();
        public GlyphRect m_GlyphRect = new GlyphRect();
        public float m_Scale = 1;
        public Int32 m_AtlasIndex = 0;

        public void Read(string str, int index)
        {
            string after_equal = str.Substring(str.LastIndexOf("=") + 1);

            switch (index)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    m_Index = int.Parse(after_equal);
                    break;
                case 3:
                    m_Metrics = new GlyphMetrics();
                    break;
                case 4:
                    m_Metrics.m_Width = float.Parse(after_equal);
                    break;
                case 5:
                    m_Metrics.m_Height = float.Parse(after_equal);
                    break;
                case 6:
                    m_Metrics.m_HorizontalBearingX = float.Parse(after_equal);
                    break;
                case 7:
                    m_Metrics.m_HorizontalBearingY = float.Parse(after_equal);
                    break;
                case 8:
                    m_Metrics.m_HorizontalAdvance = float.Parse(after_equal);
                    break;
                case 9:
                    m_GlyphRect = new GlyphRect();
                    break;
                case 10:
                    m_GlyphRect.m_X = int.Parse(after_equal);
                    break;
                case 11:
                    m_GlyphRect.m_Y = int.Parse(after_equal);
                    break;
                case 12:
                    m_GlyphRect.m_Width = int.Parse(after_equal);
                    break;
                case 13:
                    m_GlyphRect.m_Height = int.Parse(after_equal);
                    break;
                case 14:
                    m_Scale = float.Parse(after_equal);
                    break;
                case 15:
                    m_AtlasIndex = int.Parse(after_equal);
                    break;
                default:
                    Debug.WriteLine("(R) Unexpected case in Gylph!");
                    break;
            }
        }

        public string Write(int index, int param)
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
                    ret = "   0 Glyph m_GlyphTable";
                    break;
                case 2:
                    before = "    0 unsigned int m_Index = ";
                    ret = before + m_Index;
                    break;
                case 3:
                    ret = "    0 GlyphMetrics m_Metrics";
                    break;
                case 4:
                    before = "     0 float m_Width = ";
                    ret = before + m_Metrics.m_Width;
                    break;
                case 5:
                    before = "     0 float m_Height = ";
                    ret = before + m_Metrics.m_Height;
                    break;
                case 6:
                    before = "     0 float m_HorizontalBearingX = ";
                    ret = before + m_Metrics.m_HorizontalBearingX;
                    break;
                case 7:
                    before = "     0 float m_HorizontalBearingY = ";
                    ret = before + m_Metrics.m_HorizontalBearingY;
                    break;
                case 8:
                    before = "     0 float m_HorizontalAdvance = ";
                    ret = before + m_Metrics.m_HorizontalAdvance;
                    break;
                case 9:
                    ret = "    0 GlyphRect m_GlyphRect";
                    break;
                case 10:
                    before = "     0 int m_X = ";
                    ret = before + m_GlyphRect.m_X;
                    break;
                case 11:
                    before = "     0 int m_Y = ";
                    ret = before + m_GlyphRect.m_Y;
                    break;
                case 12:
                    before = "     0 int m_Width = ";
                    ret = before + m_GlyphRect.m_Width;
                    break;
                case 13:
                    before = "     0 int m_Height = ";
                    ret = before + m_GlyphRect.m_Height;
                    break;
                case 14:
                    before = "    0 float m_Scale = ";
                    ret = before + m_Scale;
                    break;
                case 15:
                    before = "    0 int m_AtlasIndex = ";
                    ret = before + m_AtlasIndex;
                    break;
                default:
                    Debug.WriteLine("(W) Unexpected case in Gylph!");
                    break;
            }

            return ret;
        }
    }
}