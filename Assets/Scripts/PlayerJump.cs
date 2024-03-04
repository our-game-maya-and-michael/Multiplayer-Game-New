using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class PlayerJump : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private Animator animator = null;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;

    [Networked(OnChanged = nameof(NetworkWalkingChanged))]
    public bool isWalking { get; set; } = false;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        var movement = new Vector3();

        if (Keyboard.current.upArrowKey.isPressed) { movement.z += 1; }
        if (Keyboard.current.downArrowKey.isPressed) { movement.z -= 1; }
        if (Keyboard.current.leftArrowKey.isPressed) { movement.x -= 1; }
        if (Keyboard.current.rightArrowKey.isPressed) { movement.x += 1; }

        bool wasWalking = isWalking;
        isWalking = movement.magnitude > 0;

        controller.Move(movement * movementSpeed * Runner.DeltaTime);

        if (controller.velocity.magnitude > 0.2f)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }

        // Only update the animator if the state changes to reduce network traffic and processing
        if (wasWalking != isWalking)
        {
            animator.SetBool("IsWalking", isWalking);
        }
    }

    private static void NetworkWalkingChanged(Changed<PlayerJump> change)
    {
        // Reflect the actual state of isWalking
        change.Behaviour.animator.SetBool("IsWalking", change.Behaviour.isWalking);
    }
}
