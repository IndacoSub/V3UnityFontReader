using System;
using System.Collections.Generic;

namespace V3UnityFontReader
{
	/*
    using GameObject = System.UInt16;
    using MonoScript = System.UInt16;
    using Material = System.UInt16;
    using Font = System.UInt16;
    using Texture2D = System.UInt16;
    using Unknown = System.UInt16;
	*/

	/*
    public class KerningTable
    {
        public List<Unknown> kerningPairs = new List<Unknown>();
    }
    */

	/*
	public class FaceInfo
	{
		public string m_FamilyName = "";
		public string m_StyleName = "";
		public Int32 m_PointSize = 0;
		public float m_Scale = 1;
		public float m_LineHeight = 0;
		public float m_AscentLine = 0;
		public float m_CapLine = 0;
		public float m_MeanLine = 0;
		public float m_Baseline = 0;
		public float m_DescentLine = 0;
		public float m_SuperscriptOffset = 0;
		public float m_SuperscriptSize = 0;
		public float m_SubscriptOffset = 0;
		public float m_SubscriptSize = 0;
		public float m_UnderlineOffset = 0;
		public float m_UnderlineThickness = 0;
		public float m_StrikethroughOffset = 0;
		public float m_StrikethroughThickness = 0;
		public float m_TabWidth = 0;
	}
	*/

	/*
	public class PPtrT<T>
	{
		public Int32 m_FileID = 0;
		public Int64 m_PathID = 0;
	}
	*/

	/*
	public class LegacyFaceInfo
	{
		public string Name = "";
		public float PointSize = 0;
		public float Scale = 0;
		public Int32 CharacterCount = 0;
		public float LineHeight = 0;
		public float Baseline = 0;
		public float Ascender = 0;
		public float CapHeight = 0;
		public float Descender = 0;
		public float CenterLine = 0;
		public float SuperscriptOffset = 0;
		public float SubscriptOffset = 0;
		public float SubSize = 0;
		public float Underline = 0;
		public float UnderlineThickness = 0;
		public float strikethrough = 0;
		public float strikethroughThickness = 0;
		public float TabWidth = 0;
		public float Padding = 0;
		public float AtlasWidth = 0;
		public float AtlasHeight = 0;
	}
	*/

	/*
	public class TMP_FontFeatureTable
	{
		public List<Unknown> m_GlyphPairAdjustmentRecords = new List<Unknown>();
	}
	*/

	/*
	public class FontAssetCreationSettings
	{
		public string sourceFontFileName = "";
		public string sourceFontFileGUID = "";
		public int pointSizeSamplingMode = 0;
		public int pointSize = 0;
		public int padding = 0;
		public int packingMode = 0;
		public int atlasWidth = 0;
		public int atlasHeight = 0;
		public int characterSetSelectionMode = 0;
		public string characterSequence = "";
		public string referencedFontAssetGUID = "";
		public string referencedTextAssetGUID = "";
		public int fontStyle = 0;
		public float fontStyleModifier = 0.0f;
		public int renderMode = 0;
		public bool includeFontFeatures = false;
	}
	*/

	/*
	class TMP_FontAsset
	{
		// ???
	}
	*/

	/*
	public class TMP_FontWeightPair
	{
		public PPtrT<TMP_FontAsset> regularTypeface = new PPtrT<TMP_FontAsset>();
		public PPtrT<TMP_FontAsset> italicTypeface = new PPtrT<TMP_FontAsset>();
	}
	*/

	internal class FontStructure
	{
		//public PPtrT<GameObject> m_GameObject = new PPtrT<GameObject>();
		//public sbyte m_Enabled = 1;
		//public PPtrT<MonoScript> m_Script = new PPtrT<MonoScript>();
		//public string m_Name = "";
		//public Int32 hashCode = 0;
		//public PPtrT<Material> material = new PPtrT<Material>();
		//public Int32 materialHashCode = 0;
		//public string m_Version = "";
		//public string m_SourceFontFileGUID = "";
		//public PPtrT<Font> m_SourceFontFile = new PPtrT<Font>();
		//public Int32 m_AtlasPopulationMode = 0;
		//public FaceInfo m_FaceInfo = new FaceInfo();
		public List<Glyph> m_GlyphTable = new List<Glyph>();
		public List<TMPCharacter> m_CharacterTable = new List<TMPCharacter>();
		//public List<PPtrT<Texture2D>> m_AtlasTextures = new List<PPtrT<Texture2D>>();
		//public Int32 m_AtlasTextureIndex = 0;
		//public bool m_IsMultiAliasTextureEnabled = false;
		public List<GlyphRect> m_UsedGlyphRects = new List<GlyphRect>();
		public List<GlyphRect> m_FreeGlyphRects = new List<GlyphRect>();
		//public LegacyFaceInfo m_fontInfo = new LegacyFaceInfo();
		//public PPtrT<Texture2D> atlas = new PPtrT<Texture2D>();
		//public Int32 m_AtlasWidth = 0;
		//public Int32 m_AtlasHeight = 0;
		//public Int32 m_AtlasPadding = 0;
		//public Int32 m_AtlasRenderMode = 0;
		//public List<Unknown> m_glyphInfoList = new List<Unknown>();
		//public KerningTable m_KerningTable = new KerningTable();
		//public TMP_FontFeatureTable m_FontFeatureTable = new TMP_FontFeatureTable();
		//public List<Unknown> fallbackFontAssets = new List<Unknown>();
		//public List<Unknown> m_FallbackFontAssetTable = new List<Unknown>();
		//public FontAssetCreationSettings m_CreationSettings = new FontAssetCreationSettings();
		//public List<TMP_FontWeightPair> m_FontWeightTable = new List<TMP_FontWeightPair>();
		//public List<Unknown> fontWeights = new List<Unknown>();
		//public float normalStyle = 0.0f;
		//public float normalSpacingOffset = 0.0f;
		//public float boldStyle = 0.0f;
		//public float boldSpacing = 0.0f;
		//public sbyte italicStyle = 0;
		//public sbyte tabSize = 0;
	}
}
