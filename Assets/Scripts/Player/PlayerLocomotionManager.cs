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
                //�Ա����ٶ��ƶ�
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f && PlayerInputManager.instance.moveAmount > 0)
            {
                //�������ٶ��ƶ�
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

    //��Ϊ������������ִ�������Ķ��������ܲ�һ��ִ�гɹ���������Attempt
    public void AttepmtToPerformDodge()
    {
        if (player.isPerformingAction)
            return;

        //�ƶ�ʱ���ܣ��ᷭ��
        if (PlayerInputManager.instance.moveAmount > 0)
        {
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
            rollDirection.Normalize();
            rollDirection.y = 0;
            

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            //ִ�з�������
            player.playerAnimatorManager.PlayTargetActionAnimation("RollForward", true, true);
        }
        //����ʱ���ܣ������
        else
        {
            //ִ�к�������
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

        // ���������ˣ�set sprinting to false

        // �ƶ��У� set sprinting to true
        if (moveAmount >= 0.5f)
        {
            player.isSprinting = true;
        }
        // ��ֹ������ set sprinting to false
        else
        {
            player.isSprinting = false;
        }
    }
}
