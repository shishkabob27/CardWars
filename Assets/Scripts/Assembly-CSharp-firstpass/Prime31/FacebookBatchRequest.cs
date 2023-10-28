using System.Collections.Generic;
using System.Text;

namespace Prime31
{
	public class FacebookBatchRequest
	{
		public Dictionary<string, string> _parameters = new Dictionary<string, string>();

		private Dictionary<string, object> _requestDict = new Dictionary<string, object>();

		public FacebookBatchRequest(string relativeUrl, string method)
		{
			_requestDict["method"] = method.ToUpper();
			_requestDict["relative_url"] = relativeUrl;
		}

		public void addParameter(string key, string value)
		{
			_parameters[key] = value;
		}

		public Dictionary<string, object> requestDictionary()
		{
			if (_parameters.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<string, string> parameter in _parameters)
				{
					stringBuilder.AppendFormat("{0}={1}&", parameter.Key, parameter.Value);
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				_requestDict["body"] = stringBuilder.ToString();
			}
			return _requestDict;
		}
	}
}
