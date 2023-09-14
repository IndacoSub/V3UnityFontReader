using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

// Copied from "DGRV3TS" so not "native" to this application

namespace V3UnityFontReader
{
    internal class FontManager
    {
        public const int FirstFont = 0;
        public Font CurrentFont;
        public float CurrentFontSize;
        public PrivateFontCollection CustomFonts;

        // Use Calibri for the displayed text
        public string DefaultFontName = "Calibri";

        public List<Font> FontList;

        public string FontName = "";
        public float FontSize;
        public bool LoadedFont;

#pragma warning disable CS8618
        public FontManager(string init_font, float size)
        {
            LoadFont(init_font, size);
        }
#pragma warning restore CS8618

        public void LoadFont(string init_font, float size)
        {
            CustomFonts = new PrivateFontCollection();
            FontSize = new float();
            FontSize = 0.0f;

            FontList = new List<Font>();

            FontSize = size;

            CurrentFontSize = FontSize;

            FontName = DefaultFontName;

            // The ideal font would be FOT-HummingStd-D (I guess?)
            if (!File.Exists(init_font))
            {
                return;
            }

            LoadedFont = LoadCustomFont(init_font);

            if(!LoadedFont)
            {
                return;
            }

            if(CustomFonts.Families.Length <= 0)
            {
                return;
            }

            FontFamily[] fontFamilies = CustomFonts.Families;

            for (int j = 0, count = fontFamilies.Length; j < count; ++j)
            {
                // Get the font family name.
                string familyName = fontFamilies[j].Name;

                FontName = familyName;

                LoadRegular(fontFamilies[j], familyName);

                LoadBold(fontFamilies[j], familyName);

                LoadItalic(fontFamilies[j], familyName);

                LoadBoldItalic(fontFamilies[j], familyName);

                LoadUnderline(fontFamilies[j], familyName);

                LoadStrikeout(fontFamilies[j], familyName);
            }
        }

        public void LoadCurrentFont()
        {
            CurrentFont = LoadedFont && FontList.Count > 0 ? FontList[FirstFont] : new Font(DefaultFontName, FontSize);
        }

        public bool LoadCustomFont(string filename)
        {
            var file = filename;
            bool valid_name = file.Length >= 1;
            bool is_ttf = file.Contains(".ttf");
            bool is_otf = file.Contains(".otf");
            bool valid_font = valid_name && (is_otf || is_ttf);

            if (valid_font)
            {
                CustomFonts.AddFontFile(file);
                return true;
            } else
            {
                Debug.WriteLine("Could not load font!");
                return false;
            }
        }

        public void LoadRegular(FontFamily ff, string fn)
        {
            // Is the regular style available?
            if (ff.IsStyleAvailable(FontStyle.Regular))
            {

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

        public void LoadBold(FontFamily ff, string fn)
        {
            // Is the bold style available?
            if (ff.IsStyleAvailable(FontStyle.Bold))
            {

                Font boldFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Bold,
                    GraphicsUnit.Pixel);

                FontList.Add(boldFont);
            }
        }

        public void LoadItalic(FontFamily ff, string fn)
        {
            // Is the italic style available?
            if (ff.IsStyleAvailable(FontStyle.Italic))
            {

                Font italicFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Italic,
                    GraphicsUnit.Pixel);

                FontList.Add(italicFont);
            }
        }

        public void LoadBoldItalic(FontFamily ff, string fn)
        {
            // Is the bold italic style available?
            if (ff.IsStyleAvailable(FontStyle.Italic) && ff.IsStyleAvailable(FontStyle.Bold))
            {

                Font italicFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Italic | FontStyle.Bold,
                    GraphicsUnit.Pixel);

                FontList.Add(italicFont);
            }
        }

        public void LoadUnderline(FontFamily ff, string fn)
        {
            // Is the underline style available?
            if (ff.IsStyleAvailable(FontStyle.Underline))
            {

                Font underlineFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Underline,
                    GraphicsUnit.Pixel);

                FontList.Add(underlineFont);
            }
        }

        public void LoadStrikeout(FontFamily ff, string fn)
        {
            // Is the strikeout style available?
            if (ff.IsStyleAvailable(FontStyle.Strikeout))
            {

                Font strikeFont = new Font(
                    fn,
                    FontSize,
                    FontStyle.Strikeout,
                    GraphicsUnit.Pixel);

                FontList.Add(strikeFont);
            }
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