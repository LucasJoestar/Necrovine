// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using DG.Tweening;
using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace Necrovine.UI  {
	public class DeathScreen : EnhancedSingleton<DeathScreen> {
		#region Global Members
		[Section("Death Screen")]

		[SerializeField, Enhanced, Required] private CanvasGroup group = null;

        [Space]

        [SerializeField, Enhanced, Range(0f, 10f)] private float fadeDelay = 1f;
        [SerializeField, Enhanced, Range(0f, 10f)] private float fadeDuration = 2f;
        [SerializeField] private Ease fadeEase = Ease.OutSine;
        #endregion

        #region Behaviour
        private Tween tween = null;

        // -----------------------

        public void Show() {
            if (tween.IsActive()) {
                tween.Kill();
            }

            tween = group.DOFade(1f, fadeDuration).SetEase(fadeEase).SetDelay(fadeDelay);
        }

        public void Hide() {
            if (tween.IsActive()) {
                tween.Kill();
            }

            group.alpha = 0f;
        }
        #endregion
    }
}
