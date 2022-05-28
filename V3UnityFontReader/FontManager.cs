using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

// Copied from "DGRV3TS" so not "native" to this application

namespace V3UnityFontReader
{
    internal class FontManager
    {
        public const int FirstFont = 0;
        public Color CurrentColor = Color.White;
        public Font CurrentFont;
        public float CurrentFontSize;
        public PrivateFontCollection CustomFonts;

        // Use Calibri for the displayed text
        public string DefaultFontName = "Calibri";

        // Use Microsoft Sans Serif for the editable text box
        public string DefaultFontNameForText = "Microsoft Sans Serif";
        public List<Font> FontList;

        public string FontName = "";
        public float FontSize;
        public int FontSizeForText = 14;
        public bool LoadedFont;

        public FontManager(string init_font)
        {
            LoadFont(init_font);
        }

        public void LoadByCSFont(string familyname, float fsize, FontStyle style)
        {
            CurrentFont = new Font(familyname, fsize, style);
            CurrentFontSize = fsize;
        }

        public void LoadFont(string init_font)
        {
            string familyName = "";
            string familyNameAndStyle = "";
            FontFamily[] fontFamilies;
            CustomFonts = new PrivateFontCollection();
            FontSize = new float();
            FontSize = 0.0f;

            FontList = new List<Font>();

            FontSize = 29.0f;

            CurrentFontSize = FontSize;

            FontName = DefaultFontName;

            // The ideal font would be FOT-HummingStd-D (I guess?)
            if (File.Exists("font.otf"))
            {
                CustomFonts.AddFontFile("font.otf");
            }
            else
            {
                if (File.Exists("font.ttf"))
                {
                    CustomFonts.AddFontFile("font.ttf");
                }
                else
                {
                    string file = init_font;
                    if (File.Exists(file))
                    {
                        LoadCustomFont(file);
                    }
                }
            }

            if (CustomFonts.Families.Length > 0)
            {
                LoadedFont = true;
            }

            if (LoadedFont)
            {
                fontFamilies = CustomFonts.Families;

                for (int j = 0, count = fontFamilies.Length; j < count; ++j)
                {
                    // Get the font family name.
                    familyName = fontFamilies[j].Name;

                    FontName = familyName;

                    LoadRegular(fontFamilies[j], familyName, familyNameAndStyle);

                    LoadBold(fontFamilies[j], familyName, familyNameAndStyle);

                    LoadItalic(fontFamilies[j], familyName, familyNameAndStyle);

                    LoadBoldItalic(fontFamilies[j], familyName, familyNameAndStyle);

                    LoadUnderline(fontFamilies[j], familyName, familyNameAndStyle);

                    LoadStrikeout(fontFamilies[j], familyName, familyNameAndStyle);
                }
            }
        }

        public void LoadCurrentFont()
        {
            CurrentFont = LoadedFont ? FontList[FirstFont] : new Font(DefaultFontName, FontSize);
        }

        public void LoadCustomFont(string filename)
        {
            var file = filename;
            if ((!file.Contains(".ttf") && !file.Contains(".otf")) || file.Length == 0)
            {
                Debug.WriteLine("Could not load font!");
                return;
            }

            CustomFonts.AddFontFile(file);
        }

        public void LoadRegular(FontFamily ff, string fn, string fns)
        {
            // Is the regular style available?
            if (ff.IsStyleAvailable(FontStyle.Regular))
            {
                fns = "";
                fns = fns + fn;
                fns = fns + " Regular";

                Font regFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Regular,
                    GraphicsUnit.Pixel);

                FontList.Add(regFont);
            }
        }

        public void SetCurrentFontSize(float size)
        {
            CurrentFont = new Font(CurrentFont.Name, size);
        }

        public void LoadBold(FontFamily ff, string fn, string fns)
        {
            // Is the bold style available?
            if (ff.IsStyleAvailable(FontStyle.Bold))
            {
                fns = "";
                fns = fns + fn;
                fns = fns + " Bold";

                Font boldFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Bold,
                    GraphicsUnit.Pixel);

                FontList.Add(boldFont);
            }
        }

        public void LoadItalic(FontFamily ff, string fn, string fns)
        {
            // Is the italic style available?
            if (ff.IsStyleAvailable(FontStyle.Italic))
            {
                fns = "";
                fns = fns + ff;
                fns = fns + " Italic";

                Font italicFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Italic,
                    GraphicsUnit.Pixel);

                FontList.Add(italicFont);
            }
        }

        public void LoadBoldItalic(FontFamily ff, string fn, string fns)
        {
            // Is the bold italic style available?
            if (ff.IsStyleAvailable(FontStyle.Italic) && ff.IsStyleAvailable(FontStyle.Bold))
            {
                fns = "";
                fns = fns + fn;
                fns = fns + "BoldItalic";

                Font italicFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Italic | FontStyle.Bold,
                    GraphicsUnit.Pixel);

                FontList.Add(italicFont);
            }
        }

        public void LoadUnderline(FontFamily ff, string fn, string fns)
        {
            // Is the underline style available?
            if (ff.IsStyleAvailable(FontStyle.Underline))
            {
                fns = "";
                fns = fns + fn;
                fns = fns + " Underline";

                Font underlineFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Underline,
                    GraphicsUnit.Pixel);

                FontList.Add(underlineFont);
            }
        }

        public void LoadStrikeout(FontFamily ff, string fn, string fns)
        {
            // Is the strikeout style available?
            if (ff.IsStyleAvailable(FontStyle.Strikeout))
            {
                fns = "";
                fns = fns + fn;
                fns = fns + " Strikeout";

                Font strikeFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Strikeout,
                    GraphicsUnit.Pixel);

                FontList.Add(strikeFont);
            }
        }

        public Font CreateFont(string name, int size, FontStyle style)
        {
            Font replacementFont = new Font(name, size, style);
            return replacementFont;
        }

        /*

        private string GetFontInfo(Font f)
        {
            string name = "Name: " + f.Name + "\n";
            string family = "Family: " + f.FontFamily.ToString() + "\n";
            string size = "Size: " + f.Size.ToString() + "\n";
            string style = "Style: " + f.Style + "\n";
            string charset = "GdiCharSet: " + f.GdiCharSet.ToString() + "\n";
            string vertical = "GdiVerticalFont: " + f.GdiVerticalFont.ToString() + "\n";
            string height = "Height: " + f.Height + "\n";
            string ogname = "OriginalFontName: " + f.OriginalFontName + "\n";
            return name + family + size + style + charset + vertical + height + ogname;
        }

        private void ViewFontInfo(Font f)
        {
            InputManager.Print(GetFontInfo(f));
        }

        */
    }
}