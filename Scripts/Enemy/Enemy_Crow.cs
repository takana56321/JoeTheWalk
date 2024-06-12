using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʓG�N���X�FCrow
/// 
/// �󒆍��E�����ړ�(�ǂŔ��])
/// </summary>
public class Enemy_Crow : EnemyBase
{
    // �摜�f��
    [SerializeField] private Sprite[] spriteList_Move = null; // �ړ��A�j���[�V����

    // �ݒ荀��
    [Header("�ړ����x")]
    public float movingSpeed;

    // �e��ϐ�
    private float moveAnimationTime; // �ړ��A�j���[�V�����o�ߎ���
    private int moveAnimationFrame; // �ړ��A�j���[�V�����̌��݂̃R�}�ԍ�
    private float previousPositionX; // �O��t���[����X���W
    private bool initialFrame = true;// ����1�񕪂̕����t���[��

    // �萔��`
    private const float MoveAnimationSpan = 0.3f; // �ړ��A�j���[�V�����̃X�v���C�g�؂�ւ�����

    // Update
    void Update()
    {
        // ���Œ��Ȃ珈�����Ȃ�
        if (isVanishing)
            return;

        // �ړ��A�j���[�V�������Ԃ��o��
        moveAnimationTime += Time.deltaTime;
        // �ړ��A�j���[�V�����R�}�����v�Z
        if (moveAnimationTime >= MoveAnimationSpan)
        {
            moveAnimationTime -= MoveAnimationSpan;
            // �R�}���𑝉�
            moveAnimationFrame++;
            // �R�}�������s�A�j���[�V�����������z���Ă���Ȃ�0�ɖ߂�
            if (moveAnimationFrame >= spriteList_Move.Length)
                moveAnimationFrame = 0;
        }

        // �ړ��A�j���[�V�����X�V
        spriteRenderer.sprite = spriteList_Move[moveAnimationFrame];
    }

    // FixedUpdate
    void FixedUpdate()
    {
        // ����1�����t���[�����͈ړ��������Ȃ�
        if (initialFrame)
        {
            initialFrame = false;
            previousPositionX = transform.position.x; // ���݂�X���W�̂݋L��
            return;
        }

        // ���Œ��Ȃ�ړ����Ȃ�
        if (isVanishing)
        {
            rigidbody2D.velocity = Vector2.zero;
            return;
        }

        // ���݂�X���W���擾
        float currentPositionX = transform.position.x;

        // �O��ʒu��X���W���قڕς���Ă��Ȃ��Ȃ�����𔽓]����
        if (Mathf.Approximately(currentPositionX, previousPositionX))
        {
            SetFacingRight(!rightFacing);
        }

        // ���݂�X���W��O���X���W�Ƃ��ĕۑ�
        previousPositionX = currentPositionX;

        // ���ړ�(����)
        float xSpeed = movingSpeed;
        if (!rightFacing)
            xSpeed *= -1.0f;
        rigidbody2D.velocity = new Vector2(xSpeed, 0.0f);
    }
}