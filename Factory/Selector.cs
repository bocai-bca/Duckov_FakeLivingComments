namespace FakeLivingComments.Factory
{
	/// <summary>
	/// 抽取器数据，类型中的结构也代表着flc_data.json中抽取器的结构
	/// </summary>
	public class Selector : NodeBase
	{
		/// <summary>
		/// 该抽取器被调用时进行抽取的次数
		/// </summary>
		public int Rolls;
		/// <summary>
		/// 该抽取器的抽取池
		/// </summary>
		public SelectorObject[]? Pool;
		/// <summary>
		/// 默认值构造函数
		/// </summary>
		/// <param name="rolls">该抽取器的抽取次数</param>
		/// <param name="pool">该抽取器的抽取池</param>
		public Selector(int rolls = 1, SelectorObject[]? pool = null)
		{
			Rolls = rolls;
			Pool = pool;
		}
	}
	/// <summary>
	/// 抽取器池对象结构
	/// </summary>
	public class SelectorObject
	{
		/// <summary>
		/// 该对象在池中的权重
		/// </summary>
		public int Weight;
		/// <summary>
		/// 该对象指向的生成器，每个值为生成器UID
		/// </summary>
		public string[]? GeneratorUIDs;

		public SelectorObject(int weight = 0, string[]? generatorUIDs = null)
		{
			Weight = weight;
			GeneratorUIDs = generatorUIDs;
		}
	}
}