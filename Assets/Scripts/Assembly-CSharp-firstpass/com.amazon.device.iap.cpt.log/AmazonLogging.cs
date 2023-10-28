using System;

namespace com.amazon.device.iap.cpt.log
{
	public class AmazonLogging
	{
		public enum AmazonLoggingLevel
		{
			Silent,
			Critical,
			ErrorsAsExceptions,
			Errors,
			Warnings,
			Verbose
		}

		public enum SDKLoggingLevel
		{
			LogOff,
			LogCritical,
			LogError,
			LogWarning
		}

		private const string errorMessage = "{0} error: {1}";

		private const string warningMessage = "{0} warning: {1}";

		private const string logMessage = "{0}: {1}";

		public static void LogError(AmazonLoggingLevel reportLevel, string service, string message)
		{
			if (reportLevel != 0)
			{
				string message2 = string.Format("{0} error: {1}", service, message);
				switch (reportLevel)
				{
				case AmazonLoggingLevel.ErrorsAsExceptions:
					throw new Exception(message2);
				case AmazonLoggingLevel.Critical:
				case AmazonLoggingLevel.Errors:
				case AmazonLoggingLevel.Warnings:
				case AmazonLoggingLevel.Verbose:
					break;
				}
			}
		}

		public static void LogWarning(AmazonLoggingLevel reportLevel, string service, string message)
		{
			switch (reportLevel)
			{
			case AmazonLoggingLevel.Silent:
			case AmazonLoggingLevel.Critical:
			case AmazonLoggingLevel.ErrorsAsExceptions:
			case AmazonLoggingLevel.Errors:
				break;
			case AmazonLoggingLevel.Warnings:
			case AmazonLoggingLevel.Verbose:
				break;
			}
		}

		public static void Log(AmazonLoggingLevel reportLevel, string service, string message)
		{
			if (reportLevel == AmazonLoggingLevel.Verbose)
			{
			}
		}

		public static SDKLoggingLevel pluginToSDKLoggingLevel(AmazonLoggingLevel pluginLoggingLevel)
		{
			switch (pluginLoggingLevel)
			{
			case AmazonLoggingLevel.Silent:
				return SDKLoggingLevel.LogOff;
			case AmazonLoggingLevel.Critical:
				return SDKLoggingLevel.LogCritical;
			case AmazonLoggingLevel.ErrorsAsExceptions:
			case AmazonLoggingLevel.Errors:
				return SDKLoggingLevel.LogError;
			case AmazonLoggingLevel.Warnings:
			case AmazonLoggingLevel.Verbose:
				return SDKLoggingLevel.LogWarning;
			default:
				return SDKLoggingLevel.LogWarning;
			}
		}
	}
}
