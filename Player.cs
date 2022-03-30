using System.Collections;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private SpriteRenderer[] _spriteRenderers;
    private Animator _animator;
    private Vector2 _respawnPoint;

    private float _inputX;
    private bool _inputJump; 

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.05f;
    [SerializeField] private LayerMask _collisionMask;

    [Header("Movement variables")]
    [SerializeField] private float _movementVel = 3f;
    [SerializeField] private float _jumpVel = 10f;
    
    [Header("Knockback")]
    [SerializeField] private Transform _center;
    [SerializeField] private float _knockbackVel = 8f;
    [SerializeField] private float _knockbackTime = 1f;
    [SerializeField] private bool _knockbacked; 

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }
    private void Update()
    {
        CaptureInput();
        HandleAnimations();
        ApplyMovement();
        ApplyJump();
        FlipWhenNeeded();
    }

    private void HandleAnimations()
    {
        _animator.SetBool("IsWalking", Input.GetAxisRaw("Horizontal") != 0 && IsGrounded());
    }

    private void CaptureInput()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
        _inputJump = Input.GetButtonDown("Jump");
    }

    private void ApplyMovement()
    {
        var newXVel = 
            _knockbacked ? Mathf.Lerp(_rigidbody.velocity.x, 0f, Time.deltaTime * 3) : _inputX * _movementVel;
        
        _rigidbody.velocity = new Vector2(newXVel, _rigidbody.velocity.y);
    }

    private void ApplyJump()
    {
        if (_inputJump && IsGrounded())
        {
            Jump();
        }
    }

    private void FlipWhenNeeded()
    {
        if (_inputX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(_inputX), 1, 1);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _collisionMask);
    }

    public void Jump()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpVel);
    }

    public void Knockback(Transform t)
    {
        var dir = _center.position - t.position;
        _knockbacked = true;
        _rigidbody.velocity = dir.normalized * _knockbackVel;
        foreach (var spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.color = Color.red;
        }

        StartCoroutine(FadeToWhite());
        StartCoroutine(Unknockback());
    }

    private IEnumerator FadeToWhite()
    {
        while (_spriteRenderers[0].color != Color.white)
        {
            yield return null;
            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, Time.deltaTime * 3);
            }
        }
    }

    private IEnumerator Unknockback()
    {
        yield return new WaitForSeconds(_knockbackTime);
        _knockbacked = false;
    }
}
