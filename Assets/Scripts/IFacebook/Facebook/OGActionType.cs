namespace Facebook
{
	public class OGActionType
	{
		public static readonly OGActionType Send = new OGActionType
		{
			actionTypeValue = "send"
		};

		public static readonly OGActionType AskFor = new OGActionType
		{
			actionTypeValue = "askfor"
		};

		private string actionTypeValue;

		public override string ToString()
		{
			return actionTypeValue;
		}
	}
}
