using System;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	public class OnStatusEffectRemoved : BaseStatusEffectEventUnit
	{
		#region Properties

		public override Type MessageListenerType => typeof(OnStatusEffectRemovedMessageListener);

		#endregion
	}
}