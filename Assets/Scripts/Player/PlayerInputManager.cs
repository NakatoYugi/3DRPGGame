using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    PlayerControls playerControls;

    [HideInInspector] public PlayerManager player;

    [Header("Player Movement Input")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Camera Movement Input")]
    [SerializeField] Vector2 cameraIntput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Player Action Input")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        else
        {
            instance.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.CameraControls.performed += i => cameraIntput = i.ReadValue<Vector2>();
            playerControls.PlayerAction.Dodge.performed += i => dodgeInput = true;
            //长按一定时间sprintInput为true
            playerControls.PlayerAction.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerAction.Sprint.canceled += i => sprintInput = false;
        }

        playerControls.Enable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
    }

    //最小化或焦点不在游戏窗口时，禁用PlayerControls;  
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount <= 0.5f && moveAmount > 0)
            moveAmount = 0.5f;
        else if (moveAmount > 0.5f && moveAmount <= 1)
            moveAmount = 1;

        //为什么horizontalValue参数传递 0 呢？ 因为不是锁定目标时，并不会左右移动和后退，摇杆往哪个方向推，角色就往哪个方向直走
        //不锁定时，只需要moveAmoung（正值）；
        player.playerAnimatorManager.UpdateAnimatorMovementParameter(0, moveAmount, player.isSprinting);

        //锁定时，
    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraIntput.y;
        cameraHorizontalInput = cameraIntput.x;
    }

    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;

            //TODO: 如果菜单或UI窗口打开着，直接return

            //执行闪避
            player.playerLocomotionManager.AttepmtToPerformDodge();
        }
    }

    private void HandleSprintInput()
    {
        if (sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.isSprinting = false;
        }
    }
}
