using System;

namespace FakeLivingComments
{
	/// <summary>
	/// 信号触发器处理类，负责手动实现对接游戏部分内容，为触发器提供信号来源
	/// </summary>
	public static class SignalTriggerHandler
	{
		/// <summary>
		/// 加载时调用
		/// </summary>
		internal static void Load()
		{
			Health.OnDead += onHealthDead;
		}
		/// <summary>
		/// 卸载时调用
		/// </summary>
		internal static void Unload()
		{
			Health.OnDead -= onHealthDead;
		}
		private static void onHealthDead(Health health, DamageInfo damageInfo)
		{
			if (health.IsMainCharacterHealth)
			{
				FactoryManager.EmitTriggerSignal("OnPlayerDeath");
			}
		}
	}
}