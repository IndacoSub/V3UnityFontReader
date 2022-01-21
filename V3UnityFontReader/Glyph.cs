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

            switch(index)
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
                    m_Metrics.m_VerticalBearingX = float.Parse(after_equal);
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
                    Debug.WriteLine("Unexpected case in Gylph!");
                    break;
            }
        }
    }
}
