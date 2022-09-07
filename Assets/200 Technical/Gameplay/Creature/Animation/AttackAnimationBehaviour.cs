// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using System;
using UnityEngine;

namespace Necrovine.Creatures {
    public class AttackAnimationBehaviour : StateMachineBehaviour {
        #region Behaviour
        [NonSerialized] private Creature creature = null;
        [NonSerialized] private bool isInitialized = false;

        // -----------------------

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        /*public override void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex) {
            
        }*/

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex) {
            if (!isInitialized) {
                creature = _animator.GetComponentInParent<Creature>();
            }

            creature.OnStopAttack();
        }
        #endregion
    }
}
