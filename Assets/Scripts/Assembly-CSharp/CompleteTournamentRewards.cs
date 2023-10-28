using Multiplayer;

public class CompleteTournamentRewards : AsyncData<SuccessCallback>
{
	public void CompleteTournamenentReward(int aTournamentID)
	{
		if (Asyncdata.processed)
		{
			global::Multiplayer.Multiplayer.CompleteTournamentReward(SessionManager.GetInstance().theSession, aTournamentID, SuccessCallback);
		}
	}

	public void SuccessCallback(ResponseFlag flag)
	{
		SuccessCallback a_MP_Data = null;
		Asyncdata.Set(flag, a_MP_Data);
	}

	private void Update()
	{
		if (!Asyncdata.processed)
		{
			Asyncdata.processed = true;
			if (Asyncdata.flag != 0)
			{
			}
		}
	}
}
