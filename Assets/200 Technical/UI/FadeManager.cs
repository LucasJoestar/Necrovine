// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using DG.Tweening;
using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

namespace Necrovine.UI  {
	public class FadeManager : EnhancedSingleton<FadeManager> {
		#region Global Members
		[Section("Fade Manager")]

		[SerializeField, Enhanced, Required] private CanvasGroup group = null;
        #endregion

        #region Behaviour
        private Tween tween = null;

        // -----------------------

        public void Fade(float _alpha, float _duration = .5f, Ease _ease = Ease.OutSine) {
            if (tween.IsActive()) {
                tween.Kill();
            }

            tween = group.DOFade(_alpha, _duration).SetEase(_ease);
        }
        #endregion
    }
}
