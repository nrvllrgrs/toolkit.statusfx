using UnityEngine;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	[AddComponentMenu("")]
	public class OnStatusEffectExpiredMessageListener : BaseStatusEffectMessageListener
	{
		protected override string hookName => EventHooks.OnStatusEffectExpired;

		protected override void Register(IStatusEffectListener effectListener)
		{
			effectListener.onExpired.AddListener(Trigger);
		}

		protected override void Unregister(IStatusEffectListener effectListener)
		{
			effectListener.onExpired.RemoveListener(Trigger);
		}
	}
}
