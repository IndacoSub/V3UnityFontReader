using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace V3UnityFontReader
{
    public partial class V3UnityFontReader
    {
        // TODO: Find a better name!
        private (int, int) AttemptNextXY(string character, int startX, int startY)
        {

            Color c = Color.FromArgb(125, 255, 255, 255); // Black with reduced alpha?
            SolidBrush mybrush = new SolidBrush(c);
            PointF point = new PointF(startX, startY);
            StringFormat format = StringFormat.GenericTypographic;

            float char_width, char_height;
            float spacing_width, spacing_height;

            using (Graphics g = Graphics.FromImage(PictureBoxImage.Image))
            {
                //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.DrawString(character, fm.CurrentFont, mybrush, point, format);
                const int maxheight = 100; // Could break with font size > 100
                SizeF size = g.MeasureString(character.Substring(0, 1), fm.CurrentFont, maxheight, format);
                char_width = size.Width;
                char_height = size.Height;
                //format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                SizeF spacing = g.MeasureString(" " + character.Substring(0, 1) + " ", fm.CurrentFont, maxheight,
                    format);
                spacing_width = spacing.Width;
                spacing_height = spacing.Height;
                //g.Save();
            }

            PictureBoxImage.Refresh();

            //MessageBox.Show("Width: " + char_width + ", Height: " + char_height);
            //MessageBox.Show("TS Width: " + spacing_width + ", TS Height: " + spacing_height);

            // Data part

            TMPCharacter tmpcharacter = new TMPCharacter();
            Glyph glyph = new Glyph();
            GlyphRect urect = new GlyphRect();

            // Get first free glyph
            uint free_glyph = GetFirstFreeGlyph();

            tmpcharacter.m_Unicode = character[0];
            tmpcharacter.m_GlyphIndex = free_glyph;

            glyph.m_Index = (int)free_glyph;
            glyph.m_GlyphRect.m_Width = (int)char_width;
            glyph.m_GlyphRect.m_Height = (int)char_height;
            glyph.m_GlyphRect.m_X = startX;
            glyph.m_GlyphRect.m_Y = startY;
            glyph.m_Metrics.m_Width = char_width;
            glyph.m_Metrics.m_Height = char_height;

            const double one_fortieth = 0.025f;

            glyph.m_Metrics.m_HorizontalBearingX = char_width * (float)one_fortieth;
            glyph.m_Metrics.m_HorizontalBearingY = glyph.m_Metrics.m_Height - glyph.m_Metrics.m_Height / 10 -
                                                   glyph.m_Metrics.m_HorizontalBearingX / 2;
            bool atsa = AllTheSameAdvance();
            if (!atsa)
            {
                glyph.m_Metrics.m_HorizontalAdvance =
                    glyph.m_Metrics.m_Width + glyph.m_Metrics.m_HorizontalBearingX * 2;
            }
            else
            {
                glyph.m_Metrics.m_HorizontalAdvance = font.m_GlyphTable[0].m_Metrics.m_HorizontalAdvance;
            }

            urect.m_Width = (int)spacing_width;
            urect.m_Height = (int)spacing_height + 1;
            //urect.m_Width = (int)char_width + 1;
            //urect.m_Height = (int)char_height + 1;

            // I have no idea
            urect.m_X = glyph.m_GlyphRect.m_X - (glyph.m_GlyphRect.m_Width - urect.m_Width) / 2;
            urect.m_Y = glyph.m_GlyphRect.m_Y - (glyph.m_GlyphRect.m_Height - urect.m_Height) / 2;

            glyph.m_GlyphRect.m_Y =
                PictureBoxImage.Image.Size.Height - glyph.m_GlyphRect.m_Y - glyph.m_GlyphRect.m_Height;
            urect.m_Y = PictureBoxImage.Image.Size.Height - urect.m_Y - urect.m_Height;

            if (CheckboxAddX.Checked)
            {
                float fl = float.Parse(TextboxX.Text);
                // (HACK) Why does this even work?!
                glyph.m_Metrics.m_Height += fl;
                glyph.m_GlyphRect.m_Height += (int)fl;
            }

            // TODO: Confirm Any vs All
            if (!font.m_GlyphTable.Any(g => g.m_Index == glyph.m_Index))
            {
                font.m_GlyphTable.Add(glyph);
            }

            // TODO: Confirm Any vs All
            if (!font.m_CharacterTable.Any(cc => cc.m_GlyphIndex == tmpcharacter.m_GlyphIndex))
            {
                //Debug.WriteLine("Adding to character table!");
                font.m_CharacterTable.Add(tmpcharacter);
            }

            // TODO: Confirm Any vs All
            if (!font.m_UsedGlyphRects.Any(u => u.m_X == urect.m_X && u.m_Y == urect.m_Y))
            {
                font.m_UsedGlyphRects.Add(urect);
            }

            // If it's a reasonable amount (performance-wise)
            if (font.m_GlyphTable.Count < 100)
            {
                font.m_GlyphTable.Sort((x, y) => x.m_Index.CompareTo(y.m_Index));
                font.m_CharacterTable.Sort((x, y) => x.m_Unicode.CompareTo(y.m_Unicode));
                font.m_UsedGlyphRects.Sort((x, y) =>
                    WhatIsInsideGlyphRect(x).m_GlyphIndex.CompareTo(WhatIsInsideGlyphRect(y).m_GlyphIndex));
            }

            int ret1 = (int)spacing_width;
            int ret2 = (int)spacing_height;

            // To account for errors in float->int
            return (ret1 + 1, ret2 + 1);
        }
    }
}