using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float baseMoveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public Vector2 dir { get; private set; }
    private PlayerInputActions inputActions;
    public bool canMove;
    [SerializeField] private SpriteRenderer sr;
    void LateUpdate()
    {
        sr.sortingOrder = -(int)transform.position.y;
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
        canMove = true;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Update()
    {
        if (canMove)
        {
            moveInput = inputActions.Player.Move.ReadValue<Vector2>();
            dir = moveInput;
        }
        else moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        float mult = 1f;
        if (MapManager.ins != null)
        {
            mult = MapManager.ins.GetSpeedMultiplier(transform.position);
        }

        rb.linearVelocity = moveInput * baseMoveSpeed * mult;
    }

}
