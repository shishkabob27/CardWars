using System.Collections.Generic;

namespace com.amazon.device.iap.cpt
{
	public sealed class GetProductDataResponseDelegator : IDelegator
	{
		public readonly GetProductDataResponseDelegate responseDelegate;

		public GetProductDataResponseDelegator(GetProductDataResponseDelegate responseDelegate)
		{
			this.responseDelegate = responseDelegate;
		}

		public void ExecuteSuccess()
		{
		}

		public void ExecuteSuccess(Dictionary<string, object> objectDictionary)
		{
			responseDelegate(GetProductDataResponse.CreateFromDictionary(objectDictionary));
		}

		public void ExecuteError(AmazonException e)
		{
		}
	}
}
