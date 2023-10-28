using com.amazon.device.iap.cpt.log;

namespace com.amazon.device.iap.cpt
{
	public class AmazonLogger
	{
		private readonly string tag;

		public AmazonLogger(string tag)
		{
			this.tag = tag;
		}

		public void Debug(string msg)
		{
			AmazonLogging.Log(AmazonLogging.AmazonLoggingLevel.Verbose, tag, msg);
		}

		public string getTag()
		{
			return tag;
		}
	}
}
