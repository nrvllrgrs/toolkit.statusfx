using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using System.Linq;

namespace ToolkitEngine.StatusFX
{
	public class StatusEffectEventArgs : System.EventArgs
	{
		#region Properties

		public StatusEffectControl statusEffectControl { get; private set; }
		public StatusEffect statusEffect { get; private set; }
		public int index { get; private set; }

		public StatusEffectState statusEffectState
		{
			get
			{
				if (statusEffectControl == null || statusEffect == null)
					return null;

				return statusEffectControl.TryGetStatusEffectState(statusEffect.statusEffectType, out StatusEffectState effectState)
					? effectState
					: null;
			}
		}

		#endregion

		#region Constructors

		public StatusEffectEventArgs(StatusEffectControl effectControl, StatusEffect effect, int index)
		{
			statusEffectControl = effectControl;
			statusEffect = effect;
			this.index = index;
		}

		#endregion
	}

	public class StatusEffectControl : MonoBehaviour, IPoolItemRecyclable, IStatusEffectListener
	{
		#region Fields

		[SerializeField]
		private StatusEffectState[] m_statusEffects;

		/// <summary>
		/// List of status effects in order of application
		/// </summary>
		private List<StatusEffect> m_appliedStatusEffects = new();
		private Dictionary<StatusEffectType, StatusEffectState> m_stateMap = new();
		private Coroutine m_thread;

		private List<GameObject> m_trackersToDestroy = new();

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

		public UnityEvent<StatusEffectEventArgs> onApplied => m_onApplied;
		public UnityEvent<StatusEffectEventArgs> onEnqueued => m_onEnqueued;
		public UnityEvent<StatusEffectEventArgs> onUpdated => m_onUpdated;
		public UnityEvent<StatusEffectEventArgs> onRemoved => m_onRemoved;
		public UnityEvent<StatusEffectEventArgs> onExpired => m_onExpired;

		#endregion

		#region Methods

		private void Awake()
		{
			foreach (var effectState in m_statusEffects)
			{
				if (effectState.statusEffectType == null)
					continue;

				// Create state for each allowed status effect
				m_stateMap.Add(effectState.statusEffectType, effectState);

				// Link state back to control
				effectState.statusEffectControl = this;

				// When status effect is added / removed from state, update active status effects
				effectState.onApplied.AddListener(StatusEffectState_Applied);
				effectState.onEnqueued.AddListener(StatusEffectState_Enqueued);
				effectState.onUpdated.AddListener(StatusEffectState_Updated);
				effectState.onRemoved.AddListener(StatusEffectState_Removed);
				effectState.onExpired.AddListener(StatusEffectState_Expired);
			}
		}

		public void Recycle()
		{
			RemoveAll();
			Start();
		}

		private void Start()
		{
			foreach (var effectState in m_statusEffects)
			{
				if (effectState.statusEffectType == null)
					continue;

				// Start effect, if applied by default
				if (effectState.applyOnStart)
				{
					Apply(effectState.statusEffectType, -1f, gameObject);
				}
			}
		}

		private void OnDestroy()
		{
			foreach (var effectState in m_statusEffects)
			{
				if (effectState.statusEffectType == null)
					continue;

				effectState.onApplied.RemoveListener(StatusEffectState_Applied);
				effectState.onEnqueued.AddListener(StatusEffectState_Enqueued);
				effectState.onUpdated.AddListener(StatusEffectState_Updated);
				effectState.onRemoved.RemoveListener(StatusEffectState_Removed);
				effectState.onExpired.AddListener(StatusEffectState_Expired);
			}
		}

		private void StatusEffectState_Applied(StatusEffectEventArgs e)
		{
			m_appliedStatusEffects.Add(e.statusEffect);
			m_onApplied?.Invoke(e);

			// Update coroutine not running, start
			if (m_thread == null)
			{
				m_thread = StartCoroutine(AsyncTick());
			}
		}

		private void StatusEffectState_Removed(StatusEffectEventArgs e)
		{
			m_appliedStatusEffects.Remove(e.statusEffect);
			onRemoved?.Invoke(e);

			// No status effects applied, stop
			if (m_appliedStatusEffects.Count == 0)
			{
				this.CancelCoroutine(ref m_thread);
			}
		}

		private void StatusEffectState_Enqueued(StatusEffectEventArgs e)
		{
			m_onEnqueued?.Invoke(e);
		}

		private void StatusEffectState_Updated(StatusEffectEventArgs e)
		{
			m_onUpdated?.Invoke(e);
		}

		private void StatusEffectState_Expired(StatusEffectEventArgs e)
		{
			m_onExpired?.Invoke(e);
		}

		public bool TryGetStatusEffectState(StatusEffectType effectType, out StatusEffectState effectState) => m_stateMap.TryGetValue(effectType, out effectState);
		public bool CanApply(StatusEffectType effectType) => m_stateMap.ContainsKey(effectType);
		public bool CanRemove(StatusEffectType effectType) => m_stateMap.TryGetValue(effectType, out StatusEffectState effect) && effect.isApplied;

		public bool Apply(StatusEffectType effectType, GameObject source = null)
		{
			return Apply(effectType, effectType.duration, source);
		}

		public bool Apply(StatusEffectType effectType, float duration, GameObject source = null)
		{
			// Cannot apply this status effect, skip
			if (!m_stateMap.TryGetValue(effectType, out StatusEffectState effectState) || effectState.isImmune)
				return false;

			StatusEffect effect;
			switch (effectType.stackMode)
			{
				case StackMode.Independent:
					// At stack capacity, remove oldest effect of type
					if (effectState.stackSize == effectType.maxStack)
					{
						effect = effectState.statusEffects[0];
						Remove(effect, effectState, m_appliedStatusEffects.IndexOf(effect));
					}
					break;

				case StackMode.Queued:
					// Already applied, add to queue
					if (effectState.isApplied)
					{
						effectState.Enqueue(new StatusEffect(effectType, duration, source));
						return false;
					}
					break;
			}

			// Create status to be applied to target
			Apply(new StatusEffect(effectType, duration, source), effectState);
			return true;
		}

		private void Apply(StatusEffect effect, StatusEffectState effectState)
		{
			// Perform actions on target
			if (effect.statusEffectType.script != null)
			{
				effect.tracker = new GameObject(string.Format("{0} Tracker", effect.statusEffectType.name));
				//effect.tracker.hideFlags |= HideFlags.HideAndDontSave;
				effect.tracker.transform.SetParent(transform, false);

				var machine = effect.tracker.AddComponent<ScriptMachine>();

				// Disable machine while initializing -- so it doesn't start prematurely
				machine.enabled = false;
				machine.nest.macro = effect.statusEffectType.script;
				Variables.Object(effect.tracker).Set("$statusEffectType", effect.statusEffectType);
				machine.enabled = true;
			}

			int index = -1;

			// Actions for Instant status effects are applied
			// But the IsApplied flag is intentionally never flipped
			if (effect.durationType != DurationMode.Instant && !effectState.isImmune)
			{
				effectState.Add(effect);
				index = m_appliedStatusEffects.Count - 1;
			}

			// Notify control listeners
			effectState.onApplied?.Invoke(new StatusEffectEventArgs(this, effect, index));

			// Mark for cleanup before applying status effect
			// Otherwise, update coroutine will immediately exit
			if (effect.durationType == DurationMode.Instant)
			{
				MarkForCleanup(effect);
			}
		}

		public bool Remove(StatusEffectType statusEffectType, GameObject source)
		{
			// Status effect doesn't exist OR isn't applied, skip
			if (!m_stateMap.TryGetValue(statusEffectType, out StatusEffectState effectState) || !effectState.isApplied)
				return false;

			effectState.ClearQueue();

			var effects = effectState.statusEffects;
			for (int i = 0; i < effects.Length; ++i)
			{
				Remove(effects[i], effectState, i);
			}

			return true;
		}

		private void Remove(StatusEffect effect, StatusEffectState effectState, int index)
		{
			// Invoke removed event
			effectState.onRemoved?.Invoke(new StatusEffectEventArgs(this, effect, index));
			effectState.Remove(effect);

			MarkForCleanup(effect);
		}

		public void RemoveAll(GameObject source = null)
		{
			foreach (var p in m_stateMap)
			{
				Remove(p.Key, source);
			}
		}

		private void Expire(StatusEffect effect, StatusEffectState effectState, int index)
		{
			Remove(effect, effectState, index);

			// Notify control listeners
			effectState.onExpired?.Invoke(new StatusEffectEventArgs(this, effect, index));

			// Apply next queued status effect, if available
			if (effect.statusEffectType.stackMode == StackMode.Queued && !effectState.isQueueEmpty)
			{
				Apply(effectState.Dequeue(), effectState);
			}
		}

		private IEnumerator AsyncTick()
		{
			while (m_appliedStatusEffects.Count > 0 || m_trackersToDestroy.Count > 0)
			{
				// Wait a frame
				yield return null;

				// Cleanup trackers removed last frame
				// Giving them time to execute onRemoved and onExpired events
				for (int i = 0; i < m_trackersToDestroy.Count; ++i)
				{
					Destroy(m_trackersToDestroy[i]);
				}
				m_trackersToDestroy.Clear();

				// Iterate using index for callbacks
				var cachedStatusEffects = new List<StatusEffect>(m_appliedStatusEffects);
				for (int i = 0; i < cachedStatusEffects.Count; ++i)
				{
					var effect = cachedStatusEffects[i];
					var effectState = m_stateMap[effect.statusEffectType];

					// Notify control listeners
					effectState?.onUpdated.Invoke(new StatusEffectEventArgs(this, effect, i));

					// Update timer
					if (effect.Tick())
					{
						Expire(effect, effectState, i);
					}
				}
			}

			// Coroutine is complete, clear handle
			m_thread = null;
		}

		public bool AddImmunity(StatusEffectType effectType)
		{
			// Status effect type does not exist on control, skip
			if (!m_stateMap.TryGetValue(effectType, out StatusEffectState effectState))
				return false;

			effectState.AddImmunity();
			if (effectState.stackSize > 0)
			{
				Remove(effectType, null);
			}
			return true;
		}

		public bool RemoveImmunity(StatusEffectType effectType)
		{
			// Status effect type does not exist on control, skip
			if (!m_stateMap.TryGetValue(effectType, out StatusEffectState effectState))
				return false;

			effectState.RemoveImmunity();
			return true;
		}

		private void MarkForCleanup(StatusEffect effect)
		{
			if (effect.tracker == null)
				return;

			m_trackersToDestroy.Add(effect.tracker);

			// Update coroutine not running, start
			if (m_thread == null)
			{
				m_thread = StartCoroutine(AsyncTick());
			}
		}

		#endregion
	}
}