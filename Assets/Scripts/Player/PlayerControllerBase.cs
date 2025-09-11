
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class PlayerControllerBase : Singleton<PlayerControllerBase>, IDamageable
{
    protected const float WALKING_THRESHOLD = 1f;
    protected const float MAX_WALK_MAGNITUDE = 4f;
    public Transform Camera { get; protected set; }

    public InputSystem_Actions Actions { get; private set; }
    protected Rigidbody2D _rb;
    protected bool _canAct = true;

    [Header("Ground Check")]
    [SerializeField] protected LayerMask _groundMask;
    protected bool _isGrounded;

    [Header("Health")]
    [SerializeField] protected int _maxHealth;
    public int Health { get; protected set; }
    protected bool _isInvincible = false;
    protected bool _isDead = false;

    [Header("Movement")]
    [SerializeField] protected float _maxSpeed;
    [SerializeField] protected float _acceleration;
    [SerializeField] protected float _groundDrag;
    protected List<float> _speedModifiers = new();
    public float Speed { get { return _maxSpeed * _speedModifiers.Aggregate(1f, (acc, val) => acc * val); } }
    protected Vector2 _moveInput;

    [Header("Combat")]
    public AbilityManager AbilityManager;
    [SerializeField] protected SkillSequencer _skillSequencer;
    [SerializeField] protected AbilitySystem.Ability _testAbility1;
    [SerializeField] protected AbilitySystem.Ability _testAbility2;

    // [Header("Interacting")]
    // [SerializeField] private InteractHitbox _interactHitbox;

    public override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        Instance.Camera = UnityEngine.Camera.main.transform;
        Actions = new InputSystem_Actions();
    }

    void Start()
    {
        Health = _maxHealth;
    }

    public virtual void OnEnable()
    {
        Actions.Player.Enable();
        // Subscribe to input events
        Actions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        Actions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
        Actions.Player.Attack.performed += ctx => Attack();
    }

    public virtual void OnDisable()
    {
        Actions.Player.Disable();
        // Unsubscribe from input events
        Actions.Player.Move.performed -= ctx => _moveInput = ctx.ReadValue<Vector2>();
        Actions.Player.Move.canceled -= ctx => _moveInput = Vector2.zero;
        Actions.Player.Attack.performed -= ctx => Attack();
    }

    public virtual void Update()
    {
        // Ground check
        _isGrounded = Physics2D.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, 0.3f, _groundMask);
    }

    public virtual void FixedUpdate() { }
    public abstract void Attack();
    public abstract void Dodge();
    public virtual void SetCameraControlActive(bool active) { }
    public void AddSpeedModifier(float modifier) { _speedModifiers.Add(modifier); }
    public void RemoveSpeedModifier(float modifier) { _speedModifiers.Remove(modifier); }

    // public virtual void Interact()
    // {
    //     if (GameManager.Instance.IsInState(GameManager.GameState.Dialogue))
    //     {
    //         DialogueManager.Instance.TryAdvanceDialogue();
    //     }
    //     else if (GameManager.Instance.IsInState(GameManager.GameState.Walking))
    //     {
    //         _interactHitbox.TryInteract();
    //     }
    //     else if (GameManager.Instance.IsInState(GameManager.GameState.Combat))
    //     {
    //         _interactHitbox.TryInteract();
    //     }
    // }

    public virtual void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        Health -= damage;
        if (Health <= 0)
        {
            _isDead = true;
            StopAllCoroutines();
            SetCameraControlActive(false);
            GameManager.Instance.OnDeath();
        }
    }

    public virtual void SpawnPlayer(Transform point)
    {
        transform.SetPositionAndRotation(point.position, point.rotation);
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        _isDead = false;
        Health = _maxHealth;
    }
}
