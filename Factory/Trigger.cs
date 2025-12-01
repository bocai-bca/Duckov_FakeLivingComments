using System;
using System.Reflection;
using HarmonyLib;

namespace FakeLivingComments.Factory
{
	/// <summary>
	/// 触发器数据，类型中的结构也代表着flc_data.json中触发器的结构
	/// </summary>
	public class Trigger : NodeBase
	{
		/// <summary>
		/// 该触发器的类型
		/// </summary>
		public TriggerType type;
		/// <summary>
		/// 该触发器的订阅目标类，需填写含命名空间的类型名称
		/// </summary>
		public string class_name;
		/// <summary>
		/// 该触发器的订阅目标，目标的类型受type决定
		/// </summary>
		public string target;
		/// <summary>
		/// 默认值构造函数
		/// </summary>
		/// <param name="type">该触发器的类型</param>
		/// <param name="className">该触发器的订阅目标类，需填写含命名空间的类型名称</param>
		/// <param name="target">该触发器的订阅目标，目标的类型受type决定</param>
		public Trigger(TriggerType type = TriggerType.Signal, string className = "", string target = "")
		{
			class_name = className;
			this.type = type;
			this.target = target;
		}
		/// <summary>
		/// 
		/// </summary>
		public void OnTriggered()
		{
			
		}
		/// <summary>
		/// 使触发器攀附到目标
		/// </summary>
		/// <param name="errorReason">错误理由</param>
		/// <returns>错误与否</returns>
		public bool Patch(out string errorReason)
		{
			if (type == TriggerType.Signal)
			{
				errorReason = "";
				return true;
			}
			if (string.IsNullOrWhiteSpace(class_name) || string.IsNullOrWhiteSpace(target))
			{
				errorReason = "订阅目标类或目标为空";
				return false;
			}
			errorReason = "";
			switch (type)
			{
				case TriggerType.Harmony:
					Type? targetClass;
					try
					{
						targetClass = Type.GetType(class_name);
					}
					catch (Exception e)
					{
						errorReason = $"Type.GetType抛出参数异常\n{e.Message}";
						return false;
					}
					if (targetClass == null)
					{
						errorReason = $"找不到订阅目标类{class_name}";
						return false;
					}
					MethodInfo? targetMethod;
					try
					{
						targetMethod = targetClass.GetMethod(target);
					}
					catch (Exception e)
					{
						errorReason = $"Type.GetMethod抛出异常\n{e.Message}";
						return false;
					}
					if (targetMethod == null)
					{
						errorReason = $"找不到订阅目标方法{target}";
						return false;
					}
					FakeLivingComments.HarmonyInstance.Patch(targetMethod, null, new HarmonyMethod(typeof(Trigger), "OnTriggered", Type.EmptyTypes));
					return true;
				default:
					errorReason = $"未定义的触发器类型{type}";
					return false;
			}
		}
	}
}