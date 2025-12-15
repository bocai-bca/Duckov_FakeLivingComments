using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
			if (jsonText == "") // 如果Json文本为空
			{
				Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: Json文本为空");
				return false;
			}
			JToken rootToken = JToken.Parse(jsonText); // 创建一个JToken持有根的数据
			if (!(rootToken is JObject rootObject)) // 确认根是否是Json对象
			{
				Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 根的类型错误，需要为对象，Json路径={}");
				return false;
			}
			// 位置: {}
			if (rootObject.TryGetValue("triggers", out JToken? triggersObjectToken)) // 确认triggers键是否存在并尝试获取
			{
				// 位置: {}.triggers
				if (!(triggersObjectToken is JObject triggersObject)) // 检查triggers键值对的类型
				{
					Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.triggers");
					return false;
				}
				foreach (JProperty triggerProperty in triggersObject.Properties()) // 解析每个触发器
				{
					// 位置: {}.triggers.<triggerUID>
					if (triggerProperty.Value.Type != JTokenType.Object) // 检查该JProperty是否是Json对象
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.triggers." + triggerProperty.Name);
						return false;
					}
					JToken? typeToken = triggerProperty.Value["type"];
					JToken? classNameToken = triggerProperty.Value["class_name"];
					JToken? targetToken = triggerProperty.Value["target"];
					if (typeToken == null || classNameToken == null || targetToken == null) // 键值不存在检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内缺少必要键，Json路径={}.triggers." + triggerProperty.Name);
						return false;
					}
					if (typeToken.Type != JTokenType.String || classNameToken.Type != JTokenType.String || targetToken.Type != JTokenType.String) // 值类型错误检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内某叶子键的值类型错误，Json路径={}.triggers." + triggerProperty.Name);
						return false;
					}
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
				if (!(filtersObjectToken is JObject filtersObject)) // 检查filters键值对的类型
				{
					Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.filters");
					return false;
				}
				foreach (JProperty filterProperty in filtersObject.Properties()) // 解析每个过滤器
				{
					// 位置: {}.filters.<filterUID>
					if (filterProperty.Value.Type != JTokenType.Object) // 检查该JProperty是否是Json对象
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.filters." + filterProperty.Name);
						return false;
					}
					JToken? scribeTriggersToken = filterProperty.Value["scribe_triggers"];
					JToken? commandsToken = filterProperty.Value["commands"];
					if (scribeTriggersToken == null || commandsToken == null) // 键值不存在检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内缺少必要键，Json路径={}.filters." + filterProperty.Name);
						return false;
					}
					if (scribeTriggersToken.Type != JTokenType.Array || commandsToken.Type != JTokenType.Array) // 值类型错误检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内某叶子键的值类型错误，Json路径={}.filters." + filterProperty.Name);
						return false;
					}
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
				if (!(selectorsObjectToken is JObject selectorsObject)) // 检查selectors键值对的类型
				{
					Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.selectors");
					return false;
				}
				foreach (JProperty selectorProperty in selectorsObject.Properties()) // 解析每个抽取器
				{
					// 位置: {}.selectors.<selectorUID>
					if (selectorProperty.Value.Type != JTokenType.Object) // 检查该JProperty是否是Json对象
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.selectors." + selectorProperty.Name);
						return false;
					}
					JToken? rollsToken = selectorProperty.Value["rolls"];
					JToken? poolToken = selectorProperty.Value["pool"];
					if (rollsToken == null || poolToken == null) // 键值不存在检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内缺少必要键，Json路径={}.selectors." + selectorProperty.Name);
						return false;
					}
					if (rollsToken.Type != JTokenType.Integer || poolToken.Type != JTokenType.Array) // 值类型错误检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内某叶子键的值类型错误，Json路径={}.selectors." + selectorProperty.Name);
						return false;
					}
					Selector thisSelectorParsed = new Selector
					{
						Rolls = rollsToken.Value<int>()
					};
					List<SelectorObject> selectorObjectsParsed = new List<SelectorObject>();
					foreach (JToken poolObjectToken in poolToken.Children<JToken>())
					{
						// 位置: {}.selectors.<selectorUID>.pool[n]
						if (poolObjectToken.Type != JTokenType.Object) // 检查池对象的类型
						{
							Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.selectors." + selectorProperty.Name + ".pool[n]");
							return false;
						}
						JToken? weightToken = poolObjectToken["weight"];
						JToken? generatorsUidsToken = poolObjectToken["generators"];
						if (weightToken == null || generatorsUidsToken == null) // 键值不存在检查
						{
							Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内缺少必要键，Json路径={}.selectors." + selectorProperty.Name + "pool[n]");
							return false;
						}
						if (weightToken.Type != JTokenType.Integer || generatorsUidsToken.Type != JTokenType.Array) // 值类型错误检查
						{
							Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内某叶子键的值类型错误，Json路径={}.selectors." + selectorProperty.Name + "pool[n]");
							return false;
						}
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
				if (!(generatorsToken is JObject generatorsObject)) // 检查generators键值对的类型
				{
					Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.generators");
					return false;
				}
				foreach (JProperty generatorProperty in generatorsObject.Properties()) // 解析每个生成器
				{
					// 位置: {}.generators.<generatorUID>
					if (generatorProperty.Value.Type != JTokenType.Object) // 检查该JProperty是否是Json对象
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.generators." + generatorProperty.Name);
						return false;
					}
					JToken? typeToken = generatorProperty.Value["type"];
					JToken? sourceToken = generatorProperty.Value["source"];
					JObject? modifiersToken = generatorProperty.Value["modifiers"] as JObject;
					JArray? delayToken = generatorProperty.Value["delay"] as JArray;
					if (typeToken == null || sourceToken == null || modifiersToken == null || delayToken == null) // 键值不存在检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内缺少必要键，Json路径={}.generators." + generatorProperty.Name);
						return false;
					}
					if (typeToken.Type != JTokenType.String || sourceToken.Type != JTokenType.String) // 值类型错误检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内某叶子键的值类型错误，Json路径={}.generators." + generatorProperty.Name);
						return false;
					}
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
								if (modifierProperty.Value.Type != JTokenType.Array) // 值类型错误检查
								{
									Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为数组，Json路径={}.generators." + generatorProperty.Name + ".modifiers.repeat");
									return false;
								}
								if (!(modifierProperty.Value is JArray repeatArguments))
								{
									Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的解析类型转换异常，JToken=>JArray，Json路径={}.generators." + generatorProperty.Name + ".modifiers.repeat");
									return false;
								}
								if (repeatArguments.Count != 2) // 值数组元素数量检查
								{
									Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 数组元素数量不符合要求，需要为2，Json路径={}.generators." + generatorProperty.Name + ".modifiers.repeat");
									return false;
								}
								if (!(repeatArguments[0] is { Type: JTokenType.Integer } repeatMinToken)) // 值数组元素类型错误检查
								{
									Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为整型，Json路径={}.generators." + generatorProperty.Name + ".modifiers.repeat[0]");
									return false;
								}
								if (!(repeatArguments[1] is { Type: JTokenType.Integer } repeatMaxToken)) // 值数组元素类型错误检查
								{
									Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为整型，Json路径={}.generators." + generatorProperty.Name + ".modifiers.repeat[1]");
									return false;
								}
								thisGeneratorParsed.Modifier.Repeat[0] = repeatMinToken.Value<int>();
								thisGeneratorParsed.Modifier.Repeat[1] = repeatMaxToken.Value<int>();
								break;
							case "misspells":
								if (modifierProperty.Value.Type != JTokenType.Array) // 值类型错误检查
								{
									Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为数组，Json路径={}.generators." + generatorProperty.Name + ".modifiers.misspells");
									return false;
								}
								List<Modifier_Misspell> misspellsParsed = new List<Modifier_Misspell>();
								foreach (JToken misspellObjectToken in modifierProperty.Value.Children<JToken>())
								{
									// 位置: {}.generators.<generatorUID>.modifiers.<modifierName>.misspells[n]
									if (misspellObjectToken.Type != JTokenType.Object) // 值类型错误检查
									{
										Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为对象，Json路径={}.generators." + generatorProperty.Name + ".modifiers.misspells[n]");
										return false;
									}
									JToken? fromToken = misspellObjectToken["from"];
									JToken? toToken = misspellObjectToken["to"];
									JToken? minChangeRateToken = misspellObjectToken["min_change_rate"];
									JToken? maxChangeRateToken = misspellObjectToken["max_change_rate"];
									if (fromToken == null || toToken == null || minChangeRateToken == null || maxChangeRateToken == null) // 键值不存在检查
									{
										Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内缺少必要键，Json路径={}.generators." + generatorProperty.Name + ".modifiers.misspells[n]");
										return false;
									}
									if (fromToken.Type != JTokenType.String || toToken.Type != JTokenType.String || minChangeRateToken.Type != JTokenType.Float || maxChangeRateToken.Type != JTokenType.Float) // 值类型错误检查
									{
										Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 对象内某叶子键的值类型错误，Json路径={}.generators." + generatorProperty.Name + ".modifiers.misspells[n]");
										return false;
									}
									misspellsParsed.Add(new Modifier_Misspell(fromToken.Value<string>() ?? "", toToken.Value<string>() ?? "", minChangeRateToken.Value<float>(), maxChangeRateToken.Value<float>()));
								}
								thisGeneratorParsed.Modifier.Misspells = misspellsParsed.ToArray();
								break;
						}
					}
					if (delayToken.Count != 2) // 值数组元素数量检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 数组元素数量不符合要求，需要为2，Json路径={}.generators." + generatorProperty.Name + ".delay");
						return false;
					}
					if (!(delayToken[0] is { Type: JTokenType.Float } delayMinToken)) // 值数组元素类型错误检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为浮点型，Json路径={}.generators." + generatorProperty.Name + ".modifiers.delay[0]");
						return false;
					}
					if (!(delayToken[1] is { Type: JTokenType.Float } delayMaxToken)) // 值数组元素类型错误检查
					{
						Debug.LogError(FakeLivingComments.MOD_NAME + "：弹幕内容数据Json加载错误: 值的类型错误，需要为浮点型，Json路径={}.generators." + generatorProperty.Name + ".modifiers.delay[1]");
						return false;
					}
					thisGeneratorParsed.Delay[0] = delayMinToken.Value<float>();
					thisGeneratorParsed.Delay[1] = delayMaxToken.Value<float>();
					dataParsed.Generators.TryAdd(generatorProperty.Name, thisGeneratorParsed);
				}
			}
			return true;
		}
	}
}