using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

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
		public Dictionary<string, Trigger> Triggers = new Dictionary<string, Trigger>();
		/// <summary>
		/// 过滤器注册表，键为过滤器的UID，值为过滤器数据
		/// </summary>
		public Dictionary<string, Filter> Filters = new Dictionary<string, Filter>();
		/// <summary>
		/// 抽取器注册表，键为抽取器的UID，值为抽取器数据
		/// </summary>
		public Dictionary<string, Selector> Selectors = new Dictionary<string, Selector>();
		/// <summary>
		/// 生成器注册表，键为生成器的UID，值为生成器数据
		/// </summary>
		public Dictionary<string, Generator> Generators = new Dictionary<string, Generator>();
		/// <summary>
		/// 将另一个工厂数据实例的内容与本实例合并
		/// </summary>
		/// <returns>成功与否</returns>
		public bool Merge(FactoryData data)
		{
			foreach (string triggerKey in data.Triggers.Keys)
			{
				if (Triggers.ContainsKey(triggerKey))
				{
					Triggers[triggerKey] = data.Triggers[triggerKey];
					continue;
				}
				Triggers.Add(triggerKey, data.Triggers[triggerKey]);
			}
			foreach (string filterKey in data.Filters.Keys)
			{
				if (Filters.ContainsKey(filterKey))
				{
					Filters[filterKey] = data.Filters[filterKey];
					continue;
				}
				Filters.Add(filterKey, data.Filters[filterKey]);
			}
			foreach (string selectorKey in data.Selectors.Keys)
			{
				if (Selectors.ContainsKey(selectorKey))
				{
					Selectors[selectorKey] = data.Selectors[selectorKey];
					continue;
				}
				Selectors.Add(selectorKey, data.Selectors[selectorKey]);
			}
			foreach (string generatorKey in data.Generators.Keys)
			{
				if (Generators.ContainsKey(generatorKey))
				{
					Generators[generatorKey] = data.Generators[generatorKey];
					continue;
				}
				Generators.Add(generatorKey, data.Generators[generatorKey]);
			}
			return true;
		}
		/// <summary>
		/// 从Json文本解析到工厂数据
		/// </summary>
		/// <param name="jsonText">Json文本</param>
		/// <param name="dataParsed">已解析的数据</param>
		/// <returns>成功与否</returns>
		public static bool FromJson(string jsonText, out FactoryData dataParsed)
		{
			dataParsed = new FactoryData(); // 实例化用于承载解析好的数据的工厂管线数据实例
			if (jsonText == "") return false; // 如果Json文本为空
			JToken rootToken = JToken.Parse(jsonText); // 创建一个JToken持有根的数据
			if (!(rootToken is JObject rootObject)) return false; // 确认根是否是Json对象
			// 位置: {}
			if (rootObject.TryGetValue("triggers", out JToken? triggersObjectToken)) // 确认triggers键是否存在并尝试获取
			{
				// 位置: {}.triggers
				if (!(triggersObjectToken is JObject triggersObject)) return false; // 检查triggers键值对的类型
				foreach (JProperty triggerProperty in triggersObject.Properties()) // 解析每个触发器
				{
					// 位置: {}.triggers.<triggerUID>
					if (triggerProperty.Type != JTokenType.Object) return false; // 检查该JToken是否是Json对象
					JToken? typeToken = triggerProperty["type"];
					JToken? classNameToken = triggerProperty["class_name"];
					JToken? targetToken = triggerProperty["target"];
					if (typeToken == null || classNameToken == null || targetToken == null) return false;
					if (typeToken.Type != JTokenType.String || classNameToken.Type != JTokenType.String || targetToken.Type != JTokenType.String) return false;
					Trigger thisTriggerParsed = new Trigger
					{
						Type = Enum.Parse<TriggerType>(typeToken.Value<string>() ?? "Signal"),
						ClassName = classNameToken.Value<string>() ?? "",
						Target = targetToken.Value<string>() ?? ""
					};
					dataParsed.Triggers.TryAdd(triggerProperty.Name, thisTriggerParsed);
				}
			}
			if (rootObject.TryGetValue("filters", out JToken? filtersObjectToken)) // 确认filters键是否存在并尝试获取
			{
				if (!(filtersObjectToken is JObject filtersObject)) return false; // 检查filters键值对的类型
				foreach (string? filterUID in filtersObject.Properties()) // 解析每个过滤器
				{
					if (!(filterUID != null && filtersObject[filterUID] is JObject thisFilterObject)) return false; // 检查该JToken是否是Json对象
					if (!(thisFilterObject.TryGetValue("scribe_triggers", out JToken? scribeTriggersToken) && thisFilterObject.TryGetValue("commands", out JToken? commandsToken))) return false;
					if (scribeTriggersToken.Type != JTokenType.Array || commandsToken.Type != JTokenType.Array) return false;
					Filter thisFilterParsed = new Filter
					{
						ScribeTriggers = scribeTriggersToken.Values<string>().OfType<string>().ToArray(),
						Commands = commandsToken.Values<string>().OfType<string>().ToArray()
					};
					dataParsed.Filters.TryAdd(filterUID, thisFilterParsed);
				}
			}
			if (rootObject.TryGetValue("selectors", out JToken? selectorsToken)) // 确认selectors键是否存在并尝试获取
			{
				// 位置: {}.selectors
				if (!(selectorsToken is JObject selectorsObject)) return false; // 检查selectors键值对的类型
				foreach (string? selectorUID in selectorsObject.Properties()) // 解析每个抽取器，必须以Json对象的属性名
				{
					// 位置: {}.selectors.<selectorUID>
					if (!(selectorUID != null && selectorsObject[selectorUID] is JObject thisSelectorObject)) return false; // 检查该JToken是否是Json对象
					if (!(thisSelectorObject.TryGetValue("rolls", out JToken? rollsToken) && thisSelectorObject.TryGetValue("pool", out JToken? poolToken))) return false;
					if (rollsToken.Type != JTokenType.Integer || poolToken.Type != JTokenType.Array) return false;
					Selector thisSelectorParsed = new Selector
					{
						Rolls = rollsToken.Value<int>()
					};
					foreach (JToken poolObjectToken in poolToken.Children()) // 遍历所有抽取池对象的token
					{
						if (!(poolObjectToken is JObject poolObjectObject)) return false;
						
					}
				}
			}
			return true;
		}
	}
}