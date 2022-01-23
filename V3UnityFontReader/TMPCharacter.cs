using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3UnityFontReader
{
    public class TMPCharacter
    {
        public Int32 m_ElementType = 1;
        public UInt32 m_Unicode = 0;
        public UInt32 m_GlyphIndex = 0;
        public float m_Scale = 1;

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
                    m_ElementType = Int32.Parse(after_equal);
                    break;
                case 3:
                    m_Unicode = UInt32.Parse(after_equal);
                    break;
                case 4:
                    m_GlyphIndex = UInt32.Parse(after_equal);
                    break;
                case 5:
                    m_Scale = float.Parse(after_equal);
                    break;
                default:
                    Debug.WriteLine("(R) Unexpected case in TMPCharacter!");
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
                    ret = before + m_ElementType;
                    break;
                case 3:
                    ret = before + m_Unicode;
                    break;
                case 4:
                    ret = before + m_GlyphIndex;
                    break;
                case 5:
                    ret = before + m_Scale;
                    break;
                default:
                    Debug.WriteLine("(W) Unexpected case in TMPCharacter!");
                    break;
            }

            return ret;
        }
    }
}
