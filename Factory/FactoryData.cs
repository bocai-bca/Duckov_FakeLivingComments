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
					if (triggerProperty.Type != JTokenType.Object) return false; // 检查该JProperty是否是Json对象
					JToken? typeToken = triggerProperty["type"];
					JToken? classNameToken = triggerProperty["class_name"];
					JToken? targetToken = triggerProperty["target"];
					if (typeToken == null || classNameToken == null || targetToken == null) return false; // 键值不存在检查
					if (typeToken.Type != JTokenType.String || classNameToken.Type != JTokenType.String || targetToken.Type != JTokenType.String) return false; // 值类型错误检查
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
				// 位置: {}.filters
				if (!(filtersObjectToken is JObject filtersObject)) return false; // 检查filters键值对的类型
				foreach (JProperty filterProperty in filtersObject.Properties()) // 解析每个过滤器
				{
					// 位置: {}.filters.<filterUID>
					if (filterProperty.Type != JTokenType.Object) return false; // 检查该JProperty是否是Json对象
					JToken? scribeTriggersToken = filterProperty["scribe_triggers"];
					JToken? commandsToken = filterProperty["commands"];
					if (scribeTriggersToken == null || commandsToken == null) return false; // 键值不存在检查
					if (scribeTriggersToken.Type != JTokenType.Array || commandsToken.Type != JTokenType.Array) return false; // 值类型错误检查
					Filter thisFilterParsed = new Filter
					{
						ScribeTriggers = scribeTriggersToken.Values<string>().OfType<string>().ToArray(),
						Commands = commandsToken.Values<string>().OfType<string>().ToArray()
					};
					dataParsed.Filters.TryAdd(filterProperty.Name, thisFilterParsed);
				}
			}
			if (rootObject.TryGetValue("selectors", out JToken? selectorsObjectToken)) // 确认selectors键是否存在并尝试获取
			{
				// 位置: {}.selectors
				if (!(selectorsObjectToken is JObject selectorsObject)) return false; // 检查selectors键值对的类型
				foreach (JProperty selectorProperty in selectorsObject.Properties()) // 解析每个抽取器
				{
					// 位置: {}.selectors.<selectorUID>
					if (selectorProperty.Type != JTokenType.Object) return false; // 检查该JProperty是否是Json对象
					JToken? rollsToken = selectorProperty["rolls"];
					JToken? poolToken = selectorProperty["pool"];
					if (rollsToken == null || poolToken == null) return false; // 键值不存在检查
					if (rollsToken.Type != JTokenType.Integer || poolToken.Type != JTokenType.Array) return false; // 值类型错误检查
					Selector thisSelectorParsed = new Selector
					{
						Rolls = rollsToken.Value<int>()
					};
					List<SelectorObject> selectorObjectsParsed = new List<SelectorObject>();
					foreach (JToken poolObjectToken in poolToken.Children<JToken>())
					{
						// 位置: {}.selectors.<selectorUID>.pool[n]
						if (poolObjectToken.Type != JTokenType.Object) return false; // 检查池对象的类型
						JToken? weightToken = poolObjectToken["weight"];
						JToken? generatorsUidsToken = poolObjectToken["generators"];
						if (weightToken == null || generatorsUidsToken == null) return false; // 键值不存在检查
						if (weightToken.Type != JTokenType.Integer || generatorsUidsToken.Type != JTokenType.Array) return false; // 值类型错误检查
						SelectorObject selectorObjectParsed = new SelectorObject
						{
							Weight = weightToken.Value<int>(),
							GeneratorUIDs = generatorsUidsToken.Values<string>().OfType<string>().ToArray()
						};
						selectorObjectsParsed.Add(selectorObjectParsed);
					}
					thisSelectorParsed.Pool = selectorObjectsParsed.ToArray();
					dataParsed.Selectors.TryAdd(selectorProperty.Name, thisSelectorParsed);
				}
			}
			if (rootObject.TryGetValue("generators", out JToken? generatorsToken)) // 确认generators键是否存在并尝试获取
			{
				// 位置: {}.generators
				if (!(generatorsToken is JObject generatorsObject)) return false; // 检查generators键值对的类型
				foreach (JProperty generatorProperty in generatorsObject.Properties()) // 解析每个生成器
				{
					// 位置: {}.generators.<generatorUID>
					if (generatorProperty.Type != JTokenType.Object) return false; // 检查该JProperty是否是Json对象
					JToken? typeToken = generatorProperty["type"];
					JToken? sourceToken = generatorProperty["source"];
					JObject? modifiersToken = generatorProperty["modifiers"] as JObject;
					JArray? delayToken = generatorProperty["delay"] as JArray;
					if (typeToken == null || sourceToken == null || modifiersToken == null || delayToken == null) return false; // 键值不存在检查
					if (typeToken.Type != JTokenType.String || sourceToken.Type != JTokenType.String) return false; // 值类型错误检查
					Generator thisGeneratorParsed = new Generator
					{
						Type = Enum.Parse<GeneratorType>(typeToken.Value<string>() ?? "Normal"),
						Source = sourceToken.Value<string>() ?? "",
						Modifier = new GeneratorModifier(),
						Delay = new float[2]
					};
					foreach (JProperty modifierProperty in modifiersToken.Properties())
					{
						// 位置: {}.generators.<generatorUID>.modifiers.<modifierName>
						switch (modifierProperty.Name) // 按名称遍历所有触发器
						{
							case "repeat":
								if (modifierProperty.Type != JTokenType.Array) return false; // 值类型错误检查
								if (modifierProperty.Count != 2) return false; // 值数组元素数量检查
								if (!(modifierProperty[0] is { Type: JTokenType.Integer } repeatMinToken)) return false; // 值数组元素类型错误检查
								if (!(modifierProperty[1] is { Type: JTokenType.Integer } repeatMaxToken)) return false; // 值数组元素类型错误检查
								thisGeneratorParsed.Modifier.Repeat[0] = repeatMinToken.Value<int>();
								thisGeneratorParsed.Modifier.Repeat[1] = repeatMaxToken.Value<int>();
								break;
							case "misspells":
								if (modifierProperty.Type != JTokenType.Array) return false; // 值类型错误检查
								List<Modifier_Misspell> misspellsParsed = new List<Modifier_Misspell>();
								foreach (JToken misspellObjectToken in modifierProperty.Children<JToken>())
								{
									if (misspellObjectToken.Type != JTokenType.Object) return false; // 值类型错误检查
									JToken? fromToken = misspellObjectToken["from"];
									JToken? toToken = misspellObjectToken["to"];
									JToken? minChangeRateToken = misspellObjectToken["minChangeRate"];
									JToken? maxChangeRateToken = misspellObjectToken["maxChangeRate"];
									if (fromToken == null || toToken == null || minChangeRateToken == null || maxChangeRateToken == null) return false; // 键值不存在检查
									if (fromToken.Type != JTokenType.String || toToken.Type != JTokenType.String || minChangeRateToken.Type != JTokenType.Float || maxChangeRateToken.Type != JTokenType.Float) return false; // 值类型错误检查
									misspellsParsed.Add(new Modifier_Misspell(fromToken.Value<string>() ?? "", toToken.Value<string>() ?? "", minChangeRateToken.Value<float>(), maxChangeRateToken.Value<float>()));
								}
								thisGeneratorParsed.Modifier.Misspells = misspellsParsed.ToArray();
								break;
						}
					}
					if (delayToken.Count != 2) return false; // 值数组元素数量检查
					if (!(delayToken[0] is { Type: JTokenType.Float } delayMinToken)) return false; // 值数组元素类型错误检查
					if (!(delayToken[1] is { Type: JTokenType.Float } delayMaxToken)) return false; // 值数组元素类型错误检查
					thisGeneratorParsed.Delay[0] = delayMinToken.Value<float>();
					thisGeneratorParsed.Delay[1] = delayMaxToken.Value<float>();
					dataParsed.Generators.TryAdd(generatorProperty.Name, thisGeneratorParsed);
				}
			}
			return true;
		}
	}
}