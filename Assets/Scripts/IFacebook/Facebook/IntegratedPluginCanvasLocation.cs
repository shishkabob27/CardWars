using System;
using UnityEngine;

namespace Facebook
{
	public sealed class IntegratedPluginCanvasLocation : ScriptableObject
	{
		private const string sdkVersion = "sdk_6.2";

		private const string relativeDllUrl = "/rsrc/unity/lib/sdk_6.2/{0}.dll";

		private const string prodDllUrl = "https://integrated-plugin-canvas-rsrc.fbsbx.com/rsrc/unity/lib/sdk_6.2/{0}.dll";

		private const string relativeFbSkinUrl = "/rsrc/unity/textures/sdk_6.2/FBSkin.unity3d";

		private const string prodFbSkinUrl = "https://integrated-plugin-canvas-rsrc.fbsbx.com/rsrc/unity/textures/sdk_6.2/FBSkin.unity3d";

		private const string relativeKeyUrl = "/rsrc/unity/key/sdk_6.2/AuthToken.unityhash";

		private const string prodKeyUrl = "https://integrated-plugin-canvas-rsrc.fbsbx.com/rsrc/unity/key/sdk_6.2/AuthToken.unityhash";

		private const string graphUrl = "https://graph.facebook.com";

		private const string connectFacebookUrl = "https://connect.facebook.net";

		public const bool isConst = true;

		private static IntegratedPluginCanvasLocation instance;

		public static bool IsConst
		{
			get
			{
				return true;
			}
		}

		public static IntegratedPluginCanvasLocation Instance
		{
			get
			{
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<IntegratedPluginCanvasLocation>();
				}
				return instance;
			}
			set
			{
			}
		}

		public static int Location
		{
			get
			{
				return 0;
			}
			set
			{
				throw new NotSupportedException("It is illegal to call this in release builds");
			}
		}

		public static string DllUrl
		{
			get
			{
				if (Application.isEditor)
				{
					return "https://integrated-plugin-canvas-rsrc.fbsbx.com/rsrc/unity/lib/sdk_6.2/{0}.dll";
				}
				return "/rsrc/unity/lib/sdk_6.2/{0}.dll";
			}
		}

		public static string FbSkinUrl
		{
			get
			{
				if (Application.isEditor)
				{
					return "https://integrated-plugin-canvas-rsrc.fbsbx.com/rsrc/unity/textures/sdk_6.2/FBSkin.unity3d";
				}
				return "/rsrc/unity/textures/sdk_6.2/FBSkin.unity3d";
			}
		}

		public static string KeyUrl
		{
			get
			{
				if (Application.isEditor)
				{
					return "https://integrated-plugin-canvas-rsrc.fbsbx.com/rsrc/unity/key/sdk_6.2/AuthToken.unityhash";
				}
				return "/rsrc/unity/key/sdk_6.2/AuthToken.unityhash";
			}
		}

		public static string GraphUrl
		{
			get
			{
				return "https://graph.facebook.com";
			}
		}

		public static string ConnectFacebookUrl
		{
			get
			{
				return "https://connect.facebook.net";
			}
		}
	}
}
