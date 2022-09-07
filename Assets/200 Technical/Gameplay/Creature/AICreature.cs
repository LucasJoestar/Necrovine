// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using EnhancedFramework.Core;
using EnhancedFramework.Physics3D;
using Necrovine.Interactable;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace Necrovine.Creatures  {
    /// <summary>
    /// Artificial intelligence class for both enemies and allies (but probably for enemies only).
    /// </summary>
	public class AICreature : Creature, IInteractable {
        InteractionType IInteractable.Type {
            get {
                return InteractionType.Attack;
            }
        }

        #region Global Members
        [Section("AI Creature")]

        [SerializeField] private TagGroup attackTags = new TagGroup();
        [SerializeField] private LayerMask detectionMask = new LayerMask();
        [SerializeField, Enhanced, Range(0f, 20f)] private float detectionRange = 5f;
        #endregion

        #region Enhanced Behaviour
        private void OnDrawGizmosSelected() {
            using (var _scope = EnhancedGUI.GizmosColor.Scope(SuperColor.Lime.Get())) {
                Gizmos.DrawWireSphere(Position, detectionRange);
            }
        }
        #endregion

        #region Interactable
        void IInteractable.Interact(PlayableCreature _player, Vector3 _position) {
            _player.Attack(this);
        }
        #endregion

        #region Update
        private static readonly Collider[] detectionBuffer = new Collider[6];
        private static readonly Vector2 detectionInterval = new Vector2(.4f, .84f);
        private float detectionTimer = 0f;

        // -----------------------

        protected override void OnUpdate() {
            base.OnUpdate();

            // Detect surrounding creatures after each interval while not attacking.
            if (!isAttacking) {
                detectionTimer -= DeltaTime;

                if (detectionTimer <= 0f) {
                    DetectCreatures();
                    detectionTimer = detectionInterval.Random();
                }
            }
        }

        private void DetectCreatures() {
            int _amount = Physics.OverlapSphereNonAlloc(Position, detectionRange, detectionBuffer, detectionMask, QueryTriggerInteraction.Ignore);
            Physics3DUtility.SortCollidersByDistance(detectionBuffer, _amount, Position);

            for (int i = 0; i < _amount; i++) {
                // Get the target with the highest priority.
                if (detectionBuffer[i].TryGetComponent(out IAttackable _attackable) && _attackable.Tags.ContainsAny(attackTags)) {
                    if (!hasAttackTarget || (attackTarget != _attackable)) {
                        Attack(_attackable);
                    }

                    break;
                }
            }
        }
        #endregion
    }
}
