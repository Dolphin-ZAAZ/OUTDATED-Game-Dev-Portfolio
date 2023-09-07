using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    float meter = 250800f;
    [SerializeField] float velocity;
    [SerializeField] Transform playerPos;
    [SerializeField] float speed = 2000f;
    [SerializeField] float nextWaypointDist = 1f;
    [SerializeField] float updateInterval = 0.5f;
    [SerializeField] float updateIntervalTimer;
    Vector2 direction;
    Vector2 lookDir;

    Path path;
    [SerializeField] int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        meter = 250800f;
        updateIntervalTimer = updateInterval;
        playerPos = GameObject.Find("PlayerCharacter").GetComponent<Transform>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        speed = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemySpeed;

        UpdatePath();
    }

    private void UpdatePath()
    {
        seeker.StartPath(rb.position, playerPos.position, OnPathComplete);
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speed = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemySpeed;
        updateIntervalTimer -= Time.fixedDeltaTime;
        LayerMask layerMask = (1 << 3) | (1 << 7);
        RaycastHit2D isObstacle = Physics2D.Linecast(rb.position, playerPos.position, layerMask);
        velocity = rb.velocity.magnitude;

        if (path == null)
        {
            return;
        }
        
        Vector2 force = direction * meter * speed * Time.fixedDeltaTime;

        rb.AddForce(force);
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
        
        if (isObstacle.collider.CompareTag("Player"))
        {
            direction = (((Vector2)playerPos.position - rb.position).normalized);
            lookDir = (Vector2)playerPos.position - rb.position;
        }
        else if (isObstacle.collider.CompareTag("Obstacles"))
        {
            if (updateIntervalTimer <= 0f)
            {
                UpdatePath();
                updateIntervalTimer = updateInterval;
            }

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDist && reachedEndOfPath == false)
            {
                currentWaypoint++;
            }
            direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            lookDir = (Vector2)path.vectorPath[currentWaypoint] - rb.position;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }
        }
    }
}
