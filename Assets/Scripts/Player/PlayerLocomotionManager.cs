using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : MonoBehaviour
{
    PlayerManager player;

    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float sprintingSpeed = 7;
    [SerializeField] float rotationSpeed = 15;

    [Header("Dodge")]
    Vector3 rollDirection;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
    }

    private void GetVerticalAndHorinzontalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }

    private void HandleGroundedMovement()
    {
        GetVerticalAndHorinzontalInputs();

        if (!player.canMove)
            return;

        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (player.isSprinting)
        {
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                //以奔跑速度移动
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f && PlayerInputManager.instance.moveAmount > 0)
            {
                //以行走速度移动
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleRotation()
    {
        if (!player.canRotate)
            return;

        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        player.transform.rotation = targetRotation;
    }

    //因为耐力条或正在执行其它的动作，闪避不一定执行成功，所以是Attempt
    public void AttepmtToPerformDodge()
    {
        if (player.isPerformingAction)
            return;

        //移动时闪避，会翻滚
        if (PlayerInputManager.instance.moveAmount > 0)
        {
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
            rollDirection.Normalize();
            rollDirection.y = 0;
            

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            //执行翻滚动画
            player.playerAnimatorManager.PlayTargetActionAnimation("RollForward", true, true);
        }
        //不动时闪避，会后跳
        else
        {
            //执行后跳动画
            player.playerAnimatorManager.PlayTargetActionAnimation("StepBack", true, true);
        }

    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            // set sprinting to false
            player.isSprinting = false;
        }

        // 耐力条空了，set sprinting to false

        // 移动中， set sprinting to true
        if (moveAmount >= 0.5f)
        {
            player.isSprinting = true;
        }
        // 静止不动， set sprinting to false
        else
        {
            player.isSprinting = false;
        }
    }
}
