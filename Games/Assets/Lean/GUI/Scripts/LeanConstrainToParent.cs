﻿using UnityEngine;
using Lean.Common;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lean.Gui
{
	/// <summary>This component will automatically constrain the current <b>RectTransform</b> to its parent.</summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanConstrainToParent")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Constrain To Parent")]
	public class LeanConstrainToParent : MonoBehaviour
	{
		[System.NonSerialized]
		private RectTransform cachedParentRectTransform;

		[System.NonSerialized]
		private RectTransform cachedRectTransform;

		protected virtual void OnEnable()
		{
			cachedRectTransform = GetComponent<RectTransform>();
		}

		protected virtual void LateUpdate()
		{
			if (cachedParentRectTransform != cachedRectTransform.parent)
			{
				cachedParentRectTransform = cachedRectTransform.parent as RectTransform;
			}

			if (cachedParentRectTransform != null)
			{
				var anchoredPosition = cachedRectTransform.anchoredPosition;
				var rect             = cachedRectTransform.rect;
				var boundary         = cachedParentRectTransform.rect;

				boundary.xMin -= rect.xMin;
				boundary.xMax -= rect.xMax;
				boundary.yMin -= rect.yMin;
				boundary.yMax -= rect.yMax;

				anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, boundary.xMin, boundary.xMax);
				anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, boundary.yMin, boundary.yMax);

				cachedRectTransform.anchoredPosition = anchoredPosition;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanConstrainToParent))]
	public class LeanConstrainToParent_Editor : LeanInspector<LeanConstrainToParent>
	{
		protected override void DrawInspector()
		{
		}
	}
}
#endif