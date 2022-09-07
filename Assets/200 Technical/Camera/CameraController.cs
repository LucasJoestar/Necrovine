// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using EnhancedFramework.Core;
using System.Collections.Generic;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace Necrovine.CameraManagement  {
	public class CameraController : EnhancedSingleton<CameraController>, ILateUpdate {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Late;

        #region Global Members
        [Section("Camera Controller")]

		[SerializeField, Enhanced, Required] private new Camera camera = null;
		[SerializeField, Enhanced, Required] private Transform target = null;

        [Space]

        [SerializeField, Enhanced, Range(0f, 1f)] private float followSpeed = .1f;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 15f, -10f);

        [Space]

        [SerializeField] private LayerMask transparencyRayMask = new LayerMask();
		[SerializeField, Enhanced, Range(0f, 1f)] private float transparency = .2f;

        [Space, HorizontalLine(SuperColor.Raspberry), Space]

        [SerializeField, Enhanced, ReadOnly] private List<Transform> importantObjects = new List<Transform>();
        [SerializeField, Enhanced, ReadOnly] private List<TransparentObject> transparentObjects = new List<TransparentObject>();
        #endregion

        #region Update
        private const float MaxRayDistance = 99f;
        private readonly RaycastHit[] raycastBuffer = new RaycastHit[8];

        // -----------------------

        void ILateUpdate.Update() {
            foreach (TransparentObject _object in transparentObjects) {
                _object.ResetState();
            }

            // Cast a ray from each object to the camera, and register transparent obstructing objects.
            Vector3 _from = camera.transform.position;

            foreach (Transform _object in importantObjects) {
                int _amount = Physics.RaycastNonAlloc(_from, (_object.position - _from).normalized, raycastBuffer, MaxRayDistance, transparencyRayMask, QueryTriggerInteraction.Ignore);

                for (int i = 0; i < _amount; i++) {

                    if (raycastBuffer[i].collider.TryGetComponent(out TransparentObject _obstacle)) {
                        _obstacle.SetTransparent(transparency);

                        if (!transparentObjects.Contains(_obstacle)) {
                            transparentObjects.Add(_obstacle);
                        }
                    }
                }
            }

            // Update transparent objects.
            for (int i = transparentObjects.Count; i-- > 0;) {
                if (!transparentObjects[i].UpdateTransparency()) {
                    transparentObjects.RemoveAt(i);
                }
            }

            // Follow player.
            float _deltaTime = DeltaTime;
            if (_deltaTime != 0f) {
                _deltaTime = Mathf.Clamp(_deltaTime / (1f / 24f), 0f, 1f);
            }

            camera.transform.position = Vector3.MoveTowards(camera.transform.position, target.position + cameraOffset, _deltaTime * followSpeed);
        }
        #endregion

        #region Transparency
        /// <summary>
        /// Important objects are always visible from the camera, even when something should be hidding them.
        /// </summary>
        public void RegisterImportantObject(Transform _object) {
            importantObjects.Add(_object);
        }

        public void UnregisterImportantObject(Transform _object) {
            importantObjects.Remove(_object);
        }
        #endregion
    }
}
