using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.StatusFX
{
    [System.Serializable]
    public class StatusEffectState : IStatusEffectListener
	{
        #region Fields

        [SerializeField]
        private StatusEffectType m_statusEffectType;

        [SerializeField]
        private bool m_isApplied;

        private List<StatusEffect> m_statusEffects = new();

        private int m_immunityCount = 0;
        private Queue<StatusEffect> m_queue;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<StatusEffectEventArgs> m_onApplied;

		[SerializeField]
		private UnityEvent<StatusEffectEventArgs> m_onEnqueued;

		[SerializeField]
		private UnityEvent<StatusEffectEventArgs> m_onUpdated;

		[SerializeField]
		private UnityEvent<StatusEffectEventArgs> m_onRemoved;

		[SerializeField]
		private UnityEvent<StatusEffectEventArgs> m_onExpired;

        #endregion

        #region Properties

        public StatusEffectControl statusEffectControl { get; internal set; }

        public StatusEffectType statusEffectType => m_statusEffectType;

        public StatusEffect[] statusEffects => m_statusEffects.ToArray();

        public int stackSize => m_statusEffects.Count;

        /// <summary>
        /// Indicates whether status effect is applied on creation
        /// </summary>
        public bool applyOnStart => m_isApplied;

        /// <summary>
        /// Indicates whether status effect is applied to gameObject
        /// </summary>
        public bool isApplied => m_statusEffects.Count > 0;

        /// <summary>
        /// Indicates whether status effect cannot be applied due to (temporary) immunity
        /// </summary>
        public bool isImmune => m_immunityCount > 0;

        public int queueCount => m_queue?.Count ?? 0;
        public bool isQueueEmpty => queueCount == 0;

		public UnityEvent<StatusEffectEventArgs> onApplied => m_onApplied;
		public UnityEvent<StatusEffectEventArgs> onEnqueued => m_onEnqueued;
		public UnityEvent<StatusEffectEventArgs> onUpdated => m_onUpdated;
		public UnityEvent<StatusEffectEventArgs> onRemoved => m_onRemoved;
		public UnityEvent<StatusEffectEventArgs> onExpired => m_onExpired;

		#endregion

		#region Methods

		internal bool Add(StatusEffect effect)
        {
            if (!m_statusEffects.Contains(effect))
            {
                m_statusEffects.Add(effect);
                return true;
            }
            return false;
        }

        internal bool Remove(StatusEffect statusEffect)
        {
            return m_statusEffects.Remove(statusEffect);
        }

        internal void Enqueue(StatusEffect statusEffect)
        {
            if (m_queue == null)
            {
                m_queue = new Queue<StatusEffect>();
            }

            m_queue.Enqueue(statusEffect);
            onEnqueued?.Invoke(new StatusEffectEventArgs(statusEffectControl, statusEffect, -1));
        }

        internal StatusEffect Dequeue()
        {
            return m_queue?.Dequeue();
        }

        internal void ClearQueue()
        {
            m_queue?.Clear();
        }

        internal void AddImmunity() => ++m_immunityCount;
        internal void RemoveImmunity() => --m_immunityCount;

        #endregion
    }
}