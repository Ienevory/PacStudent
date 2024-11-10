using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 destination;
    private bool isMoving = false;
    private Animator animator;

    private Rigidbody2D rb;
    private ParticleSystem dustEffect;

    private void Start()
    {
        animator = GetComponent<Animator>();
        destination = transform.position;

        // Gets the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Gets the ParticleSystem component
        dustEffect = GetComponentInChildren<ParticleSystem>();

        // Debug statement to confirm Start() is called
        Debug.Log("PacStudentMovement Start() called.");
    }

    private void Update()
    {
        if (!isMoving)
        {
            HandleInput();
        }
    }

    private void FixedUpdate()
    {
        MovePacStudent();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 direction = Vector2.zero;

        if (horizontal != 0)
        {
            direction = new Vector2(horizontal, 0);
        }
        else if (vertical != 0)
        {
            direction = new Vector2(0, vertical);
        }

        if (direction != Vector2.zero)
        {
            // Checks for wall in the direction
            float raycastDistance = 1f;
            int wallLayerMask = LayerMask.GetMask("WallLayer");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, wallLayerMask);

            Debug.DrawRay(transform.position, direction * raycastDistance, Color.red, 0.1f);

            if (hit.collider == null)
            {
                destination = (Vector2)transform.position + direction;
                isMoving = true;
                UpdateAnimation(direction);

                // Plays movement audio
                AudioManager.instance.PlayPacStudentMovementSFX();

                // Plays dust particle effect
                if (!dustEffect.isPlaying)
                {
                    dustEffect.Play();
                }

                Debug.Log("PacStudent started moving to " + destination);
            }
            else
            {
                UpdateAnimation(Vector2.zero);

                // Stops movement audio
                AudioManager.instance.StopPacStudentMovementSFX();

                // Stops dust particle effect
                if (dustEffect.isPlaying)
                {
                    dustEffect.Stop();
                }

                Debug.Log("PacStudent can't move, wall detected at direction " + direction);
            }
        }
    }

    private void MovePacStudent()
    {
        if (isMoving)
        {
            float step = moveSpeed * Time.fixedDeltaTime;
            Vector2 newPosition = Vector2.MoveTowards(rb.position, destination, step);
            rb.MovePosition(newPosition);

            if (Vector2.Distance(rb.position, destination) < 0.001f)
            {
                rb.position = destination;
                isMoving = false;

                // Stops movement audio when movement completes
                AudioManager.instance.StopPacStudentMovementSFX();

                // Stops dust particle effect
                if (dustEffect.isPlaying)
                {
                    dustEffect.Stop();
                }

                Debug.Log("PacStudent reached destination " + destination);
            }
        }
    }

    private void UpdateAnimation(Vector2 direction)
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

        Debug.Log("PacStudent animation updated with direction " + direction);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D with: " + collision.gameObject.name);

        if (collision.gameObject.layer == LayerMask.NameToLayer("WallLayer"))
        {
            isMoving = false;
            destination = transform.position;
            UpdateAnimation(Vector2.zero);

            // Stops movement audio
            AudioManager.instance.StopPacStudentMovementSFX();

            // Stops dust particle effect
            if (dustEffect.isPlaying)
            {
                dustEffect.Stop();
            }

            // Why are you ignoring walls?
            Debug.Log("PacStudent collided with wall and stopped.");
        }
        else if (collision.gameObject.CompareTag("Ghost"))
        {
            // Handles collision with Ghost
            GhostMovement ghostMovement = collision.gameObject.GetComponent<GhostMovement>();
            if (ghostMovement != null)
            {
                if (ghostMovement.currentState == GhostState.Normal)
                {
                    // Damage
                    Debug.Log("PacStudent caught by a ghost!");
                    GameManager.instance.PacStudentCaught();

                    // Plays PacStudent death sound
                    AudioManager.instance.PlayPacStudentDeathSFX();

                    // Space for further death animation or smthn like this
                }
                else if (ghostMovement.currentState == GhostState.Scared)
                {
                    // PacStudent eats the ghost
                    Debug.Log("PacStudent ate a scared ghost!");
                    ghostMovement.SetState(GhostState.Dead);
                    GameManager.instance.AddScore(200);

                }
            }
        }
        else if (collision.gameObject.CompareTag("Cherry"))
        {
            // Debug statement
            Debug.Log("PacStudent collided with Cherry via OnCollisionEnter2D");

            Destroy(collision.gameObject);
            GameManager.instance.AddScore(100);

            // Plays cherry collection sound
            AudioManager.instance.PlayCherryCollectionSFX();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Pellet"))
        {
            Debug.Log("Collected Pellet");
            Destroy(collision.gameObject);
            GameManager.instance.AddScore(10);

            // Plays pellet collection sound
            AudioManager.instance.PlayPelletCollectionSFX();
        }
        else if (collision.gameObject.CompareTag("PowerPellet"))
        {
            Debug.Log("Collected Power Pellet");
            Collider2D pelletCollider = collision.gameObject.GetComponent<Collider2D>();
            if (pelletCollider != null)
            {
                pelletCollider.enabled = false;
                Debug.Log("Power Pellet collider disabled to prevent multiple triggers.");
            }

            Destroy(collision.gameObject);
            GameManager.instance.AddScore(50);
            GameManager.instance.StartScaredState();

            // Plays power pellet collection sound
            AudioManager.instance.PlayPowerPelletCollectionSFX();
        }
        else if (collision.gameObject.CompareTag("Cherry"))
        {
            Debug.Log("Collected Cherry via OnTriggerEnter2D");
            Destroy(collision.gameObject);
            GameManager.instance.AddScore(100);

            // Plays cherry collection sound
            AudioManager.instance.PlayCherryCollectionSFX();
        }
        else
        {
            Debug.Log("Triggered with unhandled object: " + collision.gameObject.name);
        }
    }
}
