using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.StatusFX
{
	public enum DurationMode
	{
		Instant,
		Manual,
		Timed,
	}

	public enum StackMode
	{
		Independent,
		Queued,
	}

	[CreateAssetMenu(menuName = "Toolkit/Status Effect")]
	public class StatusEffectType : ScriptableObject
    {
		#region Fields

		[SerializeField]
		private string m_id = System.Guid.NewGuid().ToString();

		[SerializeField]
		private string m_name;

		[SerializeField, TextArea]
		private string m_description;

		[SerializeField]
		private Sprite m_icon;

		[SerializeField]
		private Color m_color = Color.white;

		[SerializeField]
		private DurationMode m_durationMode;

		[SerializeField, Min(0f)]
		private float m_timedDuration = 0f;

		[SerializeField]
		private StackMode m_stackMode = StackMode.Independent;

		[SerializeField, Min(1)]
		private int m_maxStack = 1;

		[SerializeField]
		private ScriptGraphAsset m_script;

		#endregion

		#region Properties

		public string id => m_id;
		public new string name => m_name;
		public Sprite icon => m_icon;
		public Color color => m_color;
		public DurationMode durationMode => m_durationMode;

		public float duration
		{
			get
			{
				switch (m_durationMode)
				{
					case DurationMode.Timed:
						return m_timedDuration;

					case DurationMode.Manual:
						return -1f;

					default:
						return 0f;
				}
			}
		}

		public int maxStack => m_maxStack;
		public StackMode stackMode => m_stackMode;
		public ScriptGraphAsset script => m_script;

		#endregion

		#region Methods

		public void Apply(GameObject target, GameObject source = null)
		{
			Apply(target, duration, source);
		}

		public void Apply(GameObject target, float duration, GameObject source = null)
		{
			var control = target.GetComponent<StatusEffectControl>();
			if (control == null)
				return;

			control.Apply(this, duration, source);
		}

		public void Remove(GameObject target, GameObject source = null)
		{
			var control = target.GetComponent<StatusEffectControl>();
			if (control == null)
				return;

			control.Remove(this, source);
		}

		#endregion
	}
}