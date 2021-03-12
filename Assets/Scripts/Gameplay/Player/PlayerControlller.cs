using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControlller : NetworkBehaviour
{
    [SerializeField] private float speed = 200f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private CharacterController controller = null;
    
    Controls controls;

    Vector3 point;
    Vector2 currentMoveDirection = Vector2.zero;

    [ClientCallback]
    private void Start()
    {
        InitInput();
    }

    [Client]
    private void InitInput()
    {
        controls = new Controls();

        controls.Player.Move.performed += SetMoveInput;
        controls.Player.Move.canceled += SetMoveInput;

        controls.Player.Run.performed += SetRunInput;
        controls.Player.Run.canceled += SetRunInput;

        controls.Player.Action.performed += SetDoActionInput;

        controls.Enable();
    }

    private void SetMoveInput(InputAction.CallbackContext ctx)
    {
        currentMoveDirection = ctx.ReadValue<Vector2>();
    }

    private void SetRunInput(InputAction.CallbackContext ctx)
    {

    }

    private void SetDoActionInput(InputAction.CallbackContext ctx)
    {

    }

    #region Server
    [Command]
    public void CmdMove(Vector2 direction, float currentRotationSpeed, float currentSpeed)
    {
        Vector3 rotation = new Vector3(0, direction.x * currentRotationSpeed, 0);
 
        Vector3 move = new Vector3(0, 0, direction.y * Time.deltaTime);
        move = controller.transform.TransformDirection(move);
        controller.SimpleMove(move * currentSpeed);

        controller.transform.Rotate(rotation);
    }

    #endregion

    #region Client
    
    [ClientCallback]
    private void Update()
    {
        if(!hasAuthority) { return; }
        if(currentMoveDirection.x != 0 || currentMoveDirection.y != 0)
        {
            CmdMove(currentMoveDirection, rotationSpeed, speed);
        }
    }
    
    private void FixedUpdate()
    {
        // rigidbody.velocity = new Vector3(currentMoveDirection.x, 0, currentMoveDirection.y).normalized * speed;
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }
    }

    public override void OnStopClient()
    {

    }
    #endregion

}