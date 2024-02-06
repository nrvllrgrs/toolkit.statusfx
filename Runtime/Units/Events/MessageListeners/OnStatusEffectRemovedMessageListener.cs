using UnityEngine;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	[AddComponentMenu("")]
	public class OnStatusEffectRemovedMessageListener : BaseStatusEffectMessageListener
	{
		protected override string hookName => EventHooks.OnStatusEffectRemoved;

		protected override void Register(IStatusEffectListener effectListener)
		{
			effectListener.onRemoved.AddListener(Trigger);
		}

		protected override void Unregister(IStatusEffectListener effectListener)
		{
			effectListener.onRemoved.RemoveListener(Trigger);
		}
	}
}
