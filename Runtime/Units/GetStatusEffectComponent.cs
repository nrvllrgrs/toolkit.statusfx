using System;
using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.StatusFX.VisualScripting
{
	[UnitCategory("Status Effects")]
    public class GetStatusEffectComponent : Unit
    {
		#region Fields

		[DoNotSerialize, PortLabelHidden]
		public ControlInput inputTrigger { get; private set; }

		[DoNotSerialize]
		public ValueInput statusEffectEventArgs;

		[DoNotSerialize]
		public ValueInput type;

		[DoNotSerialize]
		public ControlOutput validTrigger { get; private set; }

		[DoNotSerialize]
		public ControlOutput invalidTrigger { get; private set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput component;

		private Component m_component;

		#endregion

		#region Methods

		protected override void Definition()
		{
			inputTrigger = ControlInput(nameof(inputTrigger), Trigger);

			statusEffectEventArgs = ValueInput<StatusEffectEventArgs>(nameof(statusEffectEventArgs));
			type = ValueInput(nameof(type), default(Type));

			component = ValueOutput(nameof(component), (x) => m_component);

			validTrigger = ControlOutput("Not Null");
			invalidTrigger = ControlOutput("Null");

			Requirement(statusEffectEventArgs, inputTrigger);
			Requirement(type, inputTrigger);

			Succession(inputTrigger, validTrigger);
			Succession(inputTrigger, invalidTrigger);
		}

		private ControlOutput Trigger(Flow flow)
		{
			var _statusEffectEventArgs = flow.GetValue<StatusEffectEventArgs>(statusEffectEventArgs);
			if (_statusEffectEventArgs != null)
			{
				m_component = GetComponent(flow, _statusEffectEventArgs.statusEffectControl.transform, flow.GetValue<Type>(type));
				if (m_component != null)
				{
					return validTrigger;
				}
			}

			m_component = null;
			return invalidTrigger;
		}

		protected virtual Component GetComponent(Flow flow, Transform transform, Type type)
		{
			return transform.GetComponent(type);
		}

		#endregion
	}
}