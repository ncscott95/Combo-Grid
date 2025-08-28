using System.Collections;
using UnityEngine;

public class PlayerController2D : PlayerControllerBase
{
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 15f;
    [SerializeField] private float _gravityJumpingScale = 2f;
    [SerializeField] private float _gravityFallingScale = 4f;
    [SerializeField] private float _gravityGroundedScale = .5f;
    [SerializeField] private float _maxFallSpeed = 20f;
    [SerializeField] private float _jumpCooldown = 0.2f;
    [SerializeField] private float _airDrag;
    private bool _isJumping = false;
    private bool _canJump = true;
    private bool _wasGroundedLastFrame = false;

    public override void OnEnable()
    {
        base.OnEnable();

        Actions.Player.Jump.performed += ctx => Jump();
        Actions.Player.Jump.canceled += ctx => StopJump();
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
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // Work with a local velocity variable
        Vector2 velocity = _rb.linearVelocity;

        // Horizontal movement
        if (_moveInput.x == 0)
        {
            float deceleration = _isGrounded ? _groundDrag : _airDrag;
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, _moveInput.x * Speed, _acceleration * Time.fixedDeltaTime);
        }

        // Cap horizontal speed
        if (Mathf.Abs(velocity.x) > Speed)
        {
            velocity.x = Mathf.Sign(velocity.x) * Speed;
        }

        // Check if the player just landed
        if (!_wasGroundedLastFrame && _isGrounded)
        {
            // Player just landed on the ground, trigger jump cooldown
            // StartCoroutine(JumpCooldown());
            _canJump = true;
        }
        _wasGroundedLastFrame = _isGrounded;

        // Apply 2D gravity
        if (_isJumping && velocity.y > 0)
        {
            _rb.gravityScale = _gravityJumpingScale;
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

        // Apply the final velocity
        _rb.linearVelocity = velocity;
    }

    public override void Attack()
    {
        // Implement 2D attack logic here
    }

    public override void Dodge()
    {
        // Implement 2D dodge logic here
    }

    public void Jump()
    {
        if (_isGrounded && _canJump)
        {
            Debug.Log("Jumped");
            _isJumping = true;
            _canJump = false;
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    public void StopJump()
    {
        _isJumping = false;
    }

    private IEnumerator JumpCooldown()
    {
        Debug.Log("Jump Cooldown Started");
        yield return new WaitForSeconds(_jumpCooldown);
        _canJump = true;
    }
}
