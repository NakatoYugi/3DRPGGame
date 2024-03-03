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
        //������ֹ��ɫִ���¶���
        //�����ɫ�޷��ж�ʱ�������־��Ϊtrue
        //�ڳ���ִ���¶���ǰ���ȼ��ñ�־
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
