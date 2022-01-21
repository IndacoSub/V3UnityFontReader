using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3UnityFontReader
{
    internal class FontStructure
    {
        public PPtrT m_GameObject = new PPtrT();
        public sbyte m_Enabled = 1;
        public PPtrT m_Script = new PPtrT();
        public string m_Name = "";
        public Int32 hashCode = 0;
        public PPtrT material = new PPtrT();
        public Int32 materialHashCode = 0;
        public string m_Version = "";
        public string m_SourceFontFileGUID = "";
        public PPtrT m_SourceFontFile = new PPtrT();
        public Int32 m_AtlasPopulationMode = 0;
        public FaceInfo m_FaceInfo = new FaceInfo();
        public List<Glyph> m_GlyphTable = new List<Glyph>();
        public List<TMPCharacter> m_CharacterTable = new List<TMPCharacter>();
        public List<PPtrT> m_AtlasTextures = new List<PPtrT>();
        public Int32 m_AtlasTextureIndex = 0;
        public bool m_IsMultiAliasTextureEnabled = false;
        public List<GlyphRect> m_UsedGlyphRects = new List<GlyphRect>();
        public List<GlyphRect> m_FreeGlyphRects = new List<GlyphRect>();
        public LegacyFaceInfo m_fontInfo = new LegacyFaceInfo();
        public PPtrT atlas = new PPtrT();
        public Int32 m_AtlasWidth = 0;
        public Int32 m_AtlasHeight = 0;
        public Int32 m_AtlasPadding = 0;
        public Int32 m_AtlasRenderMode = 0;
        // Empty 
        // List`1 m_glyphInfoList
        // Array Array
        // int size = 0
        

        // TODO?
    }
}
