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
                    Debug.WriteLine("(R) Unexpected case in GlyphRect!");
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
                    ret = before + m_X;
                    break;
                case 3:
                    ret = before + m_Y;
                    break;
                case 4:
                    ret = before + m_Width;
                    break;
                case 5:
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
