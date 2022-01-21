using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3UnityFontReader
{
    internal class TMPCharacter
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
                    Debug.WriteLine("Unexpected case in TMPCharacter!");
                    break;
            }
        }
    }
}
