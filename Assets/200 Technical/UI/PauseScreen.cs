// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

namespace Necrovine.UI  {
	public class PauseScreen : EnhancedSingleton<PauseScreen> {
		#region Global Members
		[Section("Pause Screen")]

		[SerializeField, Enhanced, Required] private CanvasGroup group = null;
        #endregion

        #region Behaviour
        public void Show() {
            group.alpha = 1f;
        }

        public void Hide() {
            group.alpha = 0f;
        }
        #endregion
    }
}
