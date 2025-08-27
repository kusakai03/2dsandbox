using UnityEngine;
using System.Collections.Generic;

public class MeleeGridMoving : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRange = 6f;
    public float idleMoveInterval = 2f;
    private float repathTimer = 0f;
    public float repathInterval = 0.5f;

    public Transform target; // sẽ auto tìm nếu null

    private List<Node> path;
    private int pathIndex;
    private float idleTimer;

    private Vector2Int idleTarget;
    private bool isIdleMoving;
    private bool isDead;

    void Start()
    {
        isDead = false;
        GetComponent<EntityStats>().OnEntityZeroHealth += (sender, e) =>
        {
            isDead = true;
        };
        GetComponent<EntityStats>().OnEntityHit += (sender, e) =>
        {
            isDead = true;
            Invoke("MoveAgain", 0.5f);
        };
    }

    private void MoveAgain()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        isDead = false;
    }

    void Update()
    {
        if (isDead) return;

        if (target == null)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
            if (hit != null)
            {
                target = hit.transform;
            }
            else
            {
                IdleWander();
                return;
            }
        }
        else
        {
            float distToPlayer = Vector2.Distance(transform.position, target.position);
            if (distToPlayer > detectionRange * 1.5f)
            {
                target = null;
                return;
            }
            else
            {
                ChasePlayer();
            }
        }
    }

    void ChasePlayer()
    {
        Vector2Int start = Vector2Int.FloorToInt(transform.position);
        Vector2Int end = Vector2Int.FloorToInt(target.position);

        repathTimer -= Time.deltaTime;
        if (path == null || path.Count == 0 || pathIndex >= path.Count || repathTimer <= 0f)
        {
            path = Pathfinding.FindPath(start, end);
            pathIndex = 0;
            repathTimer = repathInterval;
        }

        if (path != null && pathIndex < path.Count)
        {
            Vector3 targetPos = new Vector3(path[pathIndex].pos.x + 0.5f, path[pathIndex].pos.y + 0.5f, 0);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
                pathIndex++;
        }
    }

    void IdleWander()
    {
        idleTimer -= Time.deltaTime;

        if (!isIdleMoving && idleTimer <= 0f)
        {
            Vector2Int start = Vector2Int.FloorToInt(transform.position);
            Vector2Int[] dirs = {
                new Vector2Int(1,0), new Vector2Int(-1,0),
                new Vector2Int(0,1), new Vector2Int(0,-1)
            };

            Vector2Int chosenDir = dirs[Random.Range(0, dirs.Length)];
            idleTarget = start + chosenDir * Random.Range(2, 5);
            path = Pathfinding.FindPath(start, idleTarget);
            pathIndex = 0;
            isIdleMoving = true;
        }

        if (isIdleMoving && path != null && pathIndex < path.Count)
        {
            Vector3 targetPos = new Vector3(path[pathIndex].pos.x + 0.5f, path[pathIndex].pos.y + 0.5f, 0);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
                pathIndex++;
        }
        else if (isIdleMoving)
        {
            isIdleMoving = false;
            idleTimer = idleMoveInterval;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (target != null && collision.gameObject == target.gameObject)
        {
            target.GetComponent<EntityStats>()
                  ?.TakeDamage(GetComponent<EntityStats>().GetFinalATK(), transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ vòng detection để dễ debug
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
