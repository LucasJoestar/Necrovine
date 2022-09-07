// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using DG.Tweening;
using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.UI;

using Range = EnhancedEditor.RangeAttribute;

namespace Necrovine.UI  {
	public class HealthGauge : EnhancedBehaviour {
		#region Global Members
		[Section("Health Gauge")]

		[SerializeField, Enhanced, Required] private CanvasGroup group = null;
		[SerializeField, Enhanced, Required] private Image background = null;
		[SerializeField, Enhanced, Required] private Image fill = null;
		[SerializeField, Enhanced, Required] private Image target = null;

		[Space]

		[SerializeField, Enhanced, Range(0f, 1f)] private float fillDuration = .2f;
		[SerializeField] private Ease fillEase = Ease.OutQuad;		
        #endregion

        #region Gauge
        private Tween tween = null;

		// -----------------------

		public void SetGaugeValue(float _percent) {
			if (tween.IsActive()) {
				tween.Kill();
			}

			// Hide the bar if full or empty.
			if ((_percent == 0f) || (_percent == 1f)) {
				group.alpha = 0f;
				fill.fillAmount = _percent;

				return;
			}

			group.alpha = 1f;
			tween = fill.DOFillAmount(_percent, fillDuration).SetEase(fillEase);
        }

		public void SetColor(Color _color) {
			background.color = _color;
			fill.color = _color;
        }

		public void EnableTarget(bool _enable) {
			target.enabled = _enable;
		}

		public void SetTransform(Transform _reference) {
			Transform _transform = transform;
			_transform.position = _reference.position;

			//_transform.rotation = Quaternion.LookRotation(_transform.forward, -_reference.forward);
        }

		// -----------------------

		public void Destroy() {
			if (tween.IsActive()) {
				tween.Kill();
			}

			Destroy(gameObject);
		}
		#endregion
	}
}
