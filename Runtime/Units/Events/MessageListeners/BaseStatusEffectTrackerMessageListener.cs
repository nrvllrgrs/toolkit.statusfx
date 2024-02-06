using Unity.VisualScripting;

namespace ToolkitEngine.StatusFX.VisualScripting
{
    public abstract class BaseStatusEffectMessageListener : MessageListener
    {
        #region Fields

        protected IStatusEffectListener m_effectListener;

        private const string STATUS_EFFECT_TYPE = "$statusEffectType";

		#endregion

		#region Properties

		protected abstract string hookName { get; }

        #endregion

        #region Methods

        private void Awake()
        {
			var effectControl = GetComponentInParent<StatusEffectControl>();
            if (effectControl == null)
                return;

            if (!Variables.Object(gameObject).IsDefined(STATUS_EFFECT_TYPE))
            {
                m_effectListener = effectControl;
                Register(effectControl);
            }
            else
            {
                var effectType = Variables.Object(gameObject).Get<StatusEffectType>(STATUS_EFFECT_TYPE);
                if (effectControl.TryGetStatusEffectState(effectType, out StatusEffectState effectState))
                {
                    m_effectListener = effectState;
                    Register(m_effectListener);
                }
            }
		}

        private void OnDestroy()
        {
            if (m_effectListener != null)
            {
                Unregister(m_effectListener);
            }
        }

        protected abstract void Register(IStatusEffectListener effectListener);
		protected abstract void Unregister(IStatusEffectListener effectListener);

        protected void Trigger(StatusEffectEventArgs e)
        {
			EventBus.Trigger(hookName, gameObject, e);
		}

		#endregion
	}
}