using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Animator animator;
    private bool isMoving = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get input from the player
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Determine the movement direction
        Vector2 movement = new Vector2(horizontal, vertical).normalized;

        // Check if there's movement input
        if (movement.magnitude > 0)
        {
            // Only play movement sound if it's not already playing
            if (!isMoving)
            {
                AudioManager.instance.PlayPacStudentMovementSFX();
                isMoving = true;
            }

            // Move PacStudent in the given direction at a consistent speed
            transform.Translate(movement * moveSpeed * Time.deltaTime);

            // Reset all animation triggers before setting a new one
            animator.ResetTrigger("WalkRight");
            animator.ResetTrigger("WalkLeft");
            animator.ResetTrigger("WalkUp");
            animator.ResetTrigger("WalkDown");

            // Play the appropriate animation based on the direction of movement
            if (horizontal > 0)
            {
                animator.SetTrigger("WalkRight");
            }
            else if (horizontal < 0)
            {
                animator.SetTrigger("WalkLeft");
            }
            else if (vertical > 0)
            {
                animator.SetTrigger("WalkUp");
            }
            else if (vertical < 0)
            {
                animator.SetTrigger("WalkDown");
            }
        }
        else
        {
            // Stop movement sound when no input
            if (isMoving)
            {
                AudioManager.instance.StopPacStudentMovementSFX();
                isMoving = false;
            }
        }
    }
}
