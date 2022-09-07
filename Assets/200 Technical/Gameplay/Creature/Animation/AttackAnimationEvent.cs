// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

namespace Necrovine.Creatures {
	public class AttackAnimationEvent : EnhancedBehaviour {
        #region Global Members
        [SerializeField, Enhanced, Required] private Creature creature = null;
        #endregion

        #region Enhanced Behaviour
        #if UNITY_EDITOR
        private void OnValidate() {
            // Editor required components validation.
            if (Application.isPlaying) {
                return;
            }

            if (!creature) {
                creature = gameObject.GetComponentInParent<Creature>();
            }
        }
        #endif
        #endregion

        #region Behaviour
        public void OnEvent() {
            creature.OnAttack();
        }
        #endregion
    }
}
