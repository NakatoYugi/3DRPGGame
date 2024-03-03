using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    PlayerManager player;

    int verticalParameterHash;
    int horizontalParameterHash;
    private void Awake()
    {
        player = GetComponent<PlayerManager>();

        verticalParameterHash = Animator.StringToHash("Vertical");
        horizontalParameterHash = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorMovementParameter(float hoprizontalValue, float verticalValue, bool isSprinting)
    {
        if (isSprinting)
        {
            verticalValue = 2;
        }

        player.animator.SetFloat(horizontalParameterHash, hoprizontalValue, 0.1f, Time.deltaTime);
        player.animator.SetFloat(verticalParameterHash, verticalValue, 0.1f, Time.deltaTime);
    }

    public void PlayTargetActionAnimation(
        string targetAnimation, 
        bool isPerformingAction, 
        bool applyRootMotion = true, 
        bool canMove = false, 
        bool canRotate = false)
    {
        player.applyRootMotion = applyRootMotion;
        player.animator.CrossFade(targetAnimation, 0.2f);
        //用于阻止角色执行新动作
        //如果角色无法行动时，这个标志会为true
        //在尝试执行新动作前，先检查该标志
        player.isPerformingAction = isPerformingAction;
        player.canMove = canMove;
        player.canRotate = canRotate;
    }

    private void OnAnimatorMove()
    {
        if (player.applyRootMotion)
        {
            Vector3 velocity = player.animator.deltaPosition;
            player.characterController.Move(velocity);
            player.transform.rotation *= player.animator.deltaRotation;
        }
    }
}
