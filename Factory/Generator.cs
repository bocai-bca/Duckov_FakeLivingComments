using System.Collections.Generic;
using UnityEngine;

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
		public GeneratorType Type;
		/// <summary>
		/// 数据源，为Normal类型时代表原初文本，为External时代表要调用的方法
		/// </summary>
		public string Source;
		/// <summary>
		/// 修饰器，仅限Normal类型可使用
		/// </summary>
		public GeneratorModifier? Modifier;
		/// <summary>
		/// 延迟器，影响文本的出现延迟时间，x=随机最小值，y=随机最大值
		/// </summary>
		public float[]? Delay;
		/// <summary>
		/// 默认值构造函数
		/// </summary>
		/// <param name="type">生成器的类型</param>
		/// <param name="source">生成器的数据源</param>
		/// <param name="modifier">生成器的修饰器表</param>
		/// <param name="delay">生成器的延迟器</param>
		public Generator(GeneratorType type = GeneratorType.Normal, string source = "", GeneratorModifier? modifier = null, float[]? delay = null)
		{
			Type = type;
			Source = source;
			Modifier = modifier;
			Delay = delay;
		}
	}
}