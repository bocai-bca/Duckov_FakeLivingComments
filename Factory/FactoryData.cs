using System.Collections.Generic;


namespace FakeLivingComments.Factory
{
	/// <summary>
	/// 工厂数据类，类型中的结构也代表着flc_data.json的结构
	/// </summary>
	public class FactoryData
	{
		/// <summary>
		/// 触发器注册表，键为触发器的UID，值为触发器数据
		/// </summary>
		public Dictionary<string, Trigger>? triggers;
		public Dictionary<string, Filter>? filters;
	}
}