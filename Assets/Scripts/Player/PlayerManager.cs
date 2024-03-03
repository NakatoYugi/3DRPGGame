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
    [HideInInspector] public bool canMove; //��ִ��ĳЩ���⶯��ʱ������ϣ����ɫ�ڸö���ִ��ʱ�Կ����ƶ�
    [HideInInspector] public bool canRotate; //��ִ��ĳЩ���⶯��ʱ������ϣ����ɫ�ڸö���ִ��ʱ�Կ���ת��
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
