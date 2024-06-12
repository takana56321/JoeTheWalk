using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʓG�N���X�FBee
/// 
/// �󒆏㉺�����ړ�
/// </summary>
public class Enemy_Bee : EnemyBase
{
    // �摜�f��
    [SerializeField] private Sprite[] spriteList_Fly = null; // ��s�A�j���[�V����

    // �ݒ荀��
    [Header("�ړ�����(���������Ԃ�������)")]
    public float MovingTime;
    [Header("�ړ�����")]
    public float MovingSpeed;

    // �e��ϐ�
    private float flyAnimationTime; // ��s�A�j���[�V�����o�ߎ���
    private int flyAnimationFrame; // ��s�A�j���[�V�����̌��݂̃R�}�ԍ�
    private Vector3 defaultPos; // �������W
    private Vector3 moveVec; // �ړ��ʃx�N�g��
    private float time; // �o�ߎ���

    // �_����`
    private const float FlyAnimationSpan = 0.3f; // ��s�A�j���[�V�����̃X�v���C�g�؂�ւ�����

    // Start
    void Start()
    {
        //- �ϐ�������
        defaultPos = transform.position;
        moveVec = Vector3.zero;
        time = 0.0f;

        // �G���[���
        if (MovingTime < 0.1f)
            MovingTime = 0.1f;
    }

    // Update
    void Update()
    {
        // ���Œ��Ȃ�ړ����Ȃ�
        if (isVanishing)
            return;

        // ���s�A�j���[�V�������Ԃ��o��
        flyAnimationTime += Time.deltaTime;
        // ���s�A�j���[�V�����R�}�����v�Z
        if (flyAnimationTime >= FlyAnimationSpan)
        {
            flyAnimationTime -= FlyAnimationSpan;
            // �R�}���𑝉�
            flyAnimationFrame++;
            // �R�}������s�A�j���[�V�����������z���Ă���Ȃ�0�ɖ߂�
            if (flyAnimationFrame >= spriteList_Fly.Length)
                flyAnimationFrame = 0;
        }

        // ���s�A�j���[�V�����X�V
        spriteRenderer.sprite = spriteList_Fly[flyAnimationFrame];

        // �㉺�ړ�
        // ���Ԍo��
        time += Time.deltaTime;
        // �ړ��x�N�g���v�Z
        Vector3 vec;
        vec = new Vector3((Mathf.Sin(time / MovingTime) + 1.0f) * MovingSpeed, 0.0f, 0.0f);
        vec = Quaternion.Euler(0, 0, 90) * vec;
        // �ړ��K�p
        rigidbody2D.MovePosition(defaultPos + vec);
    }
}