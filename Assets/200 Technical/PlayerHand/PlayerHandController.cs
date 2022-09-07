// ===== Enhanc// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using EnhancedFramework.Core;
using EnhancedFramework.Input;
using EnhancedFramework.Physics3D;
using Necrovine.Creatures;
using Necrovine.Interactable;
using Necrovine.UI;
using UnityEngine;

namespace Necrovine.Player {
	/// <summary>
	/// Player controller of the game, acting through a god interact to interact with the world.
	/// </summary>
	public class PlayerHandController : EnhancedSingleton<PlayerHandController>, IInputUpdate {
		public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init | UpdateRegistration.Input;

        #region Global Members
        [Section("Cursor Controller")]

		[SerializeField, Enhanced, Required] private PlayerHandControllerAttributes attributes = null;
		[SerializeField, Enhanced, Required] private PlayableCreature player = null;
		[SerializeField, Enhanced, Required] private GameCursor cursor = null;
		[SerializeField, Enhanced, Required] private new Camera camera = null;

        [Space]

		[SerializeField, Enhanced, Required] private BaseInputAsset cursorInput = null;
		[SerializeField, Enhanced, Required] private BaseInputAsset actionInput = null;
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
        }
        #endregion

        #region Cursor Behaviour
        private readonly RaycastHit[] hitBuffer = new RaycastHit[6];

		// -----------------------

		void IInputUpdate.Update() {
			// Cursor displacement.
			Vector2 _delta = cursorInput.GetVector2Axis();
			if (!_delta.IsNull()) {
				cursor.OffsetScreenPosition(_delta * Time.unscaledDeltaTime * attributes.Speed);
            }

			if (!player.IsPlayable) {
				cursor.SetIcon(CursorIcon.None);
				return;
			}

			// Get interacting behaviour.
			Vector3 _cursorPosition = cursor.GetViewportPosition();

			Ray _ray = camera.ViewportPointToRay(_cursorPosition);
			int _amount = Physics.RaycastNonAlloc(_ray, hitBuffer, attributes.RayMaxDistance, attributes.Layer, QueryTriggerInteraction.Collide);

			if (_amount != 0) {
				Physics3DUtility.SortRaycastHitByDistance(hitBuffer, _amount);

				for (int i = 0; i < _amount; i++) {
					RaycastHit _hit = hitBuffer[i];

					if (_hit.collider.TryGetComponent(out IInteractable _interactable)) {
						Interact(_interactable, _hit);
						return;
					}
				}

				if (actionInput.Performed()) {
					player.NavigateTo(hitBuffer[0].point);
				}
			}

			cursor.SetIcon(CursorIcon.None);
		}

		private void Interact(IInteractable _interactable, RaycastHit _hit) {
            switch (_interactable.Type) {
                case InteractionType.Move:
					cursor.SetIcon(CursorIcon.Move);
					break;

                case InteractionType.Attack:
					cursor.SetIcon(player.CanCombo ? CursorIcon.AttackCombo : CursorIcon.Attack);
                    break;

                case InteractionType.None:
					cursor.SetIcon(CursorIcon.None);
                    break;

                default:
                    break;
            }
			
			if (actionInput.Performed()) {
				_interactable.Interact(player, _hit.point);
            }
        }
        #endregion
	}
}
