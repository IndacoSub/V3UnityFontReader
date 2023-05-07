using System;
using System.Diagnostics;

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

        public string Write(int index, int param)
        {
            string ret = "";
            string before = "";
            string after = "";

            switch (index)
            {
                case 0:
                    before = "  [";
                    after = "]";
                    ret = before + param + after;
                    break;
                case 1:
                    ret = "   0 TMP_Character m_CharacterTable";
                    break;
                case 2:
                    before = "    0 int m_ElementType = ";
                    ret = before + m_ElementType;
                    break;
                case 3:
                    before = "    0 unsigned int m_Unicode = ";
                    ret = before + m_Unicode;
                    break;
                case 4:
                    before = "    0 unsigned int m_GlyphIndex = ";
                    ret = before + m_GlyphIndex;
                    break;
                case 5:
                    before = "    0 float m_Scale = ";
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