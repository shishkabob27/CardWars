using System;
using System.Globalization;
using System.Reflection;

namespace Facebook
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class FBBuildVersionAttribute : Attribute
	{
		private DateTime buildDate;

		private string buildHash;

		private string buildVersion;

		private string sdkVersion;

		public string SdkVersion
		{
			get
			{
				return sdkVersion;
			}
		}

		public string BuildVersion
		{
			get
			{
				return buildVersion;
			}
		}

		public FBBuildVersionAttribute(string sdkVersion, string buildVersion)
		{
			this.buildVersion = buildVersion;
			string[] array = buildVersion.Split('.');
			buildDate = DateTime.ParseExact(array[0], "yyMMdd", CultureInfo.InvariantCulture);
			buildHash = array[1];
			this.sdkVersion = sdkVersion;
		}

		public override string ToString()
		{
			return buildVersion;
		}

		public static FBBuildVersionAttribute GetVersionAttributeOfType(Type type)
		{
			FBBuildVersionAttribute[] attributes = getAttributes(type);
			int num = 0;
			if (num < attributes.Length)
			{
				return attributes[num];
			}
			return null;
		}

		private static FBBuildVersionAttribute[] getAttributes(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Assembly assembly = type.Assembly;
			return (FBBuildVersionAttribute[])assembly.GetCustomAttributes(typeof(FBBuildVersionAttribute), false);
		}
	}
}
