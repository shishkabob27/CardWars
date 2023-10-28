namespace Facebook
{
	public class EditorFacebookLoader : FB.CompiledFacebookLoader
	{
		protected override IFacebook fb
		{
			get
			{
				return FBComponentFactory.GetComponent<EditorFacebook>();
			}
		}
	}
}
