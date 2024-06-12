using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʓG�N���X(�{�X)�FEagle
/// 
/// �󒆕��V�E�ːi�U��
/// </summary>
public class Boss_Eagle : EnemyBase
{
    // �摜�f��
    [Header("�摜�f��")]
    [SerializeField] private Sprite[] spriteList_Fly = null; // ��s�A�j���[�V����

    // �ݒ荀��
    [Header("�U���Ԋu")]
    public float attackInterval;
    [Header("�ړ����x")]
    public float movingSpeed;

    // �e��ϐ�
    private float nextAttackTime; // ���̍U���܂ł̎c�莞��
    private float flyCount; // ��s�A�j���[�V�����p�J�E���^

    // Start
    void Start()
    {
        // �ϐ�������
        nextAttackTime = attackInterval / 2.0f;
    }
    /// <summary>
    /// ���̃����X�^�[�̋���G���A�ɃA�N�^�[���i���������̏���(�G���A�A�N�e�B�u��������)
    /// </summary>
    public override void OnAreaActivated()
    {
        base.OnAreaActivated();
    }

    // Update
    void Update()
    {
        // ���Œ��Ȃ珈�����Ȃ�
        if (isVanishing)
            return;

        // ��s�A�j���[�V����
        flyCount += Time.deltaTime;
        int animationFrame = (int)(flyCount * 3.0f);
        animationFrame %= spriteList_Fly.Length;
        spriteRenderer.sprite = spriteList_Fly[animationFrame];

        // �A�N�^�[�Ƃ̈ʒu�֌W�������������
        float xSpeed;
        if (transform.position.x > actorTransform.position.x)
        {// ������
            xSpeed = -movingSpeed;
            SetFacingRight(false);
        }
        else
        {// �E����
            xSpeed = movingSpeed;
            SetFacingRight(true);
        }

        // �ړ�����
        Vector2 vec = rigidbody2D.velocity;   // ���x�x�N�g��
        vec.x += xSpeed * Time.deltaTime;
        // ���x�x�N�g�����Z�b�g
        rigidbody2D.velocity = vec;


        // �U���Ԋu����
        nextAttackTime -= Time.deltaTime;
        if (nextAttackTime > 0.0f)
            return;
        nextAttackTime = attackInterval;
        // �ːi�U���x�N�g���v�Z
        Vector2 calcVec = actorTransform.position - transform.position;
        calcVec.x *= 0.5f; // �������ɂ͂��܂�i�܂Ȃ�
        calcVec.y *= 2.4f; // �t�����̏d�͉����x�ɋt�炦��悤��y�������x��Z
                           // �ːi�U��
        rigidbody2D.velocity += calcVec;
    }
}