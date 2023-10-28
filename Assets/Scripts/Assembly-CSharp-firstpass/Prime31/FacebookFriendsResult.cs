using System.Collections.Generic;

namespace Prime31
{
	public class FacebookFriendsResult : FacebookBaseDTO
	{
		public List<FacebookFriend> data = new List<FacebookFriend>();

		public FacebookResultsPaging paging;
	}
}
