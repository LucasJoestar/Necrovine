// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //
using EnhancedEditor;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace Necrovine.Player {
	/// <summary>
	/// <see cref="PlayerHandController"/>-related attributes.
	/// </summary>
	[CreateAssetMenu(fileName = "ATB_PlayerHandControllerAttributes", menuName = "Necrovine/Player/Player Hand Controller Attributes", order = 100)]
	public class PlayerHandControllerAttributes : ScriptableObject {
		#region Global Members
		[Section("Settings")]

		public LayerMask Layer = new LayerMask();

        [Space]

		[Enhanced, Range(.1f, 999f)] public float RayMaxDistance = 10f;
		[Enhanced, Range(.1f, 9999f)] public float Speed = 10f;
		#endregion
	}
}
