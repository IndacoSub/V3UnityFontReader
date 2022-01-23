using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    break;
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

        public string Write(string str, int index, int param)
        {
            string ret = str;
            string before = "";
            string after = "";
            if (index == 0)
            {
                before = str.Substring(0, str.LastIndexOf("[") + 1);
                after = "]";
            }
            else
            {
                if (index == 1)
                {
                    Debug.WriteLine(str);
                }
                before = str.Substring(0, str.LastIndexOf("=") + 1 + 1);
                after = "";
            }

            switch (index)
            {
                case 0:
                    ret = before + param + after;
                    break;
                case 1:
                    break;
                case 2:
                    ret = before + m_Index;
                    break;
                case 3:
                    break;
                case 4:
                    ret = before + m_Metrics.m_Width;
                    break;
                case 5:
                    ret = before + m_Metrics.m_Height;
                    break;
                case 6:
                    ret = before + m_Metrics.m_HorizontalBearingX;
                    break;
                case 7:
                    ret = before + m_Metrics.m_HorizontalBearingY;
                    break;
                case 8:
                    ret = before + m_Metrics.m_HorizontalAdvance;
                    break;
                case 9:
                    break;
                case 10:
                    ret = before + m_GlyphRect.m_X;
                    break;
                case 11:
                    ret = before + m_GlyphRect.m_Y;
                    break;
                case 12:
                    ret = before + m_GlyphRect.m_Width;
                    break;
                case 13:
                    ret = before + m_GlyphRect.m_Height;
                    break;
                case 14:
                    ret = before + m_Scale;
                    break;
                case 15:
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
