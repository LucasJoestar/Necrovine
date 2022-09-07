// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using DG.Tweening;
using EnhancedEditor;
using EnhancedFramework.Core;
using EnhancedFramework.GameStates;
using EnhancedFramework.SceneManagement;
using Necrovine.CameraManagement;
using Necrovine.UI;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Range = EnhancedEditor.RangeAttribute;

namespace Necrovine.Core  {
	public class NecrovineSceneManager : EnhancedSceneManager<NecrovineSceneManager, LoadingState> {
		#region Global Members
		[Section("Necrovine Scene Manager")]

		[SerializeField] private SceneBundle[] dungeonBundles = new SceneBundle[] { };

		[Space, HorizontalLine(SuperColor.Sapphire), Space]

		[SerializeField, Enhanced, Range(0f, 5f)] private float fadeInDuration = .5f;
		public Ease fadeInEase = Ease.OutSine;

        [Space]

		[SerializeField, Enhanced, Range(0f, 5f)] private float fadeOutDuration = .5f;
		public Ease fadeOutEase = Ease.OutSine;
		#endregion

		#region Behaviour
		private const float LoadDelay = .1f;
		private Action callback = null;
		private Tween tween = null;

		// -----------------------
		
		public void LoadDungeon(Action _callback = null, bool _isInstant = false) {
			if (tween.IsActive()) {
				tween.Kill();
			}

			FadeManager.Instance.Fade(1f, _isInstant ? 0f : fadeInDuration, fadeInEase);

			callback = _callback;
			tween = DOVirtual.DelayedCall(fadeInDuration + LoadDelay, DoLoadDungeon, false);
        }

		// -----------------------

		private void DoLoadDungeon() {
			CameraController.Instance.DisableController();

			var _opertation = LoadSceneBundleAsync(dungeonBundles.Random(), LoadSceneMode.Single);
			_opertation.OnCompleted += OnDungeonLoaded;

			NecrovineGameManager.Instance.OnStartLoading();
		}

		private void OnDungeonLoaded(LoadBundleAsyncOperation _operation) {
			FadeManager.Instance.Fade(0f, fadeOutDuration, fadeOutEase);

			callback?.Invoke();
			callback = null;

			CameraController.Instance.EnableController();
		}
        #endregion
    }
}
