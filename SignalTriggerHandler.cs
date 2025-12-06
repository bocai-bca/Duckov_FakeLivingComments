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
			ItemAgent_Gun.OnMainCharacterShootEvent += onPlayerShoot;
		}
		/// <summary>
		/// 卸载时调用
		/// </summary>
		internal static void Unload()
		{
			Health.OnDead -= onHealthDead;
			ItemAgent_Gun.OnMainCharacterShootEvent -= onPlayerShoot;
		}
		private static void onHealthDead(Health health, DamageInfo damageInfo)
		{
			if (health.IsMainCharacterHealth)
			{
				FactoryManager.EmitTriggerSignal("OnPlayerDeath");
			}
			if (!health.IsMainCharacterHealth && damageInfo.fromCharacter.IsMainCharacter)
			{
				FactoryManager.EmitTriggerSignal("OnPlayerKillEnemy");
				FactoryManager.EmitTriggerSignal("OnPlayerKillEnemy_enemy=" + health.TryGetCharacter().characterPreset.Name);
			}
		}
		private static void onPlayerShoot(ItemAgent_Gun itemAgent_Gun)
		{
			FactoryManager.EmitTriggerSignal("OnPlayerShoot");
		}
	}
}