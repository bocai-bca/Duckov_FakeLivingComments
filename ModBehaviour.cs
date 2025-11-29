namespace FakeLivingComments
{
	/// <summary>
	/// FakeLivingComments的ModBehaviour
	/// </summary>
	public class ModBehaviour : Duckov.Modding.ModBehaviour
	{
		public void Awake()
		{

		}
		public void OnEnable()
		{
			FakeLivingComments.Init();
		}
		public void OnDisable()
		{
			FakeLivingComments.Unload();
		}
		
		public void Update()
		{
			FakeLivingComments.Update();
		}
	}
}
