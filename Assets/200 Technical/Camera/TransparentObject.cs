// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

namespace Necrovine.CameraManagement  {
    /// <summary>
    /// Class for transparent objects when hiding something.
    /// </summary>
	public class TransparentObject : EnhancedBehaviour {
        public enum TransparencyState {
            Opaque,
            Pending,
            Transparent
        }

		#region Global Members
		[Section("Transparent Object")]

		[SerializeField] private Renderer[] renderers = new Renderer[] { };
        [SerializeField, Enhanced, ReadOnly] private TransparencyState state = TransparencyState.Opaque;
        #endregion

        #region Enhanced Behaviour
        #if UNITY_EDITOR
        private void OnValidate() {
            if (renderers.Length == 0) {
                GetRenderers();
            }
        }
        #endif

        [ContextMenu("Get Renderers", false, 10)]
        private void GetRenderers() {
            renderers = GetComponentsInChildren<Renderer>();
        }
        #endregion

        #region Transparency
        private int color_Id = Shader.PropertyToID("_MainColor");

        // -----------------------

        public void ResetState() {
            state = TransparencyState.Pending;
        }

        public void SetTransparent(float _alpha) {
            if (state == TransparencyState.Opaque) {
                SetTransparency(_alpha);
            }

            state = TransparencyState.Transparent;
        }

        public bool UpdateTransparency() {
            if (state == TransparencyState.Pending) {
                // Reset alpha back to 1.
                SetTransparency(1f);
                state = TransparencyState.Opaque;

                return false;
            }

            return true;
        }

        // -----------------------

        private void SetTransparency(float _alpha) {
            foreach (Renderer _renderer in renderers) {
                foreach (Material _material in _renderer.materials) {
                    _material.SetColor(color_Id, _material.GetColor(color_Id).SetAlpha(_alpha));
                }
            }
        }
        #endregion
    }
}
