// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Necrovine.UI  {
	/// <summary>
	/// All available cursor icons.
	/// </summary>
	public enum CursorIcon {
		Default = 0,
		Move,
		Attack,
		AttackCombo,

		None = 31
	}

	/// <summary>
	/// Virtual cursor behaviour class.
	/// </summary>
	public class GameCursor : EnhancedBehaviour {
		#region Global Members
		[Section("Game Cursor")]

		[SerializeField, Enhanced, Required] private RectTransform rectTransform = null;
		[SerializeField, Enhanced, Required] private Image image = null;

        [Space]

		[SerializeField, Enhanced, Required] private RectTransform anchor = null;

        [Space, HorizontalLine(SuperColor.Raspberry), Space]

		[SerializeField, Enhanced, Required] private Sprite defaultIcon		= null;
		[SerializeField, Enhanced, Required] private Sprite moveIcon		= null;
		[SerializeField, Enhanced, Required] private Sprite attackIcon		= null;
		[SerializeField, Enhanced, Required] private Sprite attackComboIcon	= null;
		[SerializeField, Enhanced, Required] private Sprite noneIcon		= null;
        #endregion

        #region Position
		public void OffsetScreenPosition(Vector2 _offset) {
			SetScreenPosition(GetScreenPosition() + _offset);
		}

		public void SetScreenPosition(Vector2 _position) {
			_position.x = Mathf.Clamp(_position.x, 0f, anchor.rect.width);
			_position.y = Mathf.Clamp(_position.y, 0f, anchor.rect.height);

			rectTransform.anchoredPosition = _position;
		}

		public Vector2 GetScreenPosition() {
			return rectTransform.anchoredPosition;
        }

		public Vector2 GetViewportPosition() {
			Vector2 _position = GetScreenPosition();

			_position.x /= anchor.rect.width;
			_position.y /= anchor.rect.height;

			return _position;
		}
        #endregion

        #region Icon
        public void SetIcon(CursorIcon _icon) {
			switch (_icon) {
				case CursorIcon.Default:
					image.sprite = defaultIcon;
					break;

				case CursorIcon.Move:
					image.sprite = moveIcon;
					break;

				case CursorIcon.Attack:
					image.sprite = attackIcon;
					break;

				case CursorIcon.AttackCombo:
					image.sprite = attackComboIcon;
					break;

				case CursorIcon.None:
					image.sprite = noneIcon;
					break;

				default:
					break;
			}
		}
        #endregion
    }
}
