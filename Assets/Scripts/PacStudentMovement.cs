using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 destination;
    private bool isMoving = false;
    private Animator animator;

    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        destination = transform.position;

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
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
            // Check for wall in the direction
            float raycastDistance = 1f;
            int wallLayerMask = LayerMask.GetMask("WallLayer");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, wallLayerMask);

            // Debugging: Visualize the raycast in the Scene view
            Debug.DrawRay(transform.position, direction * raycastDistance, Color.red, 0.1f);

            if (hit.collider == null)
            {
                destination = (Vector2)transform.position + direction;
                isMoving = true;
                UpdateAnimation(direction);

                // Play movement audio
                AudioManager.instance.PlayPacStudentMovementSFX();

                // Play dust particle effect
                // Uncomment the following lines if you have set up the particle system
                // if (!dustEffect.isPlaying)
                // {
                //     dustEffect.Play();
                // }
            }
            else
            {
                // Wall in the way - do not move
                UpdateAnimation(Vector2.zero);

                // Optionally, play a wall collision sound or effect
                // AudioManager.instance.PlayWallCollisionSFX();

                // Stop movement audio
                AudioManager.instance.StopPacStudentMovementSFX();

                // Stop dust particle effect
                // Uncomment the following line if you have set up the particle system
                // dustEffect.Stop();
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

                // Stop movement audio when movement completes
                AudioManager.instance.StopPacStudentMovementSFX();

                // Stop dust particle effect
                // Uncomment the following line if you have set up the particle system
                // dustEffect.Stop();
            }
        }
    }

    private void UpdateAnimation(Vector2 direction)
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WallLayer"))
        {
            // Collision with wall
            isMoving = false;
            destination = transform.position;
            UpdateAnimation(Vector2.zero);

            // Play wall collision sound
            // AudioManager.instance.PlayWallCollisionSFX();

            // Stop movement audio
            AudioManager.instance.StopPacStudentMovementSFX();

            // Stop dust particle effect
            // Uncomment the following line if you have set up the particle system
            // dustEffect.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pellet"))
        {
            Destroy(collision.gameObject);
            GameManager.instance.AddScore(10);

            // Play pellet collection sound
            AudioManager.instance.PlayPelletCollectionSFX();
        }
        else if (collision.gameObject.CompareTag("PowerPellet"))
        {
            Destroy(collision.gameObject);
            GameManager.instance.StartScaredState();
            GameManager.instance.AddScore(50);

            // Play power pellet collection sound
            AudioManager.instance.PlayPowerPelletCollectionSFX();
        }
        else if (collision.gameObject.CompareTag("Cherry"))
        {
            Destroy(collision.gameObject);
            GameManager.instance.AddScore(100);

            // Play cherry collection sound (if you have one)
            // AudioManager.instance.PlayCherryCollectionSFX();
        }
    }
}
