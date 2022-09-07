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
	/// <see cref="Creature"/>-related attributes.
	/// </summary>
	[CreateAssetMenu(fileName = "ATB_CreatureAttributes", menuName = "Necrovine/Creature/Creature Attributes", order = 100)]
	public class CreatureAttributes : ScriptableObject {
		#region Global Members
		[Section("Creature Attributes")]

		public LayerMask CollisionMask = new LayerMask();
		public LayerMask TriggerMask = new LayerMask();

        [Space]

		[Enhanced, Range(0f, 100f)] public float RotationSpeed = 1f;
		[Enhanced, Range(0f, 5f)] public float Radius = .5f;

		[Section("Combat")]

		[Enhanced, Range(0f, 100f)] public int MaxHealth = 100;
		[Enhanced, MinMax(0f, 10f)] public Vector2 AttackCooldown = new Vector2(.1f, .1f);

		[Space]

		[Enhanced, Range(0f, 10f)] public float AttackDistance = .2f;
		[Enhanced, Range(0f, 100f)] public int AttackDamages = 10;
		#endregion
	}
}
