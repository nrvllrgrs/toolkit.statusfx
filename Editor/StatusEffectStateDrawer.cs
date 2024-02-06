using UnityEngine;
using UnityEditor;
using ToolkitEngine.StatusFX;

namespace ToolkitEditor.StatusFX
{
	[CustomPropertyDrawer(typeof(StatusEffectState))]
	public class StatusEffectDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var statusEffectTypeProp = property.FindPropertyRelative("m_statusEffectType");
			EditorGUIRectLayout.PropertyField(ref position, statusEffectTypeProp);

			// Status effect type is undefined, skip
			if (statusEffectTypeProp.objectReferenceValue == null)
				return;

			var isAppliedProp = property.FindPropertyRelative("m_isApplied");
			if (!Application.isPlaying)
			{
				EditorGUIRectLayout.PropertyField(ref position, isAppliedProp, new GUIContent("Apply On Start"));
			}
			else
			{
				var effectState = property.GetValue<StatusEffectState>();
				EditorGUI.BeginDisabledGroup(true);

				position.height = EditorGUI.GetPropertyHeight(property);
				EditorGUI.Toggle(position, isAppliedProp.displayName, effectState.isApplied);
				position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

				EditorGUI.EndDisabledGroup();
			}

			EditorGUIRectLayout.Space(ref position);

			var onAppliedProp = property.FindPropertyRelative("m_onApplied");
			if (EditorGUIRectLayout.Foldout(ref position, onAppliedProp, "Events"))
			{
				EditorGUIRectLayout.PropertyField(ref position, onAppliedProp);

				var statusEffectType = (StatusEffectType)statusEffectTypeProp.objectReferenceValue;
				if (statusEffectType.stackMode == StackMode.Queued)
				{
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_onEnqueued"));
				}

				if (statusEffectType.durationMode != DurationMode.Instant)
				{
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_onUpdated"));
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_onRemoved"));
				}

				if (statusEffectType.durationMode == DurationMode.Timed)
				{
					EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_onExpired"));
				}
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var statusEffectTypeProp = property.FindPropertyRelative("m_statusEffectType");
			float height = EditorGUI.GetPropertyHeight(statusEffectTypeProp)
				+ EditorGUIUtility.standardVerticalSpacing;

			if (statusEffectTypeProp.objectReferenceValue != null)
			{
				height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_isApplied"))
					+ EditorGUIUtility.singleLineHeight
					+ EditorGUIRectLayout.GetSpaceHeight()
					+ (EditorGUIUtility.standardVerticalSpacing * 2f);

				var onAppliedProp = property.FindPropertyRelative("m_onApplied");
				if (onAppliedProp.isExpanded)
				{
					height += EditorGUI.GetPropertyHeight(onAppliedProp)
						+ EditorGUIUtility.standardVerticalSpacing;

					var statusEffectType = (StatusEffectType)statusEffectTypeProp.objectReferenceValue;
					if (statusEffectType.stackMode == StackMode.Queued)
					{
						height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_onEnqueued"))
							+ EditorGUIUtility.standardVerticalSpacing;
					}

					if (statusEffectType.durationMode != DurationMode.Instant)
					{
						height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_onUpdated"))
							+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_onRemoved"))
							+ (EditorGUIUtility.standardVerticalSpacing * 2f);
					}

					if (statusEffectType.durationMode == DurationMode.Timed)
					{
						height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_onExpired"))
							+ EditorGUIUtility.standardVerticalSpacing;
					}
				}
			}

			return height;
		}
	}
}