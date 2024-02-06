using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ToolkitEngine.StatusFX;
using Unity.VisualScripting;

namespace ToolkitEditor.StatusFX
{
	[CustomEditor(typeof(StatusEffectType))]
	public class StatusEffectTypeEditor : Editor
	{
		#region Fields

		protected StatusEffectType m_statusEffect;

		protected SerializedProperty m_id;
		protected SerializedProperty m_name;
		protected SerializedProperty m_description;
		protected SerializedProperty m_icon;
		protected SerializedProperty m_color;

		protected SerializedProperty m_durationMode;
		protected SerializedProperty m_timedDuration;

		protected SerializedProperty m_maxStack;
		protected SerializedProperty m_stackMode;

		protected SerializedProperty m_script;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_statusEffect = target as StatusEffectType;

			m_id = serializedObject.FindProperty(nameof(m_id));
			m_name = serializedObject.FindProperty(nameof(m_name));
			m_description = serializedObject.FindProperty(nameof(m_description));
			m_icon = serializedObject.FindProperty(nameof(m_icon));
			m_color = serializedObject.FindProperty(nameof(m_color));

			m_durationMode = serializedObject.FindProperty(nameof(m_durationMode));
			m_timedDuration = serializedObject.FindProperty(nameof(m_timedDuration));

			m_stackMode = serializedObject.FindProperty(nameof(m_stackMode));
			m_maxStack = serializedObject.FindProperty(nameof(m_maxStack));

			m_script = serializedObject.FindProperty(nameof(m_script));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(m_id, new GUIContent("ID"));
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.PropertyField(m_name);
			EditorGUILayout.PropertyField(m_description);
			EditorGUILayout.ObjectField(m_icon, typeof(Sprite), GUILayout.Height(64), GUILayout.Width(64 + EditorGUIUtility.labelWidth));
			EditorGUILayout.PropertyField(m_color);

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_durationMode);
			var durationType = (DurationMode)m_durationMode.intValue;
			if (durationType == DurationMode.Timed)
			{
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_timedDuration, new GUIContent("Time"));
				--EditorGUI.indentLevel;
			}

			EditorGUILayout.PropertyField(m_stackMode);

			if ((StackMode)m_stackMode.intValue == StackMode.Independent)
			{
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_maxStack);
				--EditorGUI.indentLevel;
			}

			EditorGUILayout.Separator();

			EditorGUILayoutUtility.ScriptableObjectField<ScriptGraphAsset>(m_script, m_statusEffect);

			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}
