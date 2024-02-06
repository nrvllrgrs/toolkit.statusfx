using System;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	public class OnStatusEffectExpired : BaseStatusEffectEventUnit
	{
		#region Properties

		public override Type MessageListenerType => typeof(OnStatusEffectExpiredMessageListener);

		#endregion
	}
}