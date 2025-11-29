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
		public Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();
		/// <summary>
		/// 过滤器注册表，键为过滤器的UID，值为过滤器数据
		/// </summary>
		public Dictionary<string, Filter> filters = new Dictionary<string, Filter>();
		/// <summary>
		/// 抽取器注册表，键为抽取器的UID，值为抽取器数据
		/// </summary>
		public Dictionary<string, Selector> selectors = new Dictionary<string, Selector>();
		/// <summary>
		/// 生成器注册表，键为生成器的UID，值为生成器数据
		/// </summary>
		public Dictionary<string, Generator> generators = new Dictionary<string, Generator>();
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool Merge(FactoryData data)
		{
			foreach (string triggerKey in data.triggers.Keys)
			{
				if (triggers.ContainsKey(triggerKey))
				{
					triggers[triggerKey] = data.triggers[triggerKey];
				}
			}
			foreach (string filterKey in data.filters.Keys)
			{
				if (filters.ContainsKey(filterKey))
				{
					filters[filterKey] = data.filters[filterKey];
				}
			}
			foreach (string selectorKey in data.selectors.Keys)
			{
				if (selectors.ContainsKey(selectorKey))
				{
					selectors[selectorKey] = data.selectors[selectorKey];
				}
			}
			foreach (string generatorKey in data.generators.Keys)
			{
				if (generators.ContainsKey(generatorKey))
				{
					generators[generatorKey] = data.generators[generatorKey];
				}
			}
			return true;
		}
	}
}