namespace Facebook
{
	public class IOSFacebookLoader : FB.CompiledFacebookLoader
	{
		protected override IFacebook fb
		{
			get
			{
				return FBComponentFactory.GetComponent<IOSFacebook>();
			}
		}
	}
}
