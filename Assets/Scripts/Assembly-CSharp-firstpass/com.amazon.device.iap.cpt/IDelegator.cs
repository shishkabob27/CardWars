using System.Collections.Generic;

namespace com.amazon.device.iap.cpt
{
	public interface IDelegator
	{
		void ExecuteSuccess();

		void ExecuteSuccess(Dictionary<string, object> objDict);

		void ExecuteError(AmazonException e);
	}
}
