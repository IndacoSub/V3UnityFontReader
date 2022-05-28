using System;
using System.Collections.Generic;

namespace V3UnityFontReader
{
    using GameObject = System.UInt16;
    using MonoScript = System.UInt16;
    using Material = System.UInt16;
    using Font = System.UInt16;
    using Texture2D = System.UInt16;
    using Unknown = System.UInt16;

    public class KerningTable
    {
        List<Unknown> kerningPairs = new List<Unknown>();
    }

    internal class FontStructure
    {

        public PPtrT<GameObject> m_GameObject = new PPtrT<GameObject>();
        public sbyte m_Enabled = 1;
        public PPtrT<MonoScript> m_Script = new PPtrT<MonoScript>();
        public string m_Name = "";
        public Int32 hashCode = 0;
        public PPtrT<Material> material = new PPtrT<Material>();
        public Int32 materialHashCode = 0;
        public string m_Version = "";
        public string m_SourceFontFileGUID = "";
        public PPtrT<Font> m_SourceFontFile = new PPtrT<Font>();
        public Int32 m_AtlasPopulationMode = 0;
        public FaceInfo m_FaceInfo = new FaceInfo();
        public List<Glyph> m_GlyphTable = new List<Glyph>();
        public List<TMPCharacter> m_CharacterTable = new List<TMPCharacter>();
        public List<PPtrT<Texture2D>> m_AtlasTextures = new List<PPtrT<Texture2D>>();
        public Int32 m_AtlasTextureIndex = 0;
        public bool m_IsMultiAliasTextureEnabled = false;
        public List<GlyphRect> m_UsedGlyphRects = new List<GlyphRect>();
        public List<GlyphRect> m_FreeGlyphRects = new List<GlyphRect>();
        public LegacyFaceInfo m_fontInfo = new LegacyFaceInfo();
        public PPtrT<Texture2D> atlas = new PPtrT<Texture2D>();
        public Int32 m_AtlasWidth = 0;
        public Int32 m_AtlasHeight = 0;
        public Int32 m_AtlasPadding = 0;
        public Int32 m_AtlasRenderMode = 0;
        List<Unknown> m_glyphInfoList = new List<Unknown>();
        KerningTable m_KerningTable = new KerningTable();


        // TODO?
    }
}
