using System;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	public class OnStatusEffectApplied : BaseStatusEffectEventUnit
	{
		#region Properties

		public override Type MessageListenerType => typeof(OnStatusEffectAppliedMessageListener);

		#endregion
	}
}