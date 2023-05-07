using System.Drawing;

namespace V3UnityFontReader
{
    public partial class V3UnityFontReader
    {
        private void PaintRectangle()
        {
            PictureBoxImage.Image = new Bitmap(png_fn);
            PictureBoxImage.Refresh();
            // Y is inverted and does not account for the character itself

            rectangle = new Rectangle(rect.m_X, InterpretY(rect.m_Y), rect.m_Width, rect.m_Height);

            //rectangle = new Rectangle(font.m_UsedGlyphRects[cur_index].m_X, InterpretY2(font.m_UsedGlyphRects[cur_index].m_Y), font.m_UsedGlyphRects[cur_index].m_Width, font.m_UsedGlyphRects[cur_index].m_Height);
            using (Graphics gr = Graphics.FromImage(PictureBoxImage.Image))
            {
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    gr.DrawRectangle(pen, rectangle);
                }
            }

            PictureBoxImage.Refresh();
        }

        private void PaintRectangleUsed()
        {
            PictureBoxImage.Image = new Bitmap(png_fn);
            PictureBoxImage.Refresh();

            //Debug.WriteLine("Width: " + font.m_UsedGlyphRects[cur_index].m_Width + ", Height: " + font.m_UsedGlyphRects[cur_index].m_Height);

            var ugr = font.m_UsedGlyphRects[cur_index];

            // Y is inverted and does not account for the character itself
            rectangle = new Rectangle(ugr.m_X,
                InterpretY2(ugr.m_Y),
                ugr.m_Width,
                ugr.m_Height);

            using (Graphics gr = Graphics.FromImage(PictureBoxImage.Image))
            {
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    gr.DrawRectangle(pen, rectangle);
                }
            }

            PictureBoxImage.Refresh();
        }

        private void UpdateTextboxString()
        {
            int index = font.m_GlyphTable[cur_index].m_Index;
            TMPCharacter character = GetCharacterFromIndex(index);
            if (character == new TMPCharacter())
            {
                return;
            }

            char c = (char)character.m_Unicode;
            int x = rect.m_X;
            int y = rect.m_Y;
            InfoTextbox.Text =
                "m_Index: " +
                index +
                ", character: \"" +
                c +
                "\" (" +
                character.m_Unicode +
                ")" +
                ", X: " +
                x +
                ", Y: " +
                y +
                ", InterpretedY: " +
                // Y is inverted and does not account for the character itself
                InterpretY(y) +
                ", cW: " +
                rect.m_Width +
                ", cH: " +
                rect.m_Height
                ;
            //InfoTextbox.Text = "Index: " + cur_index + ", inside is: " + (char)WhatIsInsideGlyphRect(font.m_UsedGlyphRects[cur_index]).m_Unicode + " (" + WhatIsInsideGlyphRect(font.m_UsedGlyphRects[cur_index]).m_Unicode + "), X: " + font.m_UsedGlyphRects[cur_index].m_X + ", Y: " + InterpretY2(font.m_UsedGlyphRects[cur_index].m_Y);
            InfoTextbox.Update();
            InfoTextbox.Refresh();

            LabelTableY.Text = "Table Rect Y (" + c + ")";
            LabelUsedY.Text = "Used Glyph Y (" + c + ")";
        }

        private void IncreaseIndex()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            if (cur_index + 1 >= font.m_CharacterTable.Count)
            {
                cur_index = font.m_CharacterTable.Count - 1;
                return;
            }

            cur_index++;
            //Debug.WriteLine("cur_index: " + cur_index + ", gTable: " + font.m_GlyphTable.Count + ", cTable: " + font.m_CharacterTable.Count);
            rect = font.m_GlyphTable[cur_index].m_GlyphRect;
        }

        private void DecreaseIndex()
        {
            if (!loaded_png || !loaded_txt)
            {
                return;
            }

            if (cur_index <= 0)
            {
                cur_index = 0;
                return;
            }

            cur_index--;
            rect = font.m_GlyphTable[cur_index].m_GlyphRect;
        }
    }
}