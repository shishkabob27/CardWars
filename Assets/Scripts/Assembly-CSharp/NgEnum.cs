public class NgEnum
{
	public enum AXIS
	{
		X,
		Y,
		Z
	}

	public enum TRANSFORM
	{
		POSITION,
		ROTATION,
		SCALE
	}

	public enum PREFAB_TYPE
	{
		All,
		ParticleSystem,
		LegacyParticle,
		NcSpriteFactory
	}

	public static string[] m_TextureSizeStrings = new string[8] { "32", "64", "128", "256", "512", "1024", "2048", "4096" };

	public static int[] m_TextureSizeIntters = new int[8] { 32, 64, 128, 256, 512, 1024, 2048, 4096 };
}
