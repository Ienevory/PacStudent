using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public ParticleSystem dustEffect; // Assign this in Inspector
    private Animator animator;
    private Vector2 currentDirection = Vector2.zero;
    private AudioManager audioManager;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioManager = AudioManager.instance;
    }

    private void Update()
    {
        HandleInput();
        MovePacStudent();
    }

    private void HandleInput()
    {
        // Get raw input for direct movement without grid snapping
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Prioritize horizontal movement if both directions are pressed
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
            currentDirection = Vector2.zero; // Stop if no input
        }
    }

    private void MovePacStudent()
    {
        // Only move if there is a direction set
        if (currentDirection != Vector2.zero)
        {
            // Move PacStudent
            transform.Translate(currentDirection * moveSpeed * Time.deltaTime);

            // Trigger movement animations based on direction
            UpdateAnimation();

            // Play movement sound
            if (audioManager != null)
            {
                audioManager.PlayPacStudentMovementSFX();
            }

            // Play dust effect if not already playing
            if (!dustEffect.isPlaying)
            {
                dustEffect.Play();
            }
        }
        else
        {
            // Stop movement sound and dust effect when idle
            if (audioManager != null)
            {
                audioManager.StopPacStudentMovementSFX();
            }
            dustEffect.Stop();
        }
    }

    private void UpdateAnimation()
    {
        // Reset all triggers before setting a new one
        animator.ResetTrigger("WalkRight");
        animator.ResetTrigger("WalkLeft");
        animator.ResetTrigger("WalkUp");
        animator.ResetTrigger("WalkDown");

        if (currentDirection == Vector2.right)
        {
            animator.SetTrigger("WalkRight");
        }
        else if (currentDirection == Vector2.left)
        {
            animator.SetTrigger("WalkLeft");
        }
        else if (currentDirection == Vector2.up)
        {
            animator.SetTrigger("WalkUp");
        }
        else if (currentDirection == Vector2.down)
        {
            animator.SetTrigger("WalkDown");
        }
    }
}
