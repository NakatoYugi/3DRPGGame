using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public Animator animator;

    [Header("Flags")]
    [HideInInspector] public bool isPerformingAction;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool applyRootMotion;
    [HideInInspector] public bool canMove; //在执行某些特殊动作时，可能希望角色在该动作执行时仍可以移动
    [HideInInspector] public bool canRotate; //在执行某些特殊动作时，可能希望角色在该动作执行时仍可以转向
    private void Awake()
    {
        DontDestroyOnLoad(this);

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        PlayerCamera.instance.player = this;
        PlayerInputManager.instance.player = this;
    }

    // Update is called once per frame
    void Update()
    {
        playerLocomotionManager.HandleAllMovement();
    }

    private void LateUpdate()
    {
        PlayerCamera.instance.HandleAllCameraAction();
    }
}
