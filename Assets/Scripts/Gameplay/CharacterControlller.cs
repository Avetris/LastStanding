using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharacterControlller : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 3.0f;
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private GameObject characterPrefab = null;

    public GameObject identity;

    Rigidbody rigidbody = null;
    Controls controls;

    Vector3 point;
    Vector2 currentMoveDirection = Vector2.zero;

    [ClientCallback]
    private void Start()
    {
        InitInput();
        identity = NetworkClient.connection.identity.gameObject; 
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
        characterPrefab.transform.Rotate(0, direction.x * currentRotationSpeed, 0);
        Vector3 forward = characterPrefab.transform.TransformDirection(Vector3.forward);
        controller.SimpleMove(forward * currentSpeed * direction.y);
    }

    #endregion

    #region Client
    
    [ClientCallback]
    private void Update()
    {
        if(!hasAuthority) { return; }
        CmdMove(currentMoveDirection, rotationSpeed, speed);
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