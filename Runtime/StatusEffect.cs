using UnityEngine;

namespace ToolkitEngine.StatusFX
{
    public class StatusEffect
    {
        #region Properties

        public StatusEffectType statusEffectType { get; private set; }
        public DurationMode durationType { get; private set; }
        public float duration { get; private set; }
        public float time { get; private set; }

        public float normalizedTime
        {
            get
            {
                switch (durationType)
                {
                    case DurationMode.Timed:
                        return Mathf.Clamp01(time / duration);

                    case DurationMode.Manual:
                        return -1f;

                    default:
                        return 0f;
                }
            }
        }

        public GameObject source { get; private set; }
        public GameObject tracker { get; internal set; }

        #endregion

        #region Constructors

        public StatusEffect(StatusEffectType effectType, GameObject source)
            : this(effectType, effectType.duration, source)
        {
            durationType = effectType.durationMode;
        }

        public StatusEffect(StatusEffectType effectType, float duration, GameObject source)
        {
            statusEffectType = effectType;

            // Overriding duration type and time with duration
            if (duration > 0f)
            {
                durationType = DurationMode.Timed;
            }
            else if (duration < 0f)
            {
                durationType = DurationMode.Manual;
            }
            else
            {
                durationType = DurationMode.Instant;
            }

            this.duration = duration;
            time = 0f;
            this.source = source;
        }

        #endregion

        #region Methods

        internal bool Tick()
        {
            // Update timer
            time += Time.deltaTime;
            return durationType == DurationMode.Timed && time >= duration;
        }

        #endregion
    }
}