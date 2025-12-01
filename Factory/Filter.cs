namespace FakeLivingComments.Factory
{
	/// <summary>
	/// 过滤器数据，类型中的结构也代表着flc_data.json中过滤器的结构
	/// </summary>
	public class Filter : NodeBase
	{
		/// <summary>
		/// 该过滤器订阅的触发器
		/// </summary>
		public string[] scribe_triggers = {};
		/// <summary>
		/// 该过滤器的命令列表
		/// </summary>
		public string[] commands = {};
	}
}