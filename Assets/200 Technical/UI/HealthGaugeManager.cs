// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

namespace Necrovine.UI  {
	public class HealthGaugeManager : EnhancedSingleton<HealthGaugeManager> {
        #region Global Members
        [SerializeField, Enhanced, Required] private HealthGauge prefab = null;
        #endregion

        #region Instantiation
        public HealthGauge InstantiateGauge(Color _color) {
            HealthGauge _gauge = Instantiate(prefab, transform);
            _gauge.SetColor(_color);

            return _gauge;
        }
        #endregion
    }
}
