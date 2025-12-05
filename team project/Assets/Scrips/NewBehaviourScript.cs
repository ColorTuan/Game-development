using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 8f;           // 最大水平速度
    [SerializeField] private float acceleration = 80f;      // 地面加速度
    [SerializeField] private float deceleration = 80f;      // 地面减速度
    [SerializeField] private float airAcceleration = 40f;   // 空中加速度
    [SerializeField] private float airDeceleration = 40f;   // 空中减速度

    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 12f;      // 起跳瞬间速度
    [SerializeField] private float fallMultiplier = 4f;     // 下落时额外重力
    [SerializeField] private float lowJumpMultiplier = 3f;  // 小跳（松开按键）额外重力
    [SerializeField] private int maxAirJumps = 0;           // 允许空中跳跃次数（0=只能跳一次）
    [SerializeField] private float coyoteTime = 0.1f;       // 离开平台后仍可跳的时间
    [SerializeField] private float jumpBuffer = 0.1f;       // 按下跳跃键后缓冲时间

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;         // 脚底空物体
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.45f, 0.08f);
    [SerializeField] private LayerMask groundLayer;

    // 私有
    private Rigidbody2D rb;
    private float moveInput;              // -1~1
    private bool jumpPressed;
    private bool jumpHeld;
    private bool facingRight = true;
    private float lastGroundedTime;       // coyote timer
    private float lastJumpPressedTime;    // jump buffer timer
    private int airJumpsLeft;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Debug.Log("Jump pressed!");
        // 输入采集
        moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
            lastJumpPressedTime = jumpBuffer;
        }
        if (Input.GetButtonUp("Jump"))
            jumpHeld = false;
        else
            jumpHeld = true;

        // 计时器
        if (IsGrounded())
            lastGroundedTime = coyoteTime;
        else
            lastGroundedTime -= Time.deltaTime;

        lastJumpPressedTime -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        HandleHorizontal();
        HandleJump();
        HandleBetterJump();
    }

    #region 地面检测
    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer);
    }
    #endregion

    #region 水平移动
    private void HandleHorizontal()
    {
        bool grounded = IsGrounded();
        float acc = grounded ? acceleration : airAcceleration;
        float dec = grounded ? deceleration : airDeceleration;

        // 目标速度
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;

        // 加速/减速
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acc : dec;
        float force = speedDiff * accelRate;
        rb.AddForce(force * Vector2.right);
    }
    #endregion

    #region 跳跃
    private void HandleJump()
    {
        if (lastJumpPressedTime > 0f && CanJump())
        {
            // 执行跳跃
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            lastJumpPressedTime = 0f;
            lastGroundedTime = 0f;

            if (!IsGrounded())
                airJumpsLeft--;          // 消耗空中跳跃
        }

        // coyote & buffer 重置
        if (IsGrounded())
            airJumpsLeft = maxAirJumps;
    }

    private bool CanJump()
    {
        return IsGrounded() || lastGroundedTime > 0f || airJumpsLeft > 0;
    }
    #endregion

    #region 手感优化（可变重力）
    private void HandleBetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !jumpHeld)
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }
    #endregion

    #region 翻转
    private void LateUpdate()
    {
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }
    #endregion

    // 可视化地面检测框
    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
