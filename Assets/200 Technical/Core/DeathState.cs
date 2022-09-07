// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedFramework.GameStates;
using Necrovine.UI;
using System;

namespace Necrovine.Core  {
	/// <summary>
	/// <see cref="GameState"/> for when the playable character died.
	/// </summary>
	[Serializable]
	public class DeathState : GameState {
		#region Global Members
		/// <summary>
		/// This is a backing state, so priority doesn't have to be high.
		/// </summary>
		public const int DeathStatePriority = 9;

		public override int Priority => DeathStatePriority;
		#endregion

		#region State Override
		public override void OnStateOverride(GameStateOverride _state) {
			base.OnStateOverride(_state);

			_state.HasControl = false;
			_state.CanPause = false;
		}
		#endregion

		#region Behaviour
		protected override void OnEnabled() {
			base.OnEnabled();

			DeathScreen.Instance.Show();
		}

		protected override void OnDisabled() {
			base.OnDisabled();

			DeathScreen.Instance.Hide();
		}
		#endregion
	}
}
