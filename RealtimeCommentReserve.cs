namespace FakeLivingComments
{
	public struct RealtimeCommentReserve
	{
		public string Text;
		public float SendTime;
		public RealtimeCommentReserve(string text, float sendTime)
		{
			Text = text;
			SendTime = sendTime;
		}
	}
}