namespace FakeLivingComments.Factory
{
	/// <summary>
	/// 触发器数据，类型中的结构也代表着flc_data.json中触发器的结构
	/// </summary>
	public class Trigger : NodeBase
	{
		/// <summary>
		/// 该触发器的类型
		/// </summary>
		public TriggerType type;
		/// <summary>
		/// 该触发器的订阅目标类，需填写含命名空间的类型名称
		/// </summary>
		public string? class_name;
		/// <summary>
		/// 该触发器的订阅目标，目标的类型受type决定
		/// </summary>
		public string? target;
	}
}