using UnityEngine.Events;

namespace ToolkitEngine.StatusFX
{
	public interface IStatusEffectListener
    {
        UnityEvent<StatusEffectEventArgs> onApplied { get; }
		UnityEvent<StatusEffectEventArgs> onEnqueued { get; }
		UnityEvent<StatusEffectEventArgs> onUpdated { get; }
		UnityEvent<StatusEffectEventArgs> onRemoved { get; }
		UnityEvent<StatusEffectEventArgs> onExpired { get; }
	}
}