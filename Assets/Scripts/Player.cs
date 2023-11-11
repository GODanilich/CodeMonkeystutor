using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private GameInput gameInput;

    private float playerRadius = 0.65f;
    private float playerHeight = 2f;
    private float moveDistance;
    private bool isWalking;
    private bool canMove;

    private Vector3 moveDirection;
    private Vector3 lastInteractDirection;
    private Vector2 movementVector;

    void Start()
    {
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();

    }

    private void HandleMovement()
    {
        movementVector = gameInput.GetMovementVectorNormalized();

        moveDirection = new(movementVector.x, 0f, movementVector.y);

        moveDistance = Time.deltaTime * moveSpeed;

        canMove = !Physics.CapsuleCast(transform.position, transform.position + transform.up * playerHeight, playerRadius, moveDirection, moveDistance);

        if (!canMove)
        {
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0f, 0f).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + transform.up * playerHeight, playerRadius, moveDirectionX, moveDistance);
            if (canMove)
            {
                moveDirection = moveDirectionX;
            }
            else
            {
                Vector3 moveDirectionZ = new Vector3(0f, 0f, moveDirection.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + transform.up * playerHeight, playerRadius, moveDirectionZ, moveDistance);

                if (canMove)
                {
                    moveDirection = moveDirectionZ;
                }
            }
        }


        if (canMove)
        {
            transform.position += moveDistance * moveDirection;
        }
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);

        isWalking = moveDirection != Vector3.zero;
    }

    private void HandleInteractions()
    {

        if (moveDirection != Vector3.zero)
        {
            lastInteractDirection = moveDirection;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit hitInfo, interactDistance))
        {
            if (hitInfo.transform.TryGetComponent<ClearCounter>(out ClearCounter clearCounter))
            {
                clearCounter.Interact();
            }
        }
        else { Debug.Log("-"); }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

}
