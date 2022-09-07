// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using UnityEngine;

namespace Necrovine.Interactable  {
	public interface IAttackable {
        #region Content
        string Name { get; }

		int Health { get; }

		bool IsDead { get; }

		float Radius { get; }

		Vector3 Position { get; }

		TagGroup Tags { get; }

		void TakeDamage(int _damages);
        #endregion
    }
}
