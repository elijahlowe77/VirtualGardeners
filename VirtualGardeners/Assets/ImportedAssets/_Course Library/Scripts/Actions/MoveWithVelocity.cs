using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Move an object using velocity
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MoveWithVelocity : MonoBehaviour
{
    [Tooltip("The speed at which the object is moved")]
    public float speed = 1.0f;

    [Tooltip("Controls the direction of movement")]
    public Transform origin = null;

    [Tooltip("Input action that supplies the left-hand move vector (X = strafe, Y = forward).")]
    [SerializeField] private InputActionProperty leftHandMoveAction;

    private Vector3 inputVelocity = Vector3.zero;
    private void OnEnable()
    {
        if (leftHandMoveAction.action != null)
            leftHandMoveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (leftHandMoveAction.action != null)
            leftHandMoveAction.action.Disable();
    }

    private void Update()
    {
        if (leftHandMoveAction.action == null)
            return;

        Vector2 move = leftHandMoveAction.action.ReadValue<Vector2>();
        inputVelocity.x = move.x;
        inputVelocity.z = move.y;

        if (move.sqrMagnitude > 0.0001f)
            Debug.Log($"Left-hand move input: {move}");
    }

    private Rigidbody rigidBody = null;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        Vector3 targetVelocity = inputVelocity * speed;
        targetVelocity = origin.TransformDirection(targetVelocity);

        Vector3 velocityChange = targetVelocity - rigidBody.velocity;
        rigidBody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void SetRightVelocity(float value)
    {
        inputVelocity.x = value;
    }

    public void SetForwardVelocity(float value)
    {
        inputVelocity.z = value;
    }

    public void SetUpVelocity(float value)
    {
        inputVelocity.y = value;
    }

    private void OnValidate()
    {
        if (!origin)
            origin = transform;
    }
}
