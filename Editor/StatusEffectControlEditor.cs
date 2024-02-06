using ToolkitEngine.StatusFX;
using UnityEditor;

namespace ToolkitEditor.StatusFX
{
	[CustomEditor(typeof(StatusEffectControl))]
    public class StatusEffectControlEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_statusEffects;

		protected SerializedProperty m_onApplied;
		protected SerializedProperty m_onEnqueued;
		protected SerializedProperty m_onUpdated;
		protected SerializedProperty m_onRemoved;
		protected SerializedProperty m_onExpired;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_statusEffects = serializedObject.FindProperty(nameof(m_statusEffects));

			m_onApplied = serializedObject.FindProperty(nameof(m_onApplied));
			m_onEnqueued = serializedObject.FindProperty(nameof(m_onEnqueued));
			m_onUpdated = serializedObject.FindProperty(nameof(m_onUpdated));
			m_onRemoved = serializedObject.FindProperty(nameof(m_onRemoved));
			m_onExpired = serializedObject.FindProperty(nameof(m_onExpired));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_statusEffects);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onApplied, "Events"))
			{
				EditorGUILayout.PropertyField(m_onApplied);
				EditorGUILayout.PropertyField(m_onEnqueued);
				EditorGUILayout.PropertyField(m_onUpdated);
				EditorGUILayout.PropertyField(m_onRemoved);
				EditorGUILayout.PropertyField(m_onExpired);
			}
		}

		#endregion
	}
}