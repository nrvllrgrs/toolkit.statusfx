using System;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	public class OnStatusEffectUpdated : BaseStatusEffectEventUnit
	{
		#region Properties

		public override Type MessageListenerType => typeof(OnStatusEffectUpdatedMessageListener);

		#endregion
	}
}