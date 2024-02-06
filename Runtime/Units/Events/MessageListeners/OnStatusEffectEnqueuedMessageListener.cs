using UnityEngine;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	[AddComponentMenu("")]
	public class OnStatusEffectEnqueuedMessageListener : BaseStatusEffectMessageListener
	{
		protected override string hookName => EventHooks.OnStatusEffectEnqueued;

		protected override void Register(IStatusEffectListener effectListener)
		{
			effectListener.onEnqueued.AddListener(Trigger);
		}

		protected override void Unregister(IStatusEffectListener effectListener)
		{
			effectListener.onEnqueued.RemoveListener(Trigger);
		}
	}
}
