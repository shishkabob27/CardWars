using System;
using System.Reflection;

namespace JsonFx
{
	public sealed class About
	{
		public static readonly About Fx = new About(typeof(About).Assembly);

		public readonly Version Version;

		public readonly string FullName;

		public readonly string Name;

		public readonly string Configuration;

		public readonly string Copyright;

		public readonly string Title;

		public readonly string Description;

		public readonly string Company;

		public About(Assembly assembly)
		{
			AssemblyName name = assembly.GetName();
			FullName = assembly.FullName;
			Version = name.Version;
			Name = name.Name;
			AssemblyCopyrightAttribute assemblyCopyrightAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
			Copyright = ((assemblyCopyrightAttribute == null) ? string.Empty : assemblyCopyrightAttribute.Copyright);
			AssemblyDescriptionAttribute assemblyDescriptionAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;
			Description = ((assemblyDescriptionAttribute == null) ? string.Empty : assemblyDescriptionAttribute.Description);
			AssemblyTitleAttribute assemblyTitleAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
			Title = ((assemblyTitleAttribute == null) ? string.Empty : assemblyTitleAttribute.Title);
			AssemblyCompanyAttribute assemblyCompanyAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute;
			Company = ((assemblyCompanyAttribute == null) ? string.Empty : assemblyCompanyAttribute.Company);
			AssemblyConfigurationAttribute assemblyConfigurationAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyConfigurationAttribute)) as AssemblyConfigurationAttribute;
			Configuration = ((assemblyConfigurationAttribute == null) ? string.Empty : assemblyConfigurationAttribute.Configuration);
		}
	}
}
