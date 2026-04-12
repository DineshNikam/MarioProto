using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.4f, 0.08f);
    [SerializeField] LayerMask groundLayers;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    Rigidbody2D _rb;
    bool _grounded;
    bool _wasGrounded;
    static readonly int SpeedHash = Animator.StringToHash("Speed");

    public bool IsGrounded => _grounded;
    public event Action Jumped;
    public event Action Landed;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (groundCheck == null)
        {
            var t = new GameObject("GroundCheck").transform;
            t.SetParent(transform, false);
            t.localPosition = new Vector3(0f, -0.52f, 0f);
            groundCheck = t;
        }

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.LeftArrow))
            h = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            h = 1f;

        float targetVx = h * moveSpeed;
        Vector2 v = _rb.linearVelocity;
        v.x = Mathf.MoveTowards(v.x, targetVx, moveSpeed * 12f * Time.deltaTime);
        _rb.linearVelocity = v;

        if (spriteRenderer != null && Mathf.Abs(h) > 0.01f)
            spriteRenderer.flipX = h < 0f;

        if (animator != null)
            animator.SetFloat(SpeedHash, Mathf.Abs(_rb.linearVelocity.x));

        _wasGrounded = _grounded;
        _grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayers);

        if (!_wasGrounded && _grounded)
            Landed?.Invoke();
        Debug.Log("Landed: " + _grounded + " Space: " + Input.GetKeyDown(KeyCode.Space));
        if (_grounded && Input.GetKeyDown(KeyCode.Space))
        {
            v = _rb.linearVelocity;
            v.y = jumpForce;
            _rb.linearVelocity = v;
            Jumped?.Invoke();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
