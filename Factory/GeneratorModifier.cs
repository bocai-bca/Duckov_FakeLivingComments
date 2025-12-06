using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace FakeLivingComments.Factory
{
	/// <summary>
	/// 生成器修饰器数据
	/// </summary>
	public class GeneratorModifier
	{
		/// <summary>
		/// 重复修饰器，[0]为随机最小值(含)，[1]为随机最大值(含)
		/// </summary>
		public int[] repeat = {0, 0};
		/// <summary>
		/// 错别字修饰器
		/// </summary>
		public Modifier_Misspell[] misspells = { };
		/// <summary>
		/// 执行修饰器
		/// </summary>
		/// <param name="theText">原始文本</param>
		/// <returns>修饰后的文本</returns>
		public string ExecuteModifier(string theText)
		{
			// 重复修饰器
			for (int repeatRandomized = Random.Range(repeat[0], repeat[1] + 1); repeatRandomized > 0; repeatRandomized--)
			{
				theText += theText;
			}
			// 错别字修饰器
			foreach (Modifier_Misspell misspell in misspells) // 遍历所有错别字修饰器
			{
				string[] splitted = theText.Split(misspell.from); // 按当前错别字from分割原始文本
				if (splitted.Length <= 1) // 如果本轮分割没有切下任何刀，意味着当前文本中不含错别字from
				{
					continue; // 结束本个错别字修饰器对象的执行
				}
				float changeRate = Random.Range(misspell.min_change_rate, misspell.max_change_rate); // 随机一个替换率
				List<bool> boolMap = new List<bool>(splitted.Length - 1); // 创建一个布尔数组，代表每个分段处是否成功替换为错别字
				for (int boolMapIndex = 0; boolMapIndex < boolMap.Capacity; boolMapIndex++) // 填充值并打乱数组
				{
					boolMap.Add(false); // 添加元素
					int targetIndex = Random.Range(0, boolMapIndex); // 随机一个被替换值的目标索引
					bool source; // 声明局部变量存储当前索引的值
					if (boolMapIndex / (float)boolMap.Count <= changeRate) // 如果当前索引在整个数组中的位置小于等于替换率表示的位置
					{
						boolMap[boolMapIndex] = source = true; // 将当前索引设为true，意味着该分割位置会成功替换为错别字
					}
					else // 否则(当前索引在整个数组中的位置大于替换率表示的位置)
					{
						source = boolMap[boolMapIndex]; // 记录source的值
					}
					bool target = boolMap[targetIndex];
					boolMap[boolMapIndex] = target; // 替换
					boolMap[targetIndex] = source; // 替换
				}
				theText = splitted[0];
				for (int index = 0; index < boolMap.Count; index++) // 拼接分段后的文本
				{
					if (boolMap[index])
					{
						theText += misspell.to + splitted[index + 1]; // 拼接错别字
						continue;
					}
					theText += misspell.from + splitted[index + 1]; // 拼接原始文本
				}
			}
			return theText;
		}
	}
	/// <summary>
	/// 错别字修饰器数据结构
	/// </summary>
	public class Modifier_Misspell
	{
		/// <summary>
		/// 将要替换的原始文本
		/// </summary>
		public string from;
		/// <summary>
		/// 替换为的目标文本
		/// </summary>
		public string to;
		/// <summary>
		/// 替换率最小随机值，填写0-1浮点数，不能大于max_change_rate
		/// </summary>
		public float min_change_rate;
		/// <summary>
		/// 替换率最大随机值，填写0-1浮点数，不能小于min_change_rate
		/// </summary>
		public float max_change_rate;
		/// <summary>
		/// 默认值构造函数
		/// </summary>
		/// <param name="from">将要替换的原始文本</param>
		/// <param name="to">替换为的目标文本</param>
		/// <param name="minChangeRate">替换率最小随机值，填写0-1浮点数，不能大于max_change_rate</param>
		/// <param name="maxChangeRate">替换率最大随机值，填写0-1浮点数，不能小于min_change_rate</param>
		public Modifier_Misspell(string from, string to, float minChangeRate, float maxChangeRate)
		{
			this.from = from;
			this.to = to;
			this.min_change_rate = minChangeRate;
			this.max_change_rate = maxChangeRate;
		}
	}
}