

using System.Collections.Generic;

namespace FakeLivingComments
{
	public static class FakeLivingComments
	{
		public const string MOD_NAME = "FakeLivingComments";
		public const string MOD_CONFIG_DIR = "BCASoft.FakeLivingComments";
		public static List<RealtimeComment> RealtimeComments = new List<RealtimeComment>();
		public static void Init()
		{
			
		}
		public static void Update()
		{
			
		}
		public static bool SendANewComment(string text)
		{
			if (RealtimeComments.Count >= Config.ConfigHolder.ConfigData.CommentMaxCount)
			{
				return false;
			}
			RealtimeComments.Add(new RealtimeComment(text));
			return true;
		}
	}
}
