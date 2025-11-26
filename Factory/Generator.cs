using System.Collections.Generic;

namespace FakeLivingComments.Factory
{
	/// <summary>
	/// 生成器数据，类型中的结构也代表着flc_data.json中生成器的结构
	/// </summary>
	public class Generator : NodeBase
	{
		/// <summary>
		/// 该生成器的类型
		/// </summary>
		public GeneratorType type;
		/// <summary>
		/// 数据源，为Normal类型时代表原初文本，为External时代表要调用的方法
		/// </summary>
		public string? source;
		/// <summary>
		/// 修饰器，仅限Normal类型可使用
		/// </summary>
		public Dictionary<string, GeneratorModifier>? modifiers;
	}
}