namespace Facebook
{
	public class AndroidFacebookLoader : FB.CompiledFacebookLoader
	{
		protected override IFacebook fb
		{
			get
			{
				return FBComponentFactory.GetComponent<AndroidFacebook>();
			}
		}
	}
}
