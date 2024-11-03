using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public ParticleSystem dustEffect;
    private Animator animator;
    private Vector2 currentDirection = Vector2.zero;
    private Vector2 lastValidPosition;

    private void Start()
    {
        animator = GetComponent<Animator>();
        lastValidPosition = transform.position;
    }

    private void Update()
    {
        HandleInput();
        MovePacStudent();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Set direction based on input, prioritizing horizontal movement
        if (horizontal != 0)
        {
            currentDirection = new Vector2(horizontal, 0).normalized;
        }
        else if (vertical != 0)
        {
            currentDirection = new Vector2(0, vertical).normalized;
        }
        else
        {
            currentDirection = Vector2.zero;
        }
    }

    private void MovePacStudent()
{
    if (currentDirection != Vector2.zero)
    {
        // Perform a Raycast to check for walls in the direction of movement
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, 0.1f);

        if (hit.collider == null || !hit.collider.CompareTag("Wall"))
        {
            // Move only if there is no wall in the desired direction
            Vector2 targetPosition = (Vector2)transform.position + currentDirection * moveSpeed * Time.deltaTime;
            transform.position = targetPosition;
            lastValidPosition = transform.position; // Update last valid position
            UpdateAnimation();

            if (!dustEffect.isPlaying)
            {
                dustEffect.Play();
            }
        }
        else
        {
            // Reset to the last valid position when a wall is encountered
            transform.position = lastValidPosition;
            currentDirection = Vector2.zero; // Stop movement
            dustEffect.Stop();
        }
    }
    else
    {
        dustEffect.Stop();
    }
}


    private void UpdateAnimation()
    {
        // Reset all triggers and set the appropriate one
        animator.ResetTrigger("WalkRight");
        animator.ResetTrigger("WalkLeft");
        animator.ResetTrigger("WalkUp");
        animator.ResetTrigger("WalkDown");

        if (currentDirection == Vector2.right)
            animator.SetTrigger("WalkRight");
        else if (currentDirection == Vector2.left)
            animator.SetTrigger("WalkLeft");
        else if (currentDirection == Vector2.up)
            animator.SetTrigger("WalkUp");
        else if (currentDirection == Vector2.down)
            animator.SetTrigger("WalkDown");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pellet"))
        {
            Destroy(collision.gameObject);
            GameManager.instance.AddScore(10);
        }
        else if (collision.gameObject.CompareTag("PowerPellet"))
        {
            Destroy(collision.gameObject);
            GameManager.instance.StartScaredState();
            GameManager.instance.AddScore(50);
        }
        else if (collision.gameObject.CompareTag("Cherry"))
        {
            Destroy(collision.gameObject);
            GameManager.instance.AddScore(100);
        }
    }
}
