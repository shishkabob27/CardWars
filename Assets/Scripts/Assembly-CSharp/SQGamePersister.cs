using System.Timers;

public class SQGamePersister
{
	private Timer timer;

	public SQGamePersister(Session session)
	{
	}

	public void Start()
	{
		Stop();
		timer = new Timer(SQSettings.SAVE_INTERVAL * 1000);
		timer.Elapsed += TimerTick;
		timer.Start();
	}

	public void Stop()
	{
		if (timer != null)
		{
			timer.Stop();
			timer = null;
		}
	}

	public void TimerTick(object sender, ElapsedEventArgs e)
	{
		SaveGame();
	}

	public void SaveGame()
	{
	}
}
