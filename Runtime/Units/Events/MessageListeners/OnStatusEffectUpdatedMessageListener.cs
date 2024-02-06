using UnityEngine;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	[AddComponentMenu("")]
	public class OnStatusEffectUpdatedMessageListener : BaseStatusEffectMessageListener
	{
		protected override string hookName => EventHooks.OnStatusEffectUpdated;

		protected override void Register(IStatusEffectListener effectListener)
		{
			effectListener.onUpdated.AddListener(Trigger);
		}

		protected override void Unregister(IStatusEffectListener effectListener)
		{
			effectListener.onUpdated.RemoveListener(Trigger);
		}
	}
}
