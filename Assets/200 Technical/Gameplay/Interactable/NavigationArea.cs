// ===== Enhanced Framework - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ========================================================================== //

using EnhancedFramework.Core;
using Necrovine.Creatures;
using UnityEngine;

namespace Necrovine.Interactable  {
	/// <summary>
	/// Indicates this object can be used for navigation.
	/// </summary>
	public class NavigationArea : EnhancedBehaviour, IInteractable {
        InteractionType IInteractable.Type {
            get { return InteractionType.Move; }
        }

        #region Behaviour
        void IInteractable.Interact(PlayableCreature _player, Vector3 _position) {
            _player.NavigateTo(_position);
        }
        #endregion
    }
}
