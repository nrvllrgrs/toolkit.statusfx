using Unity.VisualScripting;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	[UnitCategory("Events/Status Effects")]
	public abstract class BaseStatusEffectEventUnit : BaseEventUnit<StatusEffectEventArgs>
	{
		#region Fields

		[UnitHeaderInspectable("Filtered")]
		public bool filtered = false;

		[DoNotSerialize]
		public ValueInput filter { get; private set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();

			if (filtered)
			{
				filter = ValueInput<StatusEffectType>(nameof(filter), null);
			}
		}

		protected override bool ShouldTrigger(Flow flow, StatusEffectEventArgs args)
		{
			return !filtered || Equals(flow.GetValue<StatusEffectType>(filter), args.statusEffect.statusEffectType);
		}

		#endregion
	}
}