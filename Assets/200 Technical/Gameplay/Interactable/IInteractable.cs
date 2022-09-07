// ===== Enhanced Framework - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ========================================================================== //

using Necrovine.Creatures;
using UnityEngine;

namespace Necrovine.Interactable  {
	/// <summary>
	/// All available interaction types.
	/// </summary>
	public enum InteractionType {
		Move,
		Attack,

		None = 31
	}

	/// <summary>
	/// Interface used for player interaction.
	/// </summary>
	public interface IInteractable {
		InteractionType Type { get; }

		void Interact(PlayableCreature _player, Vector3 _position);
	}
}
