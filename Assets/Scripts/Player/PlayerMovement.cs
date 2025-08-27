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
    public bool isKnocked;
    [SerializeField] private SpriteRenderer sr;
    void LateUpdate()
    {
        sr.sortingOrder = -(int)transform.position.y;
    }
    private void Start()
    {
        GetComponent<EntityStats>().OnEntityHit += (sender, e) =>
        {
            canMove = false;
            isKnocked = true;
            Invoke(nameof(EnableMovement), 0.5f);
        };
    }
    private void EnableMovement()
    {
        canMove = true;
        isKnocked = false;
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
        canMove = true;
        isKnocked = false;
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
        if (!isKnocked)
            rb.linearVelocity = moveInput * baseMoveSpeed * mult;
    }

}
