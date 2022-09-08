// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using DG.Tweening;
using EnhancedEditor;
using EnhancedFramework.Core;
using EnhancedFramework.GameStates;
using Necrovine.Creatures;
using Necrovine.UI;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace Necrovine.Core  {
	public class NecrovineGameManager : EnhancedSingleton<NecrovineGameManager> {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Necrovine Game Manager")]

        [SerializeField] private FlagReference gameOverFlag = new FlagReference();
        [SerializeField] private FlagReference dungeonCompleteFlag = new FlagReference();

        [Space]

        [SerializeField, Enhanced, Required] private PlayableCreature player = null;

        [Space]

        [SerializeField, Enhanced, Range(0f, 10f)] private float gameOverLoadDelay = 5f;
        [SerializeField, Enhanced, Range(0f, 10f)] private float completeDungeonLoadDelay = 5f;
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            // Load the first dungeon on builds.
            #if !UNITY_EDITOR
            NecrovineSceneManager.Instance.LoadDungeon(null, true);
            #endif

            gameOverFlag.Flag.OnValueChanged += OnGameOver;
            dungeonCompleteFlag.Flag.OnValueChanged += OnCompleteDungeon;

            Application.targetFrameRate = 24;
        }
        #endregion

        #region Flags
        private void OnGameOver(bool _isGameOver) {
            if (_isGameOver) {
                GameOver();
                gameOverFlag.Flag.Invert();
            }
        }

        private void OnCompleteDungeon(bool _isComplete) {
            if (_isComplete) {
                CompleteDungeon();
                dungeonCompleteFlag.Flag.Invert();
            }
        }
        #endregion

        #region Dungeon
        private DeathState deathState = null;
        private Tween loadTween = null;

        // -----------------------

        public void GameOver() {
            if (loadTween.IsActive()) {
                loadTween.Kill();
            }

            deathState = GameState.CreateState<DeathState>();
            loadTween = DOVirtual.DelayedCall(gameOverLoadDelay, LoadDungeon, false);
        }

        public void CompleteDungeon() {
            if (loadTween.IsActive()) {
                loadTween.Kill();
            }

            loadTween = DOVirtual.DelayedCall(completeDungeonLoadDelay, LoadDungeon, false);
            SuccessScreen.Instance.Show();
        }

        public void OnStartLoading() {
            SuccessScreen.Instance.Hide();

            player.ResetHealth();

            if (deathState != null) {
                deathState.DestroyState();
                deathState = null;
            }
        }

        // -----------------------

        private void LoadDungeon() {
            NecrovineSceneManager.Instance.LoadDungeon(OnDungeonLoaded);
        }

        private void OnDungeonLoaded() {

        }
        #endregion
    }
}
