using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This has to be outside
public enum GhostState
{
    Normal,
    Scared,
    Dead
}

public class GhostMovement : MonoBehaviour
{
    public float normalSpeed = 2f;    
    public float scaredSpeed = 1f;    
    public float deadSpeed = 3f;      
    public float scaredDuration = 10f;
    private float scaredTimer;

    public GhostState currentState = GhostState.Normal;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private Vector2 destination;
    private bool isMoving = false;

    private Transform pacStudent;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Finds PacStudent in the scene
        GameObject pacStudentObject = GameObject.FindGameObjectWithTag("Player");
        if (pacStudentObject != null)
        {
            pacStudent = pacStudentObject.transform;
        }
        else
        {
            Debug.LogError("PacStudent not found in the scene!");
        }

        // Starts moving
        destination = transform.position;
        ChooseNewDirection();
    }

    void Update()
    {
        // Handles state timing
        if (currentState == GhostState.Scared)
        {
            scaredTimer -= Time.deltaTime;
            if (scaredTimer <= 0)
            {
                SetState(GhostState.Normal);
            }
        }

        if (!isMoving)
        {
            ChooseNewDirection();
        }
    }

    void FixedUpdate()
    {
        MoveGhost();
    }

    private void MoveGhost()
    {
        if (isMoving)
        {
            float speed = GetCurrentSpeed();
            float step = speed * Time.fixedDeltaTime;
            Vector2 newPosition = Vector2.MoveTowards(rb.position, destination, step);
            rb.MovePosition(newPosition);

            if (Vector2.Distance(rb.position, destination) < 0.001f)
            {
                rb.position = destination;
                isMoving = false;
            }
        }
    }

    private void ChooseNewDirection()
    {
        // Gets available directions
        List<Vector2> availableDirections = new List<Vector2>();

        foreach (Vector2 dir in directions)
        {
            if (CanMove(dir))
            {
                availableDirections.Add(dir);
            }
        }

        if (availableDirections.Count > 0)
        {
            // Chooses a random available direction
            Vector2 chosenDirection = availableDirections[Random.Range(0, availableDirections.Count)];
            destination = (Vector2)transform.position + chosenDirection;
            isMoving = true;
        }
        else
        {
            // Ghost is stuck
            isMoving = false;
        }
    }

    private bool CanMove(Vector2 direction)
    {
        float raycastDistance = 1f;
        int wallLayerMask = LayerMask.GetMask("WallLayer");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, wallLayerMask);

        return hit.collider == null;
    }

    private float GetCurrentSpeed()
    {
        // Reduced speed by half
        switch (currentState)
        {
            case GhostState.Scared:
                return scaredSpeed / 2f;
            case GhostState.Dead:
                return deadSpeed / 2f;
            default:
                return normalSpeed / 2f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState == GhostState.Normal)
            {
                // Damage
                GameManager.instance.PacStudentCaught();
                Debug.Log($"{gameObject.name} collided with Player while Normal.");
            }
            else if (currentState == GhostState.Scared)
            {
                // Ghost dies
                SetState(GhostState.Dead);
                GameManager.instance.AddScore(200);
                Debug.Log($"{gameObject.name} collided with Player while Scared and is now Dead.");
            }
        }
        else if (collision.gameObject.CompareTag("Ghost"))
        {
            // Collision with another ghost
            Debug.Log($"{gameObject.name} collided with another ghost! Changing direction.");
            ChooseNewDirection(); // Changing direction to avoid getting stuck
        }
    }

    public void SetState(GhostState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GhostState.Normal:
                animator.SetBool("isScared", false);
                animator.SetBool("isDead", false);
                Debug.Log($"{gameObject.name} set to Normal state.");
                break;
            case GhostState.Scared:
                animator.SetBool("isScared", true);
                animator.SetBool("isDead", false);
                scaredTimer = scaredDuration;
                Debug.Log($"{gameObject.name} set to Scared state.");
                break;
            case GhostState.Dead:
                animator.SetBool("isScared", false);
                animator.SetBool("isDead", true);
                Debug.Log($"{gameObject.name} set to Dead state.");
                // Yeah this needs further work
                StartCoroutine(ReturnToHome());
                break;
        }
    }

    private IEnumerator ReturnToHome()
    {
        // Disable collision with PacStudent while dead
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), true);
        Debug.Log($"{gameObject.name} is now ignoring collisions with Player.");

        // Move towards home position (assuming (0,0))
        while (Vector2.Distance(transform.position, Vector2.zero) > 0.1f)
        {
            float step = deadSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, Vector2.zero, step);
            yield return null;
        }

        // Reset state to Normal
        SetState(GhostState.Normal);

        // Re-enable collision with PacStudent
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), false);
        Debug.Log($"{gameObject.name} is now colliding with Player again.");

        // Choose a new direction
        destination = transform.position;
        isMoving = false;
        ChooseNewDirection();
    }
}
