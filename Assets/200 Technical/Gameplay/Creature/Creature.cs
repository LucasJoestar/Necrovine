// ===== Necrovine - https://github.com/LucasJoestar/Necrovine ===== //
//
// Notes:
//
// ================================================================= //

using EnhancedEditor;
using EnhancedFramework.Core;
using EnhancedFramework.Movable3D;
using Necrovine.CameraManagement;
using Necrovine.Interactable;
using Necrovine.UI;
using UnityEngine;
using UnityEngine.AI;

namespace Necrovine.Creatures  {
    /// <summary>
    /// Base class for creatures of the game, moving using Unity nav mesh system.
    /// </summary>
    public class Creature : EnhancedBehaviour, IMovable3DController, IAttackable {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        public override Transform Transform => movable.Transform;

        #region Attackable
        public string Name {
            get { return name; }
        }

        public Vector3 Position {
            get { return Transform.position; }
        }

        public int Health {
            get { return health; }
        }

        public float Radius {
            get { return attributes.Radius; }
        }

        public TagGroup Tags {
            get { return extended.Tags; }
        }
        #endregion

        #region Collision System
        CollisionSystem3DType IMovable3DController.CollisionType {
            get { return CollisionSystem3DType.Creature; }
        }

        int IMovable3DController.CollisionMask {
            get { return attributes.CollisionMask; }
        }

        int IMovable3DController.TriggerMask {
            get { return attributes.TriggerMask; }
        }
        #endregion

        #region Global Members
        [Section("Creature")]

        [SerializeField, Enhanced, Required] protected CreatureAttributes attributes = null;
        [SerializeField, Enhanced, Required] protected ControlledMovable3D movable = null;
        [SerializeField, Enhanced, Required] protected Animator animator = null;
        [SerializeField, Enhanced, Required] protected ExtendedBehaviour extended = null;
        [SerializeField, Enhanced, Required] protected new AudioSource audio = null;

        public ControlledMovable3D Movable => movable;

        [Space]

        [SerializeField] private FlagValueGroup onDeathFlags = new FlagValueGroup();

        [Space]

        [SerializeField] protected SuperColor gaugeColor = SuperColor.Crimson;
        [SerializeField, Enhanced, ReadOnly] protected HealthGauge gauge = null;

        [Space]

        [SerializeField] private Behaviour[] coreBehaviours = new Behaviour[] { };

        [Space, HorizontalLine(SuperColor.Crimson), Space]

        [SerializeField, Enhanced, ProgressBar("MaxHealth", SuperColor.Crimson, true)] protected int health = 50;

        [Space]

        [SerializeField, Enhanced, ReadOnly] protected bool hasAttackTarget = false;
        [SerializeField, Enhanced, ReadOnly] protected bool isAttacking = false;
        [SerializeField, Enhanced, ReadOnly] protected bool isHit = false;
        [SerializeField, Enhanced, ReadOnly] protected bool isDead = false;

        [Space]

        [SerializeField, Enhanced, ReadOnly] protected float attackTimer = 0f;

        public int MaxHealth {
            get {
                #if UNITY_EDITOR
                if (attributes == null) {
                    return 1;
                }
                #endif

                return attributes.MaxHealth;
            }
        }

        public bool IsDead {
            get { return isDead; }
        }

        [Space, HorizontalLine(SuperColor.Cyan), Space]

        #if UNITY_EDITOR
        [SerializeField] private SuperColor gizmosColor = SuperColor.Turquoise;
        #endif

        [SerializeField, Enhanced, ReadOnly] protected Vector3[] navigationPath = new Vector3[8];

        [SerializeField, Enhanced, ReadOnly] protected int pathCount = 0;
        [SerializeField, Enhanced, ReadOnly] protected int pathIndex = 0;

        [Space]

        [SerializeField, Enhanced, ReadOnly] protected float movingToTargetTimer = 0f;
        [SerializeField, Enhanced, ReadOnly] protected Vector3 targetInitialPosition = Vector3.zero;

        public bool IsMoving => (pathCount != 0) && !isHit;

        // -----------------------

        protected NavMeshPath path = null; // Not allowed to create a new instance on initialization.
        #endregion

        #region Animation
        private readonly int speed_Id = Animator.StringToHash("Speed");
        private readonly int attack_Id = Animator.StringToHash("Attack");
        private readonly int hit_Id = Animator.StringToHash("Hit");
        private readonly int death_Id = Animator.StringToHash("Death");

        private readonly int idle_Id = Animator.StringToHash("Idle");

        // -----------------------

        protected void PlaySpeed(float _speed) {
            animator.SetFloat(speed_Id, _speed);
        }

        protected void PlayAttack() {
            isAttacking = true;
            animator.SetTrigger(attack_Id);
        }

        protected void CancelAttack() {
            if (isAttacking) {
                OnStopAttack();
                animator.ResetTrigger(attack_Id);
            }
        }

        protected void PlayHit() {
            isHit = true;
            animator.SetTrigger(hit_Id);
        }

        protected void CancelHit() {
            if (isHit) {
                OnStopHit();
                animator.ResetTrigger(hit_Id);
            }
        }

        protected void PlayDeath() {
            isDead = true;
            animator.SetTrigger(death_Id);
        }

        // -----------------------

        protected void ResetAnimator() {
            PlaySpeed(0f);
            CancelAttack();
            CancelHit();

            animator.ResetTrigger(death_Id);
            animator.Play(idle_Id);
        }
        #endregion

        #region Audio

        #endregion

        #region Enhanced Behaviour
        private const float PathGizmosSize = .25f;

        // -----------------------

        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            CameraController.Instance.RegisterImportantObject(Transform);
        }

        protected override void OnInit() {
            base.OnInit();

            path = new NavMeshPath();
            movable.Controller = this;

            gauge = HealthGaugeManager.Instance.InstantiateGauge(gaugeColor.Get());
            UpdateGauge();
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            CameraController.Instance.UnregisterImportantObject(Transform);
        }

        private void OnDestroy() {
            if (!GameManager.IsQuittingApplication) {
                gauge.Destroy();
            }
        }

        protected virtual void OnDrawGizmos() {
            #if UNITY_EDITOR
            if (pathCount != 0) {
                using (var _scope = EnhancedGUI.GizmosColor.Scope(gizmosColor.Get())) {
                    for (int i = 1; i < pathCount; i++) {
                        DrawPath(navigationPath[i - 1], navigationPath[i]);
                    }
                }
            }

             // ----- Local Method ----- \\

            void DrawPath(Vector3 _from, Vector3 _to) {
                Gizmos.DrawSphere(_to, PathGizmosSize);
                Gizmos.DrawLine(_from, _to);
            }
            #endif
        }

        #if UNITY_EDITOR
        private void OnValidate() {
            // Editor required components validation.
            if (Application.isPlaying) {
                return;
            }

            if (!movable) {
                movable = gameObject.AddComponentIfNone<ControlledMovable3D>();
            }

            if (!audio) {
                audio = gameObject.AddComponentIfNone<AudioSource>();
            }

            if (!extended) {
                extended = gameObject.GetComponent<ExtendedBehaviour>();
            }
        }
        #endif
        #endregion

        // --- Creature --- \\

        #region Navigation
        protected const float MaxNavMeshDistance = 9f;
        protected const float MinDestinationDistance = .1f;

        // -----------------------

        public void NavigateTo(Vector3 _position, bool _cancelAttack = true) {
            if (_cancelAttack && hasAttackTarget) {
                hasAttackTarget = false;
            }

            path.ClearCorners();
            pathIndex = 0;

            // Stop moving if new destination cannot be reached.
            if (!NavMesh.SamplePosition(_position, out NavMeshHit _hit, MaxNavMeshDistance, NavMesh.AllAreas) || !NavMesh.CalculatePath(Transform.position, _hit.position, NavMesh.AllAreas, path)) {
                movable.ResetSpeed();
                pathCount = 0;

                return;
            }

            pathCount = path.GetCornersNonAlloc(navigationPath);

            // Expand path buffer if not large enough.
            if (navigationPath.Length < pathCount) {
                navigationPath = new Vector3[pathCount];
                path.GetCornersNonAlloc(navigationPath);
            }
        }

        public void CancelNavigation() {
            if (pathCount == 0) {
                return;
            }

            path.ClearCorners();
            pathIndex = pathCount
                      = 0;

            movable.ResetSpeed();
        }

        protected virtual void OnReachDestination() {
            movable.ResetSpeed();

            if (hasAttackTarget) {
                Attack();
            }
        }
        #endregion

        #region Attack
        protected const float AttackDistanceCoef = 2f;
        protected IAttackable attackTarget = null;

        // -----------------------

        public void Attack(IAttackable _target) {
            if (isAttacking) {
                return;
            }

            hasAttackTarget = true;
            attackTarget = _target;

            Vector3 _targetPosition = _target.Position;
            Vector3 _direction = (_targetPosition - Position).Flat();
            float _distance = _direction.AbsSum();
            float _attackDistance = _target.Radius + Radius + attributes.AttackDistance;

            // Navigate to the opponent if too far.
            if (_distance > (_attackDistance * AttackDistanceCoef)) {
                Vector3 _destination = _targetPosition - (_direction.normalized * _attackDistance);
                NavigateTo(_destination, false);

                movingToTargetTimer = 0f;
                targetInitialPosition = _targetPosition;

                return;
            }

            CancelNavigation();
            DoAttack();
        }

        protected void Attack() {
            Attack(attackTarget);
        }

        protected virtual void DoAttack() {
            if (isAttacking || (attackTimer != 0f)) {
                return;
            }

            PlayAttack();
        }

        // -----------------------

        protected virtual int GetDamages() {
            return attributes.AttackDamages;
        }

        internal void OnAttack() {
            // Fail attack if the opponent is too far.
            float _distance = (attackTarget.Position - Position).Flat().AbsSum();
            float _attackDistance = attackTarget.Radius + Radius + attributes.AttackDistance;

            if (_distance > (_attackDistance * AttackDistanceCoef)) {
                this.Log($"{Name} failed to attack {attackTarget.Name}");
                return;
            }

            // Deal damages.
            int _damages = GetDamages();
            attackTarget.TakeDamage(_damages);

            this.Log($"{Name} attacks {attackTarget.Name} and deals {_damages} damages");
        }

        internal virtual void OnStopAttack() {
            isAttacking = false;
            attackTimer = attributes.AttackCooldown.Random();
        }
        #endregion

        #region Health
        [Button(SuperColor.Crimson)]
        public virtual void TakeDamage(int _damages) {
            if (isDead) {
                return;
            }

            health -= _damages;

            if (health <= 0) {
                OnDeath();
            } else if (!isAttacking) {
                OnHit();
            }

            UpdateGauge();
        }

        [Button(SuperColor.Green)]
        public virtual void Heal(int _heal) {
            health = Mathf.Min(health + _heal, MaxHealth);
            UpdateGauge();
        }

        public virtual void ResetHealth() {
            CancelAttack();
            CancelHit();
            CancelNavigation();

            ResetAnimator();

            health = MaxHealth;
            isDead = false;
            hasAttackTarget = false;

            foreach (Behaviour _behaviour in coreBehaviours) {
                _behaviour.enabled = true;
            }

            UpdateGauge();
        }

        // -----------------------

        protected virtual void OnHit() {
            movable.ResetSpeed();
            PlayHit();
        }

        protected virtual void OnDeath() {
            health = 0;
            PlayDeath();

            CancelAttack();
            CancelHit();
            CancelNavigation();

            // Disable core components.
            foreach (Behaviour _behaviour in coreBehaviours) {
                _behaviour.enabled = false;
            }

            // Update flags.
            foreach (FlagValue _flag in onDeathFlags.Flags) {
                _flag.SetFlag();
            }
        }

        // -----------------------

        internal virtual void OnStopHit() {
            isHit = false;
        }

        protected void UpdateGauge() {
            gauge.SetGaugeValue((float)health / MaxHealth);
        }
        #endregion

        #region Rotation
        protected void RotateTo(Vector3 _forward) {
            Quaternion _from = transform.rotation;
            Quaternion _to = Quaternion.LookRotation(_forward, Transform.up);

            if (_from != _to) {
                _to = Quaternion.RotateTowards(_from, _to, DeltaTime * attributes.RotationSpeed * 100f);
                movable.SetRotation(_to);
            }
        }
        #endregion

        // --- Movable --- \\

        #region Velocity
        bool IMovable3DController.OnResetVelocity() {
            return true;
        }
        #endregion

        #region Speed
        bool IMovable3DController.OnDoIncreaseSpeed(out bool _doIncrease) {
            // Increase speed while the creature is moving.
            _doIncrease = IsMoving;
            return false;
        }

        bool IMovable3DController.OnDecreaseSpeed(float _coef) {
            // Speed is never decreased, so it doesn't matter.
            return true;
        }

        bool IMovable3DController.OnResetSpeed() {
            return true;
        }
        #endregion

        #region Update
        private const float RecalculateTargetPathTime = .1f;
        private const float RecalculateTargetPathMinDistance = .25f;

        // -----------------------

        void IMovable3DController.OnPreUpdate() {
            OnUpdate();
        }

        protected virtual void OnUpdate() {
            if (hasAttackTarget) {
                // Recalculate target path after some times if moved.
                if (IsMoving) {
                    movingToTargetTimer += DeltaTime;

                    if (movingToTargetTimer >= RecalculateTargetPathTime) {
                        movingToTargetTimer = 0f;

                        Vector3 _position = attackTarget.Position;
                        if ((_position - targetInitialPosition).Flat().AbsSum() > RecalculateTargetPathMinDistance) {
                            Attack();
                            this.Log($"{Name} recalculate path to {attackTarget.Name}");
                        }
                    }
                } else {
                    // Rotate towards target.
                    RotateTo(attackTarget.Position - Position);
                }
            }

            // Attack cooldown.
            if (attackTimer != 0f) {
                attackTimer -= DeltaTime;

                if (attackTimer <= 0f) {
                    attackTimer = 0f;

                    if (hasAttackTarget) {
                        Attack();
                    }
                }
            }
        }

        void IMovable3DController.OnPostUpdate() { }
        #endregion

        #region Gravity
        bool IMovable3DController.OnApplyGravity() {
            return true;
        }
        #endregion

        #region Computing
        bool IMovable3DController.OnComputeVelocity(Velocity _velocity, out FrameVelocity _frameVelocity) {
            if (IsMoving) {
                // Clamp velocity.
                Vector3 _destination = navigationPath[pathIndex];
                Vector3 _movement = Vector3.ClampMagnitude((_destination - Transform.position).Flat(), movable.Speed * movable.VelocityCoef * DeltaTime);

                if (!_movement.IsNull()) {
                    _velocity.Instant = _movement;
                    RotateTo(_movement);
                }
            }

            PlaySpeed(movable.GetSpeedRatio());

            // We only use Instance velocity to move for now, so leave the behaviour as it is in case of using force and movement later.
            _frameVelocity = default;
            return true;
        }
        #endregion

        #region Collision Callbacks
        bool IMovable3DController.SetGroundState(ref bool _isGrounded, RaycastHit _hit) {
            return true;
        }

        bool IMovable3DController.OnAppliedVelocity(ref FrameVelocity _velocity, CollisionInfos _infos) {
            if (pathCount != 0) {
                Vector3 _distance = (navigationPath[pathIndex] - Transform.position).Flat();

                // Next path step.
                if (_distance.AbsSum() < MinDestinationDistance) {
                    if (pathIndex == (pathCount - 1)) {
                        pathIndex = pathCount
                                  = 0;

                        OnReachDestination();
                    } else {
                        pathIndex++;
                    }
                }
            }

            return true;
        }

        bool IMovable3DController.OnRefreshedObject(ref FrameVelocity _velocity, CollisionInfos _infos) {
            gauge.SetTransform(Transform);
            return true;
        }

        bool IMovable3DController.OnSetGrounded(bool _isGrounded) {
            return true;
        }

        bool IMovable3DController.OnSetMoving(bool isMoving) {
            return true;
        }

        bool IMovable3DController.OnReachedMaxSpeed(bool _hasReachedMaxSpeed) {
            return true;
        }

        bool IMovable3DController.IsMoving(FrameVelocity _velocity, CollisionInfos _infos, out bool _isMoving) {
            _isMoving = IsMoving;
            return false;
        }
        #endregion

        #region Overlapping & Triggers
        bool IMovable3DController.OnExtractFromCollider(Collider _collider, Vector3 _direction, float _distance) {
            return true;
        }

        void IMovable3DController.OnEnterTrigger(Trigger3D _trigger) { }

        void IMovable3DController.OnExitTrigger(Trigger3D _trigger) { }
        #endregion
    }
}
