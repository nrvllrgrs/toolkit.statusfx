using System;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	public class OnStatusEffectEnqueued : BaseStatusEffectEventUnit
	{
		#region Properties

		public override Type MessageListenerType => typeof(OnStatusEffectEnqueuedMessageListener);

		#endregion
	}
}