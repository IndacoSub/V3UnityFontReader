using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    break;
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
                    Debug.WriteLine("Unexpected case in GlyphRect!");
                    break;
            }
        }
    }
}
