namespace FakeLivingComments.Factory
{
	/// <summary>
	/// 生成器修饰器数据
	/// </summary>
	public class GeneratorModifier
	{
		/// <summary>
		/// 重复修饰器
		/// </summary>
		public int[] repeat = {0, 0};
		/// <summary>
		/// 错别字修饰器
		/// </summary>
		public Modifier_Misspell[] misspell = { };
	}
	/// <summary>
	/// 错别字修饰器数据结构
	/// </summary>
	public class Modifier_Misspell
	{
		/// <summary>
		/// 将要替换的原始文本
		/// </summary>
		public string? from;
		/// <summary>
		/// 替换为的目标文本
		/// </summary>
		public string? to;
		/// <summary>
		/// 每发现一个匹配的原始文本时随机决定是否进行替换的概率
		/// </summary>
		public float? chance_per_match;
		/// <summary>
		/// 总体最多成功替换的次数
		/// </summary>
		public int? max_successes;
	}
}