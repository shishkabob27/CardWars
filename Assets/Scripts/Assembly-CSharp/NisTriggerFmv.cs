public class NisTriggerFmv : NisComponent
{
	public NisTriggerFmv() : base(default(bool))
	{
	}

	public float playDelaySecs;
	public MoviePlayerController target;
	public bool stopMusicOnStart;
	public bool restoreMusicOnStop;
}
