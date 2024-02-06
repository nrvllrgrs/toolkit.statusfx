using UnityEngine;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	[AddComponentMenu("")]
	public class OnStatusEffectAppliedMessageListener : BaseStatusEffectMessageListener
	{
		protected override string hookName => EventHooks.OnStatusEffectApplied;

		protected override void Register(IStatusEffectListener effectListener)
		{
			effectListener.onApplied.AddListener(Trigger);
		}

		protected override void Unregister(IStatusEffectListener effectListener)
		{
			effectListener.onApplied.RemoveListener(Trigger);
		}
	}
}
