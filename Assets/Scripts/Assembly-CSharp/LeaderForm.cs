public class LeaderForm
{
	public const int HP_PER_LEVEL = 5;

	public string ID;

	public string Name;

	public string CharacterID;

	public string IconAtlas;

	public string SpriteName;

	public string SpriteNameHero;

	public string FrameSpriteName;

	public string Desc;

	public string LvUpSchemeID;

	public int BaseHP;

	public int MaxXP;

	public string ScriptName;

	public int Cooldown;

	public Faction? forFaction;

	public LandscapeType? forLandscape;

	public CardType? forCardType;

	public int BaseVal1;

	public int BaseVal2;

	public float? Ring_P1_HitAreaRange;

	public float? Ring_P1_CritAreaRange;

	public float? Ring_P2_HitAreaRange;

	public float? Ring_P2_CritAreaRange;

	public string Ring_P1_HitColor;

	public string Ring_P1_CritColor;

	public string Ring_P2_HitColor;

	public string Ring_P2_CritColor;

	public float? TimeFor1SpinMin;

	public float? TimeFor1SpinMax;

	public string Ring_BGSpriteAtlas;

	public string Ring_P1_BGSprite;

	public string Ring_P1_BGColor;

	public string Ring_P2_BGSprite;

	public string Ring_P2_BGColor;

	public string Ring_P1_BarSprite;

	public string Ring_P2_BarSprite;

	public float? CritDamageMod;

	public bool FCWorld;

	public int StartLevel;
}
