using System;
using System.Collections.Generic;
using ToolkitEngine.StatusFX;
using UnityEditor;

namespace ToolkitEditor.StatusFX.VisualScripting
{
	[InitializeOnLoad]
	public static class Setup
	{
		static Setup()
		{
			var types = new List<Type>()
			{
				typeof(StatusEffectType),
				typeof(StatusEffectState),
				typeof(StatusEffect),
				typeof(StatusEffectControl),
				typeof(StatusEffectEventArgs),
			};

			ToolkitEditor.VisualScripting.Setup.Initialize("ToolkitEngine.StatusFX", types);
		}
	}
}