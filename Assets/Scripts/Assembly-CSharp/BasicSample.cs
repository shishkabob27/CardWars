public class BasicSample : SampleBase
{
	public string helpText = "Help text here";

	public string statusText = string.Empty;

	protected override string GetHelpText()
	{
		return helpText;
	}

	protected override void Start()
	{
		base.Start();
		base.UI.StatusText = statusText;
	}
}
