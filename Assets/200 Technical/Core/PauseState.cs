// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedFramework.GameStates;
using Necrovine.UI;
using UnityEngine;

namespace Necrovine.Core  {
	public class PauseState : GameState {
        #region Global Members
        /// <summary>
        /// Make sure this state is in front of all other gameplay states.
        /// </summary>
        public const int PauseStatePriority = 99;

        public override int Priority => PauseStatePriority;
        #endregion

        #region State Override
        public override void OnStateOverride(GameStateOverride _state) {
            base.OnStateOverride(_state);

            _state.IsPaused = true;
        }
        #endregion

        #region Behaviour
        protected override void OnEnabled() {
            base.OnEnabled();

            Time.timeScale = 0f;
            PauseScreen.Instance.Show();
        }

        protected override void OnDisabled() {
            base.OnDisabled();

            Time.timeScale = 1f;
            PauseScreen.Instance.Hide();
        }
        #endregion
    }
}
