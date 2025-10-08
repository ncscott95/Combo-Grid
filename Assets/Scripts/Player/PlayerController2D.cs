using System.Collections;
using UnityEngine;

public class PlayerController2D : PlayerControllerBase
{
    [Header("Wall Slide")]
    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private float _wallCheckDistance = 0.5f;
    private bool _isWallSliding = false;

    [Header("Jump")]
    [SerializeField] private int _airJumpsMax = 1;
    [SerializeField] private float _jumpForce = 15f;
    [SerializeField] private float _gravityJumpingScale = 2f;
    [SerializeField] private float _gravityFallingScale = 4f;
    [SerializeField] private float _gravityGroundedScale = .5f;
    [SerializeField] private float _gravityWallSlidingScale = 2f;
    [SerializeField] private float _maxFallSpeed = 20f;
    [SerializeField] private float _jumpAirCooldown = 0.2f;
    [SerializeField] private float _jumpLandCooldown = 0.1f;
    [SerializeField] private float _airDrag;
    private int _airJumpsRemaining;
    private Coroutine _jumpCooldownCoroutine;

    [Header("Visuals")]
    [SerializeField] private Transform _visuals;
    [SerializeField] private Animator _animator;
    private bool _isJumping = false;
    private bool _isJumpReady = true;
    private bool _wasGroundedLastFrame = false;
    public int FacingDirection => _facingDirection;
    private int _facingDirection = 1; // 1 = right, -1 = left

    public override void OnEnable()
    {
        base.OnEnable();

        Actions.Player.Jump.performed += ctx => Jump();
        Actions.Player.Jump.canceled += ctx => StopJump();
        _airJumpsRemaining = _airJumpsMax;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        Actions.Player.Jump.performed -= ctx => Jump();
        Actions.Player.Jump.canceled -= ctx => StopJump();
    }

    public override void Update()
    {
        base.Update();

        // Flip visuals based on movement input
        if (_moveInput.x > 0.01f && _facingDirection != 1)
        {
            _facingDirection = 1;
            Vector3 scale = _visuals.localScale;
            scale.x = Mathf.Abs(scale.x);
            _visuals.localScale = scale;
        }
        else if (_moveInput.x < -0.01f && _facingDirection != -1)
        {
            _facingDirection = -1;
            Vector3 scale = _visuals.localScale;
            scale.x = -Mathf.Abs(scale.x);
            _visuals.localScale = scale;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // Work with a local velocity variable
        Vector2 velocity = _rb.linearVelocity;

        // Horizontal movement
        float horizontalInput = _canMove ? _moveInput.x : 0f; // Handle movement check
        if (horizontalInput == 0)
        {
            float deceleration = _isGrounded ? _groundDrag : _airDrag;
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, horizontalInput * Speed, _acceleration * Time.fixedDeltaTime);
        }

        // Cap horizontal speed
        if (Mathf.Abs(velocity.x) > Speed)
        {
            velocity.x = Mathf.Sign(velocity.x) * Speed;
        }

        // Wall sliding
        if (!_isGrounded && velocity.y < 0)
        {
            // TODO: most likely broken because wall check needs to face opposite direction of player facing
            RaycastHit2D wallCheck = Physics2D.Raycast(transform.position, Vector2.right * _facingDirection, _wallCheckDistance, _wallMask);
            _isWallSliding = wallCheck;
        }
        else
        {
            _isWallSliding = false;
        }

        // Check if the player just landed
        if (!_wasGroundedLastFrame && _isGrounded)
        {
            // Player just landed on the ground, reset air jumps
            _airJumpsRemaining = _airJumpsMax;
            _isJumping = false;

            // Start a short cooldown before the player can jump again
            if (_jumpCooldownCoroutine != null) StopCoroutine(_jumpCooldownCoroutine);
            _jumpCooldownCoroutine = StartCoroutine(JumpCooldown(_jumpLandCooldown));
        }
        _wasGroundedLastFrame = _isGrounded;

        // Apply 2D gravity
        if (_isJumping && velocity.y > 0)
        {
            _rb.gravityScale = _gravityJumpingScale;
        }
        else if (_isWallSliding)
        {
            _rb.gravityScale = _gravityGroundedScale;
        }
        else if (!_isGrounded)
        {
            _rb.gravityScale = _gravityFallingScale;
        }
        else
        {
            _rb.gravityScale = _gravityGroundedScale;
        }

        // Cap fall speed
        if (velocity.y < -_maxFallSpeed) velocity.y = -_maxFallSpeed;

        // Set animation state
        _animator.SetBool("isRunning", velocity.x != 0);
        _animator.SetBool("isJumping", _isJumping);
        _animator.SetBool("isFalling", velocity.y < 0 && !_isGrounded);
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetBool("isWallSliding", _isWallSliding);

        // Apply the final velocity
        _rb.linearVelocity = velocity;
    }

    // Obsolete attack method, replaced by AbilityGrid and SkillSequencer
    // public override void Attack()
    // {
    //     if (SkillSequencer != null)
    //     {
    //         if (SkillSequencer.CurrentPhase == SkillSequencer.SkillPhase.Inactive && _testAbility1 != null)
    //         {
    //             AbilityManager.Instance.DebugUseAbility(_testAbility1);
    //         }
    //         else if (SkillSequencer.CurrentPhase == SkillSequencer.SkillPhase.Recovery && _testAbility2 != null)
    //         {
    //             AbilityManager.Instance.DebugUseAbility(_testAbility2);
    //         }
    //     }
    // }

    public override void Dodge()
    {
        _animator.SetTrigger("roll");
    }

    public void Jump()
    {
        if (_canMove && _isGrounded && _isJumpReady)
        {
            _isJumping = true;
            _isJumpReady = false;
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            StartCoroutine(JumpCooldown(_jumpAirCooldown));
        }
        else if (!_isGrounded && _airJumpsRemaining > 0)
        {
            _animator.SetTrigger("airJump");
            _isJumping = true;
            _airJumpsRemaining--;
            _rb.linearVelocityY = 0; // Reset vertical velocity before jumping again
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            if (_jumpCooldownCoroutine != null) StopCoroutine(_jumpCooldownCoroutine);
            _jumpCooldownCoroutine = StartCoroutine(JumpCooldown(_jumpAirCooldown));
        }
    }

    public void StopJump()
    {
        _isJumping = false;
    }

    private IEnumerator JumpCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isJumpReady = true;
    }
}
