// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedFramework.GameStates;
using System;

namespace Necrovine.Core  {
	/// <summary>
	/// <see cref="GameState"/> for when a cutscene is being played.
	/// </summary>
	[Serializable]
	public class CinematicState : GameState {
		#region Global Members
		/// <summary>
		/// Uses a high priority.
		/// </summary>
		public const int CinematicStatePriority = 50;

		public override int Priority => CinematicStatePriority;
		#endregion

		#region State Override
		public override void OnStateOverride(GameStateOverride _state) {
			base.OnStateOverride(_state);

			_state.HasControl = false;
			_state.CanPause = false;
		}
		#endregion
	}
}
