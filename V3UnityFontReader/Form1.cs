using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace V3UnityFontReader
{
    public partial class Form1 : Form
    {
        private readonly List<SpecialCharacter> specials = new();
        private string cur_filename = "";
        private int cur_index;
        private FontManager fm;
        private FontStructure font;
        private bool loaded_png;
        private bool loaded_txt;
        private int original_txt_ctable_size;
        private int original_txt_freeglyph_size;
        private int original_txt_glyph_size;
        private int original_txt_usedglyph_size;
        private string png_fn = "";
        private GlyphRect rect;
        private Rectangle rectangle;
        private int total_red_data;
        private int total_red_glyphs;
        private int total_red_image;
        private string txt_fn = "";
        private List<string> txt_lines = new();

        public Form1()
        {
            InitializeComponent();
            font = new FontStructure();
            rect = new GlyphRect();
            rectangle = new Rectangle();
            fm = new FontManager("Calibri");

            PictureBoxImage.SizeMode = PictureBoxSizeMode.AutoSize;
            ImagePanel.AutoScroll = true;
        }
    }
}