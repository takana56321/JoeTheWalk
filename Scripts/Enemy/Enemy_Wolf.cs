using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʓG�N���X�FWolf
/// 
/// ���E�����ړ�
/// </summary>
public class Enemy_Wolf : EnemyBase
{
    // �摜�f��
    [SerializeField] private Sprite[] spriteList_Walk = null; // ���s�A�j���[�V����

    // �ݒ荀��
    [Header("�ړ����x")]
    public float movingSpeed;

    // �e��ϐ�
    private float walkAnimationTime; // ���s�A�j���[�V�����o�ߎ���
    private int walkAnimationFrame; // ���s�A�j���[�V�����̌��݂̃R�}�ԍ�

    // �萔��`
    private const float WalkAnimationSpan = 0.3f; // ���s�A�j���[�V�����̃X�v���C�g�؂�ւ�����

    // Update
    void Update()
    {
        // ���s�A�j���[�V�������Ԃ��o��
        walkAnimationTime += Time.deltaTime;
        // ���s�A�j���[�V�����R�}�����v�Z
        if (walkAnimationTime >= WalkAnimationSpan)
        {
            walkAnimationTime -= WalkAnimationSpan;
            // �R�}���𑝉�
            walkAnimationFrame++;
            // �R�}�������s�A�j���[�V�����������z���Ă���Ȃ�0�ɖ߂�
            if (walkAnimationFrame >= spriteList_Walk.Length)
                walkAnimationFrame = 0;
        }

        // ���s�A�j���[�V�����X�V
        spriteRenderer.sprite = spriteList_Walk[walkAnimationFrame];
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

        // �ǂɂԂ�����������ύX
        if (rightFacing && rigidbody2D.velocity.x <= 0.0f)
            SetFacingRight(false);
        else if (!rightFacing && rigidbody2D.velocity.x >= 0.0f)
            SetFacingRight(true);

        // ���ړ�(����)
        float xSpeed = movingSpeed;
        if (!rightFacing)
            xSpeed *= -1.0f;
        rigidbody2D.velocity = new Vector2(xSpeed, rigidbody2D.velocity.y);
    }
}