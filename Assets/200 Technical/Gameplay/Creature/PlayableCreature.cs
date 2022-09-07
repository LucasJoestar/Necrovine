// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace Necrovine.Creatures {
    /// <summary>
    /// Singleton instance class used to control how the game player behaves.
    /// </summary>
    public class PlayableCreature : Creature {
        #region Global Members
        private const int MaxCombo = 3;

        [Section("Playable Creature")]

        [SerializeField, Enhanced, Range(0f, 5f)] private float comboDuration = .5f;

        [Space, HorizontalLine(SuperColor.Pumpkin), Space]

        [SerializeField, Enhanced, ReadOnly] private bool isPlayable = true;

        public bool CanCombo {
            get { return (comboIndex > 1) && !isAttacking; }
        }

        public bool IsPlayable {
            get { return isPlayable; }
        }

        [Space]

        [SerializeField, Enhanced, ReadOnly] private int comboIndex = 1;
        [SerializeField, Enhanced, ReadOnly] private float comboTimer = 0f;

        [Space]

        [SerializeField] private float[] comboCoefs = new float[3];

        [Space]

        [SerializeField, Enhanced, ReadOnly] private Vector3 originPosition = Vector3.zero;
        [SerializeField, Enhanced, ReadOnly] private Quaternion originRotation = Quaternion.identity;

        #endregion

        #region Animation
        private readonly int combo_Id = Animator.StringToHash("Combo");

        // -----------------------

        protected void PlayCombo(int _combo) {
            animator.SetInteger(combo_Id, _combo);
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            originPosition = Position;
            originRotation = Transform.rotation;
        }
        #endregion

        #region Attack
        protected override void DoAttack() {
            base.DoAttack();

            PlayCombo(comboIndex);
        }

        internal override void OnStopAttack() {
            base.OnStopAttack();

            hasAttackTarget = false;

            if (comboIndex != MaxCombo) {
                comboTimer = comboDuration;
                comboIndex = Mathf.Clamp(comboIndex + 1, 1, MaxCombo);
            } else {
                comboIndex = 1;
            }
        }

        protected override int GetDamages() {
            return (int)(base.GetDamages() * comboCoefs[comboIndex - 1]);
        }
        #endregion

        #region Health
        protected override void OnDeath() {
            base.OnDeath();

            isPlayable = false;
        }

        public override void ResetHealth() {
            base.ResetHealth();

            movable.SetPositionAndRotation(originPosition, originRotation);
            isPlayable = true;
        }
        #endregion

        #region Update
        protected override void OnUpdate() {
            base.OnUpdate();

            // Reset combo after some time.
            if (CanCombo) {
                comboTimer -= DeltaTime;

                if (comboTimer <= 0f) {
                    comboTimer = 0f;
                    comboIndex = 1;
                }
            }
        }
        #endregion
    }
}
