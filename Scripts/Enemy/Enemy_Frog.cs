using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʓG�N���X�FFrog
/// 
/// ���E�����ړ�
/// </summary>
public class Enemy_Frog : EnemyBase
{
    // �摜�f��
    [SerializeField] private Sprite sprite_Wait = null; // �ҋ@
    [SerializeField] private Sprite sprite_Jump = null; // �W�����v

    // �ݒ荀��
    [Header("�W�����v�Ԋu")]
    public float jumpInterval;
    [Header("�W�����v�́E�O")]
    public float jumpPower_Forward;
    [Header("�W�����v�́E��")]
    public float jumpPower_Upward;

    // �e��ϐ�
    private float timeCount;

    // Update
    void Update()
    {
        // �X�v���C�g�K�p
        if (rigidbody2D.velocity.magnitude < 0.1f)
            spriteRenderer.sprite = sprite_Wait;
        else
        {
            spriteRenderer.sprite = sprite_Jump;
            // �ړ����Ȃ炻�̂܂܏����I��
            return;
        }

        // �s���Ԋu����
        timeCount += Time.deltaTime;
        if (timeCount < jumpInterval)
            return;
        timeCount = 0.0f;

        // �A�N�^�[�Ƃ̈ʒu�֌W�������������
        if (transform.position.x > actorTransform.position.x)
        {// ������
            SetFacingRight(false);
        }
        else
        {// �E����
            SetFacingRight(true);
        }

        // �W�����v�ړ�
        Vector2 jumpVec = new Vector2(jumpPower_Forward, jumpPower_Upward);
        if (!rightFacing)
            jumpVec.x *= -1.0f;
        rigidbody2D.velocity += jumpVec;
    }

    // FixedUpdate
    void FixedUpdate()
    {
        // ���Œ��Ȃ�ړ����Ȃ�
        if (isVanishing)
        {
            rigidbody2D.velocity = Vector2.zero;
            return;
        }
    }
}